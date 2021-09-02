using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResetAIController", menuName = "FishAI/Actions/ResetAIController")]
public class AIA_ResetAIController : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller != null)
			controller.ResetAIController();
	}
}