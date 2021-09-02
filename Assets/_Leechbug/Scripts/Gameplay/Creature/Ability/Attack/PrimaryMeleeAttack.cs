using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PrimaryMeleeAttack : ATargetedActiveAbility
{
    [Header("Specific variables for this attack")]
    // "range" in this attack means dash range, and "hit range" means how close the fish must be to land that attack
    [SerializeField] private float _hitRange;
    [SerializeField] private float _castTime;
    [SerializeField] GameObject visualEffect;

    public override AbilityType GetAbilityType() => AbilityType.SINGLE_TARGET_OFFENSE;
    protected override void UseAbility()
    {
        if (CurrentTarget && BattleUtils.GetFishInRange(ParentFish.transform.position, range).Contains(CurrentTarget))
        {
            StartCoroutine(DashAttack(CurrentTarget));
        }
        else
        {
            Instantiate(visualEffect, ParentFish.transform.position, Quaternion.identity);
        }
    }

    public override bool CanAffectTarget(Fish target)
    {
        return BattleUtils.ShouldDamage(ParentFish, target);
    }
    
    IEnumerator DashAttack(Fish target)
    {
        float delta = 0;
        bool hasAttacked = false;
        while (delta < _castTime)
        {
			// check if the attack is no longer possible
			if (ParentFish == null || ParentFish.Defeated || target == null || target.Defeated)
				yield break;
            var direction = (target.transform.position - ParentFish.transform.position);
            if (direction.magnitude > _hitRange)
            {
                ParentFish.RequestVelocityOverride(VelocityOverride.CAST_ATTACK, (range * direction.normalized) / _castTime);
                ParentFish.RequestForwardOverride(VelocityOverride.CAST_ATTACK, direction.normalized);
            }
            else
            {
                if (!hasAttacked && target && ParentFish) // attack as soon as it's in range
                {
                    hasAttacked = DoAttack(target);
                }
                ParentFish.RequestVelocityOverride(VelocityOverride.CAST_ATTACK, Vector3.zero);
                ParentFish.RequestForwardOverride(VelocityOverride.CAST_ATTACK, direction.normalized);
            }
            delta += Time.deltaTime;
            yield return null;
        }
        if (!hasAttacked && target && ParentFish) // in case it still hasn't attacked yet
        {
            DoAttack(target);
        }
    }

    bool DoAttack(Fish target)
    {
		if (ParentFish == null || ParentFish.Defeated || target == null || target.Defeated)
			return false;
		if (CanAffectTarget(target)) 
        {
            target.Health.ModifyHealth(new HealthModification(-damage, ParentFish, target));
            Instantiate(visualEffect, target.transform.position, Quaternion.identity);
            return true;
        }

        return false;
    }
}