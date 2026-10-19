# Notas de docencia: Clase 4, integración y tercer reto (RF-04)

## Objetivo de la clase

Que cada participante arme un tercer reto con una **acción distinta** (habilidad, no solo colocar o teclear) y cierre el recorrido completo: gema → llave + código → blanco → palanca → salida. También se integra audio espacial y retroalimentación en todos los retos.

## Qué se ve en la escena (`Fase0`, zona B y salida)

1. En la galería hay dos **plataformas de 2 m** con un hueco entre ellas.
2. Se sube a la plataforma oeste por una **escalera**, agarrando peldaño por peldaño.
3. Desde arriba se lanza una **pelota** a un blanco sobre unas tablas paradas al otro lado.
4. Al atinar, las tablas **caen y forman un puente** hacia la plataforma este.
5. Ahí se jala una **palanca**: abre la puerta final y aparece el panel "¡Encontraste la salida!".
6. Una pelota que falla cae al piso y **regresa al pedestal**. Hay tres.

## Conceptos que enseña

| Concepto | Dónde se ve |
|---|---|
| Escalar | `ClimbInteractable` en la escalera + `ClimbProvider` y `GravityProvider` del rig |
| Bajar del último peldaño | `ClimbTop` → Climb Assistance Teleport Volume → `ClimbOffVolume/Anchor` sobre la plataforma |
| Filtros de interacción | `PlayerHeightFilter`: las superficies altas no aceptan teleport desde el piso, pero sí estando arriba |
| Detectar impactos | `TargetHit` en el blanco: dispara On Hit con cualquier objeto con `Rigidbody` |
| Reusar componentes | `Door` sirve para el puente (rota 90°), la palanca y la puerta final |
| Habilitar en orden | El puente (`TeleportationArea`) y la palanca (`XR Simple Interactable`) empiezan **desactivados**; los eventos los encienden |
| Audio espacial | `AudioSource` con Spatial Blend 1 en cada feedback |

## Componentes nuevos (`Assets/Taller_CENART/Scripts`)

- **`TargetHit`** dispara On Hit en cada choque con un objeto que tenga `Rigidbody`; muros y pisos sin `Rigidbody` no cuentan.
- **`PlayerHeightFilter`** acepta una interacción solo si el jugador está a una altura mínima. Se agrega en los Starting Hover/Select Filters de un interactable.
- **`Timer`** (opcional, **no se usa en el cuarto**): cuenta regresiva con On Time Up y texto `mm:ss`. Ejemplo listo en `Prefabs/TimerExample.prefab`.

## Ejercicio guiado: armar el tercer reto desde el Inspector

1. `Bridge/Target` → TargetHit → **On Hit** → `Puzzle_Target` → `PuzzleCondition.Signal`.
2. `Puzzle_Target` → **On Solved** → `Bridge` → `Door.Open`; `Bridge` → `TeleportationArea.enabled` (on); `Lever` → `XRSimpleInteractable.enabled` (on); `EastLock` → `SetActive` (off); sonidos, partículas y pista.
3. `Lever` → **Select Entered** → `Puzzle_Lever` → `PuzzleCondition.Signal`.
4. `Puzzle_Lever` → **On Solved** → `Lever` → `Door.Open`; `FinalDoor` → `Door.Open`; `ExitLock` → `SetActive` (off); `ExitPanel` → `SetActive` (on); sonidos, partículas y pista.
5. Escalera: `Rigidbody` con **Is Kinematic** activado y **Use Gravity** apagado. En Colliders, solo los peldaños.
6. Superficies altas: agregar `HighTeleportFilter` a Starting Hover Filters y Starting Select Filters de las plataformas y el puente. Los pisos no llevan filtro.

## Variaciones para practicar

- Hacer el lanzamiento más fácil o difícil: mover o escalar el blanco.
- Agregar el `TimerExample` y conectar On Time Up a una pista o un mensaje.
- Cambiar los sonidos de feedback por otros del paquete de SFX.
- Romper la configuración a propósito:
  - Quitar Is Kinematic a la escalera: al dar Play sale empujada fuera de la pared.
  - Dejar el puente con su `TeleportationArea` activa: se puede cruzar sin atinar.

## Game feel e integración

- Cada acción que resuelve algo tiene sonido y, si es importante, partículas. Comparar con y sin feedback como en la clase 2.
- Recorrer las tres pruebas seguidas y preguntar: ¿se sienten distintas? ¿se entiende qué hacer sin explicarlo?

## Preguntas para la discusión

- ¿Por qué la escalera es kinematic si nunca se mueve? (`ClimbInteractable` necesita un `Rigidbody`; sin kinematic, la física la empuja.)
- ¿Por qué las pelotas empiezan inactivas? (Un objeto dinámico dentro de `ZoneB_Lock` quedaría encima del bloqueo.)
- ¿Cómo se evita que alguien jale la palanca antes de tiempo? (Interactable desactivado y encerrado por `EastLock` hasta que cae el puente.)

## Evidencia

- **Tests EditMode:** `Fase0SceneTests.cs` (escalera escalable y más alta que la plataforma, pelotas inactivas que regresan, cableado blanco → puente/palanca, palanca → puerta final/panel, feedback espacial, cadena de pistas, sin `Timer` en la escena), `TargetHitAndTimerTests.cs` y `PlayerHeightFilterTests.cs`.
- **Play Mode:** un lanzamiento fallido no resuelve y la pelota regresa; atinar gira el puente 90° y habilita palanca y puente; jalar la palanca abre la puerta final y muestra el panel. La escalera no se mueve tras 2 s; soltar el último peldaño deja al rig en la plataforma (−2.7, 2, 12.7). Desde el piso, las superficies altas rechazan el teleport; desde la plataforma, lo aceptan.

## Pendiente

- En el simulador a mano: subir la escalera (grip + jalar hacia abajo), fallar y atinar, cruzar el puente, jalar la palanca, cruzar la puerta bajo el puente.
- En Quest: dificultad del lanzamiento, comodidad al escalar y teleport por la puerta de 2 m bajo el puente. El éxito en el simulador no prueba la dificultad en el visor.
- Prueba independiente: otra persona completa el recorrido sin que le digan las soluciones; registrar comprensión, reintentos y uso de pistas.

Referencia técnica de la escena: [Fase0.md](Fase0.md).
