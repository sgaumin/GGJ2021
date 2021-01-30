using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// https://www.immersivelimit.com/tutorials/simple-character-controller-for-unity
	Rigidbody rb;
	public HidingPlace hidingPlace;

	public float moveSpeed;
	public float horizontal;
	public float vertical;

	public bool CanHide;
	public bool IsHidden;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		hidingPlace = null;

	}

	// Update is called once per frame
	void FixedUpdate()
	{

		ProcessActions();

		

		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");

		if(Input.GetKey(KeyCode.Space))
        {
			if (CanHide)
            {
				//UIManager.Instance.SetText("Player is Hiding");
				IsHidden = true;
				hidingPlace.launchTimer = true;
            }
			else
            {
				IsHidden = false;
            }
        }
		else
        {
			IsHidden = false;
        }


		void ProcessActions()
		{
			Vector3 move = new Vector3(horizontal, 0f, vertical);
			rb.AddForce(move.normalized * moveSpeed);

		}
	}
}

