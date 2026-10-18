using UnityEngine;

namespace TallerCenart
{
    /// <summary>
    /// Returns this object to its starting pose when it falls below Min Height, or when <see cref="ResetPose"/> is called.
    /// </summary>
    public class Respawn : MonoBehaviour
    {
        [Tooltip("World height below which the object is returned to its start.")]
        public float minHeight = -2f;

        Vector3 startPosition;
        Quaternion startRotation;
        Rigidbody body;

        void Awake()
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
            body = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (transform.position.y < minHeight)
                ResetPose();
        }

        public void ResetPose()
        {
            if (body != null)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
            transform.SetPositionAndRotation(startPosition, startRotation);
        }
    }
}
