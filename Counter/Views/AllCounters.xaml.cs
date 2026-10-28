using System.Diagnostics;
using System.Threading.Tasks;
using MauiColor = Microsoft.Maui.Graphics.Color;

namespace Counter.Views;

public partial class AllCounters : ContentPage
{
	public AllCounters()
	{
        InitializeComponent();
        BindingContext = new Models.AllCounters();
        CreateCounterButton.TextColor = Application.Current?.RequestedTheme == AppTheme.Dark ?
            MauiColor.FromRgb(255, 255, 255) : MauiColor.FromRgb(0, 0, 0)
            ?? MauiColor.FromRgb(0, 0, 0);
        ClearCountersButton.TextColor = Application.Current?.RequestedTheme == AppTheme.Dark ?
            MauiColor.FromRgb(255, 255, 255) : MauiColor.FromRgb(0, 0, 0)
            ?? MauiColor.FromRgb(0, 0, 0);
    }

    protected override void OnAppearing()
    {
        ((Models.AllCounters)BindingContext).LoadCounters();
    }

    private async void Add_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///CounterCreatePage");
    }

    private void ClearCounters_Clicked(object sender, EventArgs e)
    {
        ((Models.AllCounters)BindingContext).ClearCounters();
    }
}