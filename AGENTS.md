# Agent rules for this Unity project

## Project context
- Run commands from the repository root (the Unity project root). Use the Editor version in `ProjectSettings/ProjectVersion.txt`, currently `6000.6.5f1`; do not upgrade it implicitly.
- This is a URP/XR project. Treat `Packages/manifest.json` and XR settings under `ProjectSettings/` and `Assets/XR/` as configuration, not cleanup targets.
- Imported XR samples are older than the installed packages: XR Hands samples are `1.8.1` versus package `1.9.0`, and XR Interaction Toolkit samples are `3.5.1` versus package `3.6.1`. Do not assume their APIs match or reimport samples without checking local changes.

## Live Editor changes
- Before changing scenes, prefabs, or serialized assets, run `unity status --project-path . --format json` and confirm this project's Editor is ready. Pipeline is declared in the manifest, but that does not prove an Editor is connected.
- Discover available live commands with `unity command --project-path .`; use `unity command <name> --project-path .` for Editor changes and save through the Editor. Do not hand-edit `.unity`, `.prefab`, or `.asset` YAML while a live Editor is reachable.
- If connection fails, check Safe Mode with `unity pipeline list` and possible sandbox visibility restrictions before concluding the Editor is closed. Direct scene-file edits are a last resort: explain why they are necessary before proceeding.
- This repo does not version its skills. The Unity agent skills live in `.agents/skills/` (with `skills/` and `.claude/skills/` as symlinks into it) and are gitignored: restore them with `unity skill install <client> --local`. Load the `unity-cli` skill for CLI work and `unity-pipeline` for live-Editor automation before using their workflows.

## Verification and builds
- After C# changes, run `unity recompile --project-path . --format json`, then applicable EditMode/PlayMode tests before declaring completion. Report compile failures, missing tests, or an unavailable Editor explicitly; none counts as passing.
- No project-owned test assemblies or build/test scripts are currently present. Use the connected Editor's discovered test commands; for batch tests with this project closed in the Editor, use `unity test . --mode EditMode --output Logs/editmode-results.xml` and the equivalent `--mode PlayMode --output Logs/playmode-results.xml`. Add `--filter <pattern>` for a focused run.
- Discover build profiles with `unity build . --list-profiles`; use an existing profile via `unity build . --profile "<profile.asset>" --output-path "Builds/<artifact>"`. No build profile assets are currently present: do not invent a profile or target; confirm the intended platform before creating one.
- `ProjectSettings/EditorBuildSettings.asset` currently enables only `Assets/Scenes/SampleScene.unity`; a requested build is not permission to include every sample scene. Report the actual artifact path and build result.
- Documentation-only changes need structural/diff checks, not an Editor compile or player build.

## Repository hygiene
- Keep `ProjectSettings/`, `Packages/manifest.json`, `Packages/packages-lock.json`, this file, and any new build/test scripts versioned. Preserve asset `.meta` files and GUIDs when moving or renaming assets.
- Do not commit generated `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `Builds/`, `.csproj`, or `.slnx` files; the root `.gitignore` defines the exclusions. Do not edit generated project files to configure compilation.
- Preserve unrelated working-tree changes. Do not commit automatically: show the proposed diff first and obtain explicit permission to commit.
