using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GetNearestFishTarget", menuName = "FishAI/Actions/GetNearestFishTarget")]
public class AIA_GetNearestFishTarget : AIAction {
	[Header("The range we search in. (inclusive)")]
	[Header("If the value is 0 or lower, it uses the fish's detection range.")]
	[SerializeField] private float _distance = -1;

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

		float range;
		if (_distance <= 0)
			range = controller.ParentFish.Data.DetectionRange;
		else
			range = _distance;

		Fish newTarg = null;
		// search for enemy fish if this is a friendly fish
		if (controller.ParentFish.Faction == FishFaction.A) {
			BattleUtils.GetClosestInRange(controller.ParentFish, range, out newTarg, IsEnemy);
		}
		// search for friendly fish if this is an enemy fish
		else {
			BattleUtils.GetClosestInRange(controller.ParentFish, range, out newTarg, IsFriendly);
		}
		
		if (newTarg == null) {
			Debug.Log("NO TARGET FOUND (in AIA_GetNearestFishTarget.cs)");
		}

		controller.FishAIStateInfo.EnemyTarget = newTarg;
	}

	// TODO: find a way to generalize/store this so that I dont have to copy/paste
	private bool IsFriendly(Fish fish) {
		return (fish.Faction == FishFaction.A);
	}

	private bool IsEnemy(Fish fish) {
		return (fish.Faction == FishFaction.B);
	}
}