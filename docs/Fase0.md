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

- `Assets/Taller_CENART/Tests/Editor/Fase0SceneTests.cs` (EditMode): simulator configured editor-only, one rig and one manager, both zones have teleportable floors, three pedestal instances, no missing scripts.
- Play Mode (twice, stop and re-enter): simulator instantiated with simulated HMD and both controllers, rig stays at y = 0, no console errors. XRI logs one warning about haptic capabilities of the simulated controller; it is expected.

## Pending (needs a person)

- Walk the room with the keyboard/mouse guide above and confirm move, snap turn, teleport and grab feel right.
- Lab compatibility (#7): open this scene on a lab PC and record Editor/package versions, simulator result and blockers. Unity 6000.6.5f1 is not yet confirmed by the lab.
