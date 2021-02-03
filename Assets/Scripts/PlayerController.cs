using System.Collections;
using System.Linq;
using Tools.Utils;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// https://www.immersivelimit.com/tutorials/simple-character-controller-for-unity

	[Header("Player Parameters")]
	[SerializeField] private float moveSpeed;

	[Header("Audio")]
	[SerializeField] private float footSoundDuration = 0.2f;
	[SerializeField] private LayerMask carpetLayer;
	[FMODUnity.EventRef, SerializeField] private string footStepSound;
	[Space]
	[SerializeField, FloatRangeSlider(0f, 20f)] private FloatRange heartSoundLimits = new FloatRange(1f, 3f);
	[FMODUnity.EventRef, SerializeField] private string heartSound;
	[Space]
	[SerializeField] private float distanceSafeEnemy = 7f;
	[SerializeField, IntRangeSlider(0, 10)] private IntRange enemySurpriseSound = new IntRange(1, 3);
	[FMODUnity.EventRef, SerializeField] private string surpriseSound;

	[Header("References")]
	[SerializeField] private GameObject model;

	private Rigidbody rb;
	private Animator animator;
	private bool isCrouching;
	private Quaternion rotation = Quaternion.identity;
	private float horizontal;
	private float vertical;
	private FMOD.Studio.EventInstance footStepInstance;
	private FMOD.Studio.EventInstance heartSoundInstance;
	private FMOD.Studio.EventInstance surpriseSoundInstance;
	private Coroutine playFootStepSound = null;
	private Enemy closestEnemy;
	private bool isSafe;
	private int enemySurpriseSoundCount;
	private float walkingOnCarpet;

	public LevelSpawner Level { get; set; }
	public bool IsHidden { get; set; }

	protected void Start()
	{
		rb = GetComponent<Rigidbody>();
		animator = GetComponentInChildren<Animator>();

		isSafe = true;

		footStepInstance = FMODUnity.RuntimeManager.CreateInstance(footStepSound);
		surpriseSoundInstance = FMODUnity.RuntimeManager.CreateInstance(surpriseSound);
		heartSoundInstance = FMODUnity.RuntimeManager.CreateInstance(heartSound);
		heartSoundInstance.start();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		ProcessActions();
		CheckDistanceWithEnemies();
		CheckGround();
	}

	private void CheckDistanceWithEnemies()
	{
		closestEnemy = Level.Enemies.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).FirstOrDefault();
		float distance = Vector3.Distance(transform.position, closestEnemy.transform.position);
		float ratio = (distance - heartSoundLimits.Min) / (heartSoundLimits.Max - heartSoundLimits.Min);

		heartSoundInstance.setParameterByName("Ennemie ", Mathf.Min(1f - (float)ratio, 1f));

		if (distance <= distanceSafeEnemy && isSafe == true)
		{
			isSafe = false;
			if (enemySurpriseSoundCount++ > enemySurpriseSound.RandomValue)
			{
				enemySurpriseSoundCount = 0;

				surpriseSoundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
				surpriseSoundInstance.start();
			}
		}
		else if (distance > distanceSafeEnemy && isSafe == false)
		{
			isSafe = true;
		}
	}

	private void CheckGround()
	{
		if (Physics.Raycast(transform.position, -transform.up, 2f, carpetLayer))
		{
			walkingOnCarpet = 1f;
		}
		else
		{
			walkingOnCarpet = 0f;
		}
	}

	private void ProcessActions()
	{
		if (Level.Game.GameState == GameStates.Play)
		{
			// Movements
			horizontal = Input.GetAxis("Horizontal");
			vertical = Input.GetAxis("Vertical");

			Vector3 move = new Vector3(horizontal, 0f, vertical);
			rb.AddForce(move.normalized * moveSpeed);

			// Rotation
			if (move != Vector3.zero)
			{
				Vector3 desiredForward = move.normalized;
				rotation = Quaternion.LookRotation(desiredForward);
				model.transform.rotation = Quaternion.Lerp(model.transform.rotation, rotation, 0.1f);

				// Foot Sound
				if (!string.IsNullOrEmpty(footStepSound))
				{
					if (playFootStepSound == null)
					{
						PlayFootStepSound();
					}
				}
			}

			//Animation
			//isMoving
			bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
			bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
			bool isMoving = hasHorizontalInput || hasVerticalInput;
			animator.SetBool("isMoving", isMoving);

			//isHidden
			animator.SetBool("isHidden", IsHidden);

			//isCrouching
			animator.SetBool("isCrouching", isCrouching);
		}
	}

	private void PlayFootStepSound()
	{
		playFootStepSound = StartCoroutine(PlayFootStepSoundCore());
	}

	private IEnumerator PlayFootStepSoundCore()
	{
		footStepInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
		footStepInstance.setParameterByName("Tapis", walkingOnCarpet);
		footStepInstance.start();
		yield return new WaitForSeconds(footSoundDuration);
		playFootStepSound = null;
	}

	private void OnDestroy()
	{
		footStepInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		heartSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		surpriseSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}
}

