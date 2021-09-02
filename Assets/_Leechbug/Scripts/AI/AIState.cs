using UnityEngine;

[CreateAssetMenu(menuName = "FishAI/State")]
public class AIState : ScriptableObject {
	public Color gizmoColor = Color.grey;
	public AIAction[] Actions;
	public AITransition[] Transitions;
}
