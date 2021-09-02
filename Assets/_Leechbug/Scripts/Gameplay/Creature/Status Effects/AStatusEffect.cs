using System;
using System.Security.Cryptography;
using UnityEngine;

public enum StatusEffectType
{
    Neutral,
    Buff,
    Debuff,
}

// DO NOT CHANGE ORDER OF ENUM! Add new status effects at the end of list
// (it'll mess up the prefabs' ID)

public enum StatusEffect
{
    Poisoned,
    Virulent,
    HealReceived,
    Invincible,
    // Damage
    SmallDamageBuff,
    LargeDamageBuff, //Mighty
    SmallDamageDebuff, //Feeble
    LargeDamageDebuff,
    // Defense
    SmallDefenseBuff,
    LargeDefenseBuff,
    // Speed
    SmallSpeedBuff,
    LargeSpeedBuff,
    SmallSpeedDebuff,
    LargeSpeedDebuff,
    // Others
    TardigradeSmallDefenseBuff,
    MoonrayDamageBuff,
    AfterBattleHeal,
}

public abstract class AStatusEffect : MonoBehaviour
{
    [SerializeField] public StatusEffect Id;
    [SerializeField] public StatusEffectType EffectType;
    [SerializeField] public string Title;
    [SerializeField] public bool isStackable;
    
    // state
    [HideInInspector] public Fish Fish;
    [HideInInspector] public int Stack { get; private set; }

    // events
    public event Action<AStatusEffect> OnTerminate;

    public abstract void Tick(float deltaTime);

    public virtual void Initialize(Fish fish)
    {
        if (fish.Defeated)
        {
            Destroy(gameObject);
            return;
        }
        
        Fish = fish;
        if (fish.HasStatusEffect(Id, out var effect))
        {
            if (effect.isStackable) // delete self
            {
                effect.AddStack();
                Destroy(gameObject);
            }
            else // delete existing effect
            {
                effect.Terminate();
                RegisterEffect(fish);
            }
        }
        else
        {
            RegisterEffect(fish);
        }
    }

    protected virtual void RegisterEffect(Fish fish)
    {
        fish.AddStatusEffect(this);
        AddStack();
    }

    public virtual void AddStack()
    {
        Stack++;
        Fish.ModStatusEffect();
    }

    public virtual void RemoveStack()
    {
        Stack--;
        Fish.ModStatusEffect();
    }

    public void SetStack(int n)
    {
        if (n != Stack)
        {
            Stack = n;
            Fish.ModStatusEffect();
        }
    }

    private void OnFishTerminate(Fish f)
    {
        Terminate();
    }
    protected virtual void Uninitialize()
    {
        Fish.RemoveStatusEffect(this);
    }

    public void Terminate()
    {
        Uninitialize();
        OnTerminate?.Invoke(this);
        if (!gameObject)
            Destroy(gameObject);
    }

    public override string ToString()
    {
        if (Stack > 1)
            return Title + " x" + Stack;
        return Title;
    }
}