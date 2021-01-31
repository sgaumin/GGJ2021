using UnityEngine;

public class Collectible : MonoBehaviour
{
	public GameObject enemyPrefab;
	public float timeBeforeTransformation; //after that time, the collectible turns into an enemy
	public int value; //value of the object for scoring

	[Header("Debug")]
	[SerializeField] private bool spawnEnemy;

	// Update is called once per frame
	void Update()
	{
		if (spawnEnemy)
		{
			timeBeforeTransformation -= Time.deltaTime;

			if (timeBeforeTransformation <= 0.0f)
			{
				TransformIntoEnemy();//instantiate Enemy
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			UIManager.Instance.CurrentTime -= value;
			Destroy(gameObject);//destroy gameobject
		}
	}

	void TransformIntoEnemy()
	{
		Instantiate(enemyPrefab, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
