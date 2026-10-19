# Notas de docencia: Clase 1, espacio y base XR (RF-01)

## Objetivo de la clase

Que cada participante abra la base XR, recorra el cuarto en el **simulador** sin visor y lo haga suyo: cambiar materiales, mover y agregar objetos usando prefabs. Al terminar, la escena se puede recorrer y ya tiene identidad propia.

## Qué se ve en la escena (`Fase0`)

1. Al dar Play aparece el **XR Interaction Simulator**; no hace falta visor.
2. El rig (`XR Origin (XR Rig)`) empieza en la entrada, mirando hacia la galería.
3. El cuarto tiene dos zonas: **A, entrada** (piso de madera) y **B, galería** (alfombra y estatuas), separadas por un muro con puerta.
4. Todo el arte viene de `Assets/Synty/`: muros, ventanas, columnas, lámparas y estatuas.
5. Los pedestales (`ChallengePedestal`) marcan dónde irán los retos de las siguientes clases.

## Conceptos que enseña

| Concepto | Dónde se ve |
|---|---|
| Rig XR | `XR Origin (XR Rig)`: cámara + controles; es el "cuerpo" del jugador |
| Simulador | `XRDeviceSimulatorSettings.asset` lo crea solo en el Editor; las builds no lo incluyen |
| Una sola fuente de interacción | Un `XR Interaction Manager` y un `EventSystem` en toda la escena |
| Prefabs reutilizables | `Floor_2x2_Teleport` y `ChallengePedestal` en `Assets/Taller_CENART/Prefabs` |
| Materiales | Cambiar el material de un piso o muro cambia el ambiente sin tocar la geometría |
| Escala real | 1 unidad = 1 m; pedestales de ~0.9 m para alcanzarlos sentado o de pie |

## Controles del simulador

| Tecla | Acción |
|---|---|
| W A S D, Q E | Mover el dispositivo seleccionado |
| Clic derecho + mouse | Mirar / rotar |
| Tab | Cambiar entre cabeza, control izquierdo y derecho |
| I J K L | Thumbstick del control seleccionado |
| G / T | Grip / Trigger |
| R | Reiniciar poses |

## Ejercicio guiado: personalizar el cuarto

1. Abrir `Assets/Taller_CENART/Scenes/Fase0.unity` y dar Play. Recorrer ambas zonas con el simulador.
2. Seleccionar las baldosas de piso de una zona y cambiarles el material por otro `Generic_*` de `Assets/Synty/PolygonGeneric/Materials/`.
3. Reemplazar una estatua o un prop por otro prefab de Synty.
4. Agregar una baldosa con el prefab `Floor_2x2_Teleport` (el pivote está en la esquina mínima, eso facilita alinearla).
5. Dar Play de nuevo y comprobar que todo se ve y se recorre.

## Variaciones para practicar

- Cambiar la iluminación: color o intensidad de las luces de las lámparas colgantes.
- Cambiar el skybox de la escena.
- Agregar una zona decorativa nueva con muros y props de Synty.
- Romper a propósito: quitar el collider de un piso y ver que el jugador ya no puede pararse ahí (lo retomamos en la clase 3 con teleport).

## Reglas para no romper la base

- No agregar otro rig ni otro `XR Interaction Manager`.
- Todo piso caminable necesita un collider y su `TeleportationArea`.
- Usar prefabs de Synty, no primitivas, para que el cuarto se vea coherente.

## Preguntas para la discusión

- ¿Por qué el simulador no aparece en el APK? (Solo se instancia en el Editor.)
- ¿Qué ventaja tiene usar un prefab para el piso en lugar de configurar cada baldosa? (Un cambio en el prefab llega a todas.)
- ¿Por qué importa la escala en VR más que en un juego de pantalla? (El cuerpo del jugador es la referencia.)

## Evidencia

- **Tests EditMode** en `Assets/Taller_CENART/Tests/Editor/Fase0SceneTests.cs`: simulador solo en Editor, un solo rig y un solo manager, ambas zonas con piso teleportable, tres pedestales, sin scripts faltantes.
- **Play Mode** (dos veces, deteniendo y volviendo a entrar): el simulador aparece con HMD y ambos controles, el rig queda en y = 0, sin errores en consola. XRI registra una advertencia esperada sobre la háptica del control simulado.

## Pendiente

- Recorrer el cuarto a mano con la guía de teclado y confirmar que mover, girar y agarrar se sienten bien.
- Compatibilidad con el laboratorio: abrir la escena en una PC del laboratorio y registrar versiones y bloqueos. Unity 6000.6.5f1 aún no está confirmado ahí.
- Aprobar el tema: el arte de Synty y los textos todavía no son un tema aprobado.

Referencia técnica de la escena: [Fase0.md](Fase0.md).
