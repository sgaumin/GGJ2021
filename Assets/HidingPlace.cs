using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPlace : MonoBehaviour
{
    public Vector3 ejectionDirection;
    public float ejectionForce;
    private Rigidbody rb;
     public bool launchTimer;
    private float timeLocal;
    public float timeBeforeEjection;


    private void Start()
    {
        timeLocal = timeBeforeEjection;
        ejectionDirection = -Vector3.forward;
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            
            rb = other.GetComponent<Rigidbody>();
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.hidingPlace = this;
            playerController.CanHide = true;


        }
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, ejectionDirection);
        if (launchTimer)
        {
            timeLocal -= Time.deltaTime;
        }

        if (timeLocal<=0.0f)
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
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.CanHide = false;
            launchTimer = false;//reset launchtimer
            timeLocal = timeBeforeEjection;
        }
    }

    void Eject()
    {
        rb.AddForce(ejectionDirection * ejectionForce);
       
    }
}
