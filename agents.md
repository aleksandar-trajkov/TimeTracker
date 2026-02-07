# AI Agent Instructions (Entry Point)

This repository uses scoped and purpose-based AI instructions.

## Repository Structure
- src/backend → .NET backend (including tests)
- src/frontend → React frontend (including tests)

## Instruction Routing

### By Scope
- Backend work → follow `src/backend/agents.md`
- Frontend work → follow `src/frontend/agents.md`

### By Purpose
Use the matching file under `/ai`:

- Feature implementation / bug fixing → `ai/coding.md`
- Code or PR review → `ai/review.md`
- Security review → `ai/security.md`

If both apply, scope-specific rules override global rules.

## Global Rules
- Respect backend/frontend boundaries
- Do not change API contracts without updating both sides
- Prefer minimal, focused changes
- Ask before introducing new dependencies