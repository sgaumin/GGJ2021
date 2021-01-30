using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float timeBeforeTransformation; //after that time, the collectible turns into an enemy
    public float value; //value of the object for scoring

    
   

    // Update is called once per frame
    void Update()
    {
        timeBeforeTransformation -= Time.deltaTime;

        if (timeBeforeTransformation<=0.0f)
        {
            TransformIntoEnemy();//instantiate Enemy
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            var playerScore = other.GetComponent < PlayerScore>();//Increase score of value
            playerScore.IncreaseScore(value);

            Destroy(gameObject);//destroy gameobject
        }
    }

    void TransformIntoEnemy()
    {
        Instantiate(enemyPrefab,transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
