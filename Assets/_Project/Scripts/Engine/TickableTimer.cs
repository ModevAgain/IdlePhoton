using System;
using System.Collections.Generic;

namespace _Project.Scripts.Engine
{
    public class TickableTimer : ITickable
    {
        private static TickableTimer _instance;
        
        private readonly List<TimerInfo> _timers;

        public static TimerInfo Start(float seconds, Action callback)
        {
            if (_instance == null)
            {
                _instance = new();
            }
            return _instance.AddTimer(seconds, callback);
        }
        
        public TickableTimer()
        {
            _timers = new();
            IdleEngine.Root.RegisterTickable(this);
        }

        private TimerInfo AddTimer(float seconds, Action callback)
        {
            var timerInfo = new TimerInfo(seconds, callback);
            _timers.Add(timerInfo);
            return timerInfo;
        }
        
        public void Tick(uint index, float tickBalanceValue, float deltaTime)
        {
            // Remove timers that are completed or cancelled
            _timers.RemoveAll(x => x.MarkedForDeletion);
            
            foreach (var timerInfo in _timers)
            {
                timerInfo.Time += deltaTime;

                if (timerInfo.Time <= 0)
                {
                    timerInfo.Invoke();
                }
            }
        }
    }

}
public class TimerInfo
{
    public float Time;
    public bool MarkedForDeletion;
    private readonly Action TimerAction;

    public TimerInfo(float time, Action timerAction)
    {
        Time = time;
        TimerAction = timerAction;
    }

    /// <summary>
    ///     Invokes the callback that is attached to this timer. Deletes the timer on the next tick.
    /// </summary>
    public void Invoke()
    {
        TimerAction.Invoke();
        MarkedForDeletion = true;
    }
    
    /// <summary>
    ///     Deletes the current timer on the next tick. The <c>TimerAction</c> will not be invoked.
    /// </summary>
    public void Cancel() => MarkedForDeletion = true;
}
