using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResetLastCooldownOption", menuName = "FishAI/Actions/ResetLastCooldownOption")]
public class AIA_ResetLastOptionCooldown : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller != null && controller.BehaviorInfo.LastOptionIndex >= 0) {
			// clear the cooldown so that the option can be done again (used for possible AI corrections
			controller.BehaviorInfo.Options[controller.BehaviorInfo.LastOptionIndex].CooldownCounter = controller.BehaviorInfo.Options[controller.BehaviorInfo.LastOptionIndex].CooldownTime;
			controller.BehaviorInfo.LastOptionIndex = -1;
		}
	}
}