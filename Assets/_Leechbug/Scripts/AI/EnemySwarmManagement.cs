using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwarmManagement : ASwarmManagement
{
    //Enemy Spawn Settings
    //[Header("Enemy Spawn Settings")]
	//[SerializeField] FishFactory _fishFactory;
	//fish to spawn
	//[SerializeField] List<FishSpawnCount> fishSpawnCounts;
	//Spawner Settings
	[Tooltip("When player gets this close to an enemy spawner, enemy fish will spawn")]
	[SerializeField] float spawnDistance = 100;
	[Tooltip("When player gets this far away from an enemy spawner, enemy fish will de-aggro")]
	[SerializeField] float deaggroDistance = 150;
	[Tooltip("When player gets this far away from an enemy spawner, enemy fish will despawn")]
	[SerializeField] float despawnDistance = 200;
	[Tooltip("When this swarm is defeated, if true, they can respawn")]
	[SerializeField] bool isRespawnable = false;
	[Tooltip("When this swarm is defeated, if true, they cannot respawn if player swarm already recruited fishes")]
	[SerializeField] bool blockMultipleRecruit = false;
	[Tooltip("Elite Spawn Chance (0 for no elites, 1 for all fish to be elite (assuming they have an elite version")]
	[SerializeField] float eliteChance = 0.01f;

	[Header("Idle BOID Settings")]
	[Tooltip("Other fish (in same swarm) have to be in this radius to be accounted for in BOID algorithm.\n" +
		"It also has an effect on clumping, the higher it is, the more it adds to the anti-clumping force.")]
	[Range(0.01f, 10.0f)]
	public float NeighborDistance = 8.2f;
	[Tooltip("An extra weight to help seperate fish from clumping. Higher = less clumping.")]
	public float AntiClumpingForce = 2f;
	[Tooltip("The probability that the rules are applied each frame. When it is not applied, they float in the direction they were moving already or follow the goal if it is set.")]
	[Range(0, 100)]
	public int RulesAppliedProb = 80;
	protected bool _rulesAreApplied = false;
	// min/max values percentage for fish speed to vary between
	[Range(1f, 200f)]
	public float MinSpeedPercentage = 100f;
	[Range(1f, 200f)]
	public float MaxSpeedPercentage = 100f;
	[Tooltip("Distance ahead of fish that detects an obstacle to avoid.")]
	[Range(0.01f, 25f)]
	public float CollisionRaycastLen = 10.8f;
	[Tooltip("If checked: there is an added Goal Weight that pushes fish more towards a random point in the swim limits.")]
	public bool FollowGoal = true;
	private Vector3 _goalPos = Vector3.zero;
	[Tooltip("If following goal, and the fish haven't reached it in this amount of time, a new random goal point is made.")]
	public float GoalChangeCooldown = 3f;
	private float _goalChangedTimer = 0f;
	[Tooltip("If following goal, this is how close the fish needs to be to the goal position to reach it and produce a new goal point.")]
	public float DistanceToGoal = 1f;
	[Header("EXPERIMENTAL Idle BOID Settings")]
	[Tooltip("Probability the speed changes between a min/max idle speed percentage to add floating feeling. NOTE: this option has miniscule effect right now but you are free to try use it.")]
	[Range(0, 100)]
	public int SpeedChangeProb = 55;
	private bool _changeSpeed = false;
	[Tooltip("Added measure to keep fish from changing speed too often. The speed change probability only changes after these amount of seconds pass.")]
	public float SpeedChangeCooldown = 3f;
	private float _speedChangeCooldownTimer = 0f;

	private SphereCollider _spawnTrigger;

	//If swarm is defeated, and non-respawning, set to true!
	bool isSpent = false;
	//If swarm is active, return true

    private void Start()
    {
		_spawnTrigger = GetComponent<SphereCollider>();
		//set spawn trigger radius so that it's the distance to spawn fish when player enter
		_spawnTrigger.radius = spawnDistance;
		isSpawned = false;
    }

    public override void Update() {
		if(isSpawned) {
			base.Update();

			// change goal position if needed
			if (FollowGoal) {
				_goalChangedTimer += Time.deltaTime;
				// check if probability is to change it, if not one assigned, assign it
				if ((_goalChangedTimer > GoalChangeCooldown) || _goalPos == Vector3.zero) {
					_goalChangedTimer = 0f;
					_goalPos = Vector3.right;
					_goalPos = Quaternion.AngleAxis(Random.Range(0f, 359f), Vector3.up) * _goalPos;
					_goalPos = Quaternion.AngleAxis(Random.Range(0f, 359f), Vector3.forward) * _goalPos;
					_goalPos = SwimLimits.position + (_goalPos.normalized * Random.Range(0, SwimLimits.radius));
				}
				else {
					if (allFish.Count > 0) {
						foreach (var fish in allFish) {
							// if a fish from the swarm reaches the goal, assign new one
							if ((fish.transform.position - _goalPos).sqrMagnitude <= DistanceToGoal * DistanceToGoal) {
								_goalPos = Vector3.right;
								_goalPos = Quaternion.AngleAxis(Random.Range(0f, 359f), Vector3.up) * _goalPos;
								_goalPos = Quaternion.AngleAxis(Random.Range(0f, 359f), Vector3.forward) * _goalPos;
								_goalPos = SwimLimits.position + (_goalPos.normalized * Random.Range(0, SwimLimits.radius));
								break;
							}
						}
					}
				}
			}

			// check if we should apply rules to all fish or not
			if (RulesAppliedProb > Random.Range(1, 99))
				_rulesAreApplied = true;
			else
				_rulesAreApplied = false;

			// check if we change speed
			_speedChangeCooldownTimer += Time.deltaTime;
			if ((_speedChangeCooldownTimer > SpeedChangeCooldown) && (SpeedChangeProb > Random.Range(1, 99))) {
					_changeSpeed = true;
					_speedChangeCooldownTimer = 0f;
			}
			else {
				_changeSpeed = false;
			}
		}
	}
	
	public override void MoveFish(Fish fish) {
		base.MoveFish(fish);

		// keep track of extra weight applied to movement
		Vector3 extraForwardWeight = Vector3.zero;
		bool turning = false;
		// turn around if out of bounds
		if ((SwimLimits.position - fish.transform.position).sqrMagnitude > (SwimLimits.radius * SwimLimits.radius)) {
			var back = (SwimLimits.position - fish.transform.position);
			extraForwardWeight += Vector3.Reflect(fish.transform.forward, back.normalized) + back.normalized;
			turning = true;
		}

		// if we are about to collide with something then move out of the way
		RaycastHit hit;
		// walls/nets on default physics layer (0)
		if (Physics.Raycast(transform.position, fish.transform.forward * CollisionRaycastLen, out hit, (1 << 0))) {
			extraForwardWeight += Vector3.Reflect(fish.transform.forward, hit.normal);
		}

		// add follow goal weight if there is one
		if (FollowGoal && !turning) {
			extraForwardWeight += _goalPos - fish.transform.position;		
		}

		float speed;
		// check to see if we should change speed
		if (_changeSpeed) {
			speed = (Random.Range(MinSpeedPercentage, MaxSpeedPercentage) / 100f) * fish.Data.MaxSpeed;
		}
		else {
			speed = fish.TargetVelocity.sqrMagnitude;
			if (speed == 0)
				speed = fish.Data.MaxSpeed * 0.5f;
			else
				speed = fish.TargetVelocity.magnitude;
			speed = Mathf.Clamp(speed, fish.Data.MaxSpeed * MinSpeedPercentage / 100f, fish.Data.MaxSpeed * MaxSpeedPercentage / 100f);
		}

		extraForwardWeight.Normalize();
		// apply the BOID rules with extra weight if probability allows
		if (_rulesAreApplied && !turning) {
			ApplyBoidMovementRules(fish, extraForwardWeight, speed);
		}
		// else apply extra weights, and seperate to prevent clumping
		else {
			ApplyBasicMovementSeperation(fish, extraForwardWeight, speed);
		}
	}

	/*public override void GenerateFish()
    {
		// Instantiate each fish at random positions in swarm's limits
		allFish = new List<Fish>();
		foreach (FishSpawnCount fishSpawnCount in fishSpawnCounts)
		{
			for (int i = 0; i < fishSpawnCount.amount; i++)
			{
				//get the pooler associated with this fish
				ObjectPool specifiedCreaturePooler;
				if (fishSpawnCount.fishData)
					specifiedCreaturePooler = _fishFactory.getCreatureTypes().Find(obj=>obj.getPrefab().GetComponent<Fish>().Data == fishSpawnCount.fishData);
				else //these lines can be removed once we have fishdata set in all scenes. temp bc it's a huge hassle to change all the swarms at once
					specifiedCreaturePooler = _fishFactory.getCreatureTypes().Find(obj=>obj.getPrefab().GetComponent<Fish>().Data == fishSpawnCount.fish.Data);

				var fish = specifiedCreaturePooler.Get(); //the creature prefab associated with the pooler
				Fish fishComponent = fish.GetComponent<Fish>(); //the fish.cs from the prefab
				
				fish.transform.position =  this.transform.position + new Vector3(Random.Range(-SpawnLimits.x, SpawnLimits.x),
						Random.Range(-SpawnLimits.y, SpawnLimits.y),
						Random.Range(-SpawnLimits.z, SpawnLimits.z));
				fish.transform.rotation = Quaternion.identity;
				fish.transform.parent = gameObject.transform;

				bool isElite = false;
				float eliteValue = UnityEngine.Random.Range(0.0f, 1.0f);
				if (eliteValue <= eliteChance)
				{
					isElite = true;
				}

				var fishController = new BaseAIController(fishComponent);

				fishComponent.Initialize(fishController,this, FishFaction.B, specifiedCreaturePooler, isElite);
				allFish.Add(fishComponent);
			}
		}
	}*/

	public void DespawnFish()
    {
		Debug.Log("Despawning Fish!!");
		Debug.Log(allFish.Count);
		int allFishSize = allFish.Count;
		if (allFish.Count > 0)
        {
			for (int i = allFish.Count - 1; i > -1; i--)
			{
				Debug.Log("Deleting Fish: " + i);
				allFish[i].Terminate();
			}
		}

    }

	private void ApplyRulesSheehan(Fish fish, Vector3 extraForwardWeight, float speed) {
		Vector3 flyTowardCenter = Vector3.zero;
		if (allFish.Count > 1) {
			Vector3 sum = Vector3.zero;
			for (int i = 0; i < allFish.Count; i++) {
				Fish other = allFish[i];
				if (fish == other)
					continue;
				sum += other.transform.position;
			}
			Vector3 avg = sum / allFish.Count;

			flyTowardCenter = avg - fish.transform.position * .01f;
		}

		Vector3 repelForce = Vector3.zero;
		for (int i = 0; i < allFish.Count; i++) {
			Fish other = allFish[i];
			if (fish == other)
				continue;
			if ((other.transform.position - fish.transform.position).sqrMagnitude < 10f) {
				repelForce = fish.transform.position - other.transform.position;
			}
		}

		Vector3 velocityMatch = Vector3.zero;
		if (allFish.Count > 1) {
			Vector3 sum = Vector3.zero;
			for (int i = 0; i < allFish.Count; i++) {
				Fish other = allFish[i];
				if (fish == other)
					continue;
				sum += other.Velocity * Mathf.Max(0f, 10f - (other.transform.position - fish.transform.position).sqrMagnitude);
			}
			Vector3 avg = sum / allFish.Count;

			velocityMatch = avg * .125f;
		}

		Vector3 tooFarForce = Vector3.zero;

		if (Vector3.Distance(fish.transform.position, Vector3.zero) > 20) {
			tooFarForce = -fish.transform.position;
		}

		var finalVelDir = fish.transform.forward + flyTowardCenter + repelForce + velocityMatch + tooFarForce;
		fish.TargetVelocity = finalVelDir;
		fish.TargetForward = finalVelDir.normalized;
	}

	void ApplyBoidMovementRules(Fish fish, Vector3 extraForwardWeight, float speed) {
		var currentPosition = fish.transform.position;

		// Initializes the vectors.
		var separation = Vector3.zero;
		var alignment = fish.transform.forward;
		var cohesion = fish.transform.position;

		// Accumulates the vectors.
		int fishCount = 1;
		foreach (var boid in allFish) {
			if (boid == fish || boid.transform.position == currentPosition)
				continue;
			
			var t = boid.transform;

			// get vector for seperation (other-to fish)
			var diff = currentPosition - t.transform.position;
			// only count fish that are in neighbor distance
			if (diff.sqrMagnitude > NeighborDistance * NeighborDistance)
				continue;
			fishCount++;

			// collect speed of neighbor fish to average-out speed
			speed += boid.TargetVelocity.magnitude;
			// get a force for seperation
			var diffLen = diff.magnitude;
			var scaler = Mathf.Clamp01(1.0f - diffLen / NeighborDistance);
			separation += diff * (scaler / diffLen) * AntiClumpingForce;

			alignment += boid.TargetForward;
			cohesion += t.position;
		}

		// average out all our neighbor forces
		var avg = 1.0f / fishCount;
		speed *= avg;
		alignment *= avg;
		cohesion *= avg;
		cohesion = (cohesion - currentPosition).normalized;

		// want fish to only turn for alignment, but move with other forces (more natural)
		fish.TargetForward = alignment.normalized;
		if (fish.TargetForward.sqrMagnitude == 0)
			fish.TargetForward = fish.transform.forward;
		fish.TargetVelocity = (fish.TargetForward * speed) + separation + cohesion + extraForwardWeight;
	}

	// similar to ApplyBoidMovementRules, but just applies any extra weights and 
	void ApplyBasicMovementSeperation(Fish fish, Vector3 extraForwardWeight, float speed) {
		var currPos = fish.transform.position;

		var separation = Vector3.zero;

		foreach (var boid in allFish) {
			if (boid == fish || boid.transform.position == fish.transform.position)
				continue;

			var t = boid.transform;
			var diff = currPos - t.position;
			if (diff.sqrMagnitude > NeighborDistance * NeighborDistance)
				continue;

			var diffLen = diff.magnitude;
			var scaler = Mathf.Clamp01(1.0f - diffLen / NeighborDistance);
			separation += diff * (scaler / diffLen) * AntiClumpingForce;
		}

		fish.TargetForward = (fish.TargetForward + extraForwardWeight).normalized;
		if (fish.TargetForward.sqrMagnitude == 0)
			fish.TargetForward = fish.transform.forward;
		fish.TargetVelocity = (fish.TargetForward * speed) + separation;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(_goalPos, DistanceToGoal);
	}

    private void OnTriggerEnter(Collider other)
    {
		//if Object is fish
		if(other.tag == "Fish")
        {
			//If fish aren't already spawned and the spawner isn't spent
			if (!isSpawned && !isSpent)
			{
				//If fish that enters trigger is friendly
				if (other.gameObject.GetComponent<Fish>().Faction == FishFaction.A)
				{
					Debug.Log("A player fish entered collider, spawn fish!");
					GenerateFish();
					isSpawned = true;
					//set spawn trigger radius so that it's the distance to despawn fish when players exit
					_spawnTrigger.radius = despawnDistance;
				}
			}
		}

    }

	private void OnTriggerExit(Collider other)
	{
		//if Object is fish
		if (other.tag == "Fish")
		{
			//If fish are already spawned
			if (isSpawned)
			{
				if (_spawnTrigger.radius == despawnDistance)
                {
					//If fish that exits trigger is friendly
					if (other.gameObject.GetComponent<Fish>().Faction == FishFaction.A)
					{
						Debug.Log("A player fish left collider, despawn fish!");
						_spawnTrigger.radius = spawnDistance;
						DespawnFish();
						isSpawned = false;
						//set spawn trigger radius so that it's the distance to spawn fish when players enter
					}
				}
			}
		}
	}

	public void SwarmDefeated()
    {
		if(isRespawnable)
        {
			if(blockMultipleRecruit)
            {
				PlayerSwarmManagement psm = GameObject.Find("PlayerSwarm").GetComponent<PlayerSwarmManagement>();
				if(psm != null)
                {
					isSpent = false;
					foreach(Fish fish in allFish)
                    {
						foreach(Fish playerFish in psm.allFish)
                        {
							if(fish.name.Equals(playerFish.name))
                            {
								isSpent = true;
                            }
                        }
                    }
                }
            }
        } else
        {
			isSpent = true;
		}
    }
}
