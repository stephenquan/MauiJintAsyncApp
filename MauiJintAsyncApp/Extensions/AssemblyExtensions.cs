// AssemblyExtensions.cs

using System.Reflection;

namespace MauiJintAsyncApp;

public static class AssemblyExtensions
{
	public static string GetMetadataValue(this Assembly assembly, string key)
		=> assembly
			.GetCustomAttributes<AssemblyMetadataAttribute>()
			.FirstOrDefault(a => string.Equals(a.Key, key, StringComparison.Ordinal))
			?.Value ?? string.Empty;
}
