using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class DividerVirtualButton : MonoBehaviour {
    public VirtualButtonBehaviour Vb;
    // Start is called before the first frame update
    void Start() {
        Vb.UnregisterOnButtonPressed(OnButtonPressed);
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb) {
        CombatManager.Instance.InitializeCombat();
        Debug.Log("COMBAT BUTTON PRESSED");
    }
}
