// ItemType.cs

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Jint;
using Jint.Native;

namespace MauiJintAsyncApp;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable - CancellationTokenSource is disposed when form is disposed via messaging, but the pattern is not recognized by the analyzer

public partial class ItemType : ObservableObject, IRecipient<FormDisposedMessage>
{
	Engine engine;

	public static int CalculationDelay { get; set; } = 1000;
	static Random random = new Random();

	[ObservableProperty]
	public partial int ItemId { get; set; } = 0;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ExpressionText))]
	public partial int ValueA { get; set; } = random.Next(1, 11);

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ExpressionText))]
	public partial int ValueB { get; set; } = random.Next(1, 11);

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ExpressionText))]
	public partial string Operator { get; set; } = "+-*/"[random.Next(0, 4)].ToString();

	public string ExpressionText => $"{ValueA} {Operator} {ValueB}";
	public string ExpressionScript =>
		$$"""
		async function main() {
			return await __item_type.math_promise('{{Operator}}', {{ValueA}}, {{ValueB}});
		}
		""";

	[ObservableProperty]
	public partial string CalculationText { get; set; } = string.Empty;

	[ObservableProperty]
	public partial Color CalculationColor { get; set; } = Colors.Black;

	CancellationTokenSource? cts = new();
	public static int InstanceCount { get; private set; } = 0;

	public ItemType()
	{
		engine = new Engine(SetEngineOptions);

		InstanceCount++;
		System.Diagnostics.Trace.WriteLine($"ItemType created. ItemId={ItemId}, InstanceCount={InstanceCount}");

		WeakReferenceMessenger.Default.UnregisterAll(this);
		_ = Task.Run(Start);
	}

	~ItemType()
	{
		InstanceCount--;
		System.Diagnostics.Trace.WriteLine($"ItemType finalized. ItemId={ItemId}, InstanceCount={InstanceCount}");
	}

	public async Task Start()
	{
		_ = Task.Run(async () =>
		{
			CalculationText = $"{ItemId}: Calculating {ExpressionText}: ...";
			CalculationColor = Colors.Orange;
			await Task.Delay(50);
			try
			{
				engine.SetValue("__item_type", this);
				string fullScript =
					$$"""
					{{ExpressionScript}}

					(async () => {
						try {
							var result = await main();
							__execute_task.SetResult(result);
						}
						catch (err) {
							__execute_task.SetError(err);
						}
					})();
					""";
				var result = await engine.ExecuteAsync<double>(fullScript);
				CalculationText = $"{ItemId}: Calculating {ExpressionText}: Result: {result}";
				CalculationColor = Colors.Green;
			}
			catch (Exception ex)
			{
				CalculationText = $"{ItemId}: Calculating {ExpressionText}: Error: {ex.Message}";
				CalculationColor = Colors.Red;
			}
		});
	}

	void SetEngineOptions(Options options)
	{
		options.CancellationToken(cts?.Token ?? CancellationToken.None);
	}

	public async Task<double> math_async(string op, double a, double b)
	{
		await Task.Delay(CalculationDelay);
		return op switch
		{
			"+" => a + b,
			"-" => a - b,
			"*" => a * b,
			"/" => b != 0 ? a / b : throw new DivideByZeroException(),
			_ => throw new InvalidOperationException($"Unsupported operator: {op}")
		};
	}

	public JsValue math_promise(string op, double a, double b)
		=> engine.ToJsPromise(math_async, op, a, b);

	public void Receive(FormDisposedMessage message)
	{
		if (cts is not null)
		{
			cts?.Cancel();
			cts?.Dispose();
			cts = null;
		}

		WeakReferenceMessenger.Default.UnregisterAll(this);
	}
}
