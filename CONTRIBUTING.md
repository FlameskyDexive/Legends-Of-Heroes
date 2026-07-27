# Contributing to Legends-Of-Heroes

First of all — **thank you** for taking the time to contribute! 🎉

This document explains how to get involved, whether you are reporting a bug, suggesting a feature, improving documentation, or submitting code. The project is built on the [ET framework](https://github.com/egametang/ET) and targets **Unity 2022.3.62f3** with the **.NET 10 SDK**, so most contribution paths assume that environment.

> Languages: [English](./CONTRIBUTING.md) · 中文版沟通可直接在 Issue / PR 中使用。

---

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Ways to Contribute](#ways-to-contribute)
- [Before You Start](#before-you-start)
- [Development Environment](#development-environment)
- [Workflow](#workflow)
- [Coding Standards](#coding-standards)
- [Commit Message Guidelines](#commit-message-guidelines)
- [Pull Request Checklist](#pull-request-checklist)
- [Reporting Bugs & Requesting Features](#reporting-bugs--requesting-features)
- [License](#license)

---

## Code of Conduct

Be respectful and constructive. Harassment, personal attacks, and discriminatory language will not be tolerated in any project channel (issues, PRs, discussions, etc.). By participating you agree to maintain a friendly, professional, and welcoming tone.

---

## Ways to Contribute

There are many ways to help, and not all of them require writing code:

- 🐛 **Report bugs** — open an issue with clear reproduction steps.
- 💡 **Suggest features** — describe the use case and the problem it solves.
- 📝 **Improve docs** — fix typos, clarify steps, translate, or write tutorials.
- 🎨 **Contribute assets** — icons, demo art, sound, etc. (must be properly licensed).
- 🔧 **Submit code** — pick an issue labeled `good first issue` or `help wanted`, or work on something from the project TODO list in the README.
- 🌍 **Translate** — help keep the bilingual README (`README.md` / `README.zh-CN.md`) in sync.

---

## Before You Start

1. **Search existing issues and PRs** to avoid duplicates.
2. For non-trivial changes (new systems, breaking changes, large refactors), **open an issue first** to discuss the design. This avoids wasted effort when the direction doesn't align with the project's roadmap.
3. Small fixes (typos, obvious bugs, minor tweaks) can go straight to a PR — no need to open an issue.

---

## Development Environment

- **Unity**: 2022.3.62f3
- **IDE**: Visual Studio 2022 or Rider 2023
- **.NET SDK**: 10.0
- **OS**: Windows is the primary development platform (some tools may not work on macOS/Linux).
- **Git**: latest stable version.

Make sure you can open the project in Unity and compile it without errors before making changes. If you cannot, please open an issue describing your environment.

---

## Workflow

We follow the standard fork-and-pull-request model.

1. **Fork** the repository to your own GitHub account.
2. **Clone** your fork locally:
   ```bash
   git clone https://github.com/<your-username>/Legends-Of-Heroes.git
   cd Legends-Of-Heroes
   ```
3. **Add an upstream remote** to keep up with the latest changes:
   ```bash
   git remote add upstream https://github.com/FlameskyDexive/Legends-Of-Heroes.git
   ```
4. **Create a feature branch** from `master`. Do not work directly on `master`:
   ```bash
   git checkout -b feat/short-description
   ```
   Branch naming suggestions:
   - `feat/...` — new features
   - `fix/...` — bug fixes
   - `docs/...` — documentation
   - `refactor/...` — code refactoring
   - `chore/...` — tooling, build, dependencies
5. **Make your changes**, committing in logical, focused units.
6. **Rebase on upstream** before opening the PR to keep history clean:
   ```bash
   git fetch upstream
   git rebase upstream/master
   ```
7. **Push** to your fork:
   ```bash
   git push origin feat/short-description
   ```
8. **Open a Pull Request** against `master` and fill in the PR template.

---

## Coding Standards

- **C#**: Follow the existing style of the file you are editing. The project currently targets **C# 9.0** compatibility — do not use language features from newer versions.
- **Naming**: use `PascalCase` for classes, methods, and public members; `camelCase` for local variables and parameters; prefix private instance fields with `_` where the surrounding code does so.
- **ET conventions**: respect ET's ECS/Actor architecture, namespace layout, and assembly split (`Model` / `ModelView` / `Hotfix` / `HotfixView`). When in doubt, follow the pattern of neighboring code.
- **No commented-out dead code** — delete it.
- **No `Debug.Log` spam** — use the project's logging conventions.
- **Keep diffs focused** — one PR should address one concern. Mix unrelated changes across separate PRs.

---

## Commit Message Guidelines

Use the imperative mood ("Add feature", not "Added feature" or "Adds feature").

Recommended format:

```
<type>: <short summary in lowercase>

<optional body explaining why and what>

<optional footer referencing issues>
```

Common `<type>` values: `feat`, `fix`, `docs`, `refactor`, `chore`, `test`, `perf`.

Examples:

```
feat: add buff stacking rule for poison
fix: prevent null reference on player disconnect
docs: clarify Book run steps in README
```

---

## Pull Request Checklist

Before requesting review, make sure:

- [ ] The PR targets the `master` branch.
- [ ] The branch is rebased on the latest `master`.
- [ ] The PR title and description clearly explain **what** changed and **why**.
- [ ] The change compiles in Unity without errors.
- [ ] You have tested the affected functionality manually (or added tests where applicable).
- [ ] No unrelated formatting churn is included.
- [ ] New dependencies (if any) are clearly justified and compatible with the project's license.
- [ ] Documentation (`README.md`, `README.zh-CN.md`, `Book`, etc.) is updated if behavior changed.

A maintainer will review your PR as time permits. Minor changes may be requested — please address feedback by pushing new commits (do not force-push during review unless asked).

---

## Reporting Bugs & Requesting Features

When opening an issue, please include:

**For bugs:**
- A clear title summarizing the problem.
- Steps to reproduce.
- Expected behavior vs. actual behavior.
- Environment info: Unity version, OS, .NET SDK version, branch / commit.
- Relevant logs, screenshots, or videos.

**For feature requests:**
- The problem you are trying to solve.
- The proposed solution and any alternatives you have considered.
- Whether you are willing to implement it yourself.

---

## License

By contributing, you agree that your contributions will be licensed under the project's [MIT License](./LICENSE).

---

Happy coding, and thanks again for making Legends-Of-Heroes better! 🚀
