# ECAssistantTestSupport — Architecture (2026-09-23)

**Summary:** Standalone test-harness library for ECAssistant.Core — run agent scenarios end-to-end without a real LLM. Published to NuGet as `ECAssistant.TestSupport`; consumed by test suites only, never by runtime code.

## Key Facts
- Single assembly: `ECAssistant.TestSupport` (net8.0), namespace `ECAssistant.TestSupport`
- Depends on `ECAssistant.Core` (PackageReference by default; sibling ProjectReference when `EcaUseProjectRefs=true`)
- `ECAssistant.Core` grants `InternalsVisibleTo("ECAssistant.TestSupport")` — the harness may inspect internals
- Consumers: ECAssistantCore `Tests/` and ECAssistantConsole `Tests/` (dev-only). Never referenced by any packable runtime project
- Version 12.9.12; Core pin 12.9.13 (live on nuget)
- Publish: tag `test-support-v*` → `.github/workflows/publish-testsupport.yml` (GitHub Packages + nuget.org, trusted publishing)

## Classes (2026-09-23 — +UserExperienceHarness v14.19 user-experience E2E; factory runs SessionBuilder tool registration + CreateTiered params)
| Class | Purpose |
|---|---|
| `TestRunner` | Executes `TestScenario`s end-to-end, collects `TestResult`s |
| `TestScenario` | Declarative scenario definition |
| `TestResult` | Outcome model (passed/failed, transcript) |
| `MockEngine` | Fake inference engine — no model, no GPU, no server; ctors take optional `config` (tier profiles flow through the real ctor) |
| `EcaTests` | Catalog of ready-made scenarios (tools, memory, sessions) |
| `GuiTestHarness` | TUI test harness support |
| `TestContext`, `TestSessionOutput` | Run context + captured output helpers |
| `InferenceEngineNoop`, `KvCacheNoop` | Public no-op doubles (extracted from MockEngine 2026-09-23) |
| `ProbeTestTool` | Harmless typed-schema test tool, shared across suites |
| `HarnessE2ESessionFactory` | Real-server harness e2e: client registration → X-Client-Id → fully wired AgentSession (tier-tuned inference params via `CreateTiered`; optional `prepareWorkingDir` pins `AgentSettings.WorkingDirectory` to the isolated session dir before tool construction) |
| `UserExperienceHarness` (v14.19) | User-experience E2E: drives `AgentSession.Prompt` (the real user entry point), captures every visible output line via `IOutputListener`, answers approval prompts like a user, runs **Verbose**, and registers the **real product tool set** (`SessionBuilder.RegisterBuiltInToolsAsync`). Journeys assert on the visible transcript, not engine internals. Gate: env `ECA_E2E_SERVER` (+ optional `ECA_E2E_MODEL`) |

## Dependency Flow
```
ECAssistantTestSupport ──▶ ECAssistantCore (package or project ref)
ECAssistantCore/Tests ──▶ TestSupport
ECAssistantConsole/Tests ──▶ TestSupport (dev-only)
```
Strict rule: no runtime project (Core/TUI/Console/LLM) may reference TestSupport.

## Release Checklist (before tagging `test-support-v*`)
1. ARCHITECTURE.md updated
2. README/docs touched if user-facing
3. `dotnet build` (Release) + consumers' tests pass locally
4. Present changes to Emre, wait for explicit ship-it — never auto-tag
