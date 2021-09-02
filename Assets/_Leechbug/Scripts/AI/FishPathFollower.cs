/*using UnityEngine;
using PathCreation;

// Moves along a path at constant speed.
// Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
public class FishPathFollower : MonoBehaviour {
	[Range(0.01f, 2f)]
	public float SpeedMod = 0.5f;
	public float AcclMod = 1f;

	public Fish ParentFish;
    [SerializeField] private PathCreator _pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
        
    private float _distanceTravelled;
	private float EPSILON = 0.1f;

    void Start() {
        if (_pathCreator != null) {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            _pathCreator.pathUpdated += OnPathChanged;
        }
    }

    void Update() {
        if (_pathCreator != null && ParentFish != null && ParentFish.Data != null) {
			// accelerate
			SpeedMod = SpeedMod * AcclMod * Time.deltaTime;

			// if far from point on path then approach the point we need to be at first
			// IDEA: if this is too unnatural, maybe keep moving point along path as fish speeds up to catch point then deccel to actual speed
			Vector3 toTarg = _pathCreator.path.GetPointAtDistance(_distanceTravelled, endOfPathInstruction) - ParentFish.transform.position;
			if (toTarg.sqrMagnitude > EPSILON * EPSILON) {
				toTarg.Normalize();
				ParentFish.TargetForward = toTarg;
				ParentFish.TargetVelocity = SpeedMod * ParentFish.Data.MaxSpeed * toTarg;
			}
			// follow along path if on path
			else {
				_distanceTravelled += SpeedMod * ParentFish.Data.MaxSpeed * Time.deltaTime;
				toTarg = _pathCreator.path.GetPointAtDistance(_distanceTravelled, endOfPathInstruction) - ParentFish.transform.position;
				toTarg.Normalize();
				ParentFish.TargetForward = toTarg;
				ParentFish.TargetVelocity = SpeedMod * ParentFish.Data.MaxSpeed * toTarg;
			}
                
        }
    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    void OnPathChanged() {
        _distanceTravelled = _pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }

	// unsubs from current path's event and assigns new path before subscribing to that one's event
	public void AssignPath(PathCreator path) {
		if (path == null || path == _pathCreator)
			return;
		if (_pathCreator != null)
			_pathCreator.pathUpdated -= OnPathChanged;
		_pathCreator = path;
		_pathCreator.pathUpdated += OnPathChanged;
	}

	public void ResetDistanceTravelled() {
		_distanceTravelled = 0f;
	}
}*/