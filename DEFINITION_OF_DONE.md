# Definition of Done

Checklist applied to **every increment** before it is considered complete.
This document evolves with the project — new rules are added as new capabilities are introduced.

---

## Code

- [ ] `dotnet build` passes with zero warnings (`TreatWarningsAsErrors = true`)
- [ ] `dotnet test` passes — all tests green
- [ ] No secrets committed (connection strings, keys, passwords never in tracked files)
- [ ] No `appsettings.Development.json` committed

## Architecture

- [ ] ArchUnitNET tests pass — no layer dependency violations

## Data Access

- [ ] No `FindAsync` in repositories — use `FirstOrDefaultAsync` (bypasses Global Query Filters)

## Documentation & Artifacts

- [ ] ADR written **if** an architectural decision was made (`docs/adr/`)
- [ ] Runbook written **if** an operational concept was introduced (`docs/runbooks/`)
- [ ] Bruno collection updated **if** a new endpoint was added (`bruno/`)

## Git

- [ ] PR title follows Conventional Commits: `type(scope): description`
- [ ] PR merged via squash merge into `develop`
- [ ] `develop` pulled locally after merge
- [ ] Feature branch deleted after merge

## Versioning *(milestone phases)*

- [ ] Semver tag created after merge (`vX.Y.Z`)
- [ ] `CHANGELOG.md` updated via `git-cliff` on tag push
