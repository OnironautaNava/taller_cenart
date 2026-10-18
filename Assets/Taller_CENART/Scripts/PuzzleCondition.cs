using UnityEngine;
using UnityEngine.Events;

namespace TallerCenart
{
    /// <summary>
    /// Counts Inspector-wired signals and fires <see cref="OnSolved"/> once when the count reaches Required Count.
    /// Wire an "entered/pressed" event to <see cref="Signal"/> and, optionally, the matching
    /// "exited/released" event to <see cref="Unsignal"/>. Once solved it stays solved.
    /// </summary>
    public class PuzzleCondition : MonoBehaviour
    {
        [Tooltip("Number of active signals needed to solve. Must be at least 1.")]
        public int requiredCount = 1;

        [Tooltip("Invoked once, when the count first reaches Required Count.")]
        public UnityEvent OnSolved = new UnityEvent();

        public int Count { get; private set; }
        public bool IsSolved { get; private set; }

        public void Signal()
        {
            if (IsSolved)
                return;
            if (requiredCount < 1)
            {
                Debug.LogError($"{name}: PuzzleCondition Required Count must be at least 1; the puzzle cannot be solved.", this);
                return;
            }
            Count++;
            if (Count < requiredCount)
                return;
            IsSolved = true;
            OnSolved.Invoke();
        }

        public void Unsignal()
        {
            if (!IsSolved && Count > 0)
                Count--;
        }
    }
}
