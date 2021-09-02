using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveAwayFromSupportTarget", menuName = "FishAI/Actions/MoveAwayFromSupportTarget")]
public class AIA_MoveAwayFromSupportTarget : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller == null || controller.ParentFish.Defeated 
		|| controller.FishAIStateInfo.SupportTarget == null || controller.FishAIStateInfo.SupportTarget.Defeated)
			return;

		var awayFromSupport = controller.ParentFish.transform.position - controller.FishAIStateInfo.SupportTarget.transform.position;
		awayFromSupport.Normalize();
		controller.ParentFish.TargetForward = awayFromSupport;
		controller.ParentFish.TargetVelocity = awayFromSupport * controller.ParentFish.Data.MaxSpeed;
	}
}