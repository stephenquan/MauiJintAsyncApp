// MainApplication.cs

using Android.App;
using Android.Runtime;

namespace MauiJintAsyncApp;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
