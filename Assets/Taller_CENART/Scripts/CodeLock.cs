using UnityEngine;
using UnityEngine.Events;

namespace TallerCenart
{
    /// <summary>
    /// Recognizes an ordered sequence of digit buttons. Wire each button's press event to <see cref="Press"/>
    /// with its digit (0-9). When as many digits as the Code has are entered, a match fires <see cref="OnSolved"/>
    /// once; a mismatch fires <see cref="OnWrong"/> and clears the input so the player can retry.
    /// </summary>
    public class CodeLock : MonoBehaviour
    {
        [Tooltip("Digits to enter, in order, for example 1994.")]
        public string code = "1234";

        [Tooltip("Invoked once, when the full code is entered correctly.")]
        public UnityEvent OnSolved = new UnityEvent();

        [Tooltip("Invoked when a full-length entry is wrong. The input is cleared to retry.")]
        public UnityEvent OnWrong = new UnityEvent();

        [Tooltip("Invoked with the entry padded with '_' to the code length, for a display text.")]
        public UnityEvent<string> OnInputChanged = new UnityEvent<string>();

        public string Input { get; private set; } = "";
        public bool IsSolved { get; private set; }

        public void Press(int digit)
        {
            if (IsSolved)
                return;
            if (!IsValidCode())
            {
                Debug.LogError($"{name}: CodeLock Code must be one or more digits; the lock cannot be solved.", this);
                return;
            }
            if (digit < 0 || digit > 9)
            {
                Debug.LogError($"{name}: CodeLock.Press expects a digit 0-9, got {digit}.", this);
                return;
            }
            Input += digit;
            if (Input.Length < code.Length)
            {
                Show();
                return;
            }
            if (Input == code)
            {
                IsSolved = true;
                Show();
                OnSolved.Invoke();
                return;
            }
            Input = "";
            Show();
            OnWrong.Invoke();
        }

        public void Clear()
        {
            if (IsSolved)
                return;
            Input = "";
            Show();
        }

        void Start() => Show();

        void Show() => OnInputChanged.Invoke(Input.PadRight(code.Length, '_'));

        bool IsValidCode()
        {
            if (string.IsNullOrEmpty(code))
                return false;
            foreach (var c in code)
                if (c < '0' || c > '9')
                    return false;
            return true;
        }
    }
}
