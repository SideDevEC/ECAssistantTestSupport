using ECAssistant.Core.Interfaces;

namespace ECAssistant.TestSupport;

/// <summary>
/// No-op inference engine — no HTTP, no model. GenerateStructuredAsync inherits
/// the interface default (null → structured path falls back to text).
/// </summary>
public sealed class InferenceEngineNoop : IInferenceEngine
{
    public static readonly InferenceEngineNoop Instance = new();
    public string Endpoint => "mock";
    public Task<string> GenerateAsync(string prompt, InferenceRequestParams parameters, CancellationToken ct = default)
        => Task.FromResult("");
    public IAsyncEnumerable<string> StreamAsync(string prompt, InferenceRequestParams parameters, CancellationToken ct = default)
        => StreamNoop(ct);

    private static async IAsyncEnumerable<string> StreamNoop(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        await Task.CompletedTask;
        yield break;
    }
}