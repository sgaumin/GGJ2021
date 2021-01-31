using UnityEngine;

public class HidingPlace : MonoBehaviour
{
	public Vector3 ejectionDirection = -Vector3.forward;
	public float ejectionForce;
	public bool launchTimer;
	private float timeLocal;
	public float timeBeforeEjection;
	private Rigidbody rb;


	[Header("Audio")]
	[FMODUnity.EventRef, SerializeField] private string ejectSound;
	[FMODUnity.EventRef, SerializeField] private string entranceSound;

	private FMOD.Studio.EventInstance ejectInstance;
	private FMOD.Studio.EventInstance entranceInstance;

	private void Start()
	{
		timeLocal = timeBeforeEjection;
		ejectInstance = FMODUnity.RuntimeManager.CreateInstance(ejectSound);
		entranceInstance = FMODUnity.RuntimeManager.CreateInstance(entranceSound);
	}

	private void Update()
	{
		if (launchTimer)
		{
			timeLocal -= Time.deltaTime;
		}

		if (timeLocal <= 0.0f)
		{
			Eject();
			launchTimer = false;  //reset timer
			timeLocal = timeBeforeEjection;
		}

	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position, transform.position + ejectionDirection);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			rb = other.GetComponent<Rigidbody>();
			PlayerController playerController = other.GetComponent<PlayerController>();
			playerController.IsHidden = true;
			launchTimer = true;
			entranceInstance.start();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			PlayerController playerController = other.GetComponent<PlayerController>();
			playerController.IsHidden = false;
			launchTimer = false;//reset launchtimer
			timeLocal = timeBeforeEjection;
		}
	}

	void Eject()
	{
		if (rb != null)
		{
			rb.AddForce(ejectionDirection * ejectionForce);
			ejectInstance.start();
		}
	}
}
