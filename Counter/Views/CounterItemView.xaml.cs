using System.Diagnostics;
using System.Xml.Linq;

namespace Counter.Views;

public partial class CounterItemView : ContentView
{
    private readonly string dataPath = Path.Combine(FileSystem.AppDataDirectory, "counters.xml");
    public CounterItemView()
	{
		InitializeComponent();
	}

    private void OnSubtractionButtonClicked(object sender, EventArgs e)
    {
        if(BindingContext is Models.Counter counter)
        {
            counter.Decrease();
        }
    }

    private void OnResetButtonClicked(object sender, EventArgs e)
    {
        if (BindingContext is Models.Counter counter)
        {
            counter.Reset();
        }
    }

    private void OnAdditionButtonClicked(object sender, EventArgs e)
    {
        if (BindingContext is Models.Counter counter)
        {
            counter.Increase();
        }
    }

    private void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        if(BindingContext is Models.Counter counter)
        {
            counter.Delete();
        }
    }
}