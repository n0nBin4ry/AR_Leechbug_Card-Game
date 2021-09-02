using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlwaysTrue", menuName = "FishAI/Conditions/AlwaysTrue")]
public class AIC_AlwaysTrue : AICondition {
    public override bool Check(BaseAIController controller) {
		return true;
	}
}
