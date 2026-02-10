// MainPage.xaml.cs

namespace MauiJintAsyncApp;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public partial class MainPage : ContentPage
{
	public MainViewModel VM { get; } = new();

	public MainPage()
	{
		BindingContext = VM;
		InitializeComponent();
	}
}
