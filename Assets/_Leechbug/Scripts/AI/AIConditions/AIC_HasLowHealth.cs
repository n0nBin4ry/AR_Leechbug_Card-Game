using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HasLowHealth", menuName = "FishAI/Conditions/HasLowHealth")]
public class AIC_HasLowHealth : AICondition {
	[Tooltip("The percentage of health that counts as 'low health'")]
	[Range(0f, 1f)]
	public float LowHealthThreshold = 0.25f;
	public override bool Check(BaseAIController controller) {
		float healthPercentage = (controller.ParentFish.Health.CurrentHealth * 1f) / controller.ParentFish.Health.MaxHealth;
		return (healthPercentage <= LowHealthThreshold);
	}
}
