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

	private FMOD.Studio.EventInstance ejectInstance;

	private void Start()
	{
		timeLocal = timeBeforeEjection;
		ejectInstance = FMODUnity.RuntimeManager.CreateInstance(ejectSound);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			rb = other.GetComponent<Rigidbody>();
			PlayerController playerController = other.GetComponent<PlayerController>();
			playerController.hidingPlace = this;
			playerController.CanHide = true;
		}
	}

	private void Update()
	{
		if (launchTimer)
		{
			timeLocal -= Time.deltaTime;
			//UIManager.Instance.SetText("Timer before Ejection: "+ timeLocal.ToString("f2"));
		}

		if (timeLocal <= 0.0f)
		{
			Eject();
			launchTimer = false;  //reset timer
			timeLocal = timeBeforeEjection;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			//UIManager.Instance.SetText("Player Can Hide (Space to hide)");
			PlayerController playerController = other.GetComponent<PlayerController>();
			playerController.CanHide = false;
			launchTimer = false;//reset launchtimer
			timeLocal = timeBeforeEjection;
		}
	}

	void Eject()
	{
		rb.AddForce(ejectionDirection * ejectionForce);
		ejectInstance.start();
	}
}
