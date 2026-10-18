# Notas de docencia: Clase 2, manipulación y primer acertijo (RF-02)

## Objetivo de la clase

Que cada participante agarre, suelte y lance objetos en VR y arme su primer acertijo: **colocar una pieza en un socket hace que se abra algo o que aparezca algo**. Todo se configura desde el Inspector, sin escribir código.

## Qué se ve en la escena (`Fase0`, zona A)

1. En el estante de la pared este hay una **gema** y una **pelota de tenis**.
2. La gema va sobre el pedestal `Challenge_01`.
3. Al colocarla, **se abre la tapa del cofre** y **aparece una llave**.
4. La pelota sirve para practicar lanzamientos: el socket **la rechaza**.
5. Si un objeto sale del cuarto y cae, **regresa al estante**.

## Conceptos que enseña

| Concepto | Dónde se ve |
|---|---|
| Agarrar, soltar y lanzar | `XR Grab Interactable` + `Rigidbody` en la gema y la pelota (Throw On Detach activo) |
| Socket | `XR Socket Interactor` en `Challenge_01/Slot/Socket` |
| Filtrar qué entra al socket | Interaction Layer `PuzzlePiece`: el socket solo acepta esa capa, y la gema la tiene pero la pelota no |
| Eventos del Inspector | Select Entered / Select Exited del socket, y On Solved de la condición |
| Separar la lógica de la consecuencia | `PuzzleCondition` decide si el acertijo está resuelto; `Door` y `Reveal` ejecutan lo que pasa |

## Componentes reutilizables (`Assets/Taller_CENART/Scripts`)

- **`PuzzleCondition`** espera N señales (`Signal()`) y dispara **On Solved una sola vez**. `Unsignal()` resta una señal mientras el acertijo no está resuelto. Una vez resuelto, se queda resuelto.
- **`Door`** mueve o rota un objeto. Funciones: `Open()`, `Close()`, `Toggle()`. En la escena abre la tapa del cofre (−110° en X, 1 s).
- **`Reveal`** activa objetos que empiezan ocultos con `Show()` y los oculta con `Hide()`.
- **`Respawn`** devuelve un objeto a su posición inicial si cae por debajo de una altura mínima.

## Ejercicio guiado: armar el acertijo desde el Inspector

1. En `Socket` → Interaction Layer Mask, dejar solo `PuzzlePiece`.
2. Socket → **Select Entered** → `Puzzle_01` → `PuzzleCondition.Signal`.
3. Socket → **Select Exited** → `Puzzle_01` → `PuzzleCondition.Unsignal`.
4. `Puzzle_01` → **On Solved** → `Chest_01` → `Door.Open`, y `Puzzle_01` → `Reveal.Show`.
5. A la pieza: agregar `XR Grab Interactable`, un collider, la capa `PuzzlePiece` y `Respawn`.

## Variaciones para practicar

- Cambiar `Door` para que **deslice** la tapa: usar Open Offset en lugar de Open Rotation.
- Poner **dos sockets** con Required Count = 2 en el mismo `PuzzleCondition`.
- Usar `Reveal` para encender una luz o mostrar una pista.
- Romper la configuración a propósito para ver los mensajes:
  - Required Count = 0 registra un error y el acertijo nunca se resuelve.
  - Un `Reveal` sin objetos asignados muestra una advertencia.

## Preguntas para la discusión

- ¿Por qué la pelota no activa el acertijo, aunque también se puede agarrar? (Por las capas de interacción.)
- ¿Por qué sacar la gema después de resolverlo no vuelve a cerrar el cofre? (Porque `PuzzleCondition` dispara On Solved una sola vez.)
- ¿Qué ventaja tiene que `PuzzleCondition` no sepa nada del cofre? (Que se puede reutilizar en otros acertijos.)

## Evidencia

- **Tests EditMode:** 15/15 pasan, en `Assets/Taller_CENART/Tests/Editor/PuzzleComponentTests.cs` y `Fase0SceneTests.cs`.
- **Play Mode:** la pelota es rechazada y la gema resuelve el acertijo. La caída regresa el objeto al estante, y sacar y volver a meter la gema no lo resuelve dos veces.
- **Meta Quest:** probado en el headset con Build and Run. El APK se genera con la configuración de Android de la plantilla VR de Unity.

## Pendiente

- Medir cuánto tarda otra persona en armar el acertijo con estos pasos (experimento del Inspector).

Referencia técnica de la escena: [Fase0.md](Fase0.md).
