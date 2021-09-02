using System;
using UnityEngine;

public class BossBeam : MonoBehaviour
{
	public event Action<Fish> OnEnter;
	public event Action<Fish> OnExit;
	private void OnTriggerEnter(Collider other)
	{
		var fish = other.GetComponent<Fish>();
		if (fish)
		{
			OnEnter?.Invoke(fish);
		}
	}

    private void OnTriggerExit(Collider other)
    {
		var fish = other.GetComponent<Fish>();
		if (fish)
		{
			OnExit?.Invoke(fish);
		}
	}

}