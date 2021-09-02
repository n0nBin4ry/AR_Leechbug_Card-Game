using System;
using System.Collections.Generic;
using UnityEngine;

public class MoonrayDancerPassiveAbility : APassiveAbility
{
        [SerializeField] public float healRange, buffRange;
        [SerializeField] private MoonrayGliderAttackUpEffect statusEffect;
        
        private ModifyDamageDealtEffect statusEffectInstance;
        private float _timer;

        private List<Fish> fishInRange;

        public override void Tick(float deltaTime)
        {
            var inRange = (BattleUtils.GetFishInRange(ParentFish.transform.position, healRange));

            foreach (var fish in inRange)
            {
                // if fish in range, give a new status effect if doesn't already
                if (IsValidTarget(fish))
                {
                    if (fish.HasStatusEffect(StatusEffect.MoonrayDamageBuff, out var effect) &&
                        effect.Fish == ParentFish)
                    {
                        continue;
                    }
                    var effectInst = Instantiate(statusEffect);
                    effectInst.Initialize(fish);
                    effectInst.Init(ParentFish, buffRange);
                }
            }
        }
        
        private bool IsValidTarget(Fish target)
        {
            return target != ParentFish &&
                   target.Faction == ParentFish.Faction && 
                   target.HasStatusEffect(StatusEffect.HealReceived, out var e);
        }
}