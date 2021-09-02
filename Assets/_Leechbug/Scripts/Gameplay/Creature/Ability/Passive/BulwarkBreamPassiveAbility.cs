using UnityEngine;

public class BulwarkBreamPassiveAbility : APassiveAbility
{
    [SerializeField] private float healthFractionThreshold;
    [SerializeField] private float stackCooldown;
    [SerializeField] private float buffDuration;
    [SerializeField] private GameObject visualEffect;
    [SerializeField] private ModifyDamageDealtEffect statusEffect;

    [HideInInspector] public float StackCooldownTimer;
    private float _lastHealth;

    public override void Initialize(Fish parentFish)
    {
        base.Initialize(parentFish);
        parentFish.Health.OnModHealth += ModHealth;
    }

    public void ModHealth(int obj)
    {
        var currHealth = ParentFish.Health.GetHealthFraction();
        // if dropped below threshold
        if (_lastHealth >= healthFractionThreshold
            && currHealth < healthFractionThreshold
            && currHealth > 0
            && StackCooldownTimer <= 0)
        {
            StackCooldownTimer = stackCooldown;
            
            var effect = Instantiate(statusEffect);
            effect.Initialize(ParentFish, buffDuration);

            var vfx = Instantiate(visualEffect, ParentFish.transform);
            effect.OnTerminate += e =>
            {
                if (vfx)
                    Destroy(vfx.gameObject);
            };
        }

        _lastHealth = currHealth;
    }

    public override void Tick(float deltaTime)
    {
        StackCooldownTimer = Mathf.Max(0, StackCooldownTimer - deltaTime);
    }

    public override void Terminate()
    {
        ParentFish.Health.OnModHealth -= ModHealth;
    }
}