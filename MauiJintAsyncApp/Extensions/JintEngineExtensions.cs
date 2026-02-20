// JintEngineExtensions.cs

using Jint;
using Jint.Native;

namespace MauiJintAsyncApp;

public static class JintEngineExtensions
{
	// --- Core shared implementation ---
	static JsValue ToJsPromiseInternal<T>(Engine engine, Func<Task<T>> invokeAsync, Action<Action>? finalizePromise = null)
	{
		finalizePromise ??= static a => a();

		var (promise, resolve, reject) = engine.Advanced.RegisterPromise();

		_ = Task.Run(async () =>
		{
			try
			{
				var result = await invokeAsync();
				finalizePromise(() => resolve(JsValue.FromObject(engine, result)));
			}
			catch (Exception ex)
			{
				finalizePromise(() => reject(JsValue.FromObject(engine, ex)));
			}
		});

		return promise;
	}

	// --- Strongly typed public overloads ---

	public static JsValue ToJsPromise<T1, TReturn>(this Engine engine, Func<T1, Task<TReturn>> asyncFunc, T1 p1, Action<Action>? finalizePromise = null)
		=> ToJsPromiseInternal(engine, () => asyncFunc(p1), finalizePromise);

	public static JsValue ToJsPromise<T1, T2, TReturn>(this Engine engine, Func<T1, T2, Task<TReturn>> asyncFunc, T1 p1, T2 p2, Action<Action>? finalizePromise = null)
		=> ToJsPromiseInternal(engine, () => asyncFunc(p1, p2), finalizePromise);

	public static JsValue ToJsPromise<T1, T2, T3, TReturn>(this Engine engine, Func<T1, T2, T3, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, Action<Action>? finalizePromise = null)
		=> ToJsPromiseInternal(engine, () => asyncFunc(p1, p2, p3), finalizePromise);

	// --- Func converters ---

	public static Func<T1, JsValue> ToJsPromiseFunc<T1, TReturn>(this Engine engine, Func<T1, Task<TReturn>> asyncFunc, Action<Action>? finalizePromise = null)
		=> p1 => engine.ToJsPromise(asyncFunc, p1, finalizePromise);

	public static Func<T1, T2, JsValue> ToJsPromiseFunc<T1, T2, TReturn>(this Engine engine, Func<T1, T2, Task<TReturn>> asyncFunc, Action<Action>? finalizePromise = null)
		=> (p1, p2) => engine.ToJsPromise(asyncFunc, p1, p2, finalizePromise);

	public static Func<T1, T2, T3, JsValue> ToJsPromiseFunc<T1, T2, T3, TReturn>(this Engine engine, Func<T1, T2, T3, Task<TReturn>> asyncFunc, Action<Action>? finalizePromise = null)
		=> (p1, p2, p3) => engine.ToJsPromise(asyncFunc, p1, p2, p3, finalizePromise);

	// --- SetValue helpers ---

	public static Engine SetAsyncFunc<T1, TReturn>(this Engine engine, string name, Func<T1, Task<TReturn>> asyncFunc, Action<Action>? finalizePromise = null)
		=> engine.SetValue(name, engine.ToJsPromiseFunc(asyncFunc, finalizePromise));

	public static Engine SetAsyncFunc<T1, T2, TReturn>(this Engine engine, string name, Func<T1, T2, Task<TReturn>> asyncFunc, Action<Action>? finalizePromise = null)
		=> engine.SetValue(name, engine.ToJsPromiseFunc(asyncFunc, finalizePromise));

	public static Engine SetAsyncFunc<T1, T2, T3, TReturn>(this Engine engine, string name, Func<T1, T2, T3, Task<TReturn>> asyncFunc, Action<Action>? finalizePromise = null)
		=> engine.SetValue(name, engine.ToJsPromiseFunc(asyncFunc, finalizePromise));

	public static async Task<T> ExecuteAsync<T>(this Engine engine, string script)
	{
		TaskCompletionSource<object?> tcs = new();
		engine.SetValue("__set_result", new Action<object?>(result => tcs.SetResult(result)));
		engine.SetValue("__set_error", new Action<string>((message) => tcs.SetException(new Exception(message))));
		engine.Evaluate(script);
		var result = await tcs.Task;
		if (result is not T typedResult)
		{
			throw new InvalidCastException($"Expected result of type {typeof(T)}, but got {result?.GetType()}");
		}
		return typedResult;
	}
}
