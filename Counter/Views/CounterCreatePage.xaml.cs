using MauiColor = Microsoft.Maui.Graphics.Color;

namespace Counter.Views;

public partial class CounterCreatePage : ContentPage
{

    public CounterCreatePage()
	{
		InitializeComponent();

        RedSlider.ValueChanged += OnRgbChanged;
        GreenSlider.ValueChanged += OnRgbChanged;
        BlueSlider.ValueChanged += OnRgbChanged;
        // only because if it's dark mode, the default entry/label colors assume white until updated by getcontrastingcolor, so must be called at init
        MauiColor defaultColor = ((Models.Counter)BindingContext).Color;
        ChangeEntryColors(defaultColor);
        // these only need to be called once because theyre not dynamic
        RedLabel.TextColor = Models.Counter.GetContrastingColor(defaultColor);
        GreenLabel.TextColor = Models.Counter.GetContrastingColor(defaultColor);
        BlueLabel.TextColor = Models.Counter.GetContrastingColor(defaultColor);
        BackButton.TextColor = Application.Current?.RequestedTheme == AppTheme.Dark ? 
            MauiColor.FromRgb(255,255,255) : MauiColor.FromRgb(0,0,0) 
            ?? MauiColor.FromRgb(0,0,0);
    }

    private async void Create_Counter(object sender, EventArgs e)
    {
		if(BindingContext is Models.Counter counter)
		{
            try
            {
                counter.Create();
                BindingContext = new Models.Counter();
                await Shell.Current.GoToAsync("//AllCounters");
            } catch (InvalidOperationException) { }
        }
    }

    private async void OnBackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AllCounters");
    }

    private void OnDefaultValue_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(sender is Entry entry && !string.IsNullOrEmpty(entry.Text))
        {
            if(!long.TryParse(entry.Text, out long _))
            {
                entry.Text = e.OldTextValue;
            }
        }
    }

    private void OnRgbChanged(object? sender, ValueChangedEventArgs e)
    {
        var selectedColor = MauiColor.FromRgb(
            (int)Math.Round(RedSlider.Value),
            (int)Math.Round(GreenSlider.Value),
            (int)Math.Round(BlueSlider.Value));

        if (BindingContext is Models.Counter counter)
        {
            counter.Color = selectedColor;
        }

        ChangeEntryColors(selectedColor);
        ColorPreview.BackgroundColor = selectedColor;
    }


    private void ChangeEntryColors(MauiColor color)
    {
        Name.TextColor = Models.Counter.GetContrastingColor(color);
        Value.TextColor = Models.Counter.GetContrastingColor(color);
    }
}