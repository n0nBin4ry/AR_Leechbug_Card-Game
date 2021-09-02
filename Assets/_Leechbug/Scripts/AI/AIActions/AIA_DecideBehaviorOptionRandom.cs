using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecideBehaviorOptionRandom", menuName = "FishAI/Actions/DecideBehaviorOptionRandom")]
public class AIA_DecideBehaviorOptionRandom : AIAction {
	public override void Act(BaseAIController controller) {
		// check to see that there are options not on cooldown
		List<int> available = new List<int>();
		for (int i = 0; i < controller.BehaviorInfo.Options.Length; i++) {
			if (controller.BehaviorInfo.Options[i].CooldownCounter >= controller.BehaviorInfo.Options[i].CooldownTime) {
				available.Add(i);
			}
		}

		// if they are all on cooldown then return; this will make us transition
		//	to a failsafe from the current state's transition instead of a chosen one
		if (available.Count == 0)
			return;

		AIState newState;
		if (available.Count == 1) {
			// put option on cooldown
			controller.BehaviorInfo.Options[available[0]].CooldownCounter = 0f;
			newState = controller.BehaviorInfo.Options[available[0]].State;
		}
		else { 
			int choice = Random.Range(0, available.Count);
			// put option on cooldown
			controller.BehaviorInfo.Options[available[choice]].CooldownCounter = 0f;
			newState = controller.BehaviorInfo.Options[available[choice]].State;
		}

		// override state to be the state from the chosen combat option
		controller.TransitionToState(newState);
	}
}