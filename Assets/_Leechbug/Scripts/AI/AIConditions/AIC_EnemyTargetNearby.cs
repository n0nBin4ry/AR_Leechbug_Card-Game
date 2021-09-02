using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTargetNearby", menuName = "FishAI/Conditions/EnemyTargetNearby")]
public class AIC_EnemyTargetNearby : AICondition {
	[Header("Distance to check; if 0 or less, then uses fish's detection range.")]
	[SerializeField] private float _distance = -1f;
    public override bool Check(BaseAIController controller) {
		if (controller == null || controller.FishAIStateInfo.EnemyTarget == null
		|| controller.FishAIStateInfo.EnemyTarget.Defeated)
			return false;
		float dist = _distance;
		if (dist <= 0)
			dist = controller.ParentFish.Data.DetectionRange;
		var toTarg = controller.FishAIStateInfo.EnemyTarget.transform.position - controller.ParentFish.transform.position;
		if (dist * dist >= toTarg.sqrMagnitude) {
			// make sure there is no wall in the way; walls/nets are on default collision layer (0)
			RaycastHit hitInfo;
			if (!Physics.Raycast(controller.ParentFish.transform.position, toTarg, out hitInfo, toTarg.magnitude, (1 << 0)))
				return true;
		}
		return false;
	}
}
