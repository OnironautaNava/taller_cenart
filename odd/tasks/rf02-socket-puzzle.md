RF-02 (#3): reusable PuzzleCondition/Door/Reveal and the first socket puzzle in Fase0, plus the early Android smoke build.
Branch: dev (user works on dev). Delivery: ask-on-risk. Tests: EditMode via `unity command run_tests --mode editor`.

## Specs
- S1 (#10): "`PuzzleCondition` espera N eventos y dispara `OnSolved`; `Door` / `Reveal` abre, mueve o activa algo". Fewer than N inputs do not emit OnSolved; reaching N triggers it; invalid configuration does not silently complete; missing references are diagnosable; tests pin event count and target state.
- S2 (#11): "colocar una pieza en un socket que active `Door` o `Reveal`". Grab → release → throw in simulator; only the intended piece triggers progression; remove/reinsert does not corrupt the chain; unreachable piece recovers; Inspector wiring documented.
- S3 (#12): "Realizar la compilación de prueba el día 2 y centralizar la instalación de APK en la estación del instructor." APK build with the intended scene; device install only with authorized device/session, else pending.

## Tasks
- T1 S1 inline — components + EditMode tests — done (RED: CS0246 missing types; GREEN 14/14) — uncommitted
- T2 S2 inline — Fase0 socket puzzle wiring + scene test + docs — done (15/15; Play Mode probes) — uncommitted; manual simulator grab/throw pending
- T3 S3 inline — Android profile + smoke build — blocked on target confirmation

## Log
- L1 (2026-10-17): "Resuelve el issue 3 y sus sub issues en la escena @Assets/Taller_CENART/Scenes/Fase0.unity"
- L2: Built via live Editor run_script (scratchpad builder). Play Mode probes: decoy at socket → not selected, count 0; gem at socket → solved, lid open (bounds behind chest), key active; socket off + gem dropped to y=-5 → Respawn to shelf; reinsert → still solved, count 1. No console errors in Play Mode. Scene-test RED not observed separately (test written before build but compiled after).
- L3: T3 needs target confirmation: no build profiles exist; Android module installed; Oculus/OpenXR config present. Commits await user permission (AGENTS.md).
