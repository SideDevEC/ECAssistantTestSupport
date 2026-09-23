# ECAssistantTestSupport.API.md

Types: 13  |  LOC: 1679  |  ~761 tokens

---

### Class: GuiTestHarness
> Non-interactive test harness for GuiBase.
Implements: GuiBase
Cross-package deps: ECAssistant.Core.UI

### Class: EcaTestSuite
> Predefined test scenarios for ECAssistant.
Cross-package deps: ECAssistant.Core.Orchestration

### Class: HarnessE2ESessionFactory
> Creates a REAL AgentSession stack (HttpStreamingEngine + RemoteKvCacheController
Cross-package deps: ECAssistant.Core.Config, ECAssistant.Core.Engine, ECAssistant.Core.Services, ECAssistant.Core.Services.Http, ECAssistant.Core.Session, ECAssistant.Core.Transport

### Class: InferenceEngineNoop
> No-op inference engine — no HTTP, no model. GenerateStructuredAsync inherits
Implements: IInferenceEngine
Cross-package deps: ECAssistant.Core.Interfaces

### Class: KvCacheNoop
> No-op KV cache controller — all operations succeed without a server.
Implements: IKvCacheController
Cross-package deps: ECAssistant.Core.Interfaces

### Class: MockEngine
> Mock engine for testing — no real model loaded. Returns pre-queued responses.
Implements: AgentEngine
Constructor:
  - MockEngine(Queue<string> responses, int maxIterations = 5, bool stopAfterFirstTool = false, string? workingDir = null, ISessionOutput? sessionOutput = null, string? workingDir = null, ISessionOutput? sessionOutput = null, bool cycleResponses = false, AppConfig? config = null)
Cross-package deps: ECAssistant.Core.Config, ECAssistant.Core.Engine, ECAssistant.Core.Interfaces, ECAssistant.Core.Orchestration, ECAssistant.Core.Session

### Class: ProbeTestTool
> Harmless registered test tool — typed schema, deterministic output, no side
Implements: ToolBase
Cross-package deps: ECAssistant.Core.Tools

### Class: TestContext
Cross-package deps: ECAssistant.Core.Engine, ECAssistant.Core.Orchestration

### Class: TestResult

### Class: TestRunner
> Automated test runner for ECAssistant.
Implements: IAsyncDisposable
Constructor:
  - TestRunner(string modelPath, string? testRootDir = null, ILogger? logger = null)
Cross-package deps: ECAssistant.Core.Config, ECAssistant.Core.Engine, ECAssistant.Core.Orchestration, ECAssistant.Core.Tools, ECAssistant.Core.Tools.Shell, ECAssistant.Core.Tools.Research, ECAssistant.Core.Tools.Background, ECAssistant.Core.Tools.Build, ECAssistant.Core.Tools.Git, ECAssistant.Core.Tools.Code, ECAssistant.Core.Tools.Reader, ECAssistant.Core.Analysis, ECAssistant.Core.Services, ECAssistant.Core.Services.Http, ECAssistant.Core.Interfaces, ECAssistant.Core.Transport, ECAssistant.Core.UI

### Class: TestScenario
> Scripted inputs for approval prompts (e.g., "y" to approve).
Cross-package deps: ECAssistant.Core.Orchestration

### Class: TestSessionOutput
> Test implementation of ISessionOutput.
Implements: ISessionOutput
Constructor:
  - TestSessionOutput(GuiTestHarness gui)
Cross-package deps: ECAssistant.Core.Session

### Class: UserExperienceHarness
> v14.19: user-experience E2E harness — simulates EXACTLY what a user would
Implements: IOutputListener
Cross-package deps: ECAssistant.Core.Config, ECAssistant.Core.Engine, ECAssistant.Core.Session
