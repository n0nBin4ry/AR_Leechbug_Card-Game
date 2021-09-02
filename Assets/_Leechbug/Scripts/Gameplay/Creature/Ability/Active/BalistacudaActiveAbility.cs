using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalistacudaActiveAbility : AActiveAbility
{
    [Header("Specific variables for this ability")]
    [SerializeField] private float _pushRange;
    [SerializeField] private float _pushTime;
    [SerializeField] private GameObject _trailEffectPrefab, _pushEffectPrefab;
    [SerializeField] private float _angle;

    //private FMODUnity.StudioEventEmitter _fmodEmitter;

    private void Start()
    {
        //_fmodEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }
    public override AbilityType GetAbilityType() => AbilityType.SINGLE_TARGET_OFFENSE;

    public override void Initialize(Fish parentFish)
    {
        ParentFish = parentFish;
    }

    protected override void UseAbility()
    {
        //_fmodEmitter.Play();
        var targets = BattleUtils.GetFishInRange(transform.position, range);
        foreach (var target in targets)
        {
            if (CanAffectTarget(target))
            {
                StartCoroutine(LaunchFish(target));
                Instantiate(_trailEffectPrefab, target.transform);

                if (BattleUtils.ShouldDamage(ParentFish, target))
                {
                    target.Health.ModifyHealth(new HealthModification(-damage, ParentFish, target));
                }
            }
        }
        Instantiate(_pushEffectPrefab, transform);
    }

    public override bool CanAffectTarget(Fish target)
    {
        return target != ParentFish &&
               Vector3.Angle((target.transform.position - ParentFish.transform.position), ParentFish.transform.forward) <= _angle;
    }

    IEnumerator LaunchFish(Fish target)
    {
        float delta = 0;
        var direction = ParentFish.transform.forward;
        while (delta < _pushTime)
        {
            target.RequestVelocityOverride(VelocityOverride.ROCKETGUPPPY_LAUNCH, (direction * _pushRange) / _pushTime);
            delta += Time.deltaTime;
            yield return null;
        }
    }
}