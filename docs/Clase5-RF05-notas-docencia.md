# Notas de docencia: Clase 5, entrega (RF-05)

## Objetivo de la clase

Que cada participante revise **rendimiento y confort**, compile el **APK**, lo instale y pruebe según disponibilidad del visor, y presente su cuarto. **No se agregan mecánicas obligatorias nuevas**: la clase es para pulir y entregar lo construido en las clases 1 a 4.

## Qué se revisa

1. El recorrido completo funciona de principio a fin: gema → llave + código → blanco → palanca → salida.
2. El cuarto se siente cómodo: sin movimientos de cámara por script, menús a distancia legible, objetos al alcance.
3. El rendimiento alcanza para el visor: pocas luces en tiempo real, partículas cortas, sin objetos innecesarios.
4. El APK se compila, se instala y abre en el Quest.

## Conceptos que enseña

| Concepto | Dónde se ve |
|---|---|
| Escenas en la build | Build Profiles / Scenes In Build: solo las escenas que se van a entregar |
| Plataforma Android | La configuración de Android de la plantilla VR de Unity; no se configura XR desde cero |
| Rendimiento en el visor | Profiler y estadísticas de la ventana Game; el simulador no representa el rendimiento del Quest |
| Confort | Teleport y snap turn, sin locomoción continua; alturas y distancias reales |
| Pruebas con otra persona | Ver a alguien jugar sin explicarle revela qué no se entiende |

## Lista de revisión antes de compilar

- [ ] Consola sin errores al dar Play.
- [ ] Recorrido completo en el simulador, de la bienvenida al panel de salida.
- [ ] Cada reto se puede reintentar (objetos que caen regresan con `Respawn`).
- [ ] Ningún `AudioSource` ni partícula de feedback con Play On Awake activo.
- [ ] Las pistas cambian al resolver cada reto.
- [ ] La escena del cuarto está incluida en la build y es la primera.

## Ejercicio guiado: compilar e instalar

1. Conectar el Quest por USB y aceptar la depuración en el visor.
2. Abrir la configuración de build, plataforma **Android**.
3. Confirmar qué escena se incluye y en qué orden. No incluir todas las escenas de ejemplo.
4. **Build And Run** para compilar e instalar en un paso.
5. En el visor, completar el recorrido y anotar lo que se sienta mal.

## Variaciones para practicar

- Comparar el recorrido en el simulador y en el visor: ¿qué cambia en distancias, lanzamiento y lectura de textos?
- Desactivar una luz puntual o un sistema de partículas y comparar en el Profiler.
- Pedir a otra persona que juegue sin ayuda y medir cuántas veces abre las pistas.

## Presentación

Cada participante muestra en 3 a 5 minutos:

- Su tema y cómo lo expresó en materiales, objetos y pistas.
- Los tres retos y qué componentes usó para cada uno.
- Una cosa que cambió después de probar en el visor o con otra persona.

## Preguntas para la discusión

- ¿Por qué funcionar en el simulador no prueba que funcione en el visor? (Rendimiento, escala percibida y precisión de las manos son distintos.)
- ¿Qué se puede quitar sin perder la experiencia para ganar rendimiento?
- ¿Qué se entendió sin pistas y qué no?

## Evidencia

- **Meta Quest:** el acertijo del socket (clase 2) se probó en el headset con Build and Run, usando la configuración de Android de la plantilla VR de Unity.
- **No hay perfiles de build** versionados en el proyecto. Confirmar la plataforma y la escena antes de crear uno.

## Pendiente

- Probar en Quest 3S el recorrido completo de las clases 3 y 4 (poke, menú, escalera, lanzamiento) antes del taller.
- Revisar rendimiento en el visor: partículas, luces y audio espacial.
- Registrar los resultados de la prueba independiente y de compatibilidad con el laboratorio.
- Hay un solo Quest 3S confirmado: planear turnos de prueba en el visor.

Referencia técnica de la escena: [Fase0.md](Fase0.md).
