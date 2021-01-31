using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	[SerializeField] List<Path> paths = new List<Path>();

	[Header("References")]
	[SerializeField] private Enemy enemyPrefab;
	[SerializeField] private PlayerController player;

	protected void Start()
	{
		int i = 0;
		foreach (Path path in paths)
		{
			GameObject currentEnemyHolder = new GameObject();
			currentEnemyHolder.transform.SetParent(transform);
			currentEnemyHolder.name = $"Enemy {i++}";

			Path currentPath = Instantiate(path, currentEnemyHolder.transform);
			currentPath.Init();

			Enemy currentEnemy = Instantiate(enemyPrefab, currentEnemyHolder.transform);
			currentEnemy.Init(currentPath);
		}
	}
}
