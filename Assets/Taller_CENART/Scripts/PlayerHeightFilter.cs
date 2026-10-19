using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace TallerCenart
{
    /// <summary>
    /// XRI hover/select filter that accepts an interaction only while Player stands at or above Min Height.
    /// Add it to a high teleport area's Starting Hover Filters and Starting Select Filters: from the floor the area
    /// is not a valid destination (the player must climb), and once up there the player teleports over it freely.
    /// Empty Player never blocks.
    /// </summary>
    public class PlayerHeightFilter : MonoBehaviour, IXRHoverFilter, IXRSelectFilter
    {
        [Tooltip("Player's floor position, normally the XR Origin.")]
        public Transform player;

        [Tooltip("World height the player must stand at or above.")]
        public float minHeight = 1.5f;

        public bool canProcess => isActiveAndEnabled;

        public bool Allows() => player == null || player.position.y >= minHeight;

        public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable) => Allows();

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable) => Allows();
    }
}
