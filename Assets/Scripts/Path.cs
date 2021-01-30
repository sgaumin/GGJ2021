using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path : MonoBehaviour
{
	private List<PathActionPoint> pathActionPoints = new List<PathActionPoint>();
	private int currentPointIndex = 0;
	private bool hasBeenInitialize;

	public bool HasAnotherPoint => currentPointIndex < pathActionPoints.Count();

	public void Init()
	{
		hasBeenInitialize = true;
		currentPointIndex = 0;
		pathActionPoints = GetComponentsInChildren<PathActionPoint>().ToList();
		pathActionPoints.ForEach(x => x.Init());
	}

	public PathActionPoint GetFirstActionPoint()
	{
		if (!hasBeenInitialize)
		{
			Init();
		}

		return pathActionPoints[0];
	}

	public PathActionPoint GetNextPoint()
	{
		if (!hasBeenInitialize)
		{
			Init();
		}

		pathActionPoints[currentPointIndex].SetInProgress();

		return pathActionPoints[currentPointIndex++];
	}

	public void RestartPath()
	{
		currentPointIndex = 0;
		pathActionPoints.ForEach(x => x.ResetName());
	}

	void OnDrawGizmos()
	{
		DrawGizmos(transform);
	}

	private void DrawGizmos(Transform trans)
	{
		Gizmos.color = Color.blue;
		Transform firstchild = null;
		Transform previouschild = null;

		foreach (Transform child in trans)
		{
			if (child == transform)
				continue;

			if (child.GetComponent<PathActionPoint>()?.Type == PathActionPointType.GoToFirst || child.GetComponent<PathActionPoint>()?.Type == PathActionPointType.GoToNext)
			{
				if (previouschild == null)
				{
					firstchild = child;
				}
				else
				{
					Gizmos.DrawLine(previouschild.position, child.position);
				}

				Gizmos.DrawSphere(child.position, 0.3f);

				if (child.GetComponent<PathActionPoint>()?.Type == PathActionPointType.GoToFirst)
				{
					Gizmos.DrawLine(child.position, firstchild.position);
					break;
				}

				previouschild = child;
			}
		}
	}
}

