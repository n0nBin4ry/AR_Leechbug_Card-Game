using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DestroySelf", menuName = "FishAI/Actions/DestroySelf")]
public class AIA_DestroySelf : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller == null || controller.ParentFish == null)
			return;
		controller.ParentFish.Terminate();
	}
}