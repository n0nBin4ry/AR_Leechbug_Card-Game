using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldDivider : MonoBehaviour {

    // return the team the fish would belong to in the 
    public CombatManager.TeamType DecideTeam(Fish fish) {
        if (!fish) {
            Debug.Log("No fish given to FieldDivider.DecideFish()");
            return CombatManager.TeamType.none;
        }

        // using a plane to decide what team fish on; position as a point on plane that will use the direction as the normal vector
        var toFish = fish.transform.position - transform.position;
        return ((Vector3.Dot(toFish, gameObject.transform.forward) > 0) ? CombatManager.TeamType.B : CombatManager.TeamType.A);
    }

    // Start is called before the first frame update
    private void Start() {
        // assign self to combat manager
        CombatManager.Instance.AssignFieldDivider(this); 
    }

    private void Update() {
        // debug
        if (Input.GetKeyDown(KeyCode.Space))
            CombatManager.Instance.InitializeCombat();
    }
}
