using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IsAnyEnemyNearby", menuName = "FishAI/Conditions/IsAnyEnemyNearby")]
public class AIC_IsAnyEnemyNearby : AICondition {
	[Header("Distance to check; if 0 or less, then uses fish's detection range.")]
	[SerializeField] private float _distance = -1f;
    public override bool Check(BaseAIController controller) {
		if (controller == null)
			return false;
		float dist = _distance;
		if (dist <= 0)
			dist = controller.ParentFish.Data.DetectionRange;

		Fish other;
		if ((controller.ParentFish.Faction == FishFaction.A)
		&& (BattleUtils.GetClosestInRange(controller.ParentFish, dist, out other, IsEnemy)))
			return true;

		if ((controller.ParentFish.Faction == FishFaction.B)
		&& (BattleUtils.GetClosestInRange(controller.ParentFish, dist, out other, IsFriendly)))
			return true;

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
