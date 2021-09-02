using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GetLeastTargetedEnemyTarget", menuName = "FishAI/Actions/GetLeastTargetedEnemyTarget")]
public class AIA_GetLeastTargettedEnemyTarget : AIAction {
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

		// get collection of enemies and friendlies for our searching
		List<Fish> enemies = new List<Fish>();
		List<Fish> friendlies = new List<Fish>();
		if (controller.ParentFish.Faction == FishFaction.A) {
			enemies.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.B).allFish);
			friendlies.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.A).allFish);
		}
		else {
			enemies.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.A).allFish);
			friendlies.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.B).allFish);
		}

		// search all to see which enemy has been targeted by the least friendlies
		int minCount = int.MaxValue;
		Fish newTarg = null;
		foreach(var enemy in enemies) {
			if (!enemy)
				continue;
			int currCount = 0;
			foreach(var friendly in friendlies) {
				// only count ai-controller fish and not self either
				if (!friendly || friendly == controller.ParentFish || friendly.Controller.GetType() != typeof(BaseAIController))
					continue;
				if (enemy == ((BaseAIController)friendly.Controller).FishAIStateInfo.EnemyTarget)
					currCount++;
			}
			if (currCount < minCount) {
				minCount = currCount;
				newTarg = enemy;
			}
			// exit early
			if (newTarg && minCount == 0)
				break;
		}
		
		if (!newTarg) {
			Debug.Log("NO TARGET FOUND (in AIA_GetLeastTargettedEnemyTarget.cs)");
		}

		controller.FishAIStateInfo.EnemyTarget = newTarg;
	}
}