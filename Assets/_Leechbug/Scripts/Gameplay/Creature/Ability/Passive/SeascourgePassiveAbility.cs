using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeascourgePassiveAbility : APassiveAbility
{
    [SerializeField] public float debuffRange;

    public override void Initialize(Fish parentFish)
    {
        base.Initialize(parentFish);

        foreach(AStatusEffect statusEffect in ParentFish.StatusEffects)
        {
            if(statusEffect.EffectType == StatusEffectType.Debuff)
            {
                statusEffect.OnTerminate += DistributeDebuffs;
            }
        }

        parentFish.OnAddStatusEffect += RegisterPassiveAbility;
    }

    public void RegisterPassiveAbility(AStatusEffect statusEffect)
    {
        if(statusEffect.EffectType == StatusEffectType.Debuff)
        {
            statusEffect.OnTerminate += DistributeDebuffs;
        }
    }

    public void DistributeDebuffs(AStatusEffect statusEffect)
    {
        var inRange = (BattleUtils.GetFishInRange(ParentFish.transform.position, debuffRange));

        foreach (var fish in inRange)
        {
            // if there is an enemy fish fish in range, drop the debuff
            if (IsValidTarget(fish))
            {
                var effectInst = Instantiate(statusEffect);
                effectInst.Initialize(fish);
            }
        }
    }

    private bool IsValidTarget(Fish target)
    {
        return target != ParentFish && target.Faction != ParentFish.Faction;
    }
}
