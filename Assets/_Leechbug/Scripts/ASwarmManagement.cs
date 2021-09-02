using System.Collections.Generic;
using UnityEngine;

public abstract class ASwarmManagement : MonoBehaviour {
	[Header("Spawn Settings")]
	//Array of fish contained in swarm
	public List<Fish> allFish = new List<Fish>();
	// where we want the flock to go
	public Vector3 goalPosBattleUtils;

	// the limit of where fish can be away from containing object
	public BoundingSphere SwimLimits = new BoundingSphere(Vector3.zero, 10f);
	public Vector3 SpawnLimits = new Vector3(5, 5, 5);

	public bool isSpawned = false;

	// Start is called before the first frame update
	void Start() {
		SwimLimits.position = transform.position;
	}

    // Update is called once per frame
    public virtual void Update() {
		SwimLimits.position = transform.position;
    }

	public virtual void GenerateFish()
    {

	}

	public virtual void MoveFish(Fish fish) { 
		
	}

	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, SwimLimits.radius);
	}

	public List<Fish> GetFishInSwarm()
    {
		return allFish;
    }

	public int GetFishCount()
    {
		return allFish.Count;
    }

	public virtual void RemoveFromSwarm(Fish fish)
	{
		allFish.Remove(fish);
	}

	public bool CheckIfAllDefeated()
    {
		foreach(Fish fish in allFish)
        {
			if(!fish.Defeated)
            {
				return false;
            }
        }
		return true;
    }

	public void CalmSwarm() {
		foreach (var fish in allFish) {
			if (fish.Controller?.GetType() == typeof(BaseAIController)) {
				((BaseAIController)fish.Controller).Calm();
			}
		}
	}

	public void AlertSwarm() {
		foreach (var fish in allFish) {
			if (fish.Controller?.GetType() == typeof(BaseAIController)) {
				((BaseAIController)fish.Controller).Alert();
			}
		}
	}
}
