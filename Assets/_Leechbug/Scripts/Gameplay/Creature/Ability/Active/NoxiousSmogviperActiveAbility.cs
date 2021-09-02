using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoxiousSmogviperActiveAbility : AActiveAbility
{
    public override AbilityType GetAbilityType() => AbilityType.SINGLE_TARGET_OFFENSE;
    [Header("Specific variables for this attack")]
    [SerializeField] public float speed;
    [SerializeField] public float castTime;
    [SerializeField] public float projectileRange;
    [SerializeField] BasicProjectile projectilePrefab;
    [SerializeField] private VirulentStatusEffect _virulentStatusEffect;
    [SerializeField] private float _virulentDuration;

    //private FMODUnity.StudioEventEmitter _fmodEmitter;

    private void Start()
    {
        //_fmodEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    protected override void UseAbility()
    {
        //_fmodEmitter.Play();
        StartCoroutine(BattleUtils.LockPositionForCasting(ParentFish, castTime));
        var projectile = Instantiate(projectilePrefab, ParentFish.transform.position, ParentFish.transform.rotation);

        if (BattleUtils.GetClosestInRange(ParentFish, projectileRange, out var closest, CanAffectTarget))
        {
            projectile.Initialize(ParentFish, closest, speed, projectileRange);
        }
        else
        {
            projectile.Initialize(ParentFish, speed, projectileRange);
        }
        
        projectile.OnHit += OnHit;
    }

    private void OnHit(Fish target, BasicProjectile projectile)
    {
        if (CanAffectTarget(target))
        {
            //apply damage based on poison
            if (target.HasStatusEffect(StatusEffect.Poisoned, out var effect))
            {
                int _damage = effect.Stack * 2;
                target.Health.ModifyHealth(new HealthModification(-_damage, ParentFish, target));
                print("POISON * 2 = " + _damage);
            }

            //apply virulent
            var virulentEffect = Instantiate(_virulentStatusEffect);
            virulentEffect.Initialize(target, _virulentDuration);
            Destroy(projectile.gameObject);
        }
    }

    public override bool CanAffectTarget(Fish target)
    {
        var can =  BattleUtils.ShouldDamage(ParentFish, target);

        return can;
    }
}