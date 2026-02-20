// MainPage.xaml.cs

namespace MauiJintAsyncApp;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	async void OnRunTest(object sender, EventArgs e)
	{
		if (((Button)sender).CommandParameter?.ToString() is string delayString
			&& int.TryParse(delayString, out int delay))
		{
			ItemType.CalculationDelay = delay;
			XFormFunctions.CalcTime = delay;
			await Shell.Current.GoToAsync(nameof(JintPage));
		}
	}

	void OnGC(object sender, EventArgs e)
	{
		GC.Collect();
	}
}

