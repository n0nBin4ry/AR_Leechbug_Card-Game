using UnityEngine;

public class ModifyDamageReceivedEffect : TimedStatusEffect
{
    [SerializeField] private float _multiplier;
    public bool disableTimer;
    
    private void Modify(HealthModification healthMod)
    {
        healthMod.Multiplier += _multiplier * Stack;
    }
    
    public override void Tick(float deltaTime)
    {
        if (!disableTimer)
            base.Tick(deltaTime);
    }

    protected override void RegisterEffect(Fish fish)
    {
        base.RegisterEffect(fish);
        fish.Health.InterceptDamageReceived += Modify;
    }

    protected override void Uninitialize()
    {
        base.Uninitialize();
        Fish.Health.InterceptDamageReceived -= Modify;
    }
}