using System;
using System.Collections.Generic;
using UnityEngine;

public class SunrayDancerPassiveAbility : APassiveAbility
{
        [SerializeField] public float healCooldown;
        [SerializeField] public float healRange;
        [SerializeField] public int healAmount;
        [SerializeField] public GameObject healReceivedFx;
        [SerializeField] public HealReceivedStatusEffect healStatusEffect;

        private float _timer;

        private void Start()
        {
            transform.localScale = Vector3.one * healRange * 2;
        }

        public override void Tick(float deltaTime)
        {
            if (_timer <= 0)
            {
                _timer = healCooldown;
                Heal(BattleUtils.GetFishInRange(ParentFish.transform.position, healRange));
            }
            else
            {
                _timer -= deltaTime;
            }
        }

        void Heal(List<Fish> otherFish)
        {
            foreach (var target in otherFish)
            {
                if (BattleUtils.ShouldHeal(ParentFish, target))
                {
                    var modHealth = target.Health.ModifyHealth(new HealthModification(healAmount, ParentFish, target));
                    if (modHealth > 0)
                    {
                        Instantiate(healReceivedFx, target.transform);
                        Instantiate(healStatusEffect).Initialize(target, ParentFish);
                    }
                }
            }
        }
}