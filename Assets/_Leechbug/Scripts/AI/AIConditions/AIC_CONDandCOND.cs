using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CONDandCOND", menuName = "FishAI/Conditions/CONDandCOND")]
public class AIC_CONDandCOND : AICondition {
	public AICondition Cond1;
	public bool Not1 = false;

	public AICondition Cond2;
	public bool Not2 = false;

	public override bool Check(BaseAIController controller) {
		if (controller == null || Cond1 == null || Cond2 == null)
			return false;

		bool ret1 = Cond1.Check(controller);
		if (Not1)
			ret1 = !ret1;
		bool ret2 = Cond2.Check(controller);
		if (Not2)
			ret2 = !ret2;

		return (ret1 && ret2);
	}
}
