<!--
  Sync Impact Report
  ===================
  Version Change: 1.0.0 → 1.1.0
  
  Modified Principles:
  - II. Testing Standards (domain-specific integration coverage)
  - IV. Performance Requirements (desktop latency and resource thresholds)
  - Development Workflow (constitution checks and zh-TW artifacts clarified)
  
  Added Sections:
  - None
  
  Removed Sections:
  - None
  
  Templates Status:
  - plan-template.md: ✅ Updated (constitution gates and language reminders)
  - spec-template.md: ✅ Updated (zh-TW plus performance acceptance prompts)
  - tasks-template.md: ✅ Updated (tests mandated per constitution)
  - commands/: ⚠ Not present (no command templates found to update)
  
  Follow-up TODOs: None
-->

# Rivet Constitution

## Core Principles

### I. Code Quality

All code contributed to this project MUST adhere to the following quality standards:

- **Readability**: Code MUST be self-documenting with meaningful variable, function, and class names. Comments are required ONLY when intent is non-obvious.
- **Maintainability**: Functions MUST follow single responsibility principle. Maximum cyclomatic complexity per function: 10.
- **Consistency**: All code MUST conform to the project's established linting rules and formatting standards. No exceptions without documented justification.
- **Error Handling**: All error paths MUST be explicitly handled. Silent failures are forbidden.
- **Code Review**: All code changes MUST pass peer review before merge. Reviewers MUST verify compliance with this constitution.

**Rationale**: High code quality reduces technical debt, improves onboarding speed for new contributors, and ensures long-term maintainability.

### II. Testing Standards

Testing is a NON-NEGOTIABLE requirement for this project:

- **Coverage Requirement**: New features MUST include tests. Minimum test coverage for new code: 80%.
- **Test Types**:
  - Unit tests: MUST cover all business logic and utility functions.
  - Integration tests: REQUIRED for hotkey invocation, clipboard access, translation pipeline, and external service interactions.
  - Contract tests: REQUIRED when introducing or modifying public interfaces.
- **Test Quality**: Tests MUST be deterministic, isolated, and fast. Flaky tests MUST be fixed or removed immediately.
- **Pre-merge Validation**: All tests MUST pass before code can be merged. No bypassing CI checks.

**Rationale**: Comprehensive testing ensures reliability, enables confident refactoring, and catches regressions early in the development cycle.

### III. User Experience Consistency

All user-facing features MUST maintain a consistent experience:

- **Design Language**: UI components MUST follow established design patterns and style guidelines.
- **Interaction Patterns**: Similar actions MUST behave consistently across the application.
- **Feedback**: All user actions MUST provide appropriate feedback (loading states, success/error messages).
- **Accessibility**: Features MUST meet WCAG 2.1 AA standards at minimum.
- **Responsiveness**: UI MUST remain responsive during operations; long-running tasks MUST show progress indicators.

**Rationale**: Consistent UX builds user trust, reduces learning curve, and improves overall product quality perception.

### IV. Performance Requirements

All features MUST meet performance standards:

- **Hotkey Response**: Hotkey invocation to menu display MUST be ≤300ms (p95) on supported Windows hosts.
- **Translation Cycle**: Clipboard-to-output latency MUST be ≤1500ms (p95) when Google Translation is reachable; degraded paths MUST still provide status feedback.
- **Memory Efficiency**: Memory usage after a translation cycle MUST remain under 150MB with no leaks in long-running use.
- **Resource Management**: All acquired resources (clipboard handles, translation clients, file handles) MUST be released explicitly.
- **Monitoring**: Performance-critical paths (hotkey handling, translation requests) MUST include instrumentation and logs sufficient to trace latency and failures without leaking sensitive clipboard content.

**Rationale**: Fast hotkey responsiveness and translation delivery are the product value. Guarding resource use and latency keeps the desktop experience smooth and predictable.

### V. Language Policy

Documentation and artifact language requirements:

- **Technical Artifacts in Traditional Chinese (zh-TW)**:
  - All feature specifications (`spec.md`) MUST be written in Traditional Chinese.
  - All implementation plans (`plan.md`) MUST be written in Traditional Chinese.
  - All task lists (`tasks.md`) MUST be written in Traditional Chinese.
  - All user-facing documentation MUST be written in Traditional Chinese.
- **Exceptions**:
  - This constitution document (English for governance clarity).
  - Code comments and inline documentation (English preferred for international compatibility).
  - Commit messages follow Git Commit Convention (see Principle VI).

**Rationale**: This project primarily serves Traditional Chinese users. Native language documentation improves clarity and reduces miscommunication.

### VI. Git Commit Convention

All commits MUST follow these conventions:

- **Standard**: Strictly follow the **Conventional Commits** specification.
- **Language**: All descriptions (subject and body) MUST be written in **Traditional Chinese (zh-TW)**.
- **Header Format**: `<type>(<Ticket-ID>): <description>`
  - **Type**: Use standard types: `feat`, `fix`, `docs`, `style`, `refactor`, `chore`, `ci`, `test`, `perf`.
  - **Scope**: The Ticket ID (e.g., `DG-2`, `TOOL-1`) MUST be placed inside parentheses as the scope.
  - **Description**: A concise summary of the change in Traditional Chinese.
- **Examples**:
  - `feat(DG-2): 實作全域熱鍵監聽功能`
  - `fix(TOOL-1): 修正快取失效導致的資料不一致問題`
  - `docs(DG-3): 更新 API 文件說明`
  - `refactor(DG-2): 重構事件處理邏輯提升可讀性`

**Rationale**: Consistent commit messages enable automated changelog generation, improve traceability to tickets, and maintain clear project history.

## Development Workflow

All development work MUST follow this workflow:

1. **Specification First**: Features MUST have an approved `spec.md` before implementation begins.
2. **Plan Before Code**: Implementation plans (`plan.md`) MUST be created and reviewed before coding; use speckit automation where available.
3. **Task Breakdown**: Work MUST be broken into trackable tasks (`tasks.md`) with clear dependencies and MUST reflect required tests to meet coverage goals.
4. **Branch Strategy**: Feature branches MUST follow naming convention: `<ticket-id>-<feature-name>` (e.g., `DG-2-hotkey-listener`).
5. **Review Gates**: All artifacts (specs, plans, code) MUST pass peer review, including Constitution Check items (language policy, test coverage, performance acceptance, commit/branch rules).
6. **Continuous Integration**: All commits MUST pass automated checks before merge.

## Governance

This constitution establishes the foundational rules for the Rivet project:

- **Supremacy**: This constitution supersedes all other practices. In case of conflict, this document prevails.
- **Compliance Verification**: All PRs and reviews MUST verify compliance with constitutional principles.
- **Amendment Process**:
  1. Propose amendment with rationale.
  2. Document impact on existing artifacts.
  3. Obtain maintainer approval.
  4. Update all affected templates and documentation.
- **Versioning Policy**: Constitution follows semantic versioning:
  - MAJOR: Backward-incompatible principle changes.
  - MINOR: New principles or significant expansions.
  - PATCH: Clarifications and wording improvements.
- **Exception Handling**: Deviations from constitutional principles MUST be documented with justification in the relevant PR/issue.

**Version**: 1.1.0 | **Ratified**: 2025-12-04 | **Last Amended**: 2025-12-10
