using UnityEngine;
using UnityEngine.Events;

namespace TallerCenart
{
    /// <summary>
    /// Counts down Duration seconds after <see cref="StartTimer"/> and fires <see cref="OnTimeUp"/> once at zero.
    /// StartTimer while running restarts from the full Duration; <see cref="Stop"/> pauses without firing.
    /// On Text Changed sends the remaining time as mm:ss (partial seconds round up) for a display text.
    /// </summary>
    public class Timer : MonoBehaviour
    {
        [Tooltip("Seconds to count down.")]
        public float duration = 60f;

        [Tooltip("Start counting when this object is enabled. Off means call StartTimer from an event.")]
        public bool startOnEnable;

        [Tooltip("Invoked once each time the countdown reaches zero.")]
        public UnityEvent OnTimeUp = new UnityEvent();

        [Tooltip("Invoked with the remaining time as mm:ss, for a display text.")]
        public UnityEvent<string> OnTextChanged = new UnityEvent<string>();

        public float Remaining { get; private set; }
        public bool IsRunning { get; private set; }

        void OnEnable()
        {
            if (startOnEnable)
                StartTimer();
        }

        void Update() => Tick(Time.deltaTime);

        public void StartTimer()
        {
            Remaining = Mathf.Max(0f, duration);
            IsRunning = true;
            Show();
        }

        public void Stop() => IsRunning = false;

        public void Tick(float seconds)
        {
            if (!IsRunning)
                return;
            Remaining = Mathf.Max(0f, Remaining - seconds);
            Show();
            if (Remaining > 0f)
                return;
            IsRunning = false;
            OnTimeUp.Invoke();
        }

        void Show()
        {
            var total = Mathf.CeilToInt(Remaining);
            OnTextChanged.Invoke($"{total / 60:00}:{total % 60:00}");
        }
    }
}
