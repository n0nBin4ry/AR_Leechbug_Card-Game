using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecideBehaviorOptionFirstAvailable", menuName = "FishAI/Actions/DecideBehaviorOptionFirstAvailableHighestCost")]
public class AIA_DecideBehaviorOptionFirstAvailable : AIAction {
	public override void Act(BaseAIController controller) {
		foreach (var option in controller.BehaviorInfo.Options) { 
			if (option.CooldownCounter >= option.CooldownTime) {
				option.CooldownCounter = 0f;
				controller.TransitionToState(option.State);
				return;
			}
		}
	}
}