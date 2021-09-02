using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "AI Behaviour", menuName = "FishAI/Info/AI Behaviour")]
public class FishAIBehaviorInfo : ScriptableObject {
	[Tooltip("The name of the behavior; to be used for debugging and UI (or whatever else).)")]
	public string BehaviorName = "";
	
	[Tooltip("The state that decides what Behavior Option to when returning to brain state.")]
	public AIState DecisionBrainState;
	[Tooltip("The starting state of the behavior; if blank, it uses the DecisionBrainState.")]
	public AIState InitState;

	[Tooltip("Possible Options the Behavior will decide between.")]
	public AIOption[] Options;
	[HideInInspector] public int LastOptionIndex = -1;

	[Tooltip("Actions that happen despite the current state; processed before current state's actions and transitions.")]
	public AIAction[] EveryLoopActions;

	[Tooltip("Transitions checked before happen despite the current state; processed before current state's actions and transitions.")]
	public AITransition[] EveryLoopTransitions;
}