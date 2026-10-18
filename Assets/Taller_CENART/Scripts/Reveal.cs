using UnityEngine;

namespace TallerCenart
{
    /// <summary>
    /// Activates Targets on <see cref="Show"/> and deactivates them on <see cref="Hide"/>.
    /// Leave targets inactive in the scene to keep them hidden until revealed.
    /// </summary>
    public class Reveal : MonoBehaviour
    {
        [Tooltip("Objects to activate on Show and deactivate on Hide.")]
        public GameObject[] targets;

        public void Show() => SetTargetsActive(true);
        public void Hide() => SetTargetsActive(false);

        void SetTargetsActive(bool active)
        {
            if (targets == null || targets.Length == 0)
            {
                Debug.LogWarning($"{name}: Reveal has no targets.", this);
                return;
            }
            foreach (var target in targets)
            {
                if (target != null)
                    target.SetActive(active);
                else
                    Debug.LogWarning($"{name}: Reveal has an empty target slot.", this);
            }
        }
    }
}
