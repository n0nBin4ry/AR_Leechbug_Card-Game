using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum FishFaction
{
    B,    // part of swarm B (used to be ENEMY)
    A,   // part of swarm A (used to be FRIENDLY)
	ABANDONED,  // fish not recruited after combat :(
}

public class Fish : MonoBehaviour
{
    [NonSerialized] public Fish _target;
    public Health Health { get; private set; }
    public Rigidbody rbody { get; set; }
	public Renderer Renderer { get; set; }
    [SerializeField] FishData data;
    [SerializeField] public string nickname;
    [SerializeField] public GameObject mesh;

    // Elite
    [SerializeField] FishData eliteData;
    bool isElite;

    public FishData Data => data;
    
    // COMBAT
    public ATargetedActiveAbility _basicAttackAbility { get; private set; }
    public AActiveAbility _activeAbility { get; private set; }
    public APassiveAbility _passiveAbility { get; private set; }
    private Fish combatTarget;

    public FishFaction Faction { get; set; }
    public bool Defeated { get; set; }
    
	// AI
	public AController Controller { get; set; }
	public FishAIBehaviorInfo DefaultCombatBehaviorInfo { get; set; }
	public FishAIBehaviorInfo DefaultIdleBehaviorInfo { get; set; }
	public ASwarmManagement SwarmManagement { get; set; }

    // MOVEMENT
    private float Speed;
    private float Acceleration;
    private float RotationAcceleration;
    public Vector3 Velocity { get; set; }
    public Vector3 TargetVelocity { get; set; }
    public Vector3 TargetForward { get; set; }
    private Vector3 _overrideForward;
    private Vector3 _overrideVelocity;
    private VelocityOverride _overrideVelocityPriority;

    [HideInInspector] public List<AStatusEffect> StatusEffects = new List<AStatusEffect>();
    
    // EVENTS
    public static event Action<Fish, int> OnStaticModHealth;
    public event Action OnModStatusEffects;
    public event Action<HealthModification> InterceptDamageDealt;
    public event Action<Fish> OnTerminated;
    public event Action<Fish> OnDefeat;
    public event Action<AStatusEffect> OnAddStatusEffect;
    public event Action<Fish> OnPossessFish;
    public event Action<Fish> OnDepossessFish;

    #region MONO_SHENANIGANS
    void Update() => Tick(Time.deltaTime);
    void OnDestroy() => Terminate();
    #endregion
    #region STATE_HANDLERS

    private void Start() {
        rbody = GetComponent<Rigidbody>();
		Renderer = GetComponentInChildren<Renderer>();
        
        // assign self to a team upon spawn
        CombatManager.Instance.AssignToTeam(this);
        
        // DEBUG
        if (CombatManager.Instance.NumFish >= 2)
            CombatManager.Instance.InitializeCombat();
        
        // initialize self in this AR version
        Initialize();

        new FishMaterialController().Initialize(this);

        //UI
        //HUDDamage.INSTANCE.Initialize(this);
    }

    void Tick(float deltaTime)
    {
        if (Defeated)
            return;
        
        _passiveAbility?.Tick(deltaTime);
        _basicAttackAbility?.Tick(deltaTime);
        _activeAbility?.Tick(deltaTime);
        
        UpdateTransform(deltaTime);
        Controller?.Tick();
        for (int i = StatusEffects.Count - 1; i >= 0; i--)
        {
            StatusEffects[i].Tick(deltaTime);
        }
    }
    public void Terminate()
    {
        SwarmManagement?.RemoveFromSwarm(this);
        _passiveAbility?.Terminate();
        _activeAbility?.Terminate();
        _basicAttackAbility?.Terminate();
        Controller?.Terminate();
        
        for (int i = StatusEffects.Count - 1; i >= 0; i--)
        {
            StatusEffects[i].Terminate();
        }
        //creaturePooler?.ReturnToQueue(this.gameObject);
        GameObject.Destroy(this.gameObject);
    }
    #endregion

    public void Initialize() {
        //Controller = controller;
        Controller = new BaseAIController(this);
        //SwarmManagement = swarmManagement;
        //Faction = faction;
        Defeated = false;
        //isElite = elite;
        InitializeFromData();
    }

    public void InitializeFromData()
    {
        if(isElite && eliteData)
        {
            data = eliteData;
            gameObject.name += " Elite";
        }

        nickname = data.CreatureName;

        // COMBAT
        Health = new Health(data.MaxHealth);
        Health.OnModHealth += ModHealth;

        // if we have an AI controller, then subscribe the attack detection to health's event
        if (Controller.ControllerType == ControllerType.AI)
            Health.InterceptDamageReceived += ((BaseAIController)Controller).DoWhenAttacked;

        if (data.BasicAttackPrefab)
        {
            _basicAttackAbility = Instantiate(data.BasicAttackPrefab, transform);
            _basicAttackAbility.Initialize(this);
        }

        if (data.ActiveAbilityPrefab)
        {
            _activeAbility = Instantiate(data.ActiveAbilityPrefab, transform);
            _activeAbility.Initialize(this);
        }

        if (data.PassivePrefab)
        {
            _passiveAbility = Instantiate(data.PassivePrefab, transform);
            _passiveAbility.Initialize(this);
        }

        // MOVEMENT
        Speed = data.MaxSpeed;
        Acceleration = data.Acceleration;
        RotationAcceleration = data.RotationAcceleration;

        // AI init
        DefaultCombatBehaviorInfo = data.DefaultCombatBehaviorInfo;
        DefaultIdleBehaviorInfo = data.DefaultIdleBehaviorInfo;

        // VFX
        var bubbleTrail = Instantiate(data.BubbleTrail, transform);
        bubbleTrail.transform.localPosition = Vector3.zero;

        transform.localScale = data.ScaleModifier;
        if (data.BaseBodyMaterial)
        {
            mesh.gameObject.GetComponent<MeshRenderer>().material = data.BaseBodyMaterial;
        }
    }

    /*public void Initialize(AController controller, ASwarmManagement swarmManagement, FishFaction faction, ObjectPool sourcePool, bool isElite)
    {
        Initialize(controller, swarmManagement, faction, isElite);
        //creaturePooler = sourcePool;
    }*/
    
    // MOVEMENT
    private void UpdateTransform(float deltaTime)
    {
        var t = transform;

        if (_overrideVelocityPriority > 0)
        {
            _overrideVelocityPriority = 0;
            TargetForward = _overrideForward;
            
            Velocity = Vector3.Lerp(Velocity, _overrideVelocity, Acceleration * deltaTime);
        }
        else
        {
            _overrideForward = Vector3.zero;
            _overrideVelocity = Vector3.zero;
            Velocity = Vector3.Lerp(Velocity, TargetVelocity.normalized * Speed, Acceleration * deltaTime);
        }
        rbody.velocity = Velocity;
            
        if (TargetForward.magnitude > 0)
        {
            t.forward = Vector3.Lerp(t.forward, TargetForward.normalized, RotationAcceleration * deltaTime);
        }
    }
    
    // COMBAT
    public void RequestVelocityOverride(VelocityOverride priority, Vector3 velocity)
    {
        if ((int)priority >= (int)_overrideVelocityPriority)
        {
            _overrideVelocity = velocity;
            _overrideVelocityPriority = priority;
        }
    }
    
    public void RequestForwardOverride(VelocityOverride priority, Vector3 forward)
    {
        if ((int)priority >= (int)_overrideVelocityPriority)
        {
            _overrideForward = forward;
            _overrideVelocityPriority = priority;
        }
    }
    
    // COMBAT
    public void UsePrimaryAttack() {
        if(_basicAttackAbility.isActiveAndEnabled) {
            _basicAttackAbility.CurrentTarget = null;
            _basicAttackAbility?.Execute();
        }
    }
    
    public void UsePrimaryAttack(Fish fish)
    {
        if(_basicAttackAbility.isActiveAndEnabled)
        {
            _basicAttackAbility.CurrentTarget = fish;
            _basicAttackAbility?.Execute();
        }

    }

    public void UseAbility()
    {
        if(_activeAbility)
        {
            if(_activeAbility.isActiveAndEnabled)
            {
                _activeAbility?.Execute();
            }
        }
    }

    private void ModHealth(int value)
    {
        OnStaticModHealth?.Invoke(this, value);
        if (Health.IsZeroHealth)
        {
            Defeat();
        }
    }

    private void Defeat()
    {
        MakeCry();
        Terminate();
        /*if(Faction == FishFaction.A)
        {
            if(Controller.ControllerType == ControllerType.AI)
            {
                Terminate();
            }
            if(Controller.ControllerType == ControllerType.Player)
            {
                //If possessed fish, eject
                if(!data.IsLeechbug)
                {
                    var playerSwarmManagement = (PlayerSwarmManagement)SwarmManagement;
                    playerSwarmManagement.LeechbugReappear();
                    playerSwarmManagement.ResetSupportTargetForFish();
                    playerSwarmManagement.PossessFish(playerSwarmManagement.Leechbug);
                    Terminate();
                }
                else
                {
                    //Terminate();
                    //Trigger like, a game over type thing
                    Defeated = true;
                }
            }
            
        }
        if(Faction == FishFaction.B)
        {
			//Set state to defeated
			Defeated = true;
            OnDefeat?.Invoke(this);

            for (int i=StatusEffects.Count - 1; i >= 0; i--)
            {
                StatusEffects[i].Terminate();
            }
            StatusEffects.Clear();
            OnModStatusEffects?.Invoke();
        }

		// check how combat changes now that this fish is defeated
		if (Controller.ControllerType == ControllerType.AI) {
			((BaseAIController)Controller).CombatManager.CheckEndCombatOnDeath(this);
		}*/

	}

    public void OnRecruit(ASwarmManagement swarmManagement)
    {
        Faction = FishFaction.A;
        SwarmManagement = swarmManagement;
        Defeated = false;
        Health.FullHeal();
        this.transform.parent = null;
		// reset AI
        Controller.SetBehavior(DefaultIdleBehaviorInfo);
        Controller.ResetAIController();
        OnModStatusEffects?.Invoke();

    }

    public void Revive()
    {
        Defeated = false;
        Health.FullHeal();
        Controller.SetBehavior(DefaultIdleBehaviorInfo);
        Controller.ResetAIController();
        OnModStatusEffects?.Invoke();
    }

    public void OnPossess(ASwarmManagement swarmManagement)
    {
        OnPossessFish?.Invoke(this);
    }

    public void OnDepossess()
    {
        OnDepossessFish?.Invoke(this);
    }

    // STATUS EFFECTS
    public void InvokeInterceptDamageDealt(HealthModification healthMod)
    {
        InterceptDamageDealt?.Invoke(healthMod);
    }
    
    public void AddStatusEffect(AStatusEffect effect)
    {
        StatusEffects.Add(effect);
        OnModStatusEffects?.Invoke();
        OnAddStatusEffect?.Invoke(effect);
    }

    public void RemoveStatusEffect(AStatusEffect effect)
    {
        StatusEffects.Remove(effect);
        OnModStatusEffects?.Invoke();
    }

    public void ModStatusEffect()
    {
        OnModStatusEffects?.Invoke();
    }
    
    public bool HasStatusEffect(StatusEffect statusEffectId, out AStatusEffect statusEffect)
    {
        statusEffect = null;
        bool hasEffect = false;
        foreach (var effect in StatusEffects)
        {
            if (effect.Id == statusEffectId)
            {
                statusEffect = effect;
                hasEffect = true;
            }
        }
        return hasEffect;
    }

    public void SetFishData(FishData fishData)
    {
        data = fishData;
        InitializeFromData();
    }

    public FishData GetFishData()
    {
        return data;
    }

    // Used by Combat Manager when FishSwarm is out of combat
    public void ApplyInCombatSpeedDebuff(float speedDebuff)
    {
        Speed -= speedDebuff;
    }

    public void MakeCry()
    {
        //Instantiate(data.CrySfx, transform.position, Quaternion.identity);
    }

    public IEnumerator MakeCryWithin(float seconds)
    {
        var rand = Random.Range(0, seconds);
        yield return new WaitForSeconds(rand);
        MakeCry();
    }
}
