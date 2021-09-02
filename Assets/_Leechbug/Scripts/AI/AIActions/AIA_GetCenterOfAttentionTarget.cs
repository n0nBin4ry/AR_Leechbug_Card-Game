using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GetCenterOfAttentionTarget", menuName = "FishAI/Actions/GetCenterOfAttentionTarget")]
public class AIA_GetCenterOfAttentionTarget : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller == null || controller.ParentFish.Defeated)
			return;

		// manual-override check
		if (controller.FishAIStateInfo.EnemyManuallyAssigned) {
			// if there is no target or the target is dead, find no target
			if (controller.FishAIStateInfo.EnemyTarget == null || controller.FishAIStateInfo.EnemyTarget.Defeated)
				controller.FishAIStateInfo.EnemyManuallyAssigned = false;
			// IDEA: if calm-clear-manual idea doesnt work try checking against enemies in combat (if there are enemies in combat)

			// if we have a manual target, then dont get a new target
			else
				return;
		}

		// get a list of this fish's enemies
		List<Fish> enemies = new List<Fish>();
		if (controller.ParentFish.Faction == FishFaction.B)
			enemies.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.A).allFish);
		else
			enemies.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.B).allFish);

			// go through each enemy fish and target the one surrounded by most fish
		int maxFish = -1;
		float minDist = float.MaxValue;
		Fish targ = null;

		foreach (var fish in enemies) {
			var hitColliders = Physics.OverlapSphere(fish.transform.position, controller.ParentFish.Data.DetectionRange);
			int fishInRadius = 0;
			float curDist = Vector3.SqrMagnitude(controller.ParentFish.transform.position - fish.transform.position);

			for (var i = 0; i < hitColliders.Length; i++) {
				if (hitColliders[i].tag == "Fish") {
					fishInRadius += 1;
				}
			}

			if (fishInRadius > maxFish) {
				minDist = curDist;
				maxFish = fishInRadius;
				targ = fish;
			}
			// if there are 2 fish with the same surrounding density, then pick the closest one
			if ((fishInRadius == maxFish) && (curDist < minDist)) {
					minDist = curDist;
					targ = fish;
			}
		}

		controller.FishAIStateInfo.EnemyTarget = targ;
	}
}