using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GetEnemyTargetAttackingSupportTarget", menuName = "FishAI/Actions/GetEnemyTargetAttackingSupportTarget")]
public class AIA_GetEnemyTargetAttackingSupportTarget : AIAction {
	public override void Act(BaseAIController controller) {
		// if we have no support target, then we cant find an enemy targeting them
		if (controller == null || controller.ParentFish.Defeated
		|| controller.FishAIStateInfo.SupportTarget == null || controller.FishAIStateInfo.SupportTarget.Defeated)
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

		// go through your enemies and find the one targeting your support target
		Fish newTarget = null;
		if (controller.ParentFish.Faction == FishFaction.B) {
			foreach (var fish in CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.A).allFish) {
				if (fish && !(fish.Defeated) 
				&& fish.Controller.GetType() == typeof(BaseAIController) && ((BaseAIController)(fish.Controller)).FishAIStateInfo.EnemyTarget == controller.FishAIStateInfo.SupportTarget) {
					newTarget = fish;
					break;
				}
			}
		}
		else if (controller.ParentFish.Faction == FishFaction.A) {
			foreach (var fish in CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.A).allFish) {
				if (fish && !(fish.Defeated)
					&& fish.Controller.GetType() == typeof(BaseAIController) && ((BaseAIController)(fish.Controller)).FishAIStateInfo.EnemyTarget == controller.FishAIStateInfo.SupportTarget) {
					newTarget = fish;
					break;
				}
			}
		}
		else {
			// no other faction should call this
			controller.FishAIStateInfo.EnemyTarget = null;
			return;
		}

		// assign new target
		controller.FishAIStateInfo.EnemyTarget = newTarget;
	}
}