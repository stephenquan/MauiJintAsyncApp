// MainPage.xaml.cs

using System.Reflection;

namespace MauiJintAsyncApp;

public partial class MainPage : ContentPage
{
	public List<AssemblyMetadataAttribute> AppVersionInfos { get; }
		= typeof(MainPage).Assembly
			.GetCustomAttributes<AssemblyMetadataAttribute>()
			.Where(a => a.Key.StartsWith("AppInfo."))
			.ToList();

	public MainPage()
	{
		BindingContext = this;
		InitializeComponent();
	}

	async void OnRunTest(object sender, EventArgs e)
	{
		if (((Button)sender).CommandParameter?.ToString() is string delayString
			&& int.TryParse(delayString, out int delay))
		{
			ItemType.CalculationDelay = delay;
			await Shell.Current.GoToAsync(nameof(JintPage));
		}
	}

	void OnGC(object sender, EventArgs e)
	{
		GC.Collect();
	}
}

