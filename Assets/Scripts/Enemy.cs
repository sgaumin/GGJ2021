using DG.Tweening;
using System.Collections;
using Tools.Utils;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
	[Header("Moving Parameters")]
	[SerializeField] private float minDistanceToTarget = 0.1f;
	[SerializeField] private float minDistanceToPlayer = 0.1f;
	[SerializeField] private float speedMultiplierChase = 1.5f;

	[Header("Detection Parameters")]
	[SerializeField] private float angleDetection = 90f;
	[SerializeField] private float distanceDetection = 3f;
	[SerializeField] private float raycastCount = 3f;
	[SerializeField] private LayerMask playerLayer;

	[Header("Sound")]
	[SerializeField, FloatRangeSlider(0f, 20f)] private FloatRange enemySoundLimits = new FloatRange(1f, 3f);
	[FMODUnity.EventRef, SerializeField] private string enemySound;

	[Header("Debug")]
	[SerializeField] private Path path;
	[SerializeField] private bool showDestinationDraw = true;
	[SerializeField] private bool isIgnoringPlayer;

	[Header("References")]
	[SerializeField] private Animator animator;

	private FMOD.Studio.EventInstance enemySoundInstance;
	private bool isChasingPlayer;
	private bool isDoingAction;
	private NavMeshAgent agent;
	private PathActionPoint currentPointAction;
	private Coroutine rotateCoroutine;
	private Coroutine waitCoroutine;
	private PlayerController playerToChase;
	private float startSpeed;

	public LevelSpawner Level { get; set; }

	internal void Init(Path path)
	{
		agent = GetComponent<NavMeshAgent>();
		this.path = path;

		transform.position = path.GetFirstActionPoint().transform.position;
		startSpeed = agent.speed;

		DoNextActionOnPath();

		enemySoundInstance = FMODUnity.RuntimeManager.CreateInstance(enemySound);
		enemySoundInstance.start();
	}

	private void DoNextActionOnPath()
	{
		animator.SetBool("idle", true);
		animator.SetBool("walk", false);

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
		animator.SetBool("idle", true);
		animator.SetBool("walk", false);

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
			animator.SetBool("idle", true);
			animator.SetBool("walk", false);

			Path newPath = Instantiate(currentPointAction.NewPath, path.transform.parent);
			Destroy(path.gameObject);
			path = newPath;
			path.Init();
		}

		DoNextActionOnPath();
	}

	private void DoRotate()
	{
		animator.SetBool("idle", true);
		animator.SetBool("walk", false);

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
		animator.SetBool("idle", true);
		animator.SetBool("walk", false);

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
		animator.SetBool("idle", false);
		animator.SetBool("walk", true);

		agent.destination = target;
	}

	private void DoChasePlayer()
	{
		MusicHolder.Instance.SetChaseMusic();

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
		agent.speed *= speedMultiplierChase;
		MoveToTarget(playerToChase.transform.position);
	}

	private void StopChasingPlayer()
	{
		MusicHolder.Instance.SetFirstMusic();

		isChasingPlayer = false;
		agent.speed = startSpeed;
		DoSameActionOnPath();
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

			if (!isIgnoringPlayer && Level.Game.GameState == GameStates.Play)
			{
				for (int i = 0; i < raycastCount; i++)
				{
					Vector3 direction = Quaternion.AngleAxis(-angleDetection + 2 * angleDetection / (raycastCount - 1) * i, Vector3.up) * transform.forward;
					Debug.DrawRay(transform.position, direction * distanceDetection, Color.green);

					RaycastHit hit;
					if (Physics.Raycast(transform.position, direction, out hit, distanceDetection))
					{
						playerToChase = hit.collider.GetComponent<PlayerController>();
						if (playerToChase != null && !playerToChase.IsHidden)
						{
							isChasingPlayer = true;
							DoChasePlayer();
						}
					}
				}
			}
		}
		else
		{
			if (playerToChase != null)
			{
				MoveToTarget(playerToChase.transform.position);

				if (playerToChase.IsHidden)
				{
					StopChasingPlayer();
				}

				if (agent.remainingDistance <= minDistanceToPlayer)
				{
					CatchPlayer();
					playerToChase = null;
				}
			}
			else
			{
				StopChasingPlayer();
			}
		}

		CheckDistanceWithPlayer();
	}

	private void CatchPlayer()
	{
		if (Level.Game.GameState == GameStates.Play)
		{
			Level.Game.GameState = GameStates.GameOver;

			// Play Animation
			animator.SetTrigger("punch");
			animator.SetBool("idle", true);
			animator.SetBool("walk", false);

			// Loose
			Level.Game.LoadByName("Loose");
		}
	}

	private void CheckDistanceWithPlayer()
	{
		float distance = Vector3.Distance(Level.Player.transform.position, transform.position);
		float ratio = (distance - enemySoundLimits.Min) / (enemySoundLimits.Max - enemySoundLimits.Min);

		enemySoundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
		enemySoundInstance.setParameterByName("Ennemie ", Mathf.Min(1f - (float)ratio, 1f));
	}

	private void OnDrawGizmos()
	{
		if (showDestinationDraw)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, agent.destination);
		}
	}

	private void OnDestroy()
	{
		enemySoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}
}
