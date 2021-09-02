public enum ControllerType
{
	AI,
	Player
}

[System.Serializable]
public abstract class AController
{
	public Fish ParentFish;
	public ControllerType ControllerType;

	public AController(Fish parentFish, ControllerType controllerType)
	{
		ParentFish = parentFish;
		ControllerType = controllerType;
	}

	public virtual void Tick() { }

	public abstract void Terminate();

	public virtual void ResetAIController() { }

	public virtual void SetBehavior(FishAIBehaviorInfo newBehavior) { }
}