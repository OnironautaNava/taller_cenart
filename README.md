# Taller VR CENART — Cuarto de acertijos

Proyecto de Unity para el taller CENART del **19 al 23 de octubre de 2026**. Su objetivo es construir un cuarto de acertijos de realidad virtual para **Meta Quest 3S**, mediante una progresión didáctica de cinco clases y componentes configurables desde el Inspector.

> **Estado:** el repositorio contiene escenas, recursos de la plantilla VR y dependencias XR. El alcance del cuarto está definido en el PRD; esto no significa que la experiencia completa esté implementada o validada en visor.

## Inicio rápido

1. Clona este repositorio o descarga una copia local.
2. Instala **Unity 6000.6.5f1** desde Unity Hub.
3. En Unity Hub, agrega la carpeta del repositorio como proyecto y ábrela con esa versión.
4. Espera a que Unity importe los recursos y resuelva las dependencias de `Packages/manifest.json`.
5. Abre `Assets/Scenes/SampleScene.unity` para explorar la escena incluida actualmente en la configuración de compilación. También puedes revisar `Assets/Scenes/BasicScene.unity`.
6. Comprueba la consola antes de comenzar a trabajar.

Para compilar un APK, instala también el módulo **Android Build Support**, con Android SDK & NDK Tools y OpenJDK, correspondiente a esta versión del Editor. La ejecución en Quest requiere configurar y verificar el entorno Android/XR; abrir el proyecto no certifica esa configuración.

## Experiencia prevista

El recorrido obligatorio definido en el [PRD del proyecto](docs/Taller%20VR%20CENART%202026-10%20-%20PRD.md) contempla:

- Al menos dos zonas conectadas mediante teleportación.
- Tres acertijos encadenados con acciones principales distintas.
- Interacciones de agarre y socket, y botones o poke; la mecánica concreta del tercer reto queda por definir en el PRD.
- Pistas, retroalimentación y ambientación que acompañen el recorrido.
- Comprobación principalmente en simulador y pruebas de interacción y confort en Quest 3S.

El objetivo docente es que los participantes puedan construir el recorrido obligatorio **sin programar**, utilizando componentes y eventos del Inspector. El PRD es la fuente de verdad del alcance y de sus criterios de aceptación.

### Progresión del taller

| Clase | Resultado previsto |
| --- | --- |
| 1 · Espacio | Escena propia recorrible en simulador. |
| 2 · Interacción | Objeto agarrable que activa una consecuencia al colocarlo en un socket. |
| 3 · Navegación e interfaces | Dos zonas conectadas por teleportación, desbloqueo y panel de pistas. |
| 4 · Integración | Tres acertijos distintos encadenados y recorrido completo con ambientación y retroalimentación. |
| 5 · Despliegue | APK y presentación; prueba en Quest según disponibilidad, con simulador como alternativa. |

Estos son objetivos de construcción, no una lista de funcionalidades ya terminadas.

## Tecnología

Versiones declaradas en la configuración del repositorio:

| Componente | Versión |
| --- | --- |
| Unity Editor | `6000.6.5f1` |
| Universal Render Pipeline (URP) | `17.6.0` |
| XR Interaction Toolkit | `3.6.1` |
| XR Hands | `1.9.0` |
| OpenXR Plugin | `1.18.0` |
| Unity OpenXR: Meta | `2.6.1` |
| Input System | `1.20.1` |

Consulta [`Packages/manifest.json`](Packages/manifest.json) para ver las dependencias directas y [`ProjectSettings/ProjectVersion.txt`](ProjectSettings/ProjectVersion.txt) para confirmar la versión del Editor. No actualices Unity o los paquetes implícitamente al abrir el proyecto.

## Estructura del repositorio

| Ruta | Contenido |
| --- | --- |
| `Assets/Scenes/` | Escenas `SampleScene` y `BasicScene`, además de recursos asociados. |
| `Assets/VRTemplateAssets/` | Recursos y scripts de la plantilla VR. |
| `Assets/Samples/` | Ejemplos importados de paquetes Unity. |
| `Assets/Synty/` | Recursos visuales de terceros. |
| `Assets/XR/`, `Assets/XRI/` | Recursos y configuración relacionados con XR. |
| `Assets/Settings/` | Recursos de configuración del proyecto. |
| `Packages/` | Manifiesto y archivo de bloqueo de dependencias. |
| `ProjectSettings/` | Configuración del Editor y del proyecto. |
| `docs/` | PRD y documentación del proyecto. |

## Antes de modificar o compilar

- Conserva los archivos `.meta` y sus GUID al mover o renombrar recursos.
- Mantén versionados `Assets/`, `Packages/` y `ProjectSettings/`. No agregues carpetas generadas como `Library/`, `Temp/`, `Logs/`, `UserSettings/` o `Builds/`.
- Los samples importados de **XR Hands 1.8.1** y **XR Interaction Toolkit 3.5.1** son anteriores a los paquetes instalados. Revisa compatibilidad y cambios locales antes de reimportarlos.
- La configuración de compilación incluye actualmente solo `Assets/Scenes/SampleScene.unity`; `BasicScene` no está incluida. Revisa la escena y la plataforma de destino antes de generar un APK.
- La presencia de paquetes XR no garantiza que el simulador o el visor estén listos. Verifica la configuración y prueba el recorrido en el entorno correspondiente.
- Los recursos de terceros están sujetos a sus licencias respectivas; no presupongas permiso de redistribución por estar presentes en este repositorio.

Para trabajar con agentes de desarrollo, consulta también [`AGENTS.md`](AGENTS.md).
