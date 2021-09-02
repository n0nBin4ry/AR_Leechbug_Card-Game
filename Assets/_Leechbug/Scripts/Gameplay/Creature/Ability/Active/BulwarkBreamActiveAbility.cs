using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulwarkBreamActiveAbility : AActiveAbility
{
    [Header("Specific variables for this ability")]
    [SerializeField] private float _pullRange;
    [SerializeField] private float _pullTime;
    [SerializeField] private float _holdTime;
    [SerializeField] private GameObject _trailEffectPrefab, _pullEffectPrefab;
    [SerializeField] private GameObject _rangeIndicatorPrefab;

    //private FMODUnity.StudioEventEmitter _fmodEmitter;

    private void Start()
    {
        //_fmodEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }
    public override AbilityType GetAbilityType() => AbilityType.AREA_SUPPORT;

    public override void Initialize(Fish parentFish)
    {
        ParentFish = parentFish;
    }

    protected override void UseAbility()
    {
        //_fmodEmitter.Play();
        // TODO check for line of sight
        var targets = BattleUtils.GetFishInRange(transform.position, _pullRange);
        foreach (var target in targets)
        {
            if (CanAffectTarget(target))
            {
                StartCoroutine(PullFish(target));
                Instantiate(_trailEffectPrefab, target.transform);
            }
        }

        var pullVfx = Instantiate(_pullEffectPrefab, transform);
        pullVfx.transform.localScale = _pullRange * 2 * Vector3.one;
    }

    public override bool CanAffectTarget(Fish target)
    {
        return target.Faction != ParentFish.Faction && 
               target != ParentFish;
    }

    IEnumerator PullFish(Fish target)
    {
        float delta = 0;
        while (delta < _pullTime)
        {
            var direction = (ParentFish.transform.position - target.transform.position);
            target.RequestVelocityOverride(VelocityOverride.SUNSPOT_TUNA_PULL, direction / _pullTime);
            delta += Time.deltaTime;
            yield return null;
        }

        delta = 0;
        while (delta < _holdTime)
        {
            target.RequestVelocityOverride(VelocityOverride.SUNSPOT_TUNA_PULL, Vector3.zero);
            delta += Time.deltaTime;
            yield return null;
        }
    }
}