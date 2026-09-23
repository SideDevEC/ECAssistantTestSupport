using ECAssistant.Core.Session;

namespace ECAssistant.TestSupport;

/// <summary>
/// Test implementation of ISessionOutput.
/// Captures all output and routes approval requests to the GuiTestHarness's queued input.
/// </summary>
public class TestSessionOutput : ISessionOutput
{
    private readonly GuiTestHarness _gui;
    private readonly System.Text.StringBuilder _streamBuffer = new();
    private OutputState _streamState = OutputState.Raw;
    private readonly List<(string text, OutputState state)> _outputLog = new();

    public TestSessionOutput(GuiTestHarness gui)
    {
        _gui = gui;
    }

    public IReadOnlyList<(string text, OutputState state)> OutputLog => _outputLog;

    // ── Streaming ──

    public void StartStream(OutputState state)
    {
        lock (_streamBuffer)
        {
            _streamBuffer.Clear();
            _streamState = state;
        }
    }

    public void Write(string token)
    {
        lock (_streamBuffer)
        {
            _streamBuffer.Append(token);
        }
    }

    public void StopStream()
    {
        // Nothing — caller will WriteLine the buffer
    }

    // ── Discrete output ──

    public void WriteLine(string text, OutputState state = OutputState.Info)
    {
        _outputLog.Add((text, state));
        _gui.WriteLineColored($"[{state}] {text}");
    }

    public void BlankLine() => WriteLine("", OutputState.Info);

    public void WriteInfo(string text) => WriteLine(text, OutputState.Info);
    public void WriteSuccess(string text) => WriteLine(text, OutputState.Success);
    public void WriteWarning(string text) => WriteLine(text, OutputState.Warning);
    public void WriteError(string text) => WriteLine(text, OutputState.Error);
    public void WriteDim(string text) => WriteLine(text, OutputState.Dim);

    public void WriteTag(string tag, string message, OutputState state = OutputState.Info)
        => WriteLine($"[{tag}] {message}", state);

    // ── Stream buffer access ──

    public string GetStreamBuffer()
    {
        lock (_streamBuffer) return _streamBuffer.ToString();
    }

    public OutputState GetStreamState()
    {
        lock (_streamBuffer) return _streamState;
    }

    // ── User approval ──

    public bool RequestApproval(string message)
    {
        WriteLine(message, OutputState.Warning);
        var response = _gui.PromptRaw("[y/N] ");
        return response?.Trim().ToLower() == "y" || response?.Trim().ToLower() == "yes";
    }

    /// <summary>v14.9: interactive checkpoint — prompt via GUI, 1-based pick, null = cancel.</summary>
    public int? RequestChoice(string prompt, System.Collections.Generic.IReadOnlyList<string> options)
    {
        WriteLine(prompt, OutputState.Warning);
        for (int i = 0; i < options.Count; i++)
            WriteLine($"  {i + 1}) {options[i]}", OutputState.Info);
        var response = _gui.PromptRaw($"[1-{options.Count}, Enter=skip] ");
        if (int.TryParse(response?.Trim(), out var pick) && pick >= 1 && pick <= options.Count)
            return pick;
        return null;
    }
}