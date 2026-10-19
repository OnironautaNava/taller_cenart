---
phase: descubre
project: Taller VR CENART 2026-10
status: complete
sections_complete: [problem, importance, benefits, why-now, risks-not-doing, audience, metrics, efficiency, risks-constraints, assumptions, discovery, post-discovery, workflows, gtm]
updated: 2026-10-17
sources:
  - "Profesional/Docencia/Taller VR CENART 2026-10.md"
  - "Profesional/Docencia/Taller VR CENART 2026-10 - Selección de asistentes.md"
next_phase: ana-ordena
---

# Cuarto de acertijos VR — PRD del proyecto de referencia CENART

[[Profesional/Docencia/Taller VR CENART 2026-10|← Plan del taller]]

## Resumen ejecutivo

Proyecto de referencia: cuarto de acertijos VR artístico para Meta Quest 3S, completo y probado antes del taller del 19 al 23 de octubre de 2026.
Construcción didáctica acumulativa en cinco clases: espacio, interacción, navegación e interfaces, integración y despliegue.
Recorrido obligatorio: al menos dos zonas por teleport, tres retos distintos encadenados, pistas y retroalimentación, sin exigir programación a los participantes.
Base preconfigurada y componentes del Inspector; simulador como entorno principal y visor para pruebas de interacción y confort.
Este PRD define el alcance y la aceptación en Unity; las decisiones abiertas y las pruebas pendientes se identifican al final y no se consideran resueltas.

**Uso en Unity:** copiar este documento completo al proyecto y usarlo como fuente de verdad del alcance, con RF-01 a RF-05 como referencias de implementación. Mantener una única versión vigente: cualquier decisión o cambio aprobado debe incorporarse al PRD que se use en el proyecto, no quedar solo en conversaciones. El cierre de redacción no certifica que la aplicación esté implementada o validada.

## 1. Problema a resolver

El instructor y los participantes necesitan una referencia común que defina qué experiencia construir, cómo se incorpora cada funcionalidad al avance diario y cuándo se considera terminada. Con 15 horas y un solo visor, la progresión debe poder comprobarse principalmente en el simulador, sin exigir programación a los participantes.

El PRD establecerá el alcance y los criterios de aceptación del proyecto; el documento actual seguirá siendo la referencia de horarios y dinámica docente.

El proyecto de referencia debe estar terminado antes del taller, pero permitir reconstruir sus funcionalidades siguiendo las cinco clases. Los participantes adaptarán esa referencia en Unity.

## 2. Por qué importa

El proyecto debe servir tanto para demostrar una experiencia completa como para enseñar a construirla por etapas. Cada clase debe dejar una pieza funcional del mismo cuarto, evitando ejercicios desconectados y reservando el visor para validar interacción y confort.

## 3. Cómo beneficia a los clientes

- **Participantes:** construir su propio cuarto artístico mediante componentes configurables desde el Inspector, comprobando el avance diario en el simulador.
- **Instructor:** contar con una referencia funcional y criterios de aceptación por clase para demostrar, explicar y revisar el avance.
- **Quien juega:** experimentar un cuarto resoluble de principio a fin, con pistas claras, retroalimentación y navegación orientada al confort.

## 4. Por qué esto, por qué ahora

El taller se realizará del **19 al 23 de octubre de 2026**. Antes de comenzar, se necesita el proyecto completo para la demostración inicial y una base preconfigurada para los participantes.

El cuarto de acertijos integra interacción, navegación, interfaces y ambientación en una sola experiencia, siguiendo el orden del temario. El PRD fijará ese alcance para priorizar lo necesario y mantener las extensiones opcionales fuera del recorrido obligatorio.

## 5. Riesgos de no hacerlo

Sin una referencia alineada al temario, existe el riesgo de:

- Construir funcionalidades que no puedan enseñarse dentro del tiempo disponible.
- Llegar a ejercicios que dependan de piezas todavía no preparadas.
- Obtener acertijos aislados, sin un recorrido completo y resoluble.
- Posponer problemas de compilación o confort hasta el cierre del taller.

## 6. Público objetivo

- **Participantes:** grupo multidisciplinario de arte, diseño, museografía, arquitectura, divulgación y videojuegos. Entre los 15 seleccionados propuestos, la evaluación de base técnica distribuye a 5 en 2/5, 5 en 3/5 y 5 en 4–5/5.
- **Implicación para el proyecto:** recorrido obligatorio accesible sin programación, componentes configurables desde el Inspector y extensiones opcionales para quienes tienen mayor experiencia. No se presupone dominio de Unity/XRI.
- **Instructor:** utiliza el proyecto completo y los criterios por clase para demostrar y acompañar avances.
- **Jugadores:** prueban la claridad, resolución y confort de los cuartos.

Fuente: [[Profesional/Docencia/Taller VR CENART 2026-10 - Selección de asistentes|Evaluación de asistentes]], del 14 de octubre de 2026. La calificación se basa en las respuestas del formulario, no en una prueba de dominio de Unity o XRI. La lista sigue pendiente de validación con Luis; los visores adicionales no están confirmados.

## 7. Métricas de éxito

Los criterios de esta sección, incluida la variedad obligatoria de los tres acertijos, fueron confirmados por el usuario.

| Métrica | Tipo (leading / lagging) | Comportamiento deseado vinculado |
|---|---|---|
| Día 1: escena propia recorrible en el simulador | leading | Construir y comprobar el espacio donde se desarrollará el recorrido final. |
| Día 2: objeto agarrable que, colocado en un socket, abre o revela algo | leading | Resolver la primera interacción del cuarto y comprobar su consecuencia. |
| Día 3: dos zonas conectadas por teleport, segunda zona desbloqueable y panel de pistas funcional | leading | Avanzar por el cuarto y consultar pistas para continuar. |
| Día 4: tres acertijos encadenados, distintos en la acción principal, y cuarto resoluble de principio a fin en el simulador, con ambientación y retroalimentación | leading | Integrar un recorrido variado y completo antes del despliegue. |
| Día 5: APK compilado y presentación; instalación y prueba en Quest según disponibilidad, con presentación en simulador como alternativa | lagging | Entregar y demostrar la experiencia construida durante el taller. |
| Proyecto de referencia completo y probado en Quest 3S antes del taller | lagging | Disponer de una experiencia funcional para la demostración inicial y la enseñanza por etapas. |

**Variedad obligatoria:** los tres acertijos no deben sentirse monótonos. Mantener el acertijo de socket y el de botones o poke del roadmap, pero diferenciando sus acciones principales; no basta con repetir la misma interacción cambiando el objeto, color o código. El tercer reto puede ser una prueba de habilidad al disparar o lanzar algo con precisión, sin convertir esas dos alternativas en requisitos simultáneos. La mecánica concreta del tercer reto queda por definir.

**Comprobación:** recorrer los tres retos y verificar una acción principal distinta en cada uno, además de que sus soluciones se encadenan. Los hitos diarios comprueban la progresión docente; el proyecto de referencia se prepara completo antes del taller.

## 8. Eficiencia operativa

- Partir de una **base preconfigurada**, evitando dedicar clase a configurar Android y XR desde cero.
- Reutilizar componentes y eventos del Inspector para construir acertijos distintos sin exigir programación.
- Trabajar principalmente en simulador y reservar las rondas en Quest para validar interacción y confort.
- Integrar cada ejercicio en el mismo cuarto, evitando rehacer trabajo.
- Realizar la compilación de prueba el día 2 y centralizar la instalación de APK en la estación del instructor.

## 9. Riesgos y restricciones

| Restricción o riesgo | Tratamiento |
|---|---|
| 15 horas y niveles técnicos diversos | Recorrido obligatorio sin programación; extensiones opcionales. |
| Un solo Quest 3S confirmado | Simulador principal y rondas de visor los días 2, 4 y 5. |
| Versión exacta de Unity y entorno Android pendientes | Validar el laboratorio y conservar la laptop del instructor como respaldo. |
| Internet o paquetes no disponibles | Preparar y comprobar la base y los recursos antes del taller. |
| Mareo o incomodidad | Teleport y snap turn por defecto; no desplazar artificialmente la cámara del usuario. |
| Posible dificultad excesiva del reto de precisión | Si se elige, validar que sea realizable en simulador y visor sin bloquear desproporcionadamente el recorrido. |
| Tiempo insuficiente para instalar todos los APK | Presentación en simulador e instalaciones pendientes al cierre. |

**Entorno acordado en el plan del taller:** Unity 6 LTS, OpenXR y XR Interaction Toolkit (XRI) 3.x, partiendo de una base derivada de la plantilla VR de Unity Hub, con XR Origin, XR Device Simulator y Android configurados. Conservar ese stack; la versión exacta de Unity y las versiones concretas de paquetes deberán fijarse según el entorno validado del laboratorio. Meta XR Interaction SDK no sustituye a XRI en el alcance obligatorio.

## 10. Supuestos

Estos puntos deberán validarse; no se consideran hechos comprobados:

- Las computadoras del laboratorio podrán ejecutar la base y el simulador con fluidez.
- La configuración desde el Inspector permitirá completar los retos obligatorios dentro del tiempo de práctica.
- Las pistas permitirán comprender qué hacer sin que el instructor explique cada solución.
- Los tres retos ofrecerán variedad y una dificultad adecuada; si se elige precisión, funcionará tanto en simulador como en Quest.
- La experiencia podrá adaptarse a distintos temas artísticos sin reescribir los componentes.

## Discovery — Experimentos

Plan de validación confirmado. Las pruebas no se han realizado ni sus hipótesis se consideran comprobadas; los resultados quedan pendientes y no bloquean la redacción del detalle funcional.

| Hipótesis | Método | Aprendizajes | Implicación |
|---|---|---|---|
| La base permite trabajar en las computadoras del laboratorio. | Abrir la base en una PC del laboratorio y comprobar el simulador y la configuración disponible. | — | Pendiente: determinar si el entorno permite seguir la práctica o necesita ajustes. |
| El armado desde el Inspector es comprensible y cabe en el tiempo de práctica. | Armar un acertijo solo desde el Inspector y observar claridad del proceso y tiempo requerido. | — | Pendiente: determinar si hay que simplificar la configuración o las instrucciones. |
| Las pistas y el encadenamiento permiten completar un recorrido variado. | Pedir a otra persona que recorra el cuarto sin explicarle las soluciones. | — | Pendiente: identificar dificultades de comprensión, progresión o monotonía. |
| La interacción y el confort son adecuados en el visor. | Probar el recorrido en Quest 3S, incluida la dificultad del reto de precisión si se elige. | — | Pendiente: determinar ajustes de interacción, confort o dificultad. |
| El entorno permite desplegar la experiencia antes de depender de él en clase. | Compilar e instalar el APK. | — | Pendiente: identificar y resolver bloqueos de despliegue. |

## Post-Discovery

- Solución propuesta y razonamiento: un **cuarto de acertijos VR de temática artística**, con dos zonas como mínimo y tres retos encadenados con acciones distintas: socket, combinación de botones/poke y un tercero por definir, posiblemente de precisión. La entrega incluirá el proyecto completo del instructor, la base preconfigurada y los componentes reutilizables del roadmap. Su construcción seguirá la progresión de las cinco clases.
- Valor y ROI potencial: el valor esperado es docente: reutilizar lo aprendido e integrar cada práctica en la experiencia final. Retorno económico: —.
- Costo estimado: —
- Preguntas abiertas / FAQ: tema concreto del proyecto de referencia, mecánica del tercer reto y versiones exactas del entorno pendientes de definir.

Esta solución se basa en el roadmap y las decisiones confirmadas; no implica que las pruebas del plan de validación ya hayan sido realizadas.

## Workflows

- Comportamientos actuales: experiencia previa heterogénea según la evaluación de selección; flujo actual de creación de experiencias VR del grupo: —.
- Comportamientos deseados (repetibles y medibles): construir y comprobar una pieza del mismo cuarto en cada clase, configurar la lógica obligatoria mediante componentes y eventos del Inspector y completar un recorrido con tres retos distintos.
- JTBD / historias de usuario:
  - Como participante, quiero adaptar la base a mi tema artístico y comprobar el avance en simulador sin depender del turno de visor.
  - Como instructor, quiero demostrar una referencia completa y enseñar su construcción por etapas con criterios observables por clase.
  - Como jugador, quiero comprender las pistas, resolver retos variados y avanzar por un recorrido completo con navegación orientada al confort.
- Resultados de usuario y cómo se miden: hitos de la sección 7 y criterios funcionales de la siguiente tabla. La comprobación del recorrido y sus pistas aporta evidencia del valor para quien juega; los resultados de esas pruebas siguen pendientes.

### Funcionalidades y aceptación por clase

La referencia completa debe estar preparada antes del taller. Esta tabla ordena la construcción didáctica, no pospone la preparación del instructor hasta las fechas de clase. Los hitos son acumulativos.

| ID | Día | Funcionalidades y aceptación |
|---|---|---|
| RF-01 | 1 — Espacio | Base XR y simulador funcionando; escena artística propia con objetos, materiales y prefabs. Se puede recorrer en simulador. |
| RF-02 | 2 — Manipulación | Agarrar, soltar y lanzar objetos; colocar una pieza en un socket que active `Door` o `Reveal`. Primer acertijo funcional mediante eventos del Inspector. |
| RF-03 | 3 — Navegación y lógica | Teleport y snap turn; segunda zona desbloqueable; menú de bienvenida, panel de pistas y combinación de botones/poke con `CodeLock`. Encadenamiento con `PuzzleCondition` y menú alternable desde el control. |
| RF-04 | 4 — Integración | Tercer reto con acción distinta; modelos temáticos, audio espacial y retroalimentación al resolver. Los tres retos forman un recorrido completo. `Timer` opcional. |
| RF-05 | 5 — Entrega | Revisar rendimiento y confort, compilar APK, instalar y probar según disponibilidad del visor, y presentar. No añadir mecánicas obligatorias nuevas. |

### Componentes y límites del alcance

- Los componentes tendrán ejemplos configurables desde el Inspector: `PuzzleCondition` espera N eventos y dispara `OnSolved`; `Door` / `Reveal` abre, mueve o activa algo; `CodeLock` reconoce una secuencia de botones; `Timer` ofrece cuenta regresiva con `OnTimeUp` y su uso en el cuarto es opcional.
- La lógica obligatoria se arma sin exigir programación a los participantes. Las extensiones con IA siguen siendo opcionales.
- Hand tracking será demostración, no requisito del recorrido obligatorio.
- El tercer reto tendrá una acción principal distinta de socket y combinación; lanzamiento o disparo de precisión son alternativas, no requisitos simultáneos. La mecánica concreta sigue pendiente.

## GTM (opcional)

- ¿Aplica?: omitido por decisión del usuario; no es necesario para este curso.
- Enablement interno: materiales docentes y ejemplos cubiertos por el alcance del taller; no se añade un plan comercial.
- GTM externo: fuera del alcance.

## Dudas abiertas

- **Tema concreto del proyecto de referencia:** —. No bloquea la base XR ni la lógica reutilizable; definir antes de cerrar ambientación y pistas definitivas.
- **Mecánica concreta del tercer reto:** lanzamiento de precisión (decidido 2026-10-19). El jugador sube por una escalera estática a una plataforma alta en la zona 2 y lanza una pelota a un blanco; al atinar, cae una tabla que hace de puente hacia el otro lado, donde una palanca abre la puerta final que lleva a la salida. Los únicos activadores son atinar en el blanco y jalar la palanca. Reintento: una pelota que falla vuelve a la plataforma. Simulador: agarre (G) para escalar y lanzar; en Quest se valida aparte la dificultad del lanzamiento.
- **Versiones exactas de Unity y paquetes:** —. Confirmar con el laboratorio antes de fijar el entorno distribuible; no elegir versiones incompatibles por asumir que cualquier Unity 6 LTS es equivalente.
- **Resultados de validación:** —. Ejecutar y registrar las pruebas de Discovery; la referencia deberá estar probada en Quest 3S antes del taller. No confundir el plan confirmado con evidencia de pruebas realizadas.
- **Confirmaciones operativas:** lista definitiva y posibles visores adicionales pendientes según la nota de selección. Planificar con un solo Quest 3S mientras no se confirme otro recurso.

### Decisiones de elaboración

- Se usa directamente el plan del taller como fuente, sin `01-entiende.md` previo, por autorización del usuario.
- Requisito adicional del usuario: «los 3 acertijos no deben sentirse monotonos, incluso uno puede ser una prueba de habilidad disparando o lanzando algo con precision.» La variedad es obligatoria; disparar o lanzar son alternativas posibles, no una mecánica ya elegida.
- Redacción confirmada y lista para iniciar la implementación del alcance definido; GTM omitido. Las decisiones abiertas bloquean únicamente las partes que dependen de ellas, no el inicio de todo el proyecto.
