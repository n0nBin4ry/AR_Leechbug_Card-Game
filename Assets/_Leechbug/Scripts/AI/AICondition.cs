using System.Collections.Generic;
  using UnityEngine;

public abstract class AICondition : ScriptableObject {
	public abstract bool Check(BaseAIController controller);
}
