using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAIController : AController {
	// data used/changed based on fish's behavior
	public FishAIBehaviorInfo BehaviorInfo;
    
	// data used/changed per state
    public FishAIStateInfo FishAIStateInfo { get; set; }

	//public CombatManager CombatManager;

    public BaseAIController(Fish fish) : base(fish,ControllerType.AI) {
		ControllerType = ControllerType.AI;
		// BehaviorInfo = beo;
		BehaviorInfo = ScriptableObject.CreateInstance<FishAIBehaviorInfo>();
		SetBehavior(fish.Data.DefaultIdleBehaviorInfo);
		ResetAIController();
		//CombatManager = GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>();
    }

    public override void ResetAIController() {
		FishAIStateInfo = new FishAIStateInfo();
		FishAIStateInfo.CurrState = BehaviorInfo.DecisionBrainState;
		FishAIStateInfo.CurrStateDuration = 0f;
		InitCooldowns();
	}

    public override void Terminate() {
		// unsubscribe our attack reaction to the health intercept attack event
		if (ParentFish != null && ParentFish.Health != null)
			ParentFish.Health.InterceptDamageReceived -= DoWhenAttacked;
	}

	public override void SetBehavior(FishAIBehaviorInfo newBehavior) {
		BehaviorInfo.DecisionBrainState = newBehavior.DecisionBrainState;
		BehaviorInfo.BehaviorName = newBehavior.name;
		BehaviorInfo.Options = new AIOption[newBehavior.Options.Length];
		// copy over AIOptions so that we dont overwrite the cooldown counters of the original options stored in newBehevaior
		// note: the AIOption copy constructor always sets the cooldown counter to 0
		for (int i = 0; i < newBehavior.Options.Length; i++)
			BehaviorInfo.Options[i] = new AIOption(newBehavior.Options[i]);
		BehaviorInfo.EveryLoopActions = newBehavior.EveryLoopActions;
		BehaviorInfo.EveryLoopTransitions = newBehavior.EveryLoopTransitions;
		BehaviorInfo.LastOptionIndex = -1;
		// set the state to behavior's init state; or set to brain state if there is no init state
		if (FishAIStateInfo != null && FishAIStateInfo.CurrState) {
			if (newBehavior.InitState)
				FishAIStateInfo.CurrState = newBehavior.InitState;
			else
				FishAIStateInfo.CurrState = newBehavior.DecisionBrainState;
		}
	}

    public override void Tick() {
	    //return;
        // increase ai options' counters
        UpdateCooldowns(Time.deltaTime);

        // if no state, reset to a default decision state
        if (!FishAIStateInfo.CurrState)
            FishAIStateInfo.CurrState = BehaviorInfo.DecisionBrainState;

        // increase duration timer
        FishAIStateInfo.CurrStateDuration += Time.deltaTime;

        // do default actions and transitions that are always used by controller's behavior
        DoActions(BehaviorInfo.EveryLoopActions, ParentFish);
        if (CheckTransitions(BehaviorInfo.EveryLoopTransitions, ParentFish))
            return;

        // do actions and transitions based on current state
        // keep track of old state in case decision state overrode state
        var oldState = FishAIStateInfo.CurrState;
        DoActions(oldState.Actions, ParentFish);
        // if state overridden to decision state, then do all actions from new state
        if (oldState == BehaviorInfo.DecisionBrainState && FishAIStateInfo.CurrState != oldState)
            DoActions(FishAIStateInfo.CurrState.Actions, ParentFish);
        CheckTransitions(FishAIStateInfo.CurrState.Transitions, ParentFish);
    }

    protected virtual void DoActions(AIAction[] actions, Fish fish) {
        for (int i = 0; i < actions.Length; i++) {
            if (actions[i])
                actions[i].Act(this);
        }
    }

    protected virtual bool CheckTransitions(AITransition[] transitions, Fish fish) {
        for (int i = 0; i < transitions.Length; i++) {
            if (transitions[i].Condition) {
                // if check returns true, go to true state
                if (transitions[i].Condition.Check(this)) {
                    // return early if we transtion to another state to keep from multiple transitions
                    if (TransitionToState(transitions[i].TrueState)) {
                        return true;
                    }
                }
                // else false state
                else if (TransitionToState(transitions[i].FalseState))
                    return true;
            }
        }
        return false;
    }

    // changes to given state and returns true; remains at state and returns false if there is no new state
    public virtual bool TransitionToState(AIState state) {
		// NOTE: not sure why I didn't make the second cond of 'state == FishAIStateInfo.CurrState' before; if something breaks, might be due to this; seems fine for now
		if (!state || state == FishAIStateInfo.CurrState)
            return false;
        // reset duration timer
		if (state != FishAIStateInfo.CurrState) {
			FishAIStateInfo.CurrStateDuration = 0f;
			FishAIStateInfo.CurrState = state;
		}
        return true;
    }

    // initializes AIoption cooldown counters
    protected virtual void InitCooldowns() {
		foreach (var option in BehaviorInfo.Options)
			option.CooldownCounter = 0f;
    }

    // updates cooldown counters of the AI options
    protected virtual void UpdateCooldowns(float deltaTime) {
        for (int i = 0; i < BehaviorInfo.Options.Length; i++)
			// note: if counter above cooldown time then the option is off cooldown
            if (BehaviorInfo.Options[i].CooldownCounter < BehaviorInfo.Options[i].CooldownTime)
				BehaviorInfo.Options[i].CooldownCounter += deltaTime;
    }

	public virtual void Calm() {
		FishAIStateInfo.IsCalm = true;
		// clear any enemy target
		FishAIStateInfo.EnemyTarget = null;
		FishAIStateInfo.EnemyManuallyAssigned = false;
		// change the fish to be in idle behavior
		SetBehavior(ParentFish.DefaultIdleBehaviorInfo);
		// remove enemy fish's swarm to the combat manager
		/*if (ParentFish.Faction == FishFaction.B && CombatManager.enemySwarms.Contains(ParentFish.SwarmManagement)) 
			CombatManager.enemySwarms.Remove(ParentFish.SwarmManagement);*/
	}

	public virtual void Alert() {
		FishAIStateInfo.IsCalm = false;
		// change fish to be in combat behavior
		SetBehavior(ParentFish.DefaultCombatBehaviorInfo);
		// add enemy fish's swarm to the combat manager
		//if (ParentFish.Faction == FishFaction.B)
			//CombatManager.InitializeCombat(ParentFish.SwarmManagement);
	}

	// manual target override; return false if the fish isnt allowed to be a target
	public bool AssignEnemyTarget(Fish target) {
		if (target == null || target.Defeated || target.Faction == ParentFish.Faction)
			return false;

		FishAIStateInfo.EnemyManuallyAssigned = true;
		FishAIStateInfo.EnemyTarget = target;
		return true;
	}

	public bool AssignSupportTarget(Fish target) {
		if (target == null || target.Defeated || target.Faction != ParentFish.Faction)
			return false;
		if (ParentFish._activeAbility && ParentFish._activeAbility.GetAbilityType() != AbilityType.SINGLE_TARGET_SUPPORT)
			return false;
		FishAIStateInfo.SupportManuallyAssigned = true;
		FishAIStateInfo.SupportTarget = target;
		return true;
	}

	// Needed for PlayerSwarmManagment when Leechbug 
	public void SetSupportTargetNull()
    {
		FishAIStateInfo.SupportManuallyAssigned = false;
		FishAIStateInfo.SupportTarget = null;
	}

	public void DoWhenAttacked(HealthModification attackInfo) {
		// make sure attack is valid; only do this if the parent fish is calm
		if (!FishAIStateInfo.IsCalm || !ParentFish || !(attackInfo.source)
		|| ParentFish.Defeated || attackInfo.source.Defeated ||
		ParentFish.Faction == attackInfo.source.Faction)
			return;

		Debug.Log("UPDATE: BaseAIController.cs: DoWhenAttacked(): Fish detected an attack.");

		// alert the swarm and attack the attacker
		ParentFish.SwarmManagement.AlertSwarm();
		FishAIStateInfo.EnemyTarget = attackInfo.source;
	}
}