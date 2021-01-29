using System.Collections;
using Tools.Utils;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
	[Header("Moving Parameters")]
	[SerializeField, FloatRangeSlider(0f, 10f)] private FloatRange walkRadius = new FloatRange(1f, 3f);
	[SerializeField, FloatRangeSlider(0f, 10f)] private FloatRange timeBetweenMove = new FloatRange(1f, 3f);

	private NavMeshAgent agent;
	private NavMeshHit hit;
	private Coroutine moving;

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	public void StartMovingRandomly()
	{
		if (agent == null)
		{
			agent = GetComponent<NavMeshAgent>();
		}

		moving = StartCoroutine(Move());
	}

	public void StopMovingRandomly()
	{
		if (moving != null)
		{
			StopCoroutine(moving);
		}
	}

	private IEnumerator Move()
	{
		while (true)
		{
			float currentWalkRadius = walkRadius.RandomValue;
			Vector3 randomDirection = Random.insideUnitSphere * currentWalkRadius;
			randomDirection += transform.position;

			NavMesh.SamplePosition(randomDirection, out hit, currentWalkRadius, 1);
			Vector3 finalPosition = hit.position;
			agent.destination = finalPosition;

			yield return new WaitForSeconds(timeBetweenMove.RandomValue);
		}
	}
}
