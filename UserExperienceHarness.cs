using ECAssistant.Core.Config;
using ECAssistant.Core.Engine;
using ECAssistant.Core.Session;

namespace ECAssistant.TestSupport;

/// <summary>
/// v14.19: user-experience E2E harness — simulates EXACTLY what a user would
/// see and do in the Console/TUI: sends prompts via AgentSession.Prompt (the
/// real user entry point, not direct orchestrator calls), captures every line
/// of visible output (system/info/success/warning/error/stream), and answers
/// approval prompts like a user would. Tests assert on the TRANSCRIPT — the
/// thing the user actually experiences — instead of internal engine state.
/// </summary>
public sealed class UserExperienceHarness : IOutputListener
{
    private readonly AgentSession _session;
    private readonly List<(string Text, OutputState State)> _transcript = new();
    private readonly object _lock = new();
    private readonly Func<string, ApprovalScope> _approvalResponder;
    private string _streamBuffer = "";
    private bool _streaming;

    private UserExperienceHarness(AgentSession session, Func<string, ApprovalScope> approvalResponder)
    {
        _session = session;
        _approvalResponder = approvalResponder;
        session.AddListener(this);
    }

    /// <summary>Working directory the session runs in (temp, cleaned on Dispose).</summary>
    public string WorkingDir { get; private set; } = "";

    /// <summary>Everything the user saw, in order.</summary>
    public IReadOnlyList<(string Text, OutputState State)> Transcript
    {
        get { lock (_lock) return _transcript.ToList(); }
    }

    /// <summary>Flat transcript text (newline-joined) for Contains-style asserts.</summary>
    public string TranscriptText
    {
        get { lock (_lock) return string.Join("\n", _transcript.Select(t => t.Text)); }
    }

    /// <summary>
    /// Create a harness with a real local-model session.
    /// approvalResponder: given an approval prompt, returns the user's decision.
    /// Default: approve everything (a cooperative user pressing 'y').
    /// </summary>
    public static async Task<UserExperienceHarness> CreateAsync(
        string serverUrl,
        EAgentConfig? config = null,
        Func<string, ApprovalScope>? approvalResponder = null,
        Action<EAgentEngine>? configure = null,
        string? modelId = null)
    {
        var factory = new HarnessE2ESessionFactory();
        var (session, dir) = await factory.CreateAsync(
            serverUrl,
            config,
            configure: engine =>
            {
                engine.RegisterTool(new ProbeTestTool());
                configure?.Invoke(engine);
            });
        return new UserExperienceHarness(session, approvalResponder ?? (_ => ApprovalScope.AllowOnce))
        {
            WorkingDir = dir
        };
    }

    /// <summary>
    /// Send a prompt like a user typing, then WAIT for the agent to finish
    /// (run state back to Idle), exactly like watching the TUI spinner.
    /// Returns the full transcript delta of this turn.
    /// </summary>
    public async Task<IReadOnlyList<(string Text, OutputState State)>> SendAndAwaitAsync(
        string input, TimeSpan? timeout = null)
    {
        var before = Transcript.Count;
        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromMinutes(3));

        _session.Prompt(input);
        while (_session.RunState != SessionRunState.Idle && DateTime.UtcNow < deadline)
            await Task.Delay(250);

        var all = Transcript;
        return all.Count > before ? all.Skip(before).ToList() : [];
    }

    // ── IOutputListener: capture everything the user would see ──

    public void OnOutput(string text, OutputState state)
    {
        lock (_lock) _transcript.Add((text, state));
    }

    public void OnStreamStart()
    {
        _streaming = true;
        _streamBuffer = "";
    }

    public void OnStreamStop()
    {
        // Streamed content is flushed via WriteLine by the session; nothing to do.
        _streaming = false;
    }

    public bool OnRequestApproval(string message)
        => _approvalResponder(message) != ApprovalScope.Deny;

    public ApprovalScope OnRequestApprovalScoped(string message) => _approvalResponder(message);

    public int? OnRequestChoice(string prompt, IReadOnlyList<string> options) => null;

    public void OnStatus(string? status) { }

    /// <summary>Dispose the session and remove the temp working directory.</summary>
    public async Task DisposeAsync()
    {
        await _session.DisposeAsync();
        try { Directory.Delete(WorkingDir, true); } catch { }
    }
}
