using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
	[Header("Moving Parameters")]
	[SerializeField] private float minDistanceToTarget = 0.1f;

	[Header("Detection Parameters")]
	[SerializeField] private float angleDetection = 90f;
	[SerializeField] private float distanceDetection = 3f;
	[SerializeField] private float raycastCount = 3f;
	[SerializeField] private LayerMask playerLayer;

	[Header("Debug")]
	[SerializeField] private Path path;
	[SerializeField] private bool showDestinationDraw = true;

	private bool isChasingPlayer;
	private bool isDoingAction;
	private NavMeshAgent agent;
	private PathActionPoint currentPointAction;
	private Coroutine rotateCoroutine;
	private Coroutine waitCoroutine;
	private PlayerController playerToChase;

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

	private void DoSameActionOnPath()
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

	private void DoChasePlayer()
	{
		UIManager.Instance.SetText("Player Detected");

		// Stop every action
		if (waitCoroutine != null)
		{
			StopCoroutine(waitCoroutine);
		}
		if (rotateCoroutine != null)
		{
			StopCoroutine(rotateCoroutine);
		}

		// Assign destination as Player
		MoveToTarget(playerToChase.transform.position);
	}

	private void Update()
	{
		if (!isChasingPlayer)
		{
			if (isDoingAction)
			{
				if (currentPointAction.Type == PathActionPointType.GoToNext || currentPointAction.Type == PathActionPointType.GoToFirst)
				{
					if (agent.remainingDistance <= minDistanceToTarget)
					{
						DoNextActionOnPath();
					}
				}
			}

			for (int i = 0; i < raycastCount; i++)
			{
				Vector3 direction = Quaternion.AngleAxis(-angleDetection + 2 * angleDetection / (raycastCount - 1) * i, Vector3.up) * transform.forward;
				Debug.DrawRay(transform.position, direction * distanceDetection, Color.green);

				RaycastHit hit;
				if (Physics.Raycast(transform.position, direction, out hit, distanceDetection))
				{
					if (hit.collider.GetComponent<PlayerController>())
					{
						//isChasingPlayer = true;
						//playerToChase = hit.collider.GetComponent<PlayerController>();
						//DoChasePlayer();

						UIManager.Instance.SetText("Player Detected");
					}
				}
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (showDestinationDraw)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, agent.destination);
		}
	}
}
