using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeascourgeActiveAbility : ATargetedActiveAbility
{
    [Header("Specific variables for this attack")]
    [SerializeField] public float speed;
    [SerializeField] public float castTime;
    [SerializeField] public float debuffRange;
    [SerializeField] BasicProjectile projectilePrefab;
    [SerializeField] private GameObject rangedHitEffect;

    //private FMODUnity.StudioEventEmitter _fmodEmitter;

    private void Start()
    {
        //_fmodEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }
    public override AbilityType GetAbilityType() => AbilityType.AREA_OFFENSE;

    //hit part is pretty much identical to projectile attack(because it is after all)
    protected override void UseAbility()
    {
        //_fmodEmitter.Play();
        StartCoroutine(BattleUtils.LockPositionForCasting(ParentFish, castTime));
        var projectile = Instantiate(projectilePrefab, ParentFish.transform.position, ParentFish.transform.rotation);

        // using Battle Utils so that the collider of the fish is included instead of its center position
        if (CurrentTarget && BattleUtils.GetFishInRange(ParentFish.transform.position, range).Contains(CurrentTarget))
        {
            projectile.Initialize(ParentFish, CurrentTarget, speed, range);
        }
        else
        {
            projectile.Initialize(ParentFish, speed, range);
        }

        projectile.OnHit += OnHit;
    }

    private void OnHit(Fish target, BasicProjectile projectile)
    {
        if (CanAffectTarget(target))
        {
            //give some pain to target's surroundings
            var targets = BattleUtils.GetFishInRange(target.transform.position, debuffRange);

            //get all the debuff from the target fish
            List<AStatusEffect> sharedPain = new List<AStatusEffect>();
            foreach(AStatusEffect statusEffect in target.StatusEffects)
            {
                if(statusEffect.EffectType == StatusEffectType.Debuff)
                {
                    sharedPain.Add(statusEffect);
                }
            }

            //start sharing pain with the surrounding
            foreach (var victim in targets)
            {
                if (CanAffectTarget(target))
                {
                    //give them all the pain that the target fish had
                    foreach (AStatusEffect se in sharedPain)
                    {
                        var statusEffect = Instantiate(se);
                        statusEffect.Initialize(victim);
                    }
                }
            }

            //also give damage to target fish
            target.Health.ModifyHealth(new HealthModification(-damage, ParentFish, target));
            if (rangedHitEffect)
                Instantiate(rangedHitEffect, target.transform.position, Quaternion.identity);
            Destroy(projectile.gameObject);
        }
    }

    public override bool CanAffectTarget(Fish target)
    {
        var can = BattleUtils.ShouldDamage(ParentFish, target);

        return can;
    }
}
