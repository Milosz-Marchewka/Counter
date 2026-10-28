using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Counter.Models
{
    internal class AllCounters
    {
        private readonly string dataPath = Path.Combine(FileSystem.AppDataDirectory, "counters.xml");
        public ObservableCollection<Counter> Counters { get; set; } = new();

        public string Title => "All counters";
        public string ClearCountersText => "Delete all counters";
        public string CreateCounterText => "Create a counter";
        public string NoCountersText => "No counters have been created yet. Make one!";

        public AllCounters() =>
                LoadCounters();

        public void LoadCounters()
        {
            Counters.Clear();

            XDocument document = File.Exists(dataPath)
                ? XDocument.Load(dataPath)
                : new XDocument(new XElement("Root"));

            var savedCounters = document.Root?
                .Elements("Counter")
                .Select(element => new
                {
                    GUID = (Guid?)element.Element("GUID"),
                    Name = (string?)element.Element("Name") ?? string.Empty,
                    Value = (int?)element.Element("Value") ?? 0,
                    DefaultValue = (int?)element.Element("DefaultValue") ?? 0,
                    Color = (element?.Element("Color")?.Value is string colorStr ? Color.FromHex(colorStr) : Colors.AliceBlue) ?? Colors.AliceBlue,
                    CreatedAt = (DateTime?)element?.Element("CreatedAt") ?? DateTime.Now,
                }).ToList();

            if(savedCounters is not null)
            {
                foreach(var counter in savedCounters)
                {
                    Counter init = new Counter
                    {
                        GUID = counter.GUID ?? Guid.NewGuid(),
                        Name = counter.Name,
                        Value = counter.Value,
                        DefaultValue = counter.DefaultValue,
                        Color = counter.Color,
                        CreatedAt = counter.CreatedAt,
                    };
                    init.Deleted += OnCounterDeleted;
                    Counters.Add(init);
                    Debug.WriteLine($"Added counter of color {init.Color}");
                }
            }
        }

        private void OnCounterDeleted(Counter counter)
        {
            Counters.Remove(counter);
        }

        public void SaveCounters()
        {
            XDocument document = new XDocument(
                new XElement("Root",
                    Counters.Select(counter =>
                        new XElement("Counter",
                            new XElement("GUID", counter.GUID),
                            new XElement("Name", counter.Name),
                            new XElement("Value", counter.Value),
                            new XElement("DefaultValue", counter.DefaultValue),
                            new XElement("CreatedAt", counter.CreatedAt)
                        )
                    )
                )
            );
            document.Save(dataPath);
        }

        public void ClearCounters()
        {
            Counters.Clear();
            File.Delete(dataPath);
        }
    }
}
