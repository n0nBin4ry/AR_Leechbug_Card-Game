using System.Collections;
using System.Collections.Generic;
//using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "DoBasicAttack", menuName = "FishAI/Actions/DoBasicAttack")]
public class AIA_DoBasicAttack : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller == null || controller.ParentFish.Defeated 
		|| controller.FishAIStateInfo.EnemyTarget == null || controller.FishAIStateInfo.EnemyTarget.Defeated)
			return;
		
		var target = controller.FishAIStateInfo.EnemyTarget;
		var ability = controller.ParentFish._basicAttackAbility;
		if (target && (controller != null) && controller.ParentFish && (target.transform.position - controller.ParentFish.transform.position).sqrMagnitude <
		    ability.Range * ability.Range)
		{
			controller.ParentFish.UsePrimaryAttack(controller.FishAIStateInfo.EnemyTarget);
		}
	}
}
