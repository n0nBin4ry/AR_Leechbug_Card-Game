using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IsAlert", menuName = "FishAI/Conditions/IsAlert")]
public class AIC_IsAlert : AICondition {
    public override bool Check(BaseAIController controller) {
		if (controller == null || controller.FishAIStateInfo.IsCalm == true)
			return false;
		return true;
	}
}
