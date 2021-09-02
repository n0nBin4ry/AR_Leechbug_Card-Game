using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSwarmManagement : ASwarmManagement
{
    //Player Spawn Settings
    [Header("Player Spawn Settings")]
    [SerializeField] Fish leechbugPrefab;

    [Tooltip("Settings for configuring player's initial swarm members")]
    //[SerializeField] FishFactory _fishFactory;
    //fish to spawn
    //[SerializeField] List<FishSpawnCount> fishSpawnCounts;
    [SerializeField] Transform playerSpawnPoint;

	[Header("Follow Settings")]
	[SerializeField] FishAIBehaviorInfo PlayerFollowBehavior;
	[SerializeField] float _avoidRadius = 5f;
	[SerializeField] float _backlineDistance = 3f;
	[SerializeField] float _minDistance = 5f;
	[SerializeField] float _maxDistance = 50f;

    //UI
    //[SerializeField] ActiveUIManager activeUIManager;

    //Swarm Cap
    public int SwarmCap = 6;

    //public PlayerController PlayerController { get; private set; }
    public Fish PlayerFish { get; set; }
    public Fish Leechbug { get; set; }

    private void Start()
    {
        GenerateFish();
    }
    /*public override void GenerateFish()
    {
		//Spawning the Leechbug
		allFish = new List<Fish>();
		var fish = Instantiate(leechbugPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        PlayerController = new PlayerController(fish);
        GameManager.instance.PlayerFish = fish;
		allFish.Add(fish);
        fish.Initialize(PlayerController, this, FishFaction.A,false);
        PlayerFish = fish;
        Leechbug = fish;

		//Spawning additional fish
		foreach (FishSpawnCount fishSpawnCount in fishSpawnCounts)
		{
			for (int i = 0; i < fishSpawnCount.amount; i++)
			{
				//get the pooler associated with this fish
				ObjectPool specifiedCreaturePooler;
				if (fishSpawnCount.fishData)
					specifiedCreaturePooler = _fishFactory.getCreatureTypes().Find(obj => obj.getPrefab().GetComponent<Fish>().Data == fishSpawnCount.fishData);
				else //these lines can be removed once we have fishdata set in all scenes. temp bc it's a huge hassle to change all the swarms at once
					specifiedCreaturePooler = _fishFactory.getCreatureTypes().Find(obj => obj.getPrefab().GetComponent<Fish>().Data == fishSpawnCount.fish.Data);

				var swarmFish = specifiedCreaturePooler.Get(); //the creature prefab associated with the pooler
				Fish fishComponent = swarmFish.GetComponent<Fish>(); //the fish.cs from the prefab

				swarmFish.transform.position = playerSpawnPoint.transform.position + new Vector3(Random.Range(-SpawnLimits.x, SpawnLimits.x),
						Random.Range(-SpawnLimits.y, SpawnLimits.y),
						Random.Range(-SpawnLimits.z, SpawnLimits.z));
				swarmFish.transform.rotation = Quaternion.identity;
				swarmFish.transform.parent = gameObject.transform;

				var fishController = new BaseAIController(fishComponent);
				fishComponent.Initialize(fishController, this, FishFaction.A, specifiedCreaturePooler, false);
				allFish.Add(fishComponent);
			}
		}

		activeUIManager.UpdatePlayerUI();
		activeUIManager.UpdateSwarmUI();
        isSpawned = true;
    }*/

    public void ForceAddToSwarm(Fish fish)
    {
        fish.SwarmManagement.RemoveFromSwarm(fish);
		fish.DefaultIdleBehaviorInfo = PlayerFollowBehavior;
		allFish.Add(fish);
		fish.OnRecruit(this);
		//activeUIManager.UpdateSwarmUI();
    }
    
    public void AddToSwarm(Fish fish)
    {
        if (allFish.Count < SwarmCap)
        {
			ForceAddToSwarm(fish);
        }
    }

    public override void RemoveFromSwarm(Fish fish)
    {
        base.RemoveFromSwarm(fish);
        //activeUIManager.UpdatePlayerUI();
        //activeUIManager.UpdateSwarmUI();
    }

    /*public void PossessFish(Fish newPlayerFish)
    {
        //PlayerController playerController = PlayerController;
        Fish oldPlayerFish = PlayerFish;
		// remove old player from swarm if it was leechbug
		if (oldPlayerFish == Leechbug)
		{
			allFish.Remove(oldPlayerFish);
		}
		if(newPlayerFish.Faction == FishFaction.B)
        {
			AddToSwarm(newPlayerFish);
        }
		if(newPlayerFish.Defeated)
        {
			newPlayerFish.Revive();
        }

		newPlayerFish.Controller.Terminate();
		PlayerController = new PlayerController(newPlayerFish);
		newPlayerFish.Controller = PlayerController;

		oldPlayerFish.Controller.Terminate();
		oldPlayerFish.Controller = new BaseAIController(oldPlayerFish);
		oldPlayerFish.Controller.SetBehavior(PlayerFollowBehavior);

		PlayerFish = newPlayerFish;
        GameManager.instance.PlayerFish = PlayerFish;
		
		//Make Leechbug Creature Disappear
		if (newPlayerFish != Leechbug)
        {
            LeechbugDisappear();
        }
        activeUIManager.UpdatePlayerUI();
        activeUIManager.UpdateSwarmUI();
		oldPlayerFish.OnDepossess();
        newPlayerFish.OnPossess(this);
    }*/

    /*public void LeechbugDisappear()
    {
		// remove leechbug from swarm
		if (allFish.Contains(Leechbug))
			RemoveFromSwarm(Leechbug);
        //TODO: Animation for disappearing Leechbug
        Leechbug.gameObject.SetActive(false);
    }

	public void LeechbugReappear()
	{
		//TODO: Animation for reappearing Leechbug
		Leechbug.gameObject.transform.SetPositionAndRotation(PlayerFish.gameObject.transform.position, PlayerFish.gameObject.transform.rotation);
		Leechbug.gameObject.SetActive(true);
	}*/

	public void ResetSupportTargetForFish()
    {
		// Reset fish support target when Leechbug dies
		foreach (Fish fish in allFish)
		{
			if (fish.Controller.ControllerType != ControllerType.Player)
			{
				BaseAIController baseAiController = ((BaseAIController)fish.Controller);
				if (baseAiController.FishAIStateInfo.SupportManuallyAssigned)
                {
					baseAiController.SetSupportTargetNull();
				}
			}
		}
	}
	
	public override void MoveFish(Fish fish) {
		if (!fish || fish.Defeated || fish == PlayerFish || fish.Controller.GetType() != typeof(BaseAIController))
			return;

		// get our position in vector and how many active fish we have
		int my_index = -1;
		int active_count = 0;
		var teamFish = (fish.Faction == FishFaction.A)
			? CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.A).allFish
			: CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.B).allFish;
		
		for (int i = 0; i < teamFish.Count; i++) {
			Fish currFish = teamFish[i];
			if (!currFish || currFish == PlayerFish || currFish.Defeated)
				continue;
			if (currFish == fish)
				my_index = active_count;
			active_count++;
		}

		if (my_index < 0) {
			Debug.Log("ERROR: PlayerSwarmManagement: MoveFish: given fish is not in player swarm!");
			return;
		}

		float alignment_bias = (active_count % 2) == 0 ? (_avoidRadius / 2) : 0f;

		// move if fish is blocking player's fish from camera view
		float tru_backline_dist = Vector3.Dot(PlayerFish.transform.position - Camera.main.transform.position, PlayerFish.transform.forward);
		if (tru_backline_dist <= _backlineDistance)
			tru_backline_dist = _backlineDistance;
		// adding magic number 3 to keep fish out of camera
		else
			tru_backline_dist += 3f;

		// get target position of the fish based on it's index number; even number is back-left, right is back-right
		Vector3 lineupPos = PlayerFish.transform.position - (PlayerFish.transform.forward * tru_backline_dist)
			+ (PlayerFish.transform.right * (-alignment_bias + (_avoidRadius * ((my_index + 1) / 2) * ((my_index % 2) == 0 ? -1 : 1))));

		Vector3 toTarg = lineupPos - fish.transform.position;
		// if fish is way out of bounds, teleport behind player camera
		if (toTarg.sqrMagnitude >= _maxDistance * _maxDistance) {
			// spawn behind player and under the follow camera
			Vector3 playerToCam = Camera.main.transform.position - PlayerFish.transform.position;
			fish.transform.position = PlayerFish.transform.position + Vector3.Dot(playerToCam, (-1 * PlayerFish.transform.forward).normalized) * PlayerFish.transform.forward;
		}

		// if player fish is at lineup, stop moving and face the same as fish
		if (toTarg.sqrMagnitude < _minDistance * _minDistance) {
			fish.TargetVelocity = Vector3.zero;
			fish.TargetForward = PlayerFish.transform.forward;
		}
		else {
			fish.TargetForward = PlayerFish.transform.forward;
			fish.TargetVelocity = toTarg.normalized * PlayerFish.Data.MaxSpeed;
		}
	}
}
