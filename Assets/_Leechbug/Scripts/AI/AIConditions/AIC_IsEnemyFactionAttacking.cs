using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IsEnemyFactionAttacking", menuName = "FishAI/Conditions/IsEnemyFactionAttacking")]
public class AIC_IsEnemyFactionAttacking : AICondition {
    public override bool Check(BaseAIController controller) {
	    int enemyCount;
	    enemyCount = (controller.ParentFish.Faction == FishFaction.A)
		    ? CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.B).GetFishCount()
		    : CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.A).GetFishCount(); 
		return (enemyCount != 0);
	}
}
