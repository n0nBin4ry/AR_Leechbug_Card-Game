using UnityEngine;

[CreateAssetMenu(fileName = "New Fish Data", menuName = "FishData")]
public class FishData : ScriptableObject
{
    //[Header("Creature Description")]
    [SerializeField] string creatureName;
    [SerializeField] string namingAbbreviation;
    [SerializeField] bool isLeechbug;
    
    [SerializeField] int maxHealth;
    [SerializeField] ATargetedActiveAbility basicAttack;
    [SerializeField] AActiveAbility activeAbility;
    [SerializeField] APassiveAbility passiveAbility;

    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float rotationAcceleration;

    [SerializeField] float detectionRange;
    [SerializeField] float detectionAngle;

    [SerializeField] public GameObject BubbleTrail;

	public FishAIBehaviorInfo DefaultCombatBehaviorInfo;
	public FishAIBehaviorInfo DefaultIdleBehaviorInfo;

    [SerializeField] Material baseBodyMaterial;
    [SerializeField] Vector3 scaleModifier = new Vector3(1,1,1);

    [SerializeField] Sprite uiIcon;
    //[SerializeField] GameObject crySfx;

    public string CreatureName => creatureName;
    public string NamingAbbreviation => namingAbbreviation;
    public bool IsLeechbug => isLeechbug;
    public int MaxHealth => maxHealth;
    public float MaxSpeed => maxSpeed;
    public float Acceleration => acceleration;
    public float RotationAcceleration => rotationAcceleration;
    public float DetectionRange => detectionRange;
    public float DetectionAngle => detectionAngle;
    public ATargetedActiveAbility BasicAttackPrefab => basicAttack;
    public AActiveAbility ActiveAbilityPrefab => activeAbility;
    public APassiveAbility PassivePrefab => passiveAbility;
    public Material BaseBodyMaterial => baseBodyMaterial;
    public Vector3 ScaleModifier => scaleModifier;
    public Sprite UISprite => uiIcon;
    //public GameObject CrySfx => crySfx;
}