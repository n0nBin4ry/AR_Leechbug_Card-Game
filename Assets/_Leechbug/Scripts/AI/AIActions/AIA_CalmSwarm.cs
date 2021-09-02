using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CalmSwarm", menuName = "FishAI/Actions/CalmSwarm")]
public class AIA_CalmSwarm : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller == null || controller.ParentFish.Defeated)
			return;
		controller.ParentFish.SwarmManagement.CalmSwarm();
	}
}