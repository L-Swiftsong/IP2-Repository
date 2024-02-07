using UnityEngine;

namespace HFSM
{
    /// <summary> A default timer that calculates the elapsed time based on Time.time</summary>
    public class Timer : ITimer
    {
        private float _startTime;
        public float Elapsed => Time.time - _startTime;

        public Timer() => _startTime = Time.time;
        public void Reset() => _startTime = Time.time;

        public static bool operator >(Timer timer, float duration) => timer.Elapsed > duration;
        public static bool operator <(Timer timer, float duration) => timer.Elapsed < duration;
        public static bool operator >=(Timer timer, float duration) => timer.Elapsed >= duration;
        public static bool operator <=(Timer timer, float duration) => timer.Elapsed <= duration;
    }
}