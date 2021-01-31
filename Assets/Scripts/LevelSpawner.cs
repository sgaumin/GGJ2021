using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
	[SerializeField] private List<Path> paths = new List<Path>();

	[Header("References")]
	[SerializeField] private PlayerController player;
	[SerializeField] private Enemy enemyPrefab;
	[SerializeField] private Game game;

	public List<Enemy> Enemies { get; set; } = new List<Enemy>();
	public PlayerController Player => player;
	public Game Game => game;

	protected void Start()
	{
		player.Level = this;

		int i = 0;
		Enemies.Clear();
		foreach (Path path in paths)
		{
			GameObject currentEnemyHolder = new GameObject();
			currentEnemyHolder.transform.SetParent(transform);
			currentEnemyHolder.name = $"Enemy {i++}";

			Path currentPath = Instantiate(path, currentEnemyHolder.transform);
			currentPath.Init();

			Enemy currentEnemy = Instantiate(enemyPrefab, currentEnemyHolder.transform);
			currentEnemy.Level = this;
			currentEnemy.Init(currentPath);
			Enemies.Add(currentEnemy);
		}
	}
}
