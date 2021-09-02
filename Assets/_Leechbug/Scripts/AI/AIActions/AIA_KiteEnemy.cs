using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KiteEnemy", menuName = "FishAI/Actions/KiteEnemy")]
public class AIA_KiteEnemy : AIAction {
	[Header("The distance the fish stays at from it's target.")]
	[Header("If 0 or less, then uses fish's detection radius.")]
	[SerializeField] private float _kiteDistance = 15f;
	[SerializeField] private float EPSION = 0.3f;
	public override void Act(BaseAIController controller)
	{
		if (controller == null || controller.ParentFish.Defeated 
		|| controller.FishAIStateInfo.EnemyTarget == null || controller.FishAIStateInfo.EnemyTarget.Defeated)
			return;

		float kDist = _kiteDistance;

		if (kDist <= 0f)
			kDist = controller.ParentFish.Data.DetectionRange;

		// the fish will face towards the target always
		var facing = Vector3.Normalize(controller.FishAIStateInfo.EnemyTarget.transform.position - controller.ParentFish.transform.position);

		controller.ParentFish.TargetForward = new Vector3(facing.x, facing.y, facing.z); // sorry if weird; not sure if there would be aliasing here

		// get the target position, a distance away from the target in the direction of our fish
		var targPos = controller.FishAIStateInfo.EnemyTarget.transform.position + (kDist * -facing);
		
		// to reduce jittery look, we will not move if within EPSILON of the target pos
		// TODO: override velocity here? depends on how quick decelleration is
		var t = targPos - controller.ParentFish.transform.position;
		if (t.sqrMagnitude < EPSION * EPSION)
			return;

		// move towards target position
		controller.ParentFish.TargetVelocity = facing * controller.ParentFish.Data.MaxSpeed/2;
	}
}