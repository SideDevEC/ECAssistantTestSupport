# ECAssistant.TestSupport

> **Test harness for [ECAssistant.Core](https://github.com/SideDevEC/ECAssistantCore).** Run agent scenarios without a real LLM — or drive full user-experience E2E against a real server + real model.

[![NuGet](https://img.shields.io/nuget/v/ECAssistant.TestSupport)](https://www.nuget.org/packages/ECAssistant.TestSupport)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Part of [ECAssistant](https://github.com/SideDevEC/ECAssistant). Test-suites only — never referenced by runtime/packable projects.

## Three ways to test, one package

### 1. MockEngine — pure unit, zero infrastructure

Fake inference engine with queued decisions. No model, no GPU, no server. The engine runs the real orchestrator, real tools, real permission flow — only the *model* is fake.

```csharp
using ECAssistant.TestSupport;

// Queue the decisions the "model" will make: call the shell tool, then answer.
var engine = new MockEngine(
    cycleResponses: new[]
    {
        MockEngine.ToolCall("EShellAgent", new { command = "echo hello" }),
        MockEngine.Answer("done — echoed hello")
    },
    config: new AppConfig());          // tier profiles flow through the real ctor

// TestRunner executes the scenario through the real session pipeline
var runner  = new TestRunner(engine, workingDir: tempDir);
var result  = await runner.RunAsync(TestScenario.Create("shell echo"));
Assert.True(result.Passed);
```

### 2. UserExperienceHarness — assert on what the *user sees*

Drives `AgentSession.Prompt` exactly like the TUI does: captures every visible output line via `IOutputListener`, answers approval prompts like a user would, registers the **real product tool set** (`SessionBuilder.RegisterBuiltInToolsAsync`). Journeys assert on the **visible transcript**, not engine internals.

```csharp
var harness = new UserExperienceHarness(serverUrl, config);
var (session, dir) = await harness.CreateAsync();

await session.Prompt("Create a file named notes.md containing 'journey marker 42'");

// Assert on the transcript the user actually saw
var transcript = harness.VisibleTranscript;
Assert.Contains("notes.md", transcript);
Assert.Equal("journey marker 42", File.ReadAllText(Path.Combine(dir, "notes.md")));
```

### 3. HarnessE2ESessionFactory — real server, real model, both tiers

Fully wired sessions against a live [ECAssistantLLM](https://github.com/SideDevEC/ECAssistantLLM) server. Gated on environment variables — **silently skipped when unset**, so CI without secrets stays green.

```bash
# LOCAL SMALL tier (local model, e.g. qwen35-4b — model tier "small")
export ECA_E2E_SERVER=http://localhost:48321
export ECA_E2E_MODEL=qwen35-4b          # optional, this is the default

# REMOTE LARGE tier (hosted large model — model tier "large")
export ECA_E2E_REMOTE_ENDPOINT=https://ollama.com/v1
export ECA_E2E_REMOTE_MODEL=glm-5.3-flash:cloud
export ECA_E2E_REMOTE_KEYFILE=~/.secrets/ollama.key

export EcaUseProjectRefs=true           # local dev: build against sibling sources
dotnet test --filter "FullyQualifiedName~JourneySuiteE2E"
```

Optional knobs: `ECA_JOURNEY_DEBUG=1` dumps per-turn transcripts + context snapshots; `prepareWorkingDir` isolates tool file access from the runner's directory.

## Component map

| Component | What it does |
|---|---|
| `TestRunner` / `TestScenario` / `TestResult` | Declarative scenario execution + outcome model |
| `MockEngine` | Fake inference engine — queued decisions through the real ctor (tier profiles included) |
| `UserExperienceHarness` | User-experience E2E: real prompt entry point, real tools, visible-transcript assertions |
| `HarnessE2ESessionFactory` | Real-server harness E2E: client registration → `X-Client-Id` → fully wired `AgentSession` with tier-tuned inference params |
| `EcaTests` | Catalog of ready-made scenarios (tools, memory, sessions) |
| `GuiTestHarness` | TUI test support |
| `InferenceEngineNoop`, `KvCacheNoop` | Public no-op doubles |
| `ProbeTestTool` | Harmless typed-schema test tool, shared across suites |

## Rules of the house

- **Never** reference this package from runtime/packable projects (Core/TUI/Console/LLM main csprojs). Test suites only.
- Core grants `InternalsVisibleTo("ECAssistant.TestSupport")` — the harness may inspect internals; don't leak that into product code.
- Version is **lockstep** with all ECAssistant packages (unified versioning). Publish via tag `test-support-v*` → CI publishes to GitHub Packages + nuget.org (trusted publishing).
- AI agents: see [AGENTS.md](AGENTS.md) for the compact machine-readable orientation.

## The ecosystem

| Repo | What it is |
|---|---|
| [ECAssistant](https://github.com/SideDevEC/ECAssistant) | Start here — overview & docs |
| [ECAssistantCore](https://github.com/SideDevEC/ECAssistantCore) | The embeddable agent library |
| [ECAssistantLLM](https://github.com/SideDevEC/ECAssistantLLM) | OpenAI-compatible local LLM server |
| [ECAssistantTUI](https://github.com/SideDevEC/ECAssistantTUI) | Terminal UI library |
| [ECAssistantConsole](https://github.com/SideDevEC/ECAssistantConsole) | Reference host / end-user CLI |

## License

[MIT](LICENSE) — © 2026 SideDevEC
