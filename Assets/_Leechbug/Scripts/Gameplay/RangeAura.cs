using System;
using UnityEngine;

// temp name
public class RangeAura : MonoBehaviour
{
    [SerializeField] private AActiveAbility _activeAbility;
    [SerializeField] private float maxOpacity;
    [SerializeField] public AnimationCurve curve;
    [SerializeField] private bool disableScaling;

    private Material _material;
    private static readonly int opacity = Shader.PropertyToID("Vector1_DE3EF07C");

    private void Start()
    {
        _material = GetComponent<Renderer>().material;
        if (!disableScaling)
            transform.localScale = Vector3.one * _activeAbility.Range * 2;
    }

    private void Update()
    {
        _material.SetFloat(opacity, curve.Evaluate(1-_activeAbility.CooldownTimeLeftPercent) * maxOpacity);
    }
}