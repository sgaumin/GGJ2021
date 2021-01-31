using Tools.Utils;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public Collectible collectiblePrefab;
	private float time;

	[SerializeField, FloatRangeSlider(0f, 30f)] private FloatRange spawnRate = new FloatRange(5f, 15f);

	private float currentSpawnRate;

	private void Start()
	{
		currentSpawnRate = spawnRate.RandomValue;
	}

	private void Update()
	{
		time += Time.deltaTime;

		if (time > currentSpawnRate)
		{
			Collectible currentCollectible = Instantiate(collectiblePrefab, null);
			currentCollectible.transform.position = transform.position;
			currentCollectible.transform.rotation = transform.rotation;

			currentSpawnRate = spawnRate.RandomValue;
			time = 0f;//Reset Time
		}
	}
}
