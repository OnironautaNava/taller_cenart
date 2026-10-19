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
| `Environment/Doorway_AB` | Empty marker in the door opening; the RF-03 door/unlock mounts here. |

All visible art comes from `Assets/Synty/` (PolygonPrototype building kit, PolygonGeneric props and materials). The roof is open, the skybox is Synty `Skybox_01`, and ambient light is Trilight so walls facing away from the sun stay readable.

Every floor tile carries a `TeleportationArea` and a collider whose top sits at y = 0. Pedestals are about 0.9 m tall, so the `Slot` on top can be reached seated or standing.

## Reusable assets (adapt without code)

- `Assets/Taller_CENART/Prefabs/Floor_2x2_Teleport`: Synty `SM_Buildings_Floor_2x2_01P` with a `TeleportationArea`. Pivot is the tile's min corner.
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
| `Reward_Key` | inactive Synty key | Revealed when solved. |
| `Challenge_01/SuccessFeedback` | `AudioSource` (clip `PUZZLE_Success_Harp_Three_Note_Bright_End_Wet_stereo`, imported Force To Mono; Spatial Blend 1, Play On Awake and Loop off, Min/Max Distance 1/15), child `FX_MagicBlast_01` (scale 0.15, Play On Awake and Looping off) | Success sound and particles at the socket. |

`PuzzlePiece` is interaction layer 1 in `Assets/XRI/Settings/Resources/InteractionLayerSettings.asset`.

### Inspector wiring (rebuild without code)

1. `Socket` → XR Socket Interactor → Interaction Layer Mask: only `PuzzlePiece`.
2. Socket → Interactor Events → **Select Entered**: `Puzzle_01` → `PuzzleCondition.Signal`.
3. Socket → **Select Exited**: `Puzzle_01` → `PuzzleCondition.Unsignal`.
4. `Puzzle_01` → PuzzleCondition → **On Solved**: `Chest_01` → `Door.Open`, and `Puzzle_01` → `Reveal.Show`.
5. Feedback: on the same **On Solved** list add `SuccessFeedback` → `AudioSource.Play` and `FX_MagicBlast_01` → `ParticleSystem.Play()`. Turn off Play On Awake on the AudioSource and on every particle system, or they fire when the scene loads. On Solved fires once, so retries never repeat the feedback.
6. Piece: add `XR Grab Interactable` (it adds the `Rigidbody`), a collider, and include `PuzzlePiece` in its Interaction Layer Mask. Add `Respawn`.

### With and without feedback (game feel demo)

Solve the puzzle once, then in `Puzzle_01` → On Solved set the dropdown of the two feedback calls from **Runtime Only** to **Off** and solve again. The chest and key behave the same; only the sound and particles disappear. No logic changes.

## Reusable components (`Assets/Taller_CENART/Scripts`)

| Component | Inspector fields | Callable from events | Behavior |
|---|---|---|---|
| `PuzzleCondition` | Required Count (≥ 1), On Solved | `Signal()`, `Unsignal()` | Each `Signal` adds 1, each `Unsignal` removes 1 (never below 0). On Solved fires **once**, when the count first reaches Required Count; after that it stays solved and ignores further calls. Required Count < 1 logs an error and never solves. |
| `Door` | Target (empty = itself), Open Offset, Open Rotation, Duration (0 = instant) | `Open()`, `Close()`, `Toggle()` | Moves/rotates Target from its starting local pose to start + offset/rotation. Rotation pivots on Target's origin. |
| `Reveal` | Targets | `Show()`, `Hide()` | Activates/deactivates Targets. Leave them inactive to start hidden. No targets or an empty slot logs a warning naming the object. |
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

- `Assets/Taller_CENART/Tests/Editor/Fase0SceneTests.cs` (EditMode): simulator configured editor-only, one rig and one manager, both zones have teleportable floors, three pedestal instances, no missing scripts, socket puzzle wiring.
- `Assets/Taller_CENART/Tests/Editor/PuzzleComponentTests.cs` (EditMode): PuzzleCondition event count and single solve, invalid count, Door poses, Reveal targets and warnings.
- Play Mode (twice, stop and re-enter): simulator instantiated with simulated HMD and both controllers, rig stays at y = 0, no console errors. XRI logs one warning about haptic capabilities of the simulated controller; it is expected.

## Pending (needs a person)

- Socket puzzle by hand in the simulator: grab, release and throw the ball and gem, place the gem, retry. Record how long another person takes to rebuild the wiring from the steps above (Discovery Inspector experiment).
- Walk the room with the keyboard/mouse guide above and confirm move, snap turn, teleport and grab feel right.
- Lab compatibility (#7): open this scene on a lab PC and record Editor/package versions, simulator result and blockers. Unity 6000.6.5f1 is not yet confirmed by the lab.
