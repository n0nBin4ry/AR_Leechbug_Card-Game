using System.Collections;
using UnityEngine;

public class FishLine : MonoBehaviour
{
    [SerializeField] private float destroyAfterSeconds;
    public Fish source, target;
    private LineRenderer line;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        StartCoroutine(DestroyAfterSeconds());
    }

    private void Update()
    {
        line.useWorldSpace = true;
        line.SetPosition(0, source.transform.position);
        line.SetPosition(1, target.transform.position);
    }

    IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);
        Destroy(gameObject);
    }
}