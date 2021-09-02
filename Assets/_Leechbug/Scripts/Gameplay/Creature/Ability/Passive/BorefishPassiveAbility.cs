using UnityEngine;

public class BorefishPassiveAbility : APassiveAbility
{
    [SerializeField] FishData borefishPlayerStats;
    public override void Initialize(Fish parentFish)
    {
        base.Initialize(parentFish);
        parentFish.OnDefeat += ParentFish_OnDefeat;
    }

    private void ParentFish_OnDefeat(Fish obj)
    {
        ParentFish.SetFishData(borefishPlayerStats);
    }

    public override void Tick(float deltaTime)
    {
        var currHealth = ParentFish.Health.GetHealthFraction();
        if (ParentFish.Defeated && ParentFish.Data != borefishPlayerStats)
        {
            ParentFish.SetFishData(borefishPlayerStats);
        }
    }

    public override void Terminate()
    {
        ParentFish.OnDefeat -= ParentFish_OnDefeat;
        base.Terminate();
    }
}