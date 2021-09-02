using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class CombatManager : Singleton<CombatManager> {
	public enum TeamType {
		none,
		A,
		B,
	}
	
	public int NumFish {
		get {
			int tmp = 0;
			if (_teamA)
				tmp += _teamA.GetFishCount();
			if (_teamB)
				tmp += _teamB.GetFishCount();
			return tmp;
		}
		private set { }
	}
	
	private ARSwarm _teamA = null;
	private ARSwarm _teamB = null;

	public bool IsActive = false;

	private Vector3 _combatCenter = Vector3.zero;

	private FieldDivider _divider = null;
	

    //prefab for instantiating auto heal
    //[SerializeField] private AfterBattleHealStatusEffect afterBattleHeal;
	[SerializeField] private float inCombatSpeedDebuff = 0f;

	// return teamtype of given fish
	TeamType GetTeamType(Fish fish) {
		if (!fish)
			return TeamType.none;
		if (_teamA.allFish.Contains(fish))
			return TeamType.A;
		if (_teamB.allFish.Contains(fish))
			return TeamType.B;
		return TeamType.none;
	}
	
	// return the swarm manager of the given team type
	public ARSwarm GetTeamSwarm(TeamType type) {
		if (type == TeamType.A)
			return _teamA;
		if (type == TeamType.B)
			return _teamB;
		return null;
	}
	
	// adds a fish to a team, returns the type from the team it was added to 
	public TeamType AssignToTeam(Fish fish) {
		if (!fish) {
			Debug.Log("CombatManager.AssignToTeam(): null fish given");
			return TeamType.none;
		}

		// avoid adding multiple times or in multiple teams
		if (_teamA.allFish.Contains(fish))
			_teamA.allFish.Remove(fish);
		if (_teamB.allFish.Contains(fish))
			_teamB.allFish.Remove(fish);

		// we want to use the Divider object to assign fish to a team; if there isn't one, make teams even when possible
		if (!_divider) {
			if (_teamA.GetFishCount() > _teamB.GetFishCount()) {
				_teamB.allFish.Add(fish);
				fish.SwarmManagement = _teamB;
				fish.Faction = FishFaction.B;
				return TeamType.B;
			}
			_teamA.allFish.Add(fish);
			fish.SwarmManagement = _teamA;
			fish.Faction = FishFaction.A;
			return TeamType.A;
		}

		TeamType team = _divider.DecideTeam(fish);
		fish.Faction = (team == TeamType.A) ? FishFaction.A : FishFaction.B;
		
		var swarm = GetTeamSwarm(team);
		// give fish pointer to their swarm
		fish.SwarmManagement = swarm;
		swarm.allFish.Add(fish);
		return team;
	}

	public void AssignFieldDivider(FieldDivider divider) {
		_divider = divider;
		
		// re-assign all fish based on this new divider
		List<Fish> allFish = new List<Fish>();
		allFish.AddRange(_teamA.allFish);
		allFish.AddRange(_teamB.allFish);
		// AssignToTeam already takes care of removing any redundant fish
		foreach (var fish in allFish)
			AssignToTeam(fish);
	}

	private void Awake() {
		var objA = new GameObject();
		_teamA = objA.AddComponent<ARSwarm>();
		var objB = new GameObject();
		_teamB = objB.AddComponent<ARSwarm>();
	}

	// sets up a center of the combat
	/*private void SetCombatCenter() {
		// center of combat will be the player's position at the time the function is called
		foreach (var fish in playerSwarm.GetFishInSwarm()) { 
			if (fish.Controller != null && fish.Controller.ControllerType == ControllerType.Player) {
				_combatCenter = Vector3.Lerp(fish.transform.position, enemySwarm.transform.position, 0.5f);
				return;
			}
		}
		Debug.Log("WARNING: CombatManager.cs:SetCombatCenter: Combat center set without player fish in player's swarm.");
	}*/

	// terminate all the auto heal status effect in fishes in swarm
	private void TerminateAutoHeal(ASwarmManagement swarm) {
		foreach (var fish in swarm.GetFishInSwarm())
		{
			if ((fish == null) || fish.Defeated)
				continue;
			AfterBattleHealStatusEffect abhse = fish.GetComponentInChildren<AfterBattleHealStatusEffect>();
			if(abhse != null)
            {
				abhse.Terminate();
			}
		}
	}

	// Resets outofcombat speed
	private void ResetOutOfCombatSpeed(ASwarmManagement swarm) {
		foreach (var fish in swarm.GetFishInSwarm())
		{
			if ((fish == null) || fish.Defeated)
				continue;
			fish.ApplyInCombatSpeedDebuff(inCombatSpeedDebuff * -1);
		}
	}

	// Applies a speed debuff to each fish
	private void ApplyInCombatSpeedDebuff(ASwarmManagement swarm)
	{
		foreach (var fish in swarm.GetFishInSwarm())
		{
			if ((fish == null) || fish.Defeated)
				continue;
			fish.ApplyInCombatSpeedDebuff(inCombatSpeedDebuff);
		}
	}

	private void TriggerAggroSfx(ASwarmManagement swarm) {
		foreach (var fish in swarm.GetFishInSwarm())
		{
			if ((fish == null) || fish.Defeated)
				continue;
			StartCoroutine(fish.MakeCryWithin(1));
		}
	}
	
	// Call when first agro-ing enemies, gets the enemy swarm component from an enemy fish
	// and adds it to the list of enemy swarms, if it doesn't already exist in there
	public void InitializeCombat() {
		//If new swarm is player swarm or already an enemy swarm
		if (_teamA.allFish.Count == 0 || _teamB.allFish.Count == 0)
			return;

		_teamA.AlertSwarm();
		_teamB.AlertSwarm();
        /*//If first swarm
        if (!IsActive) {
            Debug.Log("Set combat active!");
            IsActive = true;
			enemySwarms.Add(newSwarm);
			// set center of combat area
			//SetCombatCenter();

			// terminate all the autoheals and apply speed debuff for enemy swarms
			TerminateAutoHeal(playerSwarm);
			ApplyInCombatSpeedDebuff(playerSwarm);
				
			TerminateAutoHeal(newSwarm);
			ApplyInCombatSpeedDebuff(newSwarm);

			// Slow down speed of combat
			TimeScaleController.SetTimeScale(0.25f);  //MAGIC NUMBER HERE
            if (slowTimeTickerManager != null)
            {
                slowTimeTickerManager.SetBool("SlowOn", true);
            }
			var swarm = (EnemySwarmManagement)newSwarm;
        }
		//If it's another swarm added in
		else
        {
			Debug.Log("Adding new swarm");
			enemySwarms.Add(newSwarm);

			TerminateAutoHeal(newSwarm);
			ApplyInCombatSpeedDebuff(newSwarm);
			var swarm = (EnemySwarmManagement)newSwarm;
		}*/

		// Tutorial Checkpoint
		/*if (TutorialGameManager.instance && numOfFinishedCombat == 3)
		{
			fourthCombatTutorial();
		}*/
	}

    // Call when a fish is defeated - checks if it's last defeated in swarm
	// TODO (possibly): if effeciency needed then we can make this take in a fish and only check it's own swarm; this way it can also be subscribed to the OnDefeat event in Fish.cs
    /*public void CheckEndCombatOnDeath(Fish inputFish) {
		// If all player fish defeated, game over
		Debug.Log(inputFish.name);
		if(inputFish.Faction == FishFaction.A)
        {
			if(inputFish.SwarmManagement.GetFishCount() == 0)
            {
				Debug.Log("Player Swarm Defeated");
            }
			return;
        }

		if(inputFish.Faction == FishFaction.B)
        {
			if(enemySwarms.Contains(inputFish.SwarmManagement))
            {
				var enemySwarm = (EnemySwarmManagement)inputFish.SwarmManagement;
				if (enemySwarm.CheckIfAllDefeated() && enemySwarm.isSpawned)
                {
					Debug.Log("An enemy swarm is defeated!");
					numOfFinishedCombat += 1;
					enemySwarm.SwarmDefeated();
					// Dialogue 
					if (SceneManager.GetActiveScene().name == "Tutorial Level 0" &&
						inputFish.nickname == "Shockguppy")
                    {
						// Tell PostCombatMenuUI to init dialogue on close
						//postCombatMenuManager.GetComponent<PostCombatUIManager>().enableDialogue = true;
					}
					//ToggleMenuOn(enemySwarm);
					enemySwarms.Remove(enemySwarm);
				}
            }
        }

		if(enemySwarms.Count == 0)
        {
			Debug.Log("All swarms defeated!");
			playerSwarm.CalmSwarm();

			// heal the player swarm
			AutoHealSwarm(playerSwarm);
			ResetOutOfCombatSpeed(playerSwarm);

			IsActive = false;
		}
    }

    // check if the player has left the combat area to escape combat
    public bool CheckCombatEscape() {
		if (!IsActive)
			return true;
		for(int i = enemySwarms.Count - 1; i > -1; i--)
        {
			var leechbugSwarm = (PlayerSwarmManagement)playerSwarm;
			Vector3 leechbugPosition = leechbugSwarm.PlayerFish.transform.position;
			EnemySwarmManagement enemySwarm = (EnemySwarmManagement)enemySwarms[i];
			if(Vector3.Distance(leechbugPosition,enemySwarm.transform.position) > deAggroDistance)
            {
				enemySwarm.CalmSwarm();
				AutoHealSwarm(enemySwarm);
				ResetOutOfCombatSpeed(enemySwarm);
				enemySwarms.Remove(enemySwarm);
				foreach (Fish fish in enemySwarm.GetFishInSwarm())
				{
					if (fish.Defeated)
					{
						fish.Revive();
					}
				}
			}
        }

		if (enemySwarms.Count == 0)
			return true;

		else 
			return false;
	}

	// the player is fleeing and enemies will not pursue (might remove and do differently)
    public void Flee() {
		// calm all swarms and remove enemy fish from combat
		playerSwarm.CalmSwarm();

		//heal the player swarm
		AutoHealSwarm(playerSwarm);
		ResetOutOfCombatSpeed(playerSwarm);

		IsActive = false;
        //I noticed this was missing, decided to add it. -Design
        //TimeScaleController.ResetTimeScale();  //MAGIC NUMBER HERE
        /*if (slowTimeTickerManager != null)
        {
            slowTimeTickerManager.SetBool("SlowOn", false);
        }*//*
    }*/

    /*public void ToggleMenuOn(EnemySwarmManagement enemySwarmManagement)
    {
		postCombatMenuManager.GetComponent<PostCombatUIManager>().InitializeMenu(enemySwarmManagement);
    }

    public void ToggleMenuOff()
    {
		postCombatMenuManager.GetComponent<PostCombatUIManager>().CloseMenu();
	}*/
}

