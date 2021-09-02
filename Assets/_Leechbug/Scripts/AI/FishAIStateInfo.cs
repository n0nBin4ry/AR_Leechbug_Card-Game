using System.Collections.Generic;

public class FishAIStateInfo {
	public bool SupportManuallyAssigned = false;
	public bool EnemyManuallyAssigned = false;
	public bool IsCalm = true;
	public AIState CurrState = null;
	public float CurrStateDuration = 0f; // how long have we been in this state
	public Fish EnemyTarget = null;
	public Fish SupportTarget = null;
}