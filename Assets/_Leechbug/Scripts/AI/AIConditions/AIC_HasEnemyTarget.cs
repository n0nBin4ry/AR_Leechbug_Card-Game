using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HasEnemyTarget", menuName = "FishAI/Conditions/HasEnemyTarget")]
public class AIC_HasEnemyTarget : AICondition {
    public override bool Check(BaseAIController controller) {
		if (controller == null || controller.FishAIStateInfo.EnemyTarget == null
		|| controller.FishAIStateInfo.EnemyTarget.Defeated)
			return false;
		return true;
	}
}
