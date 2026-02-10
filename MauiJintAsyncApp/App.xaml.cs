// App.xaml.cs

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace MauiJintAsyncApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}
