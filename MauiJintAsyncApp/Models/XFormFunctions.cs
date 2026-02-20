// XFormFunctions.cs

using Jint;
using Jint.Native;

namespace MauiJintAsyncApp;

public class XFormFunctions
{
	Engine jintEngine;
	public static int CalcTime { get; set; } = 1000;

	public XFormFunctions(Engine jintEngine)
	{
		this.jintEngine = jintEngine;
	}

	public static async Task<object?> math_async(string op, object? x, object? y)
	{
		int workerThreads;
		int ioThreads;
		ThreadPool.GetAvailableThreads(out workerThreads, out ioThreads);
		System.Diagnostics.Trace.WriteLine($"math_async called with op={op}, x={x}, y={y} worker={workerThreads} io={ioThreads}");
		await Task.Delay(CalcTime);
		return (to_double(x, out var dblX) && to_double(y, out var dblY))
			? op switch
			{
				"add" => dblX + dblY,
				"sub" => dblX - dblY,
				"mul" => dblX * dblY,
				"div" => dblX / dblY,
				_ => null
			}
			: null;
	}

	public JsValue math_promise(string op, object? x, object? y)
		=> jintEngine.ToJsPromise(math_async, op, x, y);

	public static async Task<string?> concat_async(params object?[] args)
	{
		System.Diagnostics.Trace.WriteLine($"concat_async called with args=[{string.Join(", ", args)}]");
		await Task.Delay(CalcTime);
		return string.Concat(args);
	}

	public JsValue concat_promise(params object?[] args)
		=> jintEngine.ToJsPromise(concat_async, args);

	public static bool to_double(object? value, out double dblValue)
	{
		dblValue = double.NaN;
		return value switch
		{
			double d => (dblValue = d) == d,
			int i => (dblValue = i) == i,
			string s when double.TryParse(s, out var parsed) => (dblValue = parsed) == parsed,
			_ => false
		};
	}
}
