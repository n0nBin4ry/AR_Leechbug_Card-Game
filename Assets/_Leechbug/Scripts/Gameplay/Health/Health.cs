using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class HealthModification
{
    public int Amount;
    private bool _isModified;
    private float _multiplier = 1;
    public float Multiplier
    {
        get => _multiplier;
        set
        {
            _isModified = true;
            _multiplier = value;
        }
    }
    public Fish source, target;

    public int Result => _isModified ? Mathf.RoundToInt(Amount * Multiplier) : Amount;
    public HealthModification(int amount, Fish source, Fish target)
    {
        this.Amount = amount;
        this.source = source;
        this.target = target;
    }
}
public class Health
{
    //Properties
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public bool IsFullHealth => CurrentHealth >= MaxHealth;
    public bool IsZeroHealth => CurrentHealth <= 0;

    //Events
    public event Action<int> OnModHealth;
    public event Action<HealthModification> InterceptDamageReceived;
    public static event Action<Fish> OnTakeDamageAtFull;
    public event Action OnEmptyHealth;
    public event Action OnHealthFullyRestored;

    public Health(int maxHealth)
    {
        CurrentHealth = maxHealth;
        MaxHealth = maxHealth;
    }

    public int ModifyHealth(HealthModification healthMod)
    {
        if (!CanModifyHealth(healthMod.Amount))
            return 0;
        
        // interception points for status effects
        if (healthMod.Amount < 0)
        {
            healthMod.source?.InvokeInterceptDamageDealt(healthMod);
            InterceptDamageReceived?.Invoke(healthMod);
        }
        
        var result = Mathf.Clamp(CurrentHealth + healthMod.Result, 0, MaxHealth);
        var delta = result - CurrentHealth;

        /*//only show health bars once they've taken some damage
        if (IsFullHealth && result < CurrentHealth)
        {
            if (OnTakeDamageAtFull == null) {
                OnTakeDamageAtFull += LifebarManager.Instance.RegisterLifebar;
            }
            OnTakeDamageAtFull?.Invoke(healthMod.target);
        }*/

        CurrentHealth = result;
        if (IsZeroHealth)
        {
            OnEmptyHealth?.Invoke();
        }
        if (IsFullHealth)
        {
            OnHealthFullyRestored?.Invoke();
        }
        OnModHealth?.Invoke(delta);
        return delta;
    }

    public int ModifyHealthAbsolute(int amount)
    {
        if (!CanModifyHealth(amount))
            return 0;
        
        var result = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        var delta = result - CurrentHealth;
        CurrentHealth = result;
        OnModHealth?.Invoke(delta);
        return delta;
    }

    bool CanModifyHealth(int amount)
    {
        if (amount == 0) { return false; }
        if (amount > 0 && IsFullHealth) { return false; }
        if (amount < 0 && IsZeroHealth) { return false; }

        return true;
    }

    public void FullHeal()
    {
        CurrentHealth = MaxHealth; 
        OnHealthFullyRestored?.Invoke();
    }

    /// <summary>
    /// returns from 0 to 1.
    /// </summary>
    /// <returns></returns>
    public float GetHealthFraction() => (1.0f * CurrentHealth) / (1.0f * MaxHealth);

	public void ClearEventHandlers() {
		OnModHealth = null;
		OnHealthFullyRestored = null;
		OnEmptyHealth = null;
		OnTakeDamageAtFull = null;
	}
}
