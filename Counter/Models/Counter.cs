using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Counter.Models
{
    public partial class Counter : INotifyPropertyChanged
    {
        private readonly string dataPath = Path.Combine(FileSystem.AppDataDirectory, "counters.xml");
        public Guid GUID { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public long Value { get; set; } = 0;
        public long DefaultValue { get; set; } = 0;
        public Color Color { get; set; } = Colors.AliceBlue;
        public Color ContrastingColor => GetContrastingColor(Color);
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public string TrashIconUri => GetIcon();
        public string SubtractionButtonText => "-";
        public string AdditionButtonText => "+";
        public string ResetButtonText => "Reset";

        public string CreateCounterTitle => "Create a counter";
        public string CreateNamePlaceholder => "Your counter name...";
        public string ColorButtonText => "Choose a color";
        public string CreateButtonText => "Create";
        public string GoBackButtonText => "Go back";
        private string? validationErrorMessage;
        public string? ValidationErrorMessage
        {
            get => validationErrorMessage;
            private set
            {
                if (validationErrorMessage == value)
                {
                    return;
                }

                validationErrorMessage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValidationErrorMessage)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<Counter>? Deleted;

        public void Increase()
        {
            Value++;
            Save();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }

        public void Decrease()
        {
            Value--;
            Save();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }

        public void Reset()
        {
            Value = DefaultValue;
            Save();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }

        public void Create()
        {
            ValidationErrorMessage = null;
            if(Name == string.Empty || Name.Length > 20)
            {
                ValidationErrorMessage = "Name cannot be empty or longer than 20 characters.";
                throw new InvalidOperationException("Name cannot be empty or longer than 20 characters.");
            }
            if(DefaultValue > 999_999_999)
            {
                ValidationErrorMessage = "The value must have at most 9 digits.";
                throw new InvalidOperationException("The value must have at most 9 digits.");
            }

            Debug.WriteLine($"Creating w/ color: {this.Color.ToArgbHex()}");
            XDocument doc = File.Exists(dataPath)
                                ? XDocument.Load(dataPath)
                                : new XDocument(new XElement("Root"));

            XElement? exists = doc.Root?
                .Elements("Counter")
                .FirstOrDefault(x => x?.Element("GUID")?.Value == GUID.ToString());

            if (exists is null)
            {
                doc.Root?.Add(new XElement("Counter",
                        new XElement("GUID", GUID),
                        new XElement("Name", Name),
                        new XElement("Value", DefaultValue), 
                        new XElement("DefaultValue", DefaultValue),
                        new XElement("Color", Color.ToArgbHex()),
                        new XElement("CreatedAt", CreatedAt)
                    )
                );
            }
            else
            {
                exists.Element("Name")?.SetValue(Name);
                exists.Element("Value")?.SetValue(DefaultValue);
                exists.Element("DefaultValue")?.SetValue(DefaultValue);
                exists.Element("Color")?.SetValue(Color.ToArgbHex());
                exists.Element("CreatedAt")?.SetValue(CreatedAt);
            }

            doc.Save(dataPath);
        }
        public void Delete()
        {
            XDocument doc = XDocument.Load(dataPath);

            XElement? exists = doc.Root?
                .Elements("Counter")
                .FirstOrDefault(x => x?.Element("GUID")?.Value.ToString() == GUID.ToString());
            Debug.WriteLine($"Attempting delete on {GUID}: {exists is not null}");
            exists?.Remove();

            doc.Save(dataPath);
            Deleted?.Invoke(this);
        }

        public void Save()
        {
            XDocument doc = XDocument.Load(dataPath);

            Debug.WriteLine($"Doc: {doc}");

            XElement? exists = doc.Root?
                .Elements("Counter")
                .FirstOrDefault(x => x?.Element("GUID")?.Value.ToString() == GUID.ToString());

            Debug.WriteLine($"Save for {GUID}, previous value: {exists?.Element("Value")}");
            exists?.Element("Value")?.SetValue(Value);
            Debug.WriteLine($"Save for {GUID}, new value: {exists?.Element("Value")}");

            doc.Save(dataPath);
        }


        public static Color GetContrastingColor(Color color)
        {
            return color.GetLuminosity() < 0.5 ? Colors.White : Colors.Black;
        }

        private string GetIcon()
        {
            return Color.GetLuminosity() < 0.5 ? "trash_white.png" : "trash_black.png";
        }
    }
}
