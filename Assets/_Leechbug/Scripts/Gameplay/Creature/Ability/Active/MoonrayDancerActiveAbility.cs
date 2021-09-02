using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class MoonrayDancerActiveAbility : AActiveAbility
{
    public override AbilityType GetAbilityType() => AbilityType.SINGLE_TARGET_SUPPORT;

    [Header("Specific variables for this ability")]
    [SerializeField] private GameObject healReceivedFx, healSourceFx, healTrailFx;
    [SerializeField] private HealReceivedStatusEffect healStatusEffect;
    [SerializeField] private float healTrailLerp;
    
    //private FMODUnity.StudioEventEmitter _fmodEmitter;

    private void Start()
    {
        //_fmodEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }
    protected override void UseAbility()
    {
        //_fmodEmitter.Play();
        Instantiate(healSourceFx, ParentFish.transform);
        if (BattleUtils.GetClosestInRange(ParentFish, range, out var fish, this.CanAffectTarget))
        {
            var amount = fish.Health.ModifyHealth(new HealthModification(healing, ParentFish, fish));
            if (amount > 0)
            {
                StartCoroutine(AnimateTrail(fish));
                Instantiate(healStatusEffect).Initialize(fish, ParentFish);
            }
        }
    }

    IEnumerator AnimateTrail(Fish fish)
    {
        Instantiate(healReceivedFx, fish.transform);
        var trail = Instantiate(healTrailFx, ParentFish.transform.position, Quaternion.identity);
        float delta = 0;
        while (fish && !fish.Defeated && (trail.transform.position - fish.transform.position).magnitude > 0.01f && delta < 1)
        {
            trail.transform.position = Vector3.Lerp(trail.transform.position, fish.transform.position, healTrailLerp * Time.deltaTime);
            delta += Time.deltaTime;
            yield return null;
        }
    }

    public override bool CanAffectTarget(Fish target)
    {
        return BattleUtils.ShouldHeal(ParentFish, target)
               && target != ParentFish;
    }
}