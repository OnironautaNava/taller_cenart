# Notas de docencia: Clase 3, navegación, interfaces y lógica (RF-03)

## Objetivo de la clase

Que cada participante se mueva con **teleport y snap turn**, use menús en VR y arme un segundo acertijo que **combina dos condiciones distintas**: una llave en una cerradura y un código en un teclado. Al resolverlo se desbloquea la zona B. Todo desde el Inspector.

## Qué se ve en la escena (`Fase0`, puerta entre zonas)

1. Al entrar aparece un **panel de bienvenida** con `Comenzar` y `Ver pista`.
2. La llave que salió del cofre (clase 2) va en la **cerradura** a la izquierda de la puerta.
3. A la derecha hay un **teclado** de 10 dígitos más `C`. El código es `1994`; la pista está en la nota junto a la estatua de la entrada.
4. La puerta abre solo con **llave y código**, en cualquier orden.
5. Antes de abrir, no se puede teletransportar a la zona B. Después, se puede ir y volver.
6. El botón de menú del control abre y cierra el **menú de pistas** (en el simulador, **M**).

## Conceptos que enseña

| Concepto | Dónde se ve |
|---|---|
| Teleport y snap turn | `ControllerInputActionManager` de cada control con Smooth Motion y Smooth Turn apagados |
| Bloquear zonas | `ZoneB_Lock`: un collider sólido que invalida destinos de teleport hasta que se apaga |
| Poke y selección por rayo | Botones del teclado con `XR Simple Interactable` + `XR Poke Filter` |
| Combinar entradas distintas | `Puzzle_Door` (`PuzzleCondition`, Required Count 2): una señal de la cerradura y otra del `CodeLock` |
| Eventos con argumento | Cada botón llama `CodeLock.Press` con su dígito escrito en el evento |
| Eventos con texto dinámico | `CodeLock` → On Input Changed → `TextMeshPro.text` del display |
| UI en el mundo | Canvas world-space con `TrackedDeviceGraphicRaycaster` |
| Pistas que siguen el progreso | `GameObject.SetActive` en los On Solved de cada acertijo |

## Componentes nuevos (`Assets/Taller_CENART/Scripts`)

- **`CodeLock`** recibe dígitos con `Press(int)` y `Clear()`. Al completar la longitud del código lo compara **en orden**: si coincide dispara On Solved una vez; si no, dispara On Wrong y borra la entrada para reintentar. Sin vidas ni penalización.
- **`MenuToggle`** muestra u oculta un menú con el botón de menú de cualquier control y lo coloca frente a la cabeza, a la altura de los ojos.

## Ejercicio guiado: armar la puerta desde el Inspector

1. Cerradura `Socket` → Interaction Layer Mask: solo `Key`.
2. Cerradura → **Select Entered** → `Puzzle_Door` → `PuzzleCondition.Signal`; **Select Exited** → `PuzzleCondition.Unsignal`.
3. Cada botón → **Select Entered** → `Keypad` → `CodeLock.Press`, argumento = su dígito. `Key_C` → `CodeLock.Clear`.
4. `Keypad` → **On Solved** → `Puzzle_Door` → `PuzzleCondition.Signal` y `CodeOk` → `AudioSource.Play`. **On Wrong** → `CodeWrong` → `AudioSource.Play`. **On Input Changed** → `Display` → `TextMeshPro.text` (dinámico).
5. `Puzzle_Door` → **On Solved** → `Door_AB` → `Door.Open`; `ZoneB_Lock` → `SetActive` (off); `Lock_Closed` off y `Lock_Open` on; sonido y partículas; cambiar pista.
6. `UI` → `MenuToggle`: Menu = `HintMenu`. El botón `Cerrar` del menú llama `MenuToggle.Toggle`.

## Variaciones para practicar

- Cambiar el código y la pista de la nota.
- Agregar una tercera condición (otro socket) con Required Count = 3.
- Agregar una pista nueva al menú y encenderla desde un On Solved.
- Romper la configuración a propósito:
  - Quitar el Unsignal de la cerradura: sacar y meter la llave cuenta dos veces y abre la puerta **sin código**.
  - Un código con letras registra un error y nunca se resuelve.

## Confort

- Teleport y snap turn evitan el mareo de la locomoción continua; nada mueve la cámara por script.
- El menú se abre frente al jugador, no pegado a la cara.

## Preguntas para la discusión

- ¿Por qué la puerta necesita `Unsignal` en la cerradura pero no en el teclado? (La llave se puede sacar; el código, una vez resuelto, se queda resuelto.)
- ¿Por qué un collider invisible en vez de apagar los pisos de la zona B? (Bloquea también el teleport por encima del muro y se quita con una sola llamada.)
- ¿Qué cambia entre presionar con el rayo y con el dedo? (La misma lógica; solo cambia la forma de seleccionar.)

## Evidencia

- **Tests EditMode:** `CodeLockTests.cs` (orden parcial, incorrecto y correcto, reintento, borrar, entrada después de resolver, código y dígito inválidos), `MenuToggleTests.cs` (alterna, se abre frente a la cabeza, ambos botones de menú) y `Fase0SceneTests.cs` (solo teleport + snap turn, puerta con llave y código, zona B bloqueada, menús y pistas).
- **Play Mode:** antes de la puerta los rayos a la zona B chocan con `ZoneB_Lock`; la gema es rechazada en la cerradura; un código incorrecto se borra y la puerta sigue cerrada; el correcto abre la puerta, quita el bloqueo y cambia la pista; el botón de menú de cada control simulado alterna el menú.

## Pendiente

- En el simulador a mano: llevar la llave, teclear un código incorrecto y el correcto, cruzar por teleport y volver, abrir y cerrar pistas con M.
- En Quest: poke con la punta del dedo, botón de menú izquierdo (el derecho es del sistema), comodidad de teleport y snap turn.
- Una llave lanzada por encima del muro antes de abrir la puerta no se recupera hasta abrirla.

Referencia técnica de la escena: [Fase0.md](Fase0.md).
