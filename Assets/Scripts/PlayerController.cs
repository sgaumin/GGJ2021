using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// https://www.immersivelimit.com/tutorials/simple-character-controller-for-unity
	Rigidbody rb;
	public HidingPlace hidingPlace;
	Animator animator;
	bool isCrouching;

	public float turnSpeed = 20f;
	Quaternion rotation = Quaternion.identity;

	public float moveSpeed;
	public float horizontal;
	public float vertical;

	public bool CanHide;
	public bool IsHidden;

	public GameObject model;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		animator = GetComponentInChildren<Animator>();

		hidingPlace = null;

	}

	// Update is called once per frame
	void FixedUpdate()
	{

		ProcessActions();

		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");

		if (Input.GetKey(KeyCode.Space))
		{
			if (CanHide)
			{
				isCrouching = true;
				//UIManager.Instance.SetText("Player is Hiding");
				IsHidden = true;
				//model.SetActive(false);
				hidingPlace.launchTimer = true;
			}
			else
			{
				model.SetActive(true);
				isCrouching = false;
				IsHidden = false;
			}
		}
		else
		{
			model.SetActive(true);
			isCrouching = false;
			IsHidden = false;
		}



		void ProcessActions()
		{
			// Movements
			Vector3 move = new Vector3(horizontal, 0f, vertical);
			rb.AddForce(move.normalized * moveSpeed);

			// Rotation
			if (move != Vector3.zero)
			{
				Vector3 desiredForward = move.normalized;
				rotation = Quaternion.LookRotation(desiredForward);
				model.transform.rotation = Quaternion.Lerp(model.transform.rotation, rotation, 0.1f);
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

