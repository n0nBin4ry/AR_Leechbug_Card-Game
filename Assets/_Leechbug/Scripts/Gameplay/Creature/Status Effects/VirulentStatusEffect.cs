using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirulentStatusEffect : TimedStatusEffect
{
    private const int STATIONARY = 0;
    [SerializeField] private float _frequency = 1;
    [SerializeField] private float _cloudSize;
    [SerializeField] private float _cloudDuration;
    [SerializeField] CloudProjectile cloudPrefab;
    [SerializeField] private PoisonStatusEffect _poisonStatusEffect;
    [SerializeField] private float _poisonDuration;
    [SerializeField] private float _poisonStackFrequency;
    private float _poisonTimer;

    private List<Fish> affectedFish = new List<Fish>();

    private void Start()
    {
        cloudPrefab.transform.localScale = Vector3.one*_cloudSize;
    }
    
    //poison does 1 dmg/sec so presumably occurs every second while fish is poisoned
    public override void Tick(float deltaTime)
    {
        if (Stack <= 0)
        {
            Terminate();
        }
        else
        {
            _stackTimer += deltaTime;
            if (_stackTimer >= _duration)
            {
                RemoveStack();
                _stackTimer = 0;
            }
        }
        
        if (_poisonTimer <= 0)
        {
            _poisonTimer = _frequency;
            if (Fish.HasStatusEffect(StatusEffect.Poisoned, out var effect))
            {
                var projectile = Instantiate(cloudPrefab, Fish.transform.position, Fish.transform.rotation);
                projectile.Initialize(Fish.transform.position, STATIONARY, _cloudDuration);

                projectile.OnEnter += OnEnter;
                projectile.OnExit += OnExit;
                projectile.OnTerminate += OnCloudTerminate;
            }
        }
        else
        {
            _poisonTimer -= deltaTime;
        }
    }

     private void OnEnter(Fish fish, CloudProjectile projectile)
    {
        if (CanAffectTarget(fish))
        {
            affectedFish.Add(fish);
            StartCoroutine(ApplyPoison(fish));
        }
    }

    private void OnExit(Fish fish, CloudProjectile projectile)
    {
        if (affectedFish.Contains(fish))
            affectedFish.Remove(fish);
    }
    
    private void OnCloudTerminate(CloudProjectile obj)
    {
        affectedFish.Clear();
    }

    IEnumerator ApplyPoison(Fish fish)
    {
        while (affectedFish.Contains(fish))
        {
            var effect = Instantiate(_poisonStatusEffect);
            effect.Initialize(fish, _poisonDuration);
            yield return new WaitForSeconds(_poisonStackFrequency);
        }
    }
    
    private bool CanAffectTarget(Fish target)
    {
        //because it will only poison its allies and NOT itself
        return (target.Faction == Fish.Faction && 
               target != Fish);
    }
}