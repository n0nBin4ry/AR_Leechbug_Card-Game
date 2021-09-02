using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AtSTATE", menuName = "FishAI/Conditions/AtSTATE")]
public class AIC_AtSTATE : AICondition {
	public AIState State;
    public override bool Check(BaseAIController controller) {
		if (controller == null || State == null)
			return false;
		return (controller.FishAIStateInfo.CurrState == State);
	}
}
