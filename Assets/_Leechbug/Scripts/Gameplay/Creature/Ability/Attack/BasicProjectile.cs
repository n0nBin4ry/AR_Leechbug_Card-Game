using System;
using UnityEngine;

public class BasicProjectile : AProjectile
{
	protected float speed;
	protected float rotationSpeed;
	protected float range;
	protected float distanceFromStart;
	protected Vector3 lastPos;

	public Fish targetFish;
	public Fish parentFish;
	public event Action<Fish, BasicProjectile> OnHit;

	private bool isHoming;

	public void Initialize(Fish parent, float speed, float range)
	{
		// no target
		Initialize(parent, null, speed, range);
		isHoming = false;
	}
	public void Initialize(Fish parent, Fish target, float speed, float range)
	{
		parentFish = parent;
		parentFish.OnDefeat += Terminate;
		parentFish.OnDepossessFish += Terminate;
		lastPos = transform.position;
		this.speed = speed;
		this.range = range;
		targetFish = target;
		this.rotationSpeed = speed;
		if (target)
			isHoming = true;
	}

	private void Start()
	{
	}

	private void Update()
	{
		distanceFromStart += (transform.position - lastPos).magnitude;
		lastPos = transform.position;
		
		if (distanceFromStart > range)
		{
			Destroy(gameObject);
			return;
		}

		if (!targetFish || targetFish.Health.CurrentHealth < 0)
		{
			isHoming = false;
		}
		
		if (isHoming)
		{
			var targetPos = targetFish.transform.position;
			var dir = (targetPos - transform.position).normalized;
			Quaternion look = Quaternion.identity;
			if (dir != Vector3.zero)
			{
				look = Quaternion.LookRotation(dir);
			}
			transform.rotation = (Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime));
			transform.position += (dir * (speed * Time.deltaTime));
		}
		else
		{
			transform.position += transform.forward * (speed * Time.deltaTime);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		var fish = other.GetComponentInParent<Fish>();
		if (fish)
		{
			OnHit?.Invoke(fish, this);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void Terminate(Fish parent)
    {
		Destroy(gameObject);
    }

    private void OnDestroy()
    {
		parentFish.OnDefeat -= Terminate;
		parentFish.OnDepossessFish -= Terminate;
	}
}
