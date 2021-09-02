using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleKeyboardController : MonoBehaviour {

    public float speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        Vector3 moveVec = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            moveVec += transform.forward;
        if (Input.GetKey(KeyCode.S))
            moveVec -= transform.forward;
        if (Input.GetKey(KeyCode.D))
            moveVec += transform.right;
        if (Input.GetKey(KeyCode.A))
            moveVec -= transform.right;
        
        moveVec.Normalize();
        transform.Translate(speed * Time.deltaTime * moveVec);
    }
}
