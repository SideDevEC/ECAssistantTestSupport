using ECAssistant.Core.Tools;

namespace ECAssistant.TestSupport;

/// <summary>
/// Harmless registered test tool — typed schema, deterministic output, no side
/// effects. Shared across test suites for tier / grammar-union / e2e harness runs.
/// </summary>
public sealed class ProbeTestTool : EToolBase
{
    public override string Name => "ProbeTool";
    public override string Description =>
        "Runs a system probe and returns its status. Use when the user asks to probe or check something.";
    public override string UsageExample => "ProbeTool(action=\"probe\");";

    public override string GetParameterSchema() =>
        """
        {"type":"object","required":["action"],"properties":{"action":{"type":"string","enum":["probe"]}}}
        """;

    public override Task<EToolResult> ExecuteAsync(
        Dictionary<string, string?> arguments, CancellationToken cancellationToken = default)
        => Task.FromResult(EToolResult.Success("ProbeTool", "PROBE OK: all systems nominal"));
}