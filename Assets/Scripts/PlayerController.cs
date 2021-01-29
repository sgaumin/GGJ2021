using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // https://www.immersivelimit.com/tutorials/simple-character-controller-for-unity
    Rigidbody rb;
   

    public float moveSpeed;
    public float horizontal;
    public float vertical;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        ProcessActions();

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");



        void ProcessActions()
        {
            Vector3 move = new Vector3(horizontal, 0f, vertical);
            rb.AddForce(move.normalized * moveSpeed);

        }
    }
}

