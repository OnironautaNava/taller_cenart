using UnityEngine;

namespace TallerCenart
{
    /// <summary>
    /// Moves and rotates Target between its starting local pose (closed) and that pose plus
    /// Open Offset / Open Rotation (open). Call <see cref="Open"/>, <see cref="Close"/> or <see cref="Toggle"/> from Inspector events.
    /// </summary>
    public class Door : MonoBehaviour
    {
        [Tooltip("Object that moves. Empty means this object. Rotation pivots on its own origin.")]
        public Transform target;

        [Tooltip("Local position added when open.")]
        public Vector3 openOffset;

        [Tooltip("Local rotation (degrees) added when open.")]
        public Vector3 openRotation = new Vector3(0, 90, 0);

        [Tooltip("Seconds to open or close. 0 snaps instantly.")]
        public float duration = 1f;

        public bool IsOpen { get; private set; }

        Vector3 closedPosition;
        Quaternion closedRotation;
        float progress;
        bool initialized;

        void Awake() => Initialize();

        public void Open() => Set(true);
        public void Close() => Set(false);
        public void Toggle() => Set(!IsOpen);

        void Initialize()
        {
            if (initialized)
                return;
            if (target == null)
                target = transform;
            closedPosition = target.localPosition;
            closedRotation = target.localRotation;
            initialized = true;
        }

        void Set(bool open)
        {
            Initialize();
            IsOpen = open;
            if (duration <= 0)
                Apply(open ? 1 : 0);
        }

        void Update()
        {
            var goal = IsOpen ? 1f : 0f;
            if (initialized && progress != goal)
                Apply(Mathf.MoveTowards(progress, goal, Time.deltaTime / duration));
        }

        void Apply(float value)
        {
            progress = value;
            target.localPosition = closedPosition + openOffset * value;
            target.localRotation = closedRotation * Quaternion.Euler(openRotation * value);
        }
    }
}
