using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecideBehaviorOptionHighestCost", menuName = "FishAI/Actions/DecideBehaviorOptionHighestCost")]
public class AIA_DecideBehaviorOptionHighestCost : AIAction {
	public override void Act(BaseAIController controller) {
		// loop through all combat options to choose highest value that is not on cooldown
		int choice = -1;
		float max_val = float.MinValue;
		for (int i = 0; i < controller.BehaviorInfo.Options.Length; i++) {
			// if not on cool down, check it's value; if it's biggest so far, store it's index
			if (controller.BehaviorInfo.Options[i].CooldownCounter >= controller.BehaviorInfo.Options[i].CooldownTime) {
				if (controller.BehaviorInfo.Options[i].CostAI > max_val) {
					max_val = controller.BehaviorInfo.Options[i].CostAI;
					choice = i;
				}
			}
		}

		// if they are all on cooldown then return; this will make us transition
		//	to a failsafe state in the current state's transitions instead of a chosen one
		if (choice == -1)
			return;

		// put option on cooldown
		controller.BehaviorInfo.Options[choice].CooldownCounter = 0f;

		// change state to be the state from the chosen combat option
		controller.TransitionToState(controller.BehaviorInfo.Options[choice].State);
	}
}