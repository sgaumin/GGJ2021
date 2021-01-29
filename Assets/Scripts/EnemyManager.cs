using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private NavMeshSurface ground;

	private List<Enemy> enemies = new List<Enemy>();

	protected void Start()
	{
		enemies = FindObjectsOfType<Enemy>().ToList();
		enemies.ForEach(x => x.StartMovingRandomly());
	}
}
