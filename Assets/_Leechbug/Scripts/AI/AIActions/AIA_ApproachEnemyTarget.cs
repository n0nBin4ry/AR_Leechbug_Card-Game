using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ApproachEnemyTarget", menuName = "FishAI/Actions/ApproachEnemyTarget")]
public class AIA_ApproachEnemyTarget : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller == null || controller.ParentFish.Defeated 
		|| controller.FishAIStateInfo.EnemyTarget == null || controller.FishAIStateInfo.EnemyTarget.Defeated)
			return;
		Vector3 dir = controller.FishAIStateInfo.EnemyTarget.transform.position - controller.ParentFish.transform.position;
		dir.Normalize();
		controller.ParentFish.TargetForward = dir;
		controller.ParentFish.TargetVelocity = dir * controller.ParentFish.Data.MaxSpeed;
	}
}