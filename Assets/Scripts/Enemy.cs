using DG.Tweening;
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

	[Header("Parameters")]
	private Path path;

	private bool isDoingAction;
	private NavMeshAgent agent;
	private PathActionPoint currentPointAction;
	private Coroutine rotateCoroutine;
	private Coroutine waitCoroutine;

	internal void Init(Path path)
	{
		agent = GetComponent<NavMeshAgent>();
		this.path = path;
		transform.position = path.GetFirstActionPoint().transform.position;

		DoNextActionOnPath();
	}

	private void DoNextActionOnPath()
	{
		isDoingAction = false;

		if (currentPointAction != null)
		{
			currentPointAction.SetAsDone();
		}

		if (path.HasAnotherPoint)
		{
			currentPointAction = path.GetNextPoint();
			DoAction(currentPointAction);
		}
	}

	private void DoAction(PathActionPoint pathActionPoint)
	{
		isDoingAction = true;

		switch (pathActionPoint.Type)
		{
			case PathActionPointType.GoToNext:
				MoveToTarget(pathActionPoint.transform.position);
				break;
			case PathActionPointType.GoToFirst:
				MoveToTarget(path.GetFirstActionPoint().transform.position);
				break;
			case PathActionPointType.Wait:
				DoWait();
				break;
			case PathActionPointType.Rotate:
				DoRotate();
				break;
			case PathActionPointType.ChangePath:
				DoChangePath();
				break;
			case PathActionPointType.RestartPath:
				currentPointAction = null;
				path.RestartPath();
				DoNextActionOnPath();
				break;
			default:
				break;
		}
	}

	private void DoChangePath()
	{
		if (currentPointAction.Probability > Random.value)
		{
			Path newPath = Instantiate(currentPointAction.NewPath, path.transform.parent);
			Destroy(path.gameObject);
			path = newPath;
			path.Init();
		}

		DoNextActionOnPath();
	}

	private void DoRotate()
	{
		if (rotateCoroutine != null)
		{
			StopCoroutine(rotateCoroutine);
		}

		rotateCoroutine = StartCoroutine(DoRotateCore());
	}

	private IEnumerator DoRotateCore()
	{
		transform.DORotate(new Vector3(0f, currentPointAction.RotateAngle, 0f), currentPointAction.RotateDuration, RotateMode.FastBeyond360).SetEase(currentPointAction.RotationEase).SetRelative();

		yield return new WaitForSeconds(currentPointAction.RotateDuration);
		DoNextActionOnPath();
	}

	private void DoWait()
	{
		if (waitCoroutine != null)
		{
			StopCoroutine(waitCoroutine);
		}

		waitCoroutine = StartCoroutine(DoWaitCore());
	}

	private IEnumerator DoWaitCore()
	{
		yield return new WaitForSeconds(currentPointAction.WaitDuration);
		DoNextActionOnPath();
	}

	private void MoveToTarget(Vector3 target)
	{
		agent.destination = target;
	}

	private void Update()
	{
		if (isDoingAction)
		{
			if (currentPointAction.Type == PathActionPointType.GoToNext || currentPointAction.Type == PathActionPointType.GoToFirst)
			{
				if (agent.remainingDistance == 0)
				{
					DoNextActionOnPath();
				}
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, agent.destination);
	}
}
