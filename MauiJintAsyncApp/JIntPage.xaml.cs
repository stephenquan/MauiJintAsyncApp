// JIntPage.xaml.cs

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;

namespace MauiJintAsyncApp;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public partial class JintPage : ContentPage, IRecipient<FormDisposedMessage>
{
	public ObservableCollection<ItemType> MyItems { get; } = [];
	public static int InstanceCount { get; set; } = 0;

	public JintPage()
	{
		InstanceCount++;
		System.Diagnostics.Trace.WriteLine($"JintPage created. InstanceCount={InstanceCount}");

		BindingContext = this;
		InitializeComponent();
		//WeakReferenceMessenger.Default.RegisterAll(this);

		_ = Dispatcher.Dispatch(async () => await FinalizeInitialization());
	}

	~JintPage()
	{
		InstanceCount--;
		System.Diagnostics.Trace.WriteLine($"JintPage disposed. InstanceCount={InstanceCount}");
	}

	async Task FinalizeInitialization()
	{
		for (int i = 1; i <= 50; i++)
		{
			await Task.Delay(10);
			MyItems.Add(new ItemType { ItemId = i });
		}
	}

	void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
	{
		WeakReferenceMessenger.Default.Send(new FormDisposedMessage());
	}
	public void Receive(FormDisposedMessage message)
	{
		MyItems.Clear();
	}
}

