# Notas de docencia: Clase 1, espacio y base XR (RF-01)

## Objetivo de la clase

Que cada participante entienda de qué trata el curso y qué es XR, se ubique en Unity y en git, conozca la escena template de VR y después abra la base XR, recorra el cuarto en el **simulador** sin visor y lo haga suyo: cambiar materiales, mover y agregar objetos usando prefabs. Al terminar, la escena se puede recorrer y ya tiene identidad propia.

## Agenda

1. Presentación: de qué va el curso y quién soy.
2. XR = AR + MR + VR.
3. Introducción a Unity.
4. Introducción a git.
5. La escena template de VR y sus componentes.
6. El cuarto del taller (`Fase0`) y ejercicio guiado.

## 1. Presentación del curso y del instructor

- **Quién soy:** _[completar: nombre, trayectoria, proyectos XR, contacto]_.
- **De qué va el curso:** en cinco clases se construye un **cuarto de acertijos en VR** de temática artística, con dos zonas y tres retos encadenados. Cada clase deja una pieza funcional del mismo cuarto:

| Clase | Tema | Resultado |
|---|---|---|
| 1 | Espacio | Base XR y cuarto propio recorrible en simulador |
| 2 | Manipulación | Agarrar, lanzar y primer acertijo con socket |
| 3 | Navegación y lógica | Teleport, menús y puerta con llave + código |
| 4 | Integración | Tercer reto de lanzamiento y recorrido completo |
| 5 | Entrega | Rendimiento, confort, APK en el visor y presentación |

- **Cómo se trabaja:** la lógica se arma con componentes y eventos del Inspector, **sin escribir código**. El visor (Quest 3S) se usa para validar; el día a día se trabaja en el simulador.

## 2. XR = AR + MR + VR

XR (realidad extendida) es el término paraguas para las experiencias que mezclan mundo físico y digital:

| | Qué es | Ejemplo |
|---|---|---|
| **VR** (realidad virtual) | Sustituye por completo lo que ves por un mundo digital | Este taller en Quest 3S |
| **AR** (realidad aumentada) | Superpone contenido digital al mundo real, normalmente en un teléfono | Filtros, apps de muebles |
| **MR** (realidad mixta) | El contenido digital entiende y reacciona al espacio físico (paredes, mesas, manos) | Passthrough del Quest 3 con objetos sobre la mesa |

- Es un espectro, no tres categorías cerradas: el mismo visor puede hacer VR y MR.
- Lo que tienen en común: seguimiento de cabeza y manos, interacción en 3D, escala real (1 unidad = 1 m) y cuidado del confort.
- En Unity, el mismo **XR Interaction Toolkit** sirve para los tres; este curso se enfoca en VR.

## 3. Introducción a Unity

- **Proyecto:** carpeta con `Assets/` (lo nuestro), `Packages/` (dependencias) y `ProjectSettings/`. `Library/` se regenera y no se comparte.
- **Ventanas principales:** Scene (editar), Game (ver como jugador), Hierarchy (objetos de la escena), Project (archivos), Inspector (propiedades), Console (mensajes y errores).
- **GameObject + componentes:** todo en la escena es un GameObject; lo que hace depende de sus componentes (`Transform`, `MeshRenderer`, `Collider`, `Rigidbody`, scripts).
- **Prefab:** plantilla reutilizable de un GameObject; cambiar el prefab cambia todas sus copias.
- **Play Mode:** los cambios hechos durante Play **se pierden** al detenerlo.
- Versión del proyecto: Unity `6000.6.5f1` con URP. Abrirlo siempre con esa versión.

## 4. Introducción a git

- **Para qué:** guarda la historia del proyecto, permite volver atrás y trabajar en paralelo sin pisarse.
- **Conceptos:** repositorio, commit (foto del proyecto con un mensaje), rama (línea de trabajo), remoto (copia en GitHub).
- **Flujo básico:**

```
git clone <url>        # traer el proyecto
git status             # ver qué cambió
git add <archivos>     # elegir qué guardar
git commit -m "..."    # guardar una foto
git pull / git push    # sincronizar con el remoto
```

- **En Unity:**
  - El `.gitignore` excluye `Library/`, `Temp/`, `Logs/`, `UserSettings/` y `Builds/`.
  - Los archivos `.meta` **siempre** se versionan: guardan el GUID que conecta cada asset con sus referencias. Mover o renombrar assets desde Unity, no desde el explorador.
  - Las escenas y prefabs son difíciles de fusionar: evitar que dos personas editen la misma escena a la vez.
- **Demostración:** mostrar el historial del proyecto (`git log --oneline`); cada clase corresponde a commits del cuarto.

## 5. La escena template de VR

Abrir `Assets/Scenes/SampleScene.unity` (la escena de la plantilla VR de Unity) y dar Play con el simulador.

| Componente | Qué hace |
|---|---|
| `XR Origin Hands (XR Rig)` | El jugador: cámara (cabeza), controles y manos, con los interactores (rayo, agarre, poke) y la locomoción |
| `XR Interaction Simulator` | Simula visor y controles con teclado y mouse; solo en el Editor |
| `EventSystem` | Permite usar UI con rayos y poke |
| `Environment` | Piso, entorno y `Teleport Area` / `Teleport Anchor` para moverse por teleport |
| `Interactables` | Objetos de ejemplo para agarrar (cubo, esfera, cilindro, toro) con affordances de color, audio y forma |
| `UI` / Spatial Panels | Ejemplos de paneles en el mundo: botones, sliders, toggles, dropdown |
| `Tutorial Player` | Tarjetas de introducción que guían al usuario |
| `Lighting`, `Post Process Volume`, `Reflection Probe`, `Light Probe Group` | Iluminación y post-proceso con URP |

- La escena no tiene un `XR Interaction Manager` propio: XRI lo crea al iniciar si falta. En `Fase0` sí está explícito, y debe haber **uno solo**.
- Mensaje clave: el cuarto del taller usa **las mismas piezas** (rig, interactores, teleport, UI); lo que cambia es el arte y la lógica.

## 6. Qué se ve en la escena del taller (`Fase0`)

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

- ¿Qué experiencias de AR, MR o VR conocen? ¿Dónde caería cada una en el espectro XR?
- ¿Por qué no se versiona `Library/` pero sí los `.meta`? (`Library/` se regenera; los `.meta` guardan los GUID de las referencias.)
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
