using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudProjectile : AProjectile
{
	protected float speed;
	protected float duration;
	protected Vector3 lastPos;

	public event Action<Fish, CloudProjectile> OnEnter;
	public event Action<Fish, CloudProjectile> OnExit;
	public event Action<CloudProjectile> OnTerminate;
	protected Rigidbody _rigidbody;

	float currentTimer;
	Vector3 targetVector;
	public void Initialize(Vector3 direction, float speed, float duration)
	{
		lastPos = transform.position;
		this.speed = speed;
		this.duration = duration;
		currentTimer = duration;
		targetVector = direction;
		
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		currentTimer -= Time.deltaTime;
		if (currentTimer <= 0)
		{
			OnTerminate?.Invoke(this);
			Destroy(gameObject);
		}
		else
		{
			_rigidbody.MovePosition(transform.position + targetVector * (speed * Time.deltaTime));
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		var fish = other.GetComponent<Fish>();
		if (fish)
		{
			OnEnter?.Invoke(fish, this);
		}
	}

    private void OnTriggerExit(Collider other)
    {
		var fish = other.GetComponent<Fish>();
		if (fish)
		{
			OnExit?.Invoke(fish, this);
		}
	}
}
