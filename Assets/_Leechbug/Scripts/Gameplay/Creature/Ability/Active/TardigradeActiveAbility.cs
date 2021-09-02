using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TardigradeActiveAbility : AActiveAbility
{
    [Header("Specific variables for this ability")]
    [SerializeField] private float _cloudDuration;
    [SerializeField] private float _cloudSpeed;
    [SerializeField] CloudProjectile projectilePrefab;
    [SerializeField] private PoisonStatusEffect _statusEffect;
    [SerializeField] private float _poisonInitStacks;
    [SerializeField] private float _poisonStackFrequency;

    [SerializeField] private BossBeam beamPrefab;
    [SerializeField] private GameObject beamAura;
    [SerializeField] private float beamDuration;

    [SerializeField] int beamDamage;
    
    private List<Fish> affectedFish = new List<Fish>();

    //private FMODUnity.StudioEventEmitter _fmodEmitter;
    private int phase = 0;

    public override AbilityType GetAbilityType() => AbilityType.AREA_OFFENSE;

    private void Start()
    {
        ParentFish.Health.OnModHealth += HealthOnOnModHealth;
        //_fmodEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    private void HealthOnOnModHealth(int obj)
    {
        if (ParentFish.Health.GetHealthFraction() <= 0.5f)
        {
            phase = 1;
            beamAura.SetActive(true);
        }
    }

    protected override void UseAbility()
    {
        //_fmodEmitter.Play();
        // two phase attack
        if (phase == 0)
        {
            var projectile = Instantiate(projectilePrefab, ParentFish.transform.position, ParentFish.transform.rotation);

            if (BattleUtils.GetClosestInRange(ParentFish, ParentFish.Data.DetectionRange, out var closest, CanAffectTarget))
            {
                Vector3 direction = Vector3.Normalize(closest.transform.position - ParentFish.transform.position);
                projectile.Initialize(direction, _cloudSpeed, _cloudDuration);
            }
            else
            {
                projectile.Initialize(ParentFish.transform.right, _cloudSpeed, _cloudDuration);
            }

            projectile.OnEnter += OnEnter;
            projectile.OnExit += OnExit;
            projectile.OnTerminate += OnTerminate;
        }
        else
        {
            StartCoroutine(Beam());
            beamPrefab.OnEnter += BeamPrefabOnOnEnter;
            beamPrefab.OnExit += BeamPrefabOnOnExit;
        }
    }

    private void BeamPrefabOnOnExit(Fish fish)
    {
        if (affectedFish.Contains(fish))
        {
            affectedFish.Remove(fish);
            StopCoroutine(ApplyBeam(fish));
        }
    }

    private void BeamPrefabOnOnEnter(Fish fish)
    {
        if (CanAffectTarget(fish) && !affectedFish.Contains(fish))
        {
            affectedFish.Add(fish);
            StartCoroutine(ApplyBeam(fish));
        }
    }

    private void OnTerminate(CloudProjectile obj)
    {
        affectedFish.Clear();
    }

    private void OnEnter(Fish fish, CloudProjectile projectile)
    {
        if (CanAffectTarget(fish) && !affectedFish.Contains(fish))
        {
            affectedFish.Add(fish);
            for (int i = 0; i < _poisonInitStacks; i++)
                StartCoroutine(ApplyPoison(fish));
        }
    }

    private void OnExit(Fish fish, CloudProjectile projectile)
    {
        if (affectedFish.Contains(fish))
        {
            affectedFish.Remove(fish);
            StopCoroutine(ApplyPoison(fish));
        }
    }
    

    IEnumerator ApplyPoison(Fish fish)
    {
        float delta = _poisonStackFrequency + 1;
        while (affectedFish.Contains(fish))
        {
            if (delta >= _poisonStackFrequency)
            {
                var effect = Instantiate(_statusEffect);
                effect.Initialize(fish, PoisonStatusEffect.POISON_TICK_RATE);
                delta = 0;
            }
            else
            {
                delta += Time.deltaTime;
            }
            yield return null;
        }
    }

    IEnumerator ApplyBeam(Fish fish)
    {
        float delta = _poisonStackFrequency + 1;
        while (affectedFish.Contains(fish))
        {
            if (delta >= _poisonStackFrequency)
            {
                fish.Health.ModifyHealth(new HealthModification(-damage, ParentFish, fish));
                delta = 0;
            }
            else
            {
                delta += Time.deltaTime;
            }
            yield return null;
        }
    }
    
    public override bool CanAffectTarget(Fish target)
    {
        return BattleUtils.ShouldDamage(ParentFish, target);
    }

    IEnumerator Beam()
    {
        float delta = 0;
        beamPrefab.gameObject.SetActive(true);
        while (delta < beamDuration)
        { 
            ParentFish.RequestVelocityOverride(VelocityOverride.CAST_ATTACK, Vector3.zero);
            delta += Time.deltaTime;
            yield return null;
        }
        beamPrefab.gameObject.SetActive(false);
        affectedFish.Clear();
    }
}