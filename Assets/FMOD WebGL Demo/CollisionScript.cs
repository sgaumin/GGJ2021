using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionScript : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string eventRef;

#if UNITY_EDITOR // Used for testing in Editor since banks are loaded in a different scene.
    private void Awake()
    {
        FMODUnity.RuntimeManager.LoadBank("Master Bank");
    }
#endif

    private void OnCollisionEnter(Collision collision)
    {
        FMODUnity.RuntimeManager.PlayOneShot(eventRef, collision.transform.position);
    }
}
