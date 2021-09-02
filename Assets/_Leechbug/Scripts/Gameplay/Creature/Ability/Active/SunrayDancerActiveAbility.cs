using System.Collections;
using UnityEngine;

public class SunrayDancerActiveAbility : AActiveAbility
{
    [Header("Specific variables for this ability")]

    [SerializeField] private float _dashSeconds;
    [SerializeField] private float _dashDistance;
    [SerializeField] private TrailRenderer _trailEffectPrefab;

    private TrailRenderer _trail;
    //private FMODUnity.StudioEventEmitter _fmodEmitter;

    private void Start()
    {
        //_fmodEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    public override AbilityType GetAbilityType() => AbilityType.MOBILITY;
    protected override void UseAbility()
    {
        //_fmodEmitter.Play();
        _trail = Instantiate(_trailEffectPrefab, ParentFish.transform);
        _trail.time = _dashSeconds;
        StartCoroutine(Dash());
    }

    public override bool CanAffectTarget(Fish target)
    {
        return true;
    }
    
    IEnumerator Dash()
    {
        _trail.enabled = true;
        float delta = 0;
        var direction = ParentFish.TargetForward.normalized;
        while (delta < _dashSeconds)
        {
            ParentFish.RequestVelocityOverride(VelocityOverride.SUNRAY_DANCER_DASH, (_dashDistance * direction) / _dashSeconds);
            delta += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(_dashSeconds); // extend trail time
        if (_trail)
            Destroy(_trail.gameObject);
    }
}
