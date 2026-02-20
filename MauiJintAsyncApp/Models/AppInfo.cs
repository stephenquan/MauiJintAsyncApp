// AppInfo.cs

namespace MauiJintAsyncApp;

public static class AppInfo
{
	public static string MauiVersion { get; } = typeof(AppInfo).Assembly.GetMetadataValue("AppInfo.MauiVersion");
	public static string CommunityToolkitMauiVersion { get; } = typeof(AppInfo).Assembly.GetMetadataValue("AppInfo.CommunityToolkitMauiVersion");
	public static string CommunityToolkitMvvmVersion { get; } = typeof(AppInfo).Assembly.GetMetadataValue("AppInfo.CommunityToolkitMvvmVersion");
	public static string JintVersion { get; } = typeof(AppInfo).Assembly.GetMetadataValue("AppInfo.JintVersion");
}
