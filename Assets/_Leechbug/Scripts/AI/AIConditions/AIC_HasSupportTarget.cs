using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HasSupportTarget", menuName = "FishAI/Conditions/HasSupportTarget")]
public class AIC_HasSupportTarget : AICondition {
    public override bool Check(BaseAIController controller) {
		if (controller == null || controller.FishAIStateInfo.SupportTarget == null
		|| controller.FishAIStateInfo.SupportTarget.Defeated)
			return false;
		return true;
	}
}
