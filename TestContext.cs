using ECAssistant.Core.Engine;
using ECAssistant.Core.Orchestration;
namespace ECAssistant.TestSupport;

public class TestContext
{
    public string WorkingDir { get; set; } = "";
    public GuiTestHarness Gui { get; set; } = null!;
    public AgentEngine Engine { get; set; } = null!;
    public AgentOrchestrator Orchestrator { get; set; } = null!;
    public OrchestratorResult Result { get; set; } = null!;
}
