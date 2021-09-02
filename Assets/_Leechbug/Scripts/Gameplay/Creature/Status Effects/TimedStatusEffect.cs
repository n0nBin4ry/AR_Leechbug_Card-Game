using System.Collections.Generic;
using UnityEngine;

public abstract class TimedStatusEffect : AStatusEffect
{
    protected float _duration;
    protected float _stackTimer;
    private List<float> _timers = new List<float>();
    
    public override void Tick(float deltaTime)
    {
        if (Stack <= 0)
        {
            Terminate();
        }
        else
        {
            for (int i=_timers.Count-1; i>=0; i--)
            {
                _timers[i] += Time.deltaTime;
                _stackTimer = _timers[i];
                if (_timers[i] >= _duration)
                {
                    RemoveStack();
                    _timers.RemoveAt(i);
                }
            }
        }
    }

    public virtual void Initialize(Fish fish, float duration)
    {
        _duration = duration;
        Initialize(fish);
    }

    public override void AddStack()
    {
        base.AddStack();
        _timers.Add(0);
    }

    public float getDuration() { return _duration;}
    public float getStackTimer() { return _stackTimer;}
}