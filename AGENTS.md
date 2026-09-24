# AGENTS.md — ECAssistantTestSupport (AI-consumable)

Compact orientation for AI agents working in this repo. Humans: read README.md.

## Identity
- **Package:** `ECAssistant.TestSupport` v15.0.0 · net8.0 · namespace `ECAssistant.TestSupport`
- **Purpose:** Test harness for ECAssistant.Core — run agent scenarios without a real LLM, or drive full E2E against a real server.
- **Hard rule:** NEVER referenced by runtime/packable projects (Core/TUI/Console/LLM main csprojs). Test suites only.

## Types (complete list — 13 types, no others exist)
| Type | Kind | Use when |
|---|---|---|
| `TestRunner` | class | Executing `TestScenario`s end-to-end → `TestResult` |
| `TestScenario` | class | Declarative scenario definition (prompt + expected behavior) |
| `TestResult` | class | Outcome model (passed/failed + transcript) |
| `EcaTests` | class | Catalog of ready-made scenarios (tools, memory, sessions) |
| `MockEngine : AgentEngine` | class | Fake inference engine — queued decisions, no model/GPU/server. Ctors take optional `config`; tier profiles flow through the real ctor |
| `UserExperienceHarness` | class | User-experience E2E: drives `AgentSession.Prompt`, captures every visible output line via `IOutputListener`, answers approval prompts, registers the REAL product tool set (`SessionBuilder.RegisterBuiltInToolsAsync`). Assert on the transcript the user would see. Gate: `ECA_E2E_SERVER` (+ optional `ECA_E2E_MODEL`) |
| `HarnessE2ESessionFactory` | class | Real-server wired sessions: client registration → `X-Client-Id` → `AgentSession` with tier-tuned params (`CreateTiered`). Optional `prepareWorkingDir` pins `AgentSettings.WorkingDirectory` to an isolated dir BEFORE tool construction. Gate: `ECA_E2E_SERVER`; silently skipped when unset (CI-safe). Remote tier: `ECA_E2E_REMOTE_ENDPOINT` + `ECA_E2E_REMOTE_MODEL` + `ECA_E2E_REMOTE_KEYFILE` |
| `GuiTestHarness : GuiBase` | class | TUI test support |
| `TestContext` | class | Run context |
| `TestSessionOutput` | class | Captured output helper |
| `InferenceEngineNoop : IInferenceEngine` | class | Public no-op double |
| `KvCacheNoop : IKvCacheController` | class | Public no-op double |
| `ProbeTestTool` | class | Harmless typed-schema test tool, shared across suites |

## Dependency rules
```
ECAssistantTestSupport ──▶ ECAssistant.Core (PackageReference default; sibling ProjectReference when EcaUseProjectRefs=true)
ECAssistantCore/Tests ──▶ this package
ECAssistantConsole/Tests ──▶ this package (dev-only)
```
- Core grants `InternalsVisibleTo("ECAssistant.TestSupport")` — harness may inspect internals.

## E2E tier conventions (remote = large model, local = small model)
- LOCAL SMALL: `ECA_E2E_SERVER=http://localhost:<port>` (+ `ECA_E2E_MODEL`, default `qwen35-4b`)
- REMOTE LARGE: `ECA_E2E_REMOTE_ENDPOINT` (e.g. `https://ollama.com/v1`) + `ECA_E2E_REMOTE_MODEL` (e.g. `glm-5.3-flash:cloud`) + `ECA_E2E_REMOTE_KEYFILE`
- Debug: `ECA_JOURNEY_DEBUG=1` dumps per-turn transcripts + context snapshots
- ⚠ Server binds `localhost` → IPv6; use `http://localhost:PORT`, NOT `127.0.0.1`

## Build & test
```bash
# local dev (sibling project refs — no NuGet restore needed)
export EcaUseProjectRefs=true
dotnet build ../ECAssistantCore/Tests/ECAssistant.Core.Tests.csproj

# CI mode (package refs from feeds)
unset EcaUseProjectRefs
```

## Release (tag `test-support-v*`)
1. ARCHITECTURE.md updated 2. README touched if user-facing 3. build + consumers' tests green 4. Emre says "ship it" — NEVER auto-tag.
Version is LOCKSTEP with all ECAssistant packages (all = 15.0.0).
