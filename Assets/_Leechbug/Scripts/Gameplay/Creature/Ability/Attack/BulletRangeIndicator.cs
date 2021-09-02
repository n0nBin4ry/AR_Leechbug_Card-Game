using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BulletRangeIndicator : MonoBehaviour {

     public float _range;
    private void Start() {
        transform.localScale = Vector3.one*_range;
    }

    private void Update() {
        this.transform.up = Camera.main.transform.forward;
    }
    
}