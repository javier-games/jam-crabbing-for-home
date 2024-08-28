using System.Collections;
using UnityEngine;

namespace CrabAssets.Scripts.Game
{
    public class Timer
    {
        private readonly float _totalTime;
        private readonly TimeInEvent _timeInEvent;
        private readonly TimeOutEvent _timeOutEvent;
        private readonly TimerUpdateEvent _timerUpdateEvent;

        private Coroutine _coroutine;
        private float _currentTime;

        public bool IsRunning => _coroutine != null;

        public Timer(float time, TimeInEvent timeIn, TimeOutEvent timeOut, TimerUpdateEvent timeUpdate)
        {
            _timeOutEvent = timeOut;
            _timerUpdateEvent = timeUpdate;
            _timeInEvent = timeIn;
            _totalTime = time;
        }

        public void Start(MonoBehaviour monoBehaviour)
        {
            if (_coroutine != null)
            {
                monoBehaviour.StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = monoBehaviour.StartCoroutine(Coroutine());
        }

        private IEnumerator Coroutine()
        {
            _currentTime = _totalTime;
            _timeInEvent?.Invoke();
            _timerUpdateEvent?.Invoke(1);
            
            while (_currentTime > 0 && _totalTime > 0)
            {
                yield return null;
                _currentTime -= Time.deltaTime;
                _timerUpdateEvent?.Invoke(_currentTime / _totalTime);
            }
            
            _timerUpdateEvent?.Invoke(0);
            _timeOutEvent?.Invoke();
        }

        public void Stop(MonoBehaviour monoBehaviour)
        {
            if (_coroutine == null)
            {
                return;
            }
            
            monoBehaviour.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public delegate void TimeInEvent();
    public delegate void TimeOutEvent();
    public delegate void TimerUpdateEvent(float progress);
}