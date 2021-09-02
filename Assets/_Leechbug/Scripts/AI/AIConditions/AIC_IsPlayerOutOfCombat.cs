using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IsPlayerOutOfCombat", menuName = "FishAI/Conditions/IsPlayerOutOfCombat")]
public class AIC_IsPlayerOutOfCombat : AICondition {
    public override bool Check(BaseAIController controller) {
		// not used in AR version

		// TODO: remove the scriptable obj from the AI flow

		return false;
    }
}
