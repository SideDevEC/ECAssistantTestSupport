# ECAssistantTestSupport — Architecture (2026-09-21)

**Summary:** Standalone test-harness library for ECAssistant.Core — run agent scenarios end-to-end without a real LLM. Published to NuGet as `ECAssistant.TestSupport`; consumed by test suites only, never by runtime code.

## Key Facts
- Single assembly: `ECAssistant.TestSupport` (net8.0), namespace `ECAssistant.TestSupport`
- Depends on `ECAssistant.Core` (PackageReference by default; sibling ProjectReference when `EcaUseProjectRefs=true`)
- `ECAssistant.Core` grants `InternalsVisibleTo("ECAssistant.TestSupport")` — the harness may inspect internals
- Consumers: ECAssistantCore `Tests/` and ECAssistantConsole `Tests/` (dev-only). Never referenced by any packable runtime project
- Publish: tag `test-support-v*` → `.github/workflows/publish-testsupport.yml` (GitHub Packages + nuget.org, trusted publishing)

## Classes
| Class | Purpose |
|---|---|
| `TestRunner` | Executes `TestScenario`s end-to-end, collects `TestResult`s |
| `TestScenario` | Declarative scenario definition |
| `TestResult` | Outcome model (passed/failed, transcript) |
| `MockEngine` | Fake inference engine — no model, no GPU, no server |
| `EcaTests` | Catalog of ready-made scenarios (tools, memory, sessions) |
| `EGuiTestHarness` | TUI test harness support |
| `TestContext`, `TestSessionOutput` | Run context + captured output helpers |

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
