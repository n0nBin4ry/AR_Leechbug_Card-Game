using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DurationVARIABLESecs", menuName = "FishAI/Conditions/DurationVariableSecs")]
public class AIC_DurationVariableSecs : AICondition {
	[Header("Number of seconds we are checking have elapsed since current state set")]
	public float Seconds = 30f;
    public override bool Check(BaseAIController controller) {
		return (controller.FishAIStateInfo.CurrStateDuration >= Seconds);
	}
}
