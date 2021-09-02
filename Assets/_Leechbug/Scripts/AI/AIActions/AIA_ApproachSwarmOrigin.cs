using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ApproachSwarmOrigin", menuName = "FishAI/Actions/ApproachSwarmOrigin")]
public class AIA_ApproachSwarmOrigin : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller == null || controller.ParentFish.Defeated)
			return;
		var toOrigin = controller.ParentFish.SwarmManagement.transform.position - controller.ParentFish.transform.position;
		toOrigin.Normalize();
		controller.ParentFish.TargetForward = toOrigin;
		controller.ParentFish.TargetVelocity = toOrigin * controller.ParentFish.Data.MaxSpeed;
	}
}