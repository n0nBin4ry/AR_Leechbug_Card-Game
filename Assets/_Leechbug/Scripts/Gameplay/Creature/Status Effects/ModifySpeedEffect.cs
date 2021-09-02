using UnityEngine;

public class ModifySpeedEffect : TimedStatusEffect
{
    [SerializeField] private float _frequency;
    [SerializeField] private float _multiplier;
    
    private float _timer;
    
    public override void Tick(float deltaTime)
    {
        if (Stack <= 0)
        {
            Terminate();
        }
        else
        {
            _stackTimer += deltaTime;
            if (_stackTimer >= _duration)
            {
                RemoveStack();
                _stackTimer = 0;
            }
        }

        if (_timer <= 0)
        {
            _timer = _frequency;
            
            if (Fish.TargetVelocity != Vector3.zero) {
                Fish.RequestVelocityOverride(VelocityOverride.SPEED_DEBUFF, Fish.TargetForward*_multiplier);
            }
        }
        else
        {
            _timer -= deltaTime;
        }
    }
}