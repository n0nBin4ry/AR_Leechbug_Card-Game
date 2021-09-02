using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonStatusEffect : TimedStatusEffect
{
    public const int POISON_TICK_RATE = 1;
    [SerializeField] private float _frequency = 1;
    [SerializeField] private int _damage;
    
    private float _damageTimer;
    
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
        
        if (_damageTimer <= 0)
        {
            _damageTimer = _frequency;
            Fish.Health.ModifyHealthAbsolute(-_damage);
        }
        else
        {
            _damageTimer -= deltaTime;
        }
    }
}