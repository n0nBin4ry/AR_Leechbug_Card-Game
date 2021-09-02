using System;
using System.Collections;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] public float Seconds;

    private void Start()
    {
        StartCoroutine(DestroyAfter());
    }

    IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(Seconds);
        if (gameObject)
            Destroy(gameObject);
    }
}