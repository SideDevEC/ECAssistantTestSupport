using System.Text.Json;
using ECAssistant.Core.Config;
using ECAssistant.Core.Engine;
using ECAssistant.Core.Services;
using ECAssistant.Core.Services.Http;
using ECAssistant.Core.Session;
using ECAssistant.Core.Transport;

namespace ECAssistant.TestSupport;

/// <summary>
/// Creates a REAL AgentSession stack (HttpStreamingEngine + RemoteKvCacheController
/// → AgentEngine → AgentOrchestrator) against a LIVE ECAssistantLLM server — the
/// harness end-to-end entry point. Follows the server contract: register a client
/// identity (/eca/clients), then carry X-Client-Id on every request.
/// The working directory is a fresh temp dir — the caller deletes it after
/// disposing the session.
/// </summary>
public sealed class HarnessE2ESessionFactory
{
    /// <summary>
    /// Register a client and build a fully wired local-mode session.
    /// configure runs before the first use — register tools there.
    /// </summary>
    public async Task<(AgentSession Session, string WorkingDir)> CreateAsync(
        string endpoint,
        AppConfig? config = null,
        Action<AgentEngine>? configure = null,
        string? clientName = null,
        Action<string>? prepareWorkingDir = null)
    {
        var regClient = new OpenAIClient(endpoint);
        string clientId;
        try
        {
            var regBody = JsonSerializer.Serialize(
                new { client_name = clientName ?? "harness-e2e", version = "1.0" });
            var regJson = await regClient.PostJsonAsync("/eca/clients", regBody);
            using var regDoc = JsonDocument.Parse(regJson);
            clientId = regDoc.RootElement.GetProperty("client_id").GetString()
                ?? throw new InvalidOperationException("client registration returned no client_id");
        }
        finally
        {
            // HttpClient inside OpenAIClient must not leak per harness creation.
            regClient.Dispose();
        }

        var workingDir = Directory.CreateTempSubdirectory("eca-harness-e2e").FullName;
        prepareWorkingDir?.Invoke(workingDir);
        var effectiveConfig = config ?? new AppConfig();
        // v14.17: tier-tuned params — match product wiring (small tier tightens default sampling).
        var tierIsLarge = effectiveConfig.ModelTier?.IsLargeRuntime(
            effectiveConfig.LlmProvider?.ModelId, effectiveConfig.LlmProvider?.IsRemote ?? false)
            ?? false;
        var session = new AgentSession(
            key: "e2e-" + Guid.NewGuid().ToString("N")[..8],
            sessionId: "e2e-sess-" + Guid.NewGuid().ToString("N")[..8],
            endpoint: endpoint,
            clientId: clientId,
            inferenceParams: InferenceParamsFactory.Default.CreateTiered(effectiveConfig, tierIsLarge),
            workingDir: workingDir,
            inferenceLock: new SemaphoreSlim(1, 1),
            config: effectiveConfig,
            isLocalMode: true);
        configure?.Invoke(session.Engine);
        return (session, workingDir);
    }
}