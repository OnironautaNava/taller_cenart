using UnityEngine;
using UnityEngine.SceneManagement;

namespace TallerCenart
{
    /// <summary>
    /// Main menu actions for UI buttons: restart the active scene or quit the app.
    /// The active scene must be in the build settings to be restarted.
    /// </summary>
    public class SceneMenu : MonoBehaviour
    {
        public void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void Quit()
        {
#if UNITY_EDITOR
            // Application.Quit is ignored in the Editor; stop Play Mode instead.
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
