using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GetLowestHealthSupportTarget", menuName = "FishAI/Actions/GetLowestHealthSupportTarget")]
public class AIA_GetLowestHealthSupportTarget : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller == null || controller.ParentFish.Defeated)
			return;

		// manual-override check
		if (controller.FishAIStateInfo.SupportManuallyAssigned) {
			// if there is no target or the target is dead, find no target
			if (controller.FishAIStateInfo.SupportTarget == null || controller.FishAIStateInfo.SupportTarget.Defeated)
				controller.FishAIStateInfo.SupportManuallyAssigned = false;
			// IDEA: if calm-clear-manual idea doesnt work try checking against enemies in combat (if there are enemies in combat)

			// if we have a manual target, then dont get a new target
			else
				return;
		}

		// go through your teammates and find one with lowest health ratio
		float minHealth = float.MaxValue;
		Fish newTarget = null;
		if (controller.ParentFish.Faction == FishFaction.A) {
			foreach (var fish in CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.A).allFish) {
				if (fish && fish != controller.ParentFish
				&& !(fish.Defeated)
				&& (fish.Health.GetHealthFraction() < minHealth)) {
					minHealth = fish.Health.GetHealthFraction();
					newTarget = fish;
				}
			}
		}
		else if (controller.ParentFish.Faction == FishFaction.B) {
			foreach (var fish in CombatManager.Instance.GetTeamSwarm((CombatManager.TeamType.B)).allFish) {
				if (fish && fish != controller.ParentFish
				&& !(fish.Defeated)
				&& (fish.Health.GetHealthFraction() < minHealth)) {
					minHealth = fish.Health.GetHealthFraction();
					newTarget = fish;
				}
			}
		}
		else {
			// no other faction should call this
			controller.FishAIStateInfo.SupportTarget = null;
			return;
		}

		// assign new target
		controller.FishAIStateInfo.SupportTarget = newTarget;
	}
}