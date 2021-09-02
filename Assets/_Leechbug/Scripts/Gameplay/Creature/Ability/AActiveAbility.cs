using UnityEngine;

public abstract class AActiveAbility : MonoBehaviour
{
	[Header("Generalized variables for all Active Abilities")]
	[SerializeField] public string Title;
	[SerializeField] protected float cooldown;
	[SerializeField] protected float range;
	[SerializeField] protected int damage;
	[SerializeField] protected int healing;
	[TextArea(3,20)]
	[SerializeField] public string Description;

	protected Fish ParentFish;
	public abstract AbilityType GetAbilityType();

	float _cooldownTimer;

	public float MaxCooldown => cooldown;
	public float CooldownTimeLeft => _cooldownTimer;
	public float CooldownTimeLeftPercent => _cooldownTimer / cooldown;

	public virtual bool IsAbilityReady => _cooldownTimer <= 0;

	public float Range => range;
	public float Damage => damage;
	public float Healing => healing;

	public virtual void Initialize(Fish parentFish)
	{
		ParentFish = parentFish;
	}

	public virtual void Terminate() { }

	public virtual void Tick(float deltaTime)
	{
		_cooldownTimer = Mathf.Max(_cooldownTimer - deltaTime, 0f);
	}

	public void Execute()
	{
		if (IsAbilityReady)
		{
			UseAbility();
			_cooldownTimer = cooldown;
		}
	}

	protected abstract void UseAbility();

	/// <summary>
	/// Check that a target can be casted upon.
	/// Appropriate faction, within distance, etc.
	/// </summary>
	/// <param name="target"></param>
	/// <returns></returns>
	public abstract bool CanAffectTarget(Fish target);

	

}