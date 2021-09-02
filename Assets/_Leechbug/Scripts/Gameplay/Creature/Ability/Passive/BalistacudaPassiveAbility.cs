using UnityEngine;

public class BalistacudaPassiveAbility : APassiveAbility
{
    [SerializeField] private float damageMultiplier;
    public override void Initialize(Fish parentFish)
    {
        base.Initialize(parentFish);
        ParentFish.InterceptDamageDealt += BeforeDamageDealt;
    }

    private void BeforeDamageDealt(HealthModification mod)
    {
        bool debuffed = false;
        foreach (var status in mod.target.StatusEffects)
        {
            if (status.EffectType == StatusEffectType.Debuff)
            {
                debuffed = true;
                break;
            }
        }

        if (debuffed)
        {
            mod.Multiplier += damageMultiplier;
        }
    }
}