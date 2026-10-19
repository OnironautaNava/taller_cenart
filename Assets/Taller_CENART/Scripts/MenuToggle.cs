using UnityEngine;
using UnityEngine.InputSystem;

namespace TallerCenart
{
    /// <summary>
    /// Shows or hides Menu when Toggle Action is performed (default: menu button of either controller; on Quest
    /// only the left one is available, the right one is the system button; M in the XR Interaction Simulator).
    /// On opening, places Menu in front of Head at eye height, facing the player.
    /// <see cref="Toggle"/> can also be called from a UI button.
    /// </summary>
    public class MenuToggle : MonoBehaviour
    {
        [Tooltip("Button that opens and closes the menu.")]
        public InputActionProperty toggleAction = new InputActionProperty(DefaultAction());

        [Tooltip("World-space menu to show and hide.")]
        public GameObject menu;

        [Tooltip("Player head. Empty means the main camera.")]
        public Transform head;

        [Tooltip("Meters in front of the head where the menu opens.")]
        public float distance = 1f;

        static InputAction DefaultAction()
        {
            var action = new InputAction("Toggle Menu", InputActionType.Button, "<XRController>{LeftHand}/{MenuButton}");
            action.AddBinding("<XRController>{RightHand}/{MenuButton}");
            return action;
        }

        void OnEnable()
        {
            toggleAction.action.performed += OnToggle;
            toggleAction.action.Enable();
        }

        void OnDisable() => toggleAction.action.performed -= OnToggle;

        void OnToggle(InputAction.CallbackContext _) => Toggle();

        public void Toggle()
        {
            if (menu == null)
            {
                Debug.LogWarning($"{name}: MenuToggle has no menu.", this);
                return;
            }
            if (!menu.activeSelf)
                PlaceInFront();
            menu.SetActive(!menu.activeSelf);
        }

        void PlaceInFront()
        {
            if (head == null && Camera.main != null)
                head = Camera.main.transform;
            if (head == null)
                return;
            var forward = Vector3.ProjectOnPlane(head.forward, Vector3.up).normalized;
            if (forward == Vector3.zero)
                forward = Vector3.ProjectOnPlane(head.up, Vector3.up).normalized;
            menu.transform.SetPositionAndRotation(head.position + forward * distance, Quaternion.LookRotation(forward));
        }
    }
}
