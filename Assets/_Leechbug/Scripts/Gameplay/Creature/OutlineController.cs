using System;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private int enemyOutlineLayer, friendlyOutlineLayer, defaultOutlineLayer;
    [SerializeField] private CombatManager _manager;

    private Camera cam;
    private float sqrRange;

    // TODO temp audio things that don't belong here
    //private FMODUnity.StudioEventEmitter _fmodEmitter;
    //private FMODUnity.StudioGlobalParameterTrigger _fmodParam;

    private void Start()
    {
        //_fmodEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
        //_fmodParam = GetComponent<FMODUnity.StudioGlobalParameterTrigger>();

        // prevent combat music from playing when restarting
        //_fmodParam.value = 0;
        //_fmodParam.TriggerParameters();

        cam = Camera.main;
        sqrRange = range * range;
        /*if (!_fmodEmitter.IsPlaying())
        {
            _fmodEmitter.Play();
        }*/
    }

    private void OnDestroy()
    {
        //_fmodEmitter.Stop();
    }

    private void Update()
    {
        bool hasEnemy = false;
        var colliders = Physics.OverlapSphere(cam.transform.position, range + 1);
        foreach (var coll in colliders)
        {
            var fish = coll.GetComponentInParent<Fish>();
            var camPos = cam.transform.position;
            if (fish && fish.Health != null)
            {
                // assuming mesh is in same object as Fish script
                if (fish.Health.IsZeroHealth ||
                    (coll.ClosestPoint(camPos) - camPos).sqrMagnitude > sqrRange)
                {
                    fish.mesh.layer = defaultOutlineLayer;
                }
                else if (fish.Faction == FishFaction.B)
                {
                    fish.mesh.layer = enemyOutlineLayer;
                    hasEnemy = true;
                }
                else if (fish.Faction == FishFaction.A)
                {
                    fish.mesh.layer = friendlyOutlineLayer;
                }
            }
             
            var bullet = coll.GetComponent<BasicProjectile>(); // TODO temp
            if (bullet && bullet.targetFish)
            {
                if (bullet.targetFish.Faction == FishFaction.A)
                    bullet.gameObject.layer = enemyOutlineLayer;
                else if (bullet.targetFish.Faction == FishFaction.B)
                    bullet.gameObject.layer = friendlyOutlineLayer;
            }
        }

        if (hasEnemy || _manager.IsActive)
        {
            /*if (_fmodParam.value != 1)
            {
                _fmodParam.value = 1;
                _fmodParam.TriggerParameters();
            }*/
        }
        else
        {
            /*if (_fmodParam.value != 0)
            {
                _fmodParam.value = 0;
                _fmodParam.TriggerParameters();
            }*/
        }
    }
}