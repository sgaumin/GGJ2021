using UnityEngine;

public class Spawner : MonoBehaviour
{
	public GameObject objectToSpawn;
	private float time;
	public float spawnRate;

	private Transform spawner;


	private void Start()
	{
		spawner = GetComponent<Transform>();

	}


	private void Update()
	{
		time += Time.deltaTime;

		if (time > spawnRate)
		{
			Instantiate(objectToSpawn, spawner.position, spawner.rotation);//Instantiate
			time = 0f;//Reset Time
		}

	}

}
