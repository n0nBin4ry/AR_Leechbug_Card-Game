using UnityEngine;

[System.Serializable]
public class AIOption {
	// copy constructor copies all but resets the cooldown counter
	public AIOption(AIOption other) {
		State = other.State;
		CooldownTime = other.CooldownTime;
		CooldownCounter = 0f;
		CostAI = other.CostAI;
	}
	// The state we would choose
	public AIState State;
	// cooldown time before being able to choose again
	public float CooldownTime;
	// time counter for amount of time since last chosen; note: if counter above cooldown time then the option is off cooldown
	[HideInInspector] public float CooldownCounter = 0f;
	// cost of using the option (for AI)
	public float CostAI = 0f;
}
