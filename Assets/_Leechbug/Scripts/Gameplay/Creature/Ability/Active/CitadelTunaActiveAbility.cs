using UnityEngine;

public class CitadelTunaActiveAbility : AActiveAbility
{
    public override AbilityType GetAbilityType() => AbilityType.AREA_SUPPORT;
     [Header("Specific variables for this ability")]
    [SerializeField] private ModifyDamageReceivedEffect _statusEffect;
    [SerializeField] private GameObject _rangeIndicatorPrefab;

    [SerializeField] private float _range;
    [SerializeField] private float _duration;
    [SerializeField] private GameObject _buffReceivedFx;

    //private FMODUnity.StudioEventEmitter _fmodEmitter;

    private void Start()
    {
        //_fmodEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }
    protected override void UseAbility()
    {
        //_fmodEmitter.Play();
        var targets = BattleUtils.GetFishInRange(ParentFish.transform.position, _range);
        foreach (var target in targets)
        {
            if (CanAffectTarget(target))
            {
                Instantiate(_buffReceivedFx, target.transform);
                var statusEffect = Instantiate(_statusEffect, target.transform);
                statusEffect.Initialize(target, _duration);
            }
        }

        var range = Instantiate(_rangeIndicatorPrefab, transform.position, Quaternion.identity);
        range.transform.parent = this.gameObject.transform;
        var rangeScript = range.GetComponent<RangeIndicator>();
        rangeScript.SetCircleAnimating(0.5f, _range); //0.5f surrounds just the creature
    }

    public override bool CanAffectTarget(Fish target)
    {
        return (target != ParentFish &&
                target.Faction == ParentFish.Faction);
    }
}