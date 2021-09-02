using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HasNoFaction", menuName = "FishAI/Conditions/HasNoFaction")]
public class AIC_HasNoFaction : AICondition {
    public override bool Check(BaseAIController controller) {
		if (controller.ParentFish == null)
			return false;
		return (controller.ParentFish.Faction == FishFaction.ABANDONED);
	}
}
