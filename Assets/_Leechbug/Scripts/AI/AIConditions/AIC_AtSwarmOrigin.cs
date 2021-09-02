using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AtSwarmOrigin", menuName = "FishAI/Conditions/AtSwarmOrigin")]
public class AIC_AtSwarmOrigin : AICondition {
	private float EPSILON = 1f;
	public override bool Check(BaseAIController controller) {
		var toTarg = controller.ParentFish.SwarmManagement.transform.position - controller.ParentFish.transform.position;
		return (Vector3.SqrMagnitude(toTarg) <= (EPSILON * EPSILON));
	}
}
