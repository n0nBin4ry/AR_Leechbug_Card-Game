using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class ShockGuppyActiveAbility : AActiveAbility
{
    [Header("Specific variables for this ability")]
    [SerializeField] private float _castSeconds;
    [SerializeField] private GameObject _shockEffectPrefab;
    [SerializeField] private ModifyDamageDealtEffect _statusEffect;
    [SerializeField] private float _damageDebuffSeconds;
    //private FMODUnity.StudioEventEmitter _fmodEmitter;

    private void Start()
    {
        //_fmodEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    public override AbilityType GetAbilityType() => AbilityType.AREA_OFFENSE;

    protected override void UseAbility()
    {
        //_fmodEmitter.Play();
        StartCoroutine(BattleUtils.LockPositionForCasting(ParentFish, _castSeconds));

        var targets = BattleUtils.GetFishInRange(ParentFish.transform.position, range);
        foreach (var target in targets)
        {
            if (CanAffectTarget(target))
            {
                target.Health.ModifyHealth(new HealthModification(-damage, ParentFish, target));

                var statusEffect = Instantiate(_statusEffect);
                statusEffect.Initialize(target, _damageDebuffSeconds);
            }
        }
        
        var effects = Instantiate(_shockEffectPrefab, ParentFish.transform);
        effects.transform.localScale = Vector3.one * range * 2;
    }

    public override bool CanAffectTarget(Fish target)
    {
        // TODO line of sight
        return (BattleUtils.ShouldDamage(ParentFish, target));
    }
}
