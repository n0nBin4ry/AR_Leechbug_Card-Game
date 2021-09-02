using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenseKite", menuName = "FishAI/Actions/DefenseKite")]
public class AIA_DefenseKite : AIAction {
	[Header("The distance the fish stays at from it's support target (the one they are defending).")]
	public float KiteDistance = 5f;
	float EPSION = 0.3f;

	public override void Act(BaseAIController controller) {
		if (controller == null || controller.ParentFish.Defeated)
			return;
		// can only do this with an attack target and defense target
		if (controller.FishAIStateInfo.EnemyTarget == null || controller.FishAIStateInfo.SupportTarget == null)
			return;
		if (controller.FishAIStateInfo.EnemyTarget.Defeated || controller.FishAIStateInfo.SupportTarget.Defeated)
			return;

		// the fish will face towards the enemy always
		var facing = Vector3.Normalize(controller.FishAIStateInfo.EnemyTarget.transform.position - controller.FishAIStateInfo.SupportTarget.transform.position);

		// lerp rotation to face direction towards enemy
		controller.ParentFish.TargetForward = facing;
		
		// get the target position, a distance between the support target and the attack target (who is likely targeting them)
		var targPos = controller.FishAIStateInfo.SupportTarget.transform.position + (KiteDistance * facing);

		// if the positon is farther out from support than the enemy, then move closer
		if (Vector3.SqrMagnitude(targPos - controller.FishAIStateInfo.SupportTarget.transform.position)
			>= Vector3.SqrMagnitude(controller.FishAIStateInfo.EnemyTarget.transform.position - controller.FishAIStateInfo.SupportTarget.transform.position)) {
			Vector3 temp = controller.FishAIStateInfo.EnemyTarget.transform.position - controller.FishAIStateInfo.SupportTarget.transform.position;
			targPos = temp.normalized * (temp.magnitude / 2);
		}

		// to reduce jittery look, we will not move if within EPSILON of the target pos
		var moveDir = targPos - controller.ParentFish.transform.position;
		if (moveDir.sqrMagnitude < EPSION * EPSION) {
			controller.ParentFish.TargetVelocity = Vector3.zero;
			return;
		}

		// move towards target position
		moveDir.Normalize();
		controller.ParentFish.TargetVelocity = moveDir * controller.ParentFish.Data.MaxSpeed;
	}
}