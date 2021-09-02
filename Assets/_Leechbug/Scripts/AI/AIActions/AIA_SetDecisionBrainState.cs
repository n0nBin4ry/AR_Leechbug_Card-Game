using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetDecisionBrainState", menuName = "FishAI/Actions/SetDecisionBrainState")]
public class AIA_SetDecisionBrainState : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller != null && controller.BehaviorInfo != null && controller.BehaviorInfo.DecisionBrainState != null)
			controller.TransitionToState(controller.BehaviorInfo.DecisionBrainState);
	}
}