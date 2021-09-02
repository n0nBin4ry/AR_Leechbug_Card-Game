using System.Collections;
using UnityEngine;

public class CitadelTunaPassiveAbility : APassiveAbility
{
    [SerializeField] private float _pullRange, _pullRate, _pullStrength;
    [SerializeField] private GameObject _trailEffectPrefab;
    private float _timer = 0;

    private void Start()
    {
        transform.localScale = Vector3.one * _pullRange*2;
    }

    //TODO add functions in Fish for modifying velocity that isn't a complete override
    //TODO this is spawning multiple trail effects, should optimize
    //adjust pull rate so enemy has a chance to get away
    public override void Tick(float deltaTime)
    {
        if (_timer <= 0)
        {
            _timer = _pullRate;
            var inRange = (BattleUtils.GetFishInRange(ParentFish.transform.position, _pullRange));
            foreach (var fish in inRange)
            {
                if (IsValidTarget(fish))
                {
                    PullFish(fish);
                    Instantiate(_trailEffectPrefab, fish.transform);
                }
            }
        }
        else
        {
            _timer -= deltaTime;
        }
        
    }

    private void PullFish(Fish target)
    {
        var direction = (ParentFish.transform.position - target.transform.position).normalized;
        target.RequestVelocityOverride(VelocityOverride.ANCIENT_BULWARK_TYRANT_PULL, direction * _pullStrength);
    }

    private bool IsValidTarget(Fish target)
    {
        return (target != ParentFish &&
                target.Faction != ParentFish.Faction);
    }
}