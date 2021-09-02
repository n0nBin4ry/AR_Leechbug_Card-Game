using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTargetViewed", menuName = "FishAI/Conditions/EnemyTargetViewed")]
public class AIC_EnemyTargetViewed : AICondition {
    public override bool Check(BaseAIController controller) {
		if (controller == null || controller.FishAIStateInfo.EnemyTarget == null
		|| controller.FishAIStateInfo.EnemyTarget.Defeated)
			return false;

		// the enemy fish needs to be in detection range to even be viewed
		Vector3 fishToEnemy = controller.FishAIStateInfo.EnemyTarget.transform.position - controller.ParentFish.transform.position;
		if (fishToEnemy.sqrMagnitude > controller.ParentFish.Data.DetectionRange * controller.ParentFish.Data.DetectionRange)
			return false;

		// check if the target is in the detection angle's "cone"
		if (Vector3.Angle(fishToEnemy.normalized, controller.ParentFish.transform.forward) <= controller.ParentFish.Data.DetectionAngle) {
			RaycastHit hitInfo;
			if (!Physics.Raycast(controller.ParentFish.transform.position, fishToEnemy.normalized, out hitInfo, fishToEnemy.magnitude, (1 << 0)))
				return true;
		}

		return false;
	}
}
