using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
//using FMODUnity;
using UnityEngine;

public static class BattleUtils
{
    // Returns all fish in range including itself
    public static List<Fish> GetFishInRange(Vector3 position, float range) {
	    // since there are no walls (for now?) in the AR version, and the number of fish will always be trivial: brute force

	    var allFish = new List<Fish>();
	    allFish.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.A).allFish);
	    allFish.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.B).allFish);
	    
        List<Fish> outList = new List<Fish>();
        range *= range;
        foreach (var fish in allFish) {
	        if (!fish)
		        continue;
	        var toOther = fish.transform.position - position;
	        var sqrDist = Vector3.Dot(toOther, toOther);
	        if (sqrDist < range)
		        outList.Add(fish);
        }

        return outList;
        
        /*Collider[] hitColliders = Physics.OverlapSphere(position, range, (1 << 19)); // fish layer is layer 19
        foreach (var hitCollider in hitColliders) {
            var fish = hitCollider.GetComponentInParent<Fish>();
            if (fish) {
				// make sure there is no wall in the way; walls/nets are on default collision layer (0)
				RaycastHit hitInfo;
				var toFish = fish.transform.position - position;
				toFish.Normalize();
				if (!Physics.Raycast(position, toFish, out hitInfo,  toFish.magnitude, (1 << 0)))
					list.Add(fish);
            }
        }

        return list;*/
    }

    // Returns the closest fish in range
    public static bool GetClosestInRange(Fish source, float range, out Fish result, Func<Fish, bool> shouldInclude) {
        var minDist = range * range;
        Fish minFish = null;
        
        // since there are no walls (for now?) in the AR version, and the number of fish will always be trivial: brute force

        var allFish = new List<Fish>();
        allFish.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.A).allFish);
        allFish.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.B).allFish);

        foreach (var fish in allFish) {
	        if (fish && fish != source && shouldInclude(fish)) {
		        var toOther = fish.transform.position - source.transform.position;
		        var sqrDist = Vector3.Dot(toOther, toOther);
		        if (sqrDist < minDist) {
			        minDist = sqrDist;
			        minFish = fish;
		        }
	        }
        }

        result = minFish;
        return minFish;
        
        /*Collider[] hitColliders = Physics.OverlapSphere(source.transform.position, range, (1 << 19)); // fish layer is layer 19
		foreach (var hitCollider in hitColliders)
        {
            var fish = hitCollider.GetComponentInParent<Fish>();
            if (fish && fish != source && shouldInclude(fish))
            {
				// using sqr of distance until we absolutely need it for raycast
                var sqrDist = (hitCollider.ClosestPoint(source.transform.position) - source.transform.position).sqrMagnitude;
                if (sqrDist <= minDist * minDist) {
					float dist = Mathf.Sqrt(sqrDist);
					// make sure there is no wall in the way; walls/nets are on default collision layer (0)
					RaycastHit hitInfo;
					if (!Physics.Raycast(source.transform.position, fish.transform.position - source.transform.position, out hitInfo, dist, (1 << 0))) {
						minDist = dist;
						minFish = fish;
					}
                }
            }
        }

        result = minFish;
        return minFish;*/
    }

    // stop movement when casting attack or ability
    public static IEnumerator LockPositionForCasting(Fish fish, float castTime)
    {
        float delta = 0;
        while (delta < castTime)
        {
            delta += Time.deltaTime;
            fish.RequestVelocityOverride(VelocityOverride.CAST_ATTACK, Vector3.zero);
            yield return null;
        }
    }

    public static bool ShouldHeal(Fish source, Fish target)
    {
		if (!target || !source)
			return false;
        var targetHealth = target.Health;
        bool isFriendly = source.Faction == target.Faction;
        return isFriendly &&
               targetHealth != null &&
               !targetHealth.IsFullHealth &&
               !targetHealth.IsZeroHealth &&
               !target.Defeated;
    }

    public static bool ShouldDamage(Fish source, Fish target)
    {
        var targetHealth = target.Health;
        bool isFriendly = source.Faction == target.Faction;
        return !isFriendly &&
               targetHealth != null &&
               !targetHealth.IsZeroHealth &&
               !target.Defeated;
    }

	public static bool IsA(Fish fish) {
		return (fish.Faction == FishFaction.A);
	}
	
	public static bool IsB(Fish fish) {
		return (fish.Faction == FishFaction.B);
	}
}