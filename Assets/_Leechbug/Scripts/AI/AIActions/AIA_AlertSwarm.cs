using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlertSwarm", menuName = "FishAI/Actions/AlertSwarm")]
public class AIA_AlertSwarm : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller == null || controller.ParentFish.Defeated)
			return;
		controller.ParentFish.SwarmManagement.AlertSwarm();
	}
}