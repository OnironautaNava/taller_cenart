RF-03 (#4): door between Zone A and Zone B that needs the key in the lock AND the numeric code (CodeLock), teleport/snap turn gating, welcome menu, hint panel and controller menu toggle in Fase0.
Branch: clase3. Delivery: ask-on-risk; no automatic commits (AGENTS.md). Tests: EditMode via `unity command run_tests --mode editor`.

## Specs
- S1 (#13): "`CodeLock` reconoce una secuencia de botones". A partial or incorrect sequence cannot unlock; the complete ordered sequence fires the solved event with visible feedback; after a wrong entry the documented retry permits success; input after completion does not corrupt progression; buttons/poke work with simulated controllers; Inspector configuration is understandable.
- S2 (#14): "Teleport y snap turn; segunda zona desbloqueable". Before the prerequisite the second zone is unavailable through supported navigation; solving unlocks the route; teleport between zones and snap turn work in simulator; no forced camera translation; device comfort validation pending.
- S3 (#15): "menú de bienvenida, panel de pistas ... menú alternable desde el control". Welcome visible at entry; hints open/read/close; controller toggle alternates the menu; player returns to interacting without losing input; hints follow puzzle progression; third-puzzle clues stay placeholder.
- S4 (user, 2026-10-18): "la puerta entre la sección 1 y 2 necesite instertar la llave en la cerradura y resolver el código numérico. Así podríamos ejemplificar el caso en el que varias condiciones diferentes se tienen que cumlir para un acertijo."

## Tasks
- T1 S1,S4 inline — CodeLock component + EditMode tests — done (RED: CS0246; GREEN 8/8) — uncommitted
- T2 S3 inline — MenuToggle component + EditMode tests — done (RED: CS0246; GREEN 4/4; both-controller binding changed with its test, no separate RED) — uncommitted
- T3 S2,S4 inline — Fase0: grabbable key, lock socket, keypad, door, Zone B blocker, Puzzle_Door (Required Count 2) + scene tests — done (RED 5 failing; GREEN 34/34; Play Mode probe) — uncommitted; manual simulator/Quest pending
- T4 S3 inline — Fase0: welcome panel, hint menu wired to progression, controller toggle + scene tests — done (GREEN; M toggles in simulator with Editor focus) — uncommitted; manual pending
- T5 S1-S4 inline — docs/Fase0.md + Play Mode probes — done — uncommitted

## Log
- L1 (2026-10-18): "REsuelve el issue 4 y sus sub issues. Puedes preguntarme si te surgen dudas importantes"
- L2: Decisions (defaults, reversible): code "1994" (placeholder clue on a plaque at the entrance statue); wrong code clears input and fires OnWrong; Zone B gated by an invisible solid collider that the teleport ray cannot target, disabled on unlock; hints are child objects switched with GameObject.SetActive from OnSolved events; menu toggled by the left controller menu button (M in the simulator). Locomotion: the scene already disables smooth motion (teleport + snap turn only). Challenge_02 pedestal stays for RF-04 since the code now lives on the door.
- L3: Builder: scratchpad BuildRF03.cs via run_script (idempotent; rebuilds Doorway_AB children, ZoneB_Lock, UI). Real scene diff +8723/-2 (patience), no objects removed. Interaction layer 2 = Key. Clips CodeOk/DoorOpen/CodeWrong set Force To Mono. TMP LiberationSans SDF dynamic atlas dirtied by new glyphs.
- L4: Play Mode probe: rays into Zone B hit ZoneB_Lock before; gem rejected by lock; key counts 1; wrong code clears ("____"), door shut; "1994" opens door (rot 95), blocker off, hints switch; ray through door hits Zone B floor; Near-Far Interactor selects keypad keys; injected menuButton on each simulated controller alternates menu open/closed.
- L5: Simulator M applies to the manipulated controller (right by default), so MenuToggle binds both controllers' {MenuButton}. simulate_key while the Editor loses focus leaves the action stuck Performed (harness artifact; injected device-state test alternates reliably).
