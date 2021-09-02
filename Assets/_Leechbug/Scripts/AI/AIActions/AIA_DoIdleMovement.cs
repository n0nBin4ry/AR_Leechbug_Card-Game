using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoIdleMovement", menuName = "FishAI/Actions/DoIdleMovement")]
public class AIA_DoIdleMovement : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller == null || controller.ParentFish.Defeated)
			return;
		controller.ParentFish.SwarmManagement.MoveFish(controller.ParentFish);
	}
}