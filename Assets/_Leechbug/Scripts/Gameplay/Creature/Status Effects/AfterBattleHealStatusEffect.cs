using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterBattleHealStatusEffect : TimedStatusEffect
{
    [SerializeField] private float _recoverTime = 4f;

    private float _healTimer = 0f;
    private int _hpPerSecond = 0;
    private float _secondTimer = 0f;

    public void Init()
    {
        _hpPerSecond = Mathf.CeilToInt((Fish.Health.MaxHealth - Fish.Health.CurrentHealth) / _recoverTime);
        _hpPerSecond = Mathf.Max(_hpPerSecond, 1);

        Fish.OnTerminated += FishOnOnTerminated;
    }

    private void FishOnOnTerminated(Fish obj)
    {
        Terminate();
    }

    public override void Tick(float deltaTime)
    {
        if (_healTimer <= _recoverTime)
        {
            _secondTimer += deltaTime;
            if(_secondTimer >= 1)
            {
                Fish.Health.ModifyHealthAbsolute(_hpPerSecond);
                _secondTimer = 0f;
                _healTimer += 1.0f;
            }
        }
        else
        {
            if(_hpPerSecond > 0)
            {
                Terminate();
            }
        }
    }
}
