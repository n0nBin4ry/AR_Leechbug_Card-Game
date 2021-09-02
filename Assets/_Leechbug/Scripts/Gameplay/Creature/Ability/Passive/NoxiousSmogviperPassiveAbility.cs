using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoxiousSmogviperPassiveAbility : APassiveAbility
{
    private const int STATIONARY = 0;
    [SerializeField] private float _cloudSize;
    [SerializeField] private float _cloudDuration;
    [SerializeField] CloudProjectile cloudPrefab;
    [SerializeField] private PoisonStatusEffect _poisonStatusEffect;
    [SerializeField] private ModifySpeedEffect _speedDebuffStatusEffect;
    [SerializeField] private float _poisonInitStacks;
    [SerializeField] private float _poisonStackFrequency;
    [SerializeField] private float _slowDuration;
    [SerializeField] private float _slowStackFrequency;

    [SerializeField] private float _cloudGenerationRate;
    

    private List<Fish> affectedFish = new List<Fish>();

    private float _timer = 0;

    private void Start()
    {
        cloudPrefab.transform.localScale = Vector3.one*_cloudSize;
    }

    //puffs out smoke at our defined rate
    public override void Tick(float deltaTime)
    {
        if (_timer <= 0)
        {
            _timer = _cloudGenerationRate;
            var projectile = Instantiate(cloudPrefab, ParentFish.transform.position, ParentFish.transform.rotation);
            projectile.Initialize(ParentFish.transform.position, STATIONARY, _cloudDuration);

            projectile.OnEnter += OnEnter;
            projectile.OnExit += OnExit;
            projectile.OnTerminate += OnTerminate;
        }
        else
        {
            _timer -= deltaTime;
        }
    }

    private void OnTerminate(CloudProjectile obj)
    {
        affectedFish.Clear();
    }

    private void OnEnter(Fish fish, CloudProjectile projectile)
    {
        if (CanAffectTarget(fish))
        {
            affectedFish.Add(fish);
            for (int i = 0; i < _poisonInitStacks; i++)
                StartCoroutine(ApplyPoison(fish));
            StartCoroutine(ApplySlow(fish));
        }
    }

    private void OnExit(Fish fish, CloudProjectile projectile)
    {
        if (affectedFish.Contains(fish))
            affectedFish.Remove(fish);
    }
    

    IEnumerator ApplyPoison(Fish fish)
    {
        while (affectedFish.Contains(fish))
        {
            var effect = Instantiate(_poisonStatusEffect);
            effect.Initialize(fish, PoisonStatusEffect.POISON_TICK_RATE);
            yield return new WaitForSeconds(_poisonStackFrequency);
        }
    }

    IEnumerator ApplySlow(Fish fish)
    {
        while (affectedFish.Contains(fish))
        {
            var effect = Instantiate(_speedDebuffStatusEffect);
            effect.Initialize(fish, _slowDuration);
            yield return new WaitForSeconds(_slowStackFrequency);
        }
    }
    
    private bool CanAffectTarget(Fish target)
    {
        return BattleUtils.ShouldDamage(ParentFish, target);
    }
}