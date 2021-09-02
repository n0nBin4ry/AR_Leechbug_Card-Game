using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IsAnyEnemyViewed", menuName = "FishAI/Conditions/IsAnyEnemyViewed")]
public class AIC_IsAnyEnemyViewed : AICondition {
	[Header("Distance to check; if 0 or less, then uses fish's detection range.")]
	[SerializeField] private float _distance = -1f;
    public override bool Check(BaseAIController controller) {
		if (controller == null)
			return false;
		float dist = _distance;
		if (dist <= 0)
			dist = controller.ParentFish.Data.DetectionRange;

		// if friendly, search enemies
		if (controller.ParentFish.Faction == FishFaction.A) {
			// get all fish in search distance
			var nearbyOthers = BattleUtils.GetFishInRange(controller.ParentFish.transform.position, dist);
			foreach(var other in nearbyOthers) {
				if (IsFriendly(other))
					continue;
				// if any of the fish in view range then return true
				var fishToEnemy = other.transform.position - controller.ParentFish.transform.position;
				if (Vector3.Angle(fishToEnemy.normalized, controller.ParentFish.transform.forward) <= controller.ParentFish.Data.DetectionAngle)
					return true;
			}
		}

		// if enemy, search friendlies
		else if (controller.ParentFish.Faction == FishFaction.B) {
			var nearbyOthers = BattleUtils.GetFishInRange(controller.ParentFish.transform.position, dist);
			foreach (var other in nearbyOthers) {
				if (IsEnemy(other))
					continue;
				var fishToEnemy = other.transform.position - controller.ParentFish.transform.position;
				if (Vector3.Angle(fishToEnemy.normalized, controller.ParentFish.transform.forward) <= controller.ParentFish.Data.DetectionAngle)
					return true;
			}
		}
		return false;
	}

	// TODO: find a way to generalize/store this so that I dont have to copy/paste
	private bool IsFriendly(Fish fish) {
		return (fish.Faction == FishFaction.A);
	}

	private bool IsEnemy(Fish fish) {
		return (fish.Faction == FishFaction.B);
	}
}
