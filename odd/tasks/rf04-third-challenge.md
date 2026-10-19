RF-04 (#5, #16-#21): third challenge (climb, throw at a target that drops a bridge, pull a lever that opens the final door to the exit), feedback, full route and optional Timer in Fase0.
Branch: clase4. Delivery: ask-on-risk; no automatic commits (AGENTS.md). Tests: EditMode via `unity command run_tests --mode editor`.

## Specs
- S1 (#17, user 2026-10-19): "el último reto consta de escalar a una plataforma alta y lanzar una pelota para golpear un blanco este objetivo tira una tabla o algo similar a un puente por el cual el jugador pueda llegar al otro lado. En ese lado habrá una palanca y al jalar se abre la puerta final que lo lleva a la salida. Realmente los unicos activadores son el atinar en el blanco y jalar la palanca. La escalera es estática y está en la zona 2"
- S2 (#18): primary action differs from socket and button sequence; success emits the chain event; a miss does not solve and a retry can succeed; Inspector-reproducible example.
- S3 (#19): "modelos temáticos, audio espacial y retroalimentación al resolver". Each puzzle has an observable solved consequence; the room communicates completion; hints match the actual sequence.
- S4 (#20): single progression entry → socket → door (key + code) → target → lever → exit; a later station cannot be solved early; misses/menu use never dead-end the route. Independent playtest needs a person (pending).
- S5 (#21): "`Timer` ofrece cuenta regresiva con `OnTimeUp` y su uso en el cuarto es opcional." Tests cover expiry and start/restart; the room stays completable with Timer unused.
- S6 (#16): theme approval needs the instructor; not inferred (pending).

## Tasks
- T1 S1,S2 inline — TargetHit component + EditMode tests — done (RED: CS0246; GREEN 4/4 with T2) — uncommitted
- T2 S5 inline — Timer component + tests + TimerExample prefab — done (RED: CS0246; GREEN) — uncommitted
- T3 S1-S4 inline — Fase0 Zone B: platforms, ladder, balls, target, bridge, lever, final door, exit, feedback, hints + scene tests — done (scene tests written before the build but not run RED; GREEN 46/46; Play Mode probe) — uncommitted; manual simulator/Quest pending
- T4 S1-S6 inline — docs/Fase0.md, PRD mechanic decision, Play Mode probe — done — uncommitted; #16 theme and #20 independent playtest pending (need a person)

## Log
- L1 (2026-10-19): "resuelve el issue 5 y sus sub issues. Considera que el último reto consta de escalar a una plataforma alta y lanzar una pelota para golpear un blanco este objetivo tira una tabla o algo similar a un puente por el cual el jugador pueda llegar al otro lado. En ese lado habrá una palanca y al jalar se abre la puerta final que lo lleva a la salida. Realmente los unicos activadores son el atinar en el blanco y jalar la palanca. La escalera es estática y está en la zona 2"
- L2: Decisions (defaults, reversible): platforms x −4..−1 / 1..4, z 12..16, y 2 with a 2 m gap; bridge = 3 Synty planks hinged at x 0.95 falling west (Door, rot z 90); target on the planks below 4 m; lever = XRSimpleInteractable select + Door on the stick (no physical hinge); final door in the north wall under the bridge, exit room z 16..20; EastLock/ExitLock solid blockers like ZoneB_Lock; lever and bridge teleport area start disabled and are enabled by Puzzle_Target. Statues moved to z 9; pedestals Challenge_02/03 moved onto the platforms.
- L3: Builder: scratchpad BuildRF04.cs / BuildTimer.cs via run_script (idempotent). Synty tennis ball has no collider: SphereCollider copied from Decoy_Ball. Balls inside ZoneB_Lock were depenetrated onto its top (y 4.06): they now start inactive and Puzzle_Door activates them. Prototype yellow materials replaced by the room's wall/floor materials. Editor emptied ProjectSettings preloadedAssets again: reverted (unrelated). Scene diff (patience) +5315/−93; only Wall_North_1/2 removed.
- L4: Play Mode probe: Zone A ray to target hits Divider_Door; miss leaves Puzzle_Target unsolved and the ball returns to (−2.75, 2.95, 14.6); hit solves, bridge z 90, lever + bridge area enabled, east platform ray hits Top; Near-Far select on the lever opens FinalDoor (y 95), ExitPanel active, exit floor reachable.
