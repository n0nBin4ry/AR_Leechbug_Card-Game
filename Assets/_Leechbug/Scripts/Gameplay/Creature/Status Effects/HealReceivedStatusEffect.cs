using System.Collections.Generic;
using UnityEngine;

public class HealReceivedStatusEffect : TimedStatusEffect
{
    [SerializeField] private float statusDuration;
    public Fish SourceFish;

    public void Initialize(Fish fish, Fish source)
    {
        SourceFish = source;

        for(var i=fish.StatusEffects.Count-1; i>=0; i--)
        {
            var status = fish.StatusEffects[i];
            if (status.Id == this.Id &&
                ((HealReceivedStatusEffect) status).SourceFish == this.SourceFish)
            {
               status.Terminate();
            }
        }

        base.Initialize(fish, statusDuration);
    }

    public override void Initialize(Fish fish)
    {
        Fish = fish;
        RegisterEffect(fish);
    }

    public override string ToString()
    {
        int count = 0;
        foreach (var effect in Fish.StatusEffects)
        {
            if (effect.Id == Id)
                count++;
        }
        
        if (count > 1)
            return Title + " x" + count;
        return Title;
    }
}