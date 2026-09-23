# ECAssistantTestSupport

Test-harness library for [ECAssistant.Core](https://github.com/SideDevEC/ECAssistantCore) — run agent scenarios without a real LLM, or drive full user-experience E2E against a real server + real local model.

- **MockEngine** — fake inference engine with queued decisions; no model, no GPU, no server
- **UserExperienceHarness** — drives `AgentSession.Prompt` exactly like the TUI does: captures every visible output line, answers approval prompts, runs the real product tool set (`SessionBuilder`), and lets tests assert on the *transcript* the user would see
- **HarnessE2ESessionFactory** — fully wired sessions against a live ECAssistantLLM server (gated on `ECA_E2E_SERVER`; silently skipped when unset — CI-safe). Optional `prepareWorkingDir` callback isolates tool file access from the test runner's directory
- **ProbeTestTool, no-op doubles** — typed-schema test tool plus `InferenceEngineNoop`/`KvCacheNoop`

Test-suites only — never referenced by runtime/packable projects.

Part of [ECAssistant](https://github.com/SideDevEC/ECAssistant).
