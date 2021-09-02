using UnityEngine;
using UnityEngine.Serialization;

public class ModifyDamageDealtEffect : TimedStatusEffect
{
    [SerializeField] public float Multiplier;
    public bool disableTimer;
    
    private void Modify(HealthModification healthMod)
    {
        healthMod.Multiplier += Multiplier * Stack;
    }
    
    public override void Tick(float deltaTime)
    {
        if (!disableTimer)
            base.Tick(deltaTime);
    }

    protected override void RegisterEffect(Fish fish)
    {
        base.RegisterEffect(fish);
        fish.InterceptDamageDealt += Modify;
    }

    protected override void Uninitialize()
    {
        base.Uninitialize();
        Fish.InterceptDamageDealt -= Modify;
    }
}