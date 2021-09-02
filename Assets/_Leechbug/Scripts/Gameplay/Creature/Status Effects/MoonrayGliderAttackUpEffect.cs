using UnityEngine;

public class MoonrayGliderAttackUpEffect : AStatusEffect
{
    [SerializeField] private ModifyDamageDealtEffect attackUpStatusEffect;
    public Fish SourceFish { get; private set; }
    private float sourceFishRange;
    
    private void Modify(HealthModification healthMod)
    {
        healthMod.Multiplier += attackUpStatusEffect.Multiplier;
    }
    
    public override void Tick(float deltaTime)
    {
        if (!BattleUtils.GetFishInRange(SourceFish.transform.position, sourceFishRange).Contains(Fish) ||
            !Fish.HasStatusEffect(StatusEffect.HealReceived, out var e))
        {
            Terminate();
        }
    }

    public void Init(Fish source, float range)
    {
        sourceFishRange = range;
        SourceFish = source;
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