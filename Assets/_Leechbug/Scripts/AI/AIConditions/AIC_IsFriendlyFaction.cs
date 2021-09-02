using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IsFriendlyFaction", menuName = "FishAI/Conditions/IsFriendlyFaction")]
public class AIC_IsFriendlyFaction : AICondition {
    public override bool Check(BaseAIController controller) {
		return (controller.ParentFish.Faction == FishFaction.A);
	}
}
