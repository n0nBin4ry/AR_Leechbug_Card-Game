using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IsCalm", menuName = "FishAI/Conditions/IsCalm")]
public class AIC_IsCalm : AICondition {
    public override bool Check(BaseAIController controller) {
		if (controller == null || controller.FishAIStateInfo.IsCalm == false)
			return false;
		return true;
	}
}
