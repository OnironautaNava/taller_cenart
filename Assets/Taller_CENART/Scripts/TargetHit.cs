using UnityEngine;
using UnityEngine.Events;

namespace TallerCenart
{
    /// <summary>
    /// Fires <see cref="OnHit"/> each time an object with a Rigidbody (a thrown ball, for example) collides with
    /// this object's collider. Walls and floors without a Rigidbody do not count. Wire On Hit to
    /// <see cref="PuzzleCondition.Signal"/> so the puzzle solves once and later hits are ignored.
    /// </summary>
    public class TargetHit : MonoBehaviour
    {
        [Tooltip("Invoked on every hit by an object with a Rigidbody.")]
        public UnityEvent OnHit = new UnityEvent();

        void OnCollisionEnter(Collision collision) => Hit(collision.rigidbody);

        public void Hit(Rigidbody body)
        {
            if (body != null)
                OnHit.Invoke();
        }
    }
}
