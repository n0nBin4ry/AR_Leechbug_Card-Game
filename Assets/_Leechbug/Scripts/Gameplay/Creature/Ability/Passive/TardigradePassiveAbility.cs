using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TardigradePassiveAbility : APassiveAbility
{
        [SerializeField] private ModifyDamageReceivedEffect invincibilityStatusEffect;
        [SerializeField] private GameObject breakingEffect;

        private ModifyDamageReceivedEffect invincibilityInstance;
        private bool hasReachedFinalFish;
        
        private void Start()
        {
            invincibilityInstance = Instantiate(invincibilityStatusEffect, transform);
            invincibilityInstance.Initialize(ParentFish);
            invincibilityInstance.disableTimer = true;
        }

        public override void Tick(float deltaTime)
        {
            var swarm = ParentFish.SwarmManagement.GetFishInSwarm();
            bool hasRemainingAllies = false;
            if (!hasReachedFinalFish)
            {
                for (int i=swarm.Count - 1; i >= 0; i--)
                {
                    var ally = swarm[i];
                    if (ally != ParentFish)
                    {
                        if (ally.Defeated)
                        {
                            // TODO: make it work with new swarm system
                            //((PlayerSwarmManagement)GameManager.instance.PlayerFish.SwarmManagement).ForceAddToSwarm(ally);
                        }
                        else
                        {
                            hasRemainingAllies = true;
                        }
                    }
                }

                if (!hasRemainingAllies)
                {
                    hasReachedFinalFish = true;
                    invincibilityInstance.Terminate();
                    Instantiate(breakingEffect, ParentFish.transform);
                }
            }
        }
}