using ECAssistant.Core.Interfaces;

namespace ECAssistant.TestSupport;

/// <summary>No-op KV cache controller — all operations succeed without a server.</summary>
public sealed class KvCacheNoop : IKvCacheController
{
    public static readonly KvCacheNoop Instance = new();
    public Task<bool> CreateSessionAsync(string sessionId, string? modelId = null, CancellationToken ct = default)
        => Task.FromResult(true);
    public Task<bool> DestroySessionAsync(string sessionId, CancellationToken ct = default)
        => Task.FromResult(true);
    public Task<bool> PrefillAsync(string sessionId, string text, CancellationToken ct = default)
        => Task.FromResult(true);
    public Task<bool> SaveStateAsync(string sessionId, CancellationToken ct = default)
        => Task.FromResult(true);
    public Task<bool> RewindAsync(string sessionId, CancellationToken ct = default)
        => Task.FromResult(true);
    public Task<bool> ResetAsync(string sessionId, CancellationToken ct = default)
        => Task.FromResult(true);
    public Task<KvCacheStatus?> GetStatusAsync(string sessionId, CancellationToken ct = default)
        => Task.FromResult<KvCacheStatus?>(null);
}