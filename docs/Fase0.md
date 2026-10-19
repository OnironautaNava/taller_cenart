# Fase 0 — XR base and explorable room (RF-01)

Entry scene: `Assets/Taller_CENART/Scenes/Fase0.unity`. Open it and press Play; no headset is required.

## What is in the scene

| Object | Purpose |
|---|---|
| `XR Origin (XR Rig)` | The only rig (project copy in `Scenes/Fase0/`). Starts at `(0, 0, 1.5)` facing the gallery. |
| `XR Interaction Manager`, `EventSystem` | The only interaction manager and UI event system. |
| `Environment/Zone_A_Entrance` | Zone A, x −4..4, z 0..8, wood floor. Holds `Challenge_01`. |
| `Environment/Zone_B_Gallery` | Zone B, x −4..4, z 8..16, carpet floor, statues. Holds `Challenge_02` and `Challenge_03`. |
| `Environment/Walls` | Synty walls, windows and columns, plus a divider at z = 8 built around a Synty door wall. |
| `Environment/Props` | Synty wall sconces and two hanging lamps with point lights. |
| `Environment/Doorway_AB` | The door between zones and its two conditions: key lock and keypad (RF-03). |
| `UI` | Welcome panel at the entry and the hint menu toggled from the controller (RF-03). |

All visible art comes from `Assets/Synty/` (PolygonPrototype building kit, PolygonGeneric props and materials). The roof is open, the skybox is Synty `Skybox_01`, and ambient light is Trilight so walls facing away from the sun stay readable.

Every floor tile carries a `TeleportationArea` and a collider whose top sits at y = 0. Pedestals are about 0.9 m tall, so the `Slot` on top can be reached seated or standing.

## Reusable assets (adapt without code)

- `Assets/Taller_CENART/Prefabs/Floor_2x2_Teleport`: Synty `SM_Buildings_Floor_2x2_01P` with a `TeleportationArea` on the `Teleport` interaction layer only, so the teleport ray targets it and the Near-Far (grab) interactor ignores it. Pivot is the tile's min corner.
- `Assets/Taller_CENART/Prefabs/ChallengePedestal`: Synty `Plinth_02` raised to reach height, with a `Slot` child marking where a challenge mounts.
- Walls and props are Synty prefabs used directly.

To re-theme the room, change the material on a zone's floor tiles or walls (for example the `Generic_*` materials in `Assets/Synty/PolygonGeneric/Materials/`), or swap the statues and props for other Synty prefabs. Keep a collider under every walkable surface and keep the `TeleportationArea` on floors.

## First puzzle: socket (RF-02)

`Challenge_01` in Zone A. Take the gem from the east shelf, place it on the pedestal: the chest opens and a key appears. The tennis ball next to the gem is a decoy for practicing grab, release and throw; the socket rejects it.

| Object (under `Environment/Zone_A_Entrance`) | Components | Role |
|---|---|---|
| `Challenge_01/Slot/Socket` | `XRSocketInteractor`, trigger `SphereCollider` (r 0.15) | Accepts only the `PuzzlePiece` interaction layer. |
| `Piece_Gem` | `XRGrabInteractable` (layers `Default` + `PuzzlePiece`, Throw On Detach), `Rigidbody`, `BoxCollider`, `Respawn` | The piece. |
| `Decoy_Ball` | Same, layers `Default` only | Throwable; cannot enter the socket. |
| `Puzzle_01` | `PuzzleCondition` (Required Count 1), `Reveal` (Targets: `Reward_Key`) | Puzzle logic. |
| `Chest_01` | `Door` (Target: lid, Open Rotation −110, 0, 0, Duration 1) | Synty chest whose lid opens. |
| `Reward_Key` | inactive Synty key, `XRGrabInteractable` (layers `Default` + `Key`), `Rigidbody`, `BoxCollider`, `Respawn` | Revealed when solved; opens the door lock (RF-03). |
| `Challenge_01/SuccessFeedback` | `AudioSource` (clip `PUZZLE_Success_Harp_Three_Note_Bright_End_Wet_stereo`, imported Force To Mono; Spatial Blend 1, Play On Awake and Loop off, Min/Max Distance 1/15), child `FX_MagicBlast_01` (scale 0.15, Play On Awake and Looping off) | Success sound and particles at the socket. |

`PuzzlePiece` is interaction layer 1 and `Key` is layer 2 in `Assets/XRI/Settings/Resources/InteractionLayerSettings.asset`.

### Inspector wiring (rebuild without code)

1. `Socket` → XR Socket Interactor → Interaction Layer Mask: only `PuzzlePiece`.
2. Socket → Interactor Events → **Select Entered**: `Puzzle_01` → `PuzzleCondition.Signal`.
3. Socket → **Select Exited**: `Puzzle_01` → `PuzzleCondition.Unsignal`.
4. `Puzzle_01` → PuzzleCondition → **On Solved**: `Chest_01` → `Door.Open`, and `Puzzle_01` → `Reveal.Show`.
5. Feedback: on the same **On Solved** list add `SuccessFeedback` → `AudioSource.Play` and `FX_MagicBlast_01` → `ParticleSystem.Play()`. Turn off Play On Awake on the AudioSource and on every particle system, or they fire when the scene loads. On Solved fires once, so retries never repeat the feedback.
6. Piece: add `XR Grab Interactable` (it adds the `Rigidbody`), a collider, and include `PuzzlePiece` in its Interaction Layer Mask. Add `Respawn`.

### With and without feedback (game feel demo)

Solve the puzzle once, then in `Puzzle_01` → On Solved set the dropdown of the two feedback calls from **Runtime Only** to **Off** and solve again. The chest and key behave the same; only the sound and particles disappear. No logic changes.

## Door between zones: key + code (RF-03)

The door in `Doorway_AB` opens only when **two different conditions** are met, in any order: the key from the chest is in the lock (left of the door) and the code is entered on the keypad (right of the door). It shows how one `PuzzleCondition` combines different kinds of input: Required Count 2, one signal from the socket, one from the `CodeLock`.

Code: `1994`. Clue (placeholder copy): the note on the wall beside the entrance statue, "CENART fundado en 1994".

| Object (under `Environment/Doorway_AB`) | Components | Role |
|---|---|---|
| `KeyLock/Socket` | `XRSocketInteractor` (layer `Key` only), trigger `SphereCollider` (r 0.15) | Lock. The gem does not fit. |
| `KeyLock/Lock_Closed`, `Lock_Open` | Synty lock icons | Swapped when the door opens. |
| `Keypad` | `CodeLock` (Code `1994`) | Keypad logic. |
| `Keypad/Key_0` … `Key_9`, `Key_C` | `XRSimpleInteractable`, `XRPokeFilter`, `BoxCollider`, Synty button + label | Select with the controller ray/trigger or poke with the finger tip. |
| `Keypad/Display` | `TextMeshPro` | Shows the entry, `_` for missing digits. |
| `Puzzle_Door` | `PuzzleCondition` (Required Count 2) | Door logic. |
| `Door_AB` | `Door` (Target: itself, Open Rotation 0, 95, 0, Duration 1.5), child Synty door `Leaf` | Swings into Zone B on its hinge. |
| `DoorFeedback/CodeOk`, `CodeWrong`, `DoorOpen` (+ `FX_MagicBlast_01`) | `AudioSource` (spatial, Play On Awake off) | Feedback for right code, wrong code and door opening. |
| `Environment/Zone_B_Gallery/ZoneB_Lock` | solid `BoxCollider` covering Zone B up to 4 m | Blocks teleport into Zone B (through the door or over the wall) until the door opens. |

### CodeLock behavior

- Each button calls `CodeLock.Press(digit)`; the digit is the int argument typed in the event (0–9). `Key_C` calls `CodeLock.Clear()`.
- When as many digits as the Code has are entered, the lock checks them **in order**. Match: On Solved fires once. Mismatch: On Wrong fires and the entry clears, so the player simply types again. No lives, penalties or timer.
- On Input Changed sends the padded entry (`19__`) for a display text.
- After solving, the lock ignores more input and keeps showing the code.
- A Code with anything other than digits logs an error and never solves; a digit outside 0–9 logs an error and is ignored.

### Inspector wiring (rebuild without code)

1. Lock `Socket` → **Select Entered**: `Puzzle_Door` → `PuzzleCondition.Signal`; **Select Exited**: `PuzzleCondition.Unsignal`. Without the Unsignal, taking the key out and putting it back counts twice and opens the door with no code.
2. Each keypad button → XR Simple Interactable → **Select Entered**: `Keypad` → `CodeLock.Press`, argument = its digit. `Key_C` → `CodeLock.Clear`.
3. `Keypad` → CodeLock → **On Solved**: `Puzzle_Door` → `PuzzleCondition.Signal`, and `CodeOk` → `AudioSource.Play`. **On Wrong**: `CodeWrong` → `AudioSource.Play`. **On Input Changed**: `Display` → `TextMeshPro.text` (dynamic string).
4. `Puzzle_Door` → **On Solved**: `Door_AB` → `Door.Open`; `ZoneB_Lock` → `GameObject.SetActive` (off); `Lock_Closed` off, `Lock_Open` on; `DoorOpen` → `AudioSource.Play`; `FX_MagicBlast_01` → `ParticleSystem.Play()`; hints (below).

## Navigation (RF-03)

Teleport and snap turn are the only locomotion: both controllers' `ControllerInputActionManager` have Smooth Motion and Smooth Turn off. Push the thumbstick forward to aim the teleport arc, release to jump; left/right snaps the view. The camera is never moved by script. Before the door opens, `ZoneB_Lock` makes every Zone B target invalid; after it opens, both zones are teleportable and the player can return to Zone A.

## Welcome and hints (RF-03)

| Object (under `UI`) | Components | Role |
|---|---|---|
| `UI` | `MenuToggle` (Menu: `HintMenu`, Head: Main Camera, Distance 1.2) | Opens/closes the hint menu with the **menu button** of either controller (on Quest the left one; the right one is the system button). In the simulator press **M**. |
| `WelcomePanel` | world-space `Canvas`, `TrackedDeviceGraphicRaycaster` | Visible at the entry, 1.5 m in front of the start. `Comenzar` hides it; `Ver pista` opens the hint menu. |
| `HintMenu` | world-space `Canvas`, `TrackedDeviceGraphicRaycaster`, starts off | Shows the current hint; opens in front of the player at eye height. `Cerrar` → `MenuToggle.Toggle`. |
| `HintMenu/Hint_Gem`, `Hint_Door`, `Hint_Gallery` | `TextMeshProUGUI` | One active at a time. |

Hints follow progression with plain `GameObject.SetActive` calls: `Puzzle_01` On Solved turns `Hint_Gem` off and `Hint_Door` on; `Puzzle_Door` On Solved turns `Hint_Door` off and `Hint_Gallery` on. `Hint_Gallery` is placeholder copy until the RF-04 puzzles are decided. Press the UI buttons with the controller ray and trigger.

`Challenge_02` and `Challenge_03` in Zone B are still empty pedestals for RF-04: the code puzzle lives on the door.

## Reusable components (`Assets/Taller_CENART/Scripts`)

| Component | Inspector fields | Callable from events | Behavior |
|---|---|---|---|
| `PuzzleCondition` | Required Count (≥ 1), On Solved | `Signal()`, `Unsignal()` | Each `Signal` adds 1, each `Unsignal` removes 1 (never below 0). On Solved fires **once**, when the count first reaches Required Count; after that it stays solved and ignores further calls. Required Count < 1 logs an error and never solves. |
| `Door` | Target (empty = itself), Open Offset, Open Rotation, Duration (0 = instant) | `Open()`, `Close()`, `Toggle()` | Moves/rotates Target from its starting local pose to start + offset/rotation. Rotation pivots on Target's origin. |
| `Reveal` | Targets | `Show()`, `Hide()` | Activates/deactivates Targets. Leave them inactive to start hidden. No targets or an empty slot logs a warning naming the object. |
| `CodeLock` | Code (digits), On Solved, On Wrong, On Input Changed (string) | `Press(int)`, `Clear()` | Checks the digits in order once the entry is as long as Code. See *CodeLock behavior*. |
| `MenuToggle` | Toggle Action (default: menu button of either controller), Menu, Head (empty = main camera), Distance | `Toggle()` | Shows/hides Menu; on opening places it in front of Head at eye height, upright. |
| `Respawn` | Min Height (default −2) | `ResetPose()` | Returns the object to its start pose and stops it when it falls below Min Height (for example, thrown over a wall). |

For N inputs (for example, three sockets), set Required Count to N and wire each source to the same `PuzzleCondition`.

## Simulator

`Assets/XRI/Settings/Resources/XRDeviceSimulatorSettings.asset` auto-instantiates the XR Interaction Simulator (`Assets/Samples/XR Interaction Toolkit/3.6.1/XR Interaction Simulator/`) **in the Editor only**. Player builds and a real headset path are unaffected; the scene contains no simulator object.

Controls (from the sample's input actions):

| Input | Action |
|---|---|
| W A S D, Q E | Move the selected device (forward/left/back/right, down/up) |
| Right mouse button + mouse | Look / rotate the selected device; arrow keys also rotate |
| Tab | Cycle the selected device: head, left controller, right controller |
| `[` / `]` / H | Toggle manipulating the left controller / right controller / head |
| I J K L | Thumbstick of the selected controller (left: move; right: turn and teleport aim) |
| G / T | Grip / Trigger |
| 1, 2, M | Primary, secondary, menu buttons |
| R | Reset device poses |

## Verification

- `Assets/Taller_CENART/Tests/Editor/Fase0SceneTests.cs` (EditMode): simulator configured editor-only, one rig and one manager, both zones have teleportable floors, three pedestal instances, no missing scripts, socket puzzle wiring, teleport + snap turn only, door needs key and code, keypad enters each digit once, Zone B blocked until the door opens, welcome/hint menu and controller toggle, hints follow progression.
- `Assets/Taller_CENART/Tests/Editor/PuzzleComponentTests.cs` (EditMode): PuzzleCondition event count and single solve, invalid count, Door poses, Reveal targets and warnings.
- `CodeLockTests.cs`, `MenuToggleTests.cs` (EditMode): partial/wrong/correct order, retry, clear, input after solve, invalid code and digit; menu alternates, opens in front of the head at eye height, default binding on both menu buttons.
- Play Mode probe (RF-03): before the door, rays into Zone B hit `ZoneB_Lock`; gem at the lock is rejected; key at the lock counts 1; wrong code clears and leaves the door shut; right code opens the door, removes the blocker and switches hints; after that a ray through the door lands on a Zone B floor; the rig's Near-Far Interactor selects keypad buttons; the menu button of each simulated controller alternates the menu. In the simulator, M opened the menu while the Editor had focus.
- Play Mode (twice, stop and re-enter): simulator instantiated with simulated HMD and both controllers, rig stays at y = 0, no console errors. XRI logs one warning about haptic capabilities of the simulated controller; it is expected.

## Pending (needs a person)

- Socket puzzle by hand in the simulator: grab, release and throw the ball and gem, place the gem, retry. Record how long another person takes to rebuild the wiring from the steps above (Discovery Inspector experiment).
- Walk the room with the keyboard/mouse guide above and confirm move, snap turn, teleport and grab feel right.
- RF-03 by hand in the simulator: open the chest, carry the key to the lock, type a wrong code and the right one with the ray and trigger, walk through the door by teleport and come back, open/close the hints with M, press the welcome and menu buttons. On Quest: poke the keypad with the finger tip, the left menu button, and comfort of teleport and snap turn.
- A key or gem thrown over the wall into Zone B before the door opens cannot be recovered until then (it rests on `ZoneB_Lock` or behind the wall); `Respawn` only resets objects that fall below its Min Height.
- Lab compatibility (#7): open this scene on a lab PC and record Editor/package versions, simulator result and blockers. Unity 6000.6.5f1 is not yet confirmed by the lab.
