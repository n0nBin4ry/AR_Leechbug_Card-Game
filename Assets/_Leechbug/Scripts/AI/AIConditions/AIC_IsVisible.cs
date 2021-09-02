using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IsVisible", menuName = "FishAI/Conditions/IsVisible")]
public class AIC_IsVisible : AICondition {
    public override bool Check(BaseAIController controller) {
		if (controller.ParentFish == null)
			return false;
		return controller.ParentFish.Renderer.isVisible;
	}
}
