using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryProjectileAttack : ATargetedActiveAbility
{
    [Header("Specific variables for this attack")]
    [SerializeField] public float speed;
    [SerializeField] public float castTime;
    [SerializeField] BasicProjectile projectilePrefab;
    [SerializeField] private GameObject rangedHitEffect;
    //[SerializeField] private GameObject shootSfx;

    public override AbilityType GetAbilityType() => AbilityType.SINGLE_TARGET_OFFENSE;

    protected override void UseAbility()
    {
        //Instantiate(shootSfx, transform.position, Quaternion.identity);
        StartCoroutine(BattleUtils.LockPositionForCasting(ParentFish, castTime));

        var projectile = Instantiate(projectilePrefab, ParentFish.transform.position, ParentFish.transform.rotation);

        // using Battle Utils so that the collider of the fish is included instead of its center position
        if (CurrentTarget && BattleUtils.GetFishInRange(ParentFish.transform.position, range).Contains(CurrentTarget))
        {
            //check if the parent fish is controlled by the player
            if(ParentFish.Controller.ControllerType == ControllerType.Player)
            {
                Vector3 towardTarget = (CurrentTarget.transform.position - ParentFish.transform.position).normalized;
                // check if the camera is facing toward the target. if not, shoot non-targeted projectile
                if (Vector3.Dot(Camera.main.transform.forward, towardTarget) < 0)
                {
                    projectile.Initialize(ParentFish, speed, range);
                }
                else
                {
                    ParentFish.transform.forward = towardTarget;
                    projectile.Initialize(ParentFish, CurrentTarget, speed, range);
                }
            } else
            {
                projectile.Initialize(ParentFish, CurrentTarget, speed, range);
            }
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
            target.Health.ModifyHealth(new HealthModification(-damage, ParentFish, target));
            if (rangedHitEffect)
                Instantiate(rangedHitEffect, target.transform.position, Quaternion.identity);
            Destroy(projectile.gameObject);
        }
    }

    public override bool CanAffectTarget(Fish target)
    {
        var can =  BattleUtils.ShouldDamage(ParentFish, target);

        return can;
    }
}