using UnityEngine;

public abstract class APassiveAbility : MonoBehaviour
{
	[HideInInspector] public Fish ParentFish;
	[SerializeField] public string Title;

	[TextArea(3,10)]
	[SerializeField] public string Description;

	public virtual void Initialize(Fish parentFish)
	{
		ParentFish = parentFish;
	}

	public virtual void Tick(float deltaTime) { }

	public virtual void Terminate() { }
}
