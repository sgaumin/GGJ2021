using DG.Tweening;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PathActionPoint : MonoBehaviour
{
	[SerializeField] private PathActionPointType type;
	[SerializeField, Range(0f, 10f)] private float waitDuration = 1f;
	[SerializeField, Range(0f, 360f)] private float rotateAngle;
	[SerializeField, Range(0f, 10f)] private float rotateDuration;
	[SerializeField] private Ease rotationEase = Ease.InOutSine;
	[SerializeField] private Path newPath;
	[SerializeField, Range(0f, 1f)] private float probability;

	private string originalName = "";

	public PathActionPointType Type => type;
	public float WaitDuration => waitDuration;
	public float RotateAngle => rotateAngle;
	public float RotateDuration => rotateDuration;
	public Ease RotationEase => rotationEase;
	public Path NewPath => newPath;
	public float Probability => probability;

	public void Init()
	{
		originalName = gameObject.name;
		ResetName();
	}

	public void ResetName()
	{
		if (type == PathActionPointType.Wait)
		{
			gameObject.name = $"{originalName}: {type} {waitDuration}s";
		}
		else if (type == PathActionPointType.Rotate)
		{
			gameObject.name = $"{originalName}: {type} {rotateAngle}d";
		}
		else
		{
			gameObject.name = $"{originalName}: {type}";
		}
	}

	public void SetInProgress()
	{
		ResetName();
		gameObject.name += " - IN PROGRESS";
	}

	public void SetAsDone()
	{
		ResetName();
		gameObject.name += " - DONE";
	}

	[ContextMenu("Create Point After")]
	private void CreatePointAfter()
	{
		GameObject o = new GameObject();
		o.AddComponent<PathActionPoint>();
		o.transform.parent = transform.parent;
		o.name = "Point";
		o.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
	}

	[ContextMenu("Create Point Before")]
	private void CreatePointBefore()
	{
		GameObject o = new GameObject();
		o.AddComponent<PathActionPoint>();
		o.transform.parent = transform.parent;
		o.name = "Point";
		o.transform.SetSiblingIndex(transform.GetSiblingIndex());
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		PathActionPoint pathActionPoint = this;
		UnityEditor.Handles.Label(pathActionPoint.transform.position, "" + pathActionPoint.type);
	}
#endif
}


#if UNITY_EDITOR
[CustomEditor(typeof(PathActionPoint))]
[CanEditMultipleObjects]
public class PathActionPointDrawer : Editor
{

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		SerializedProperty type = serializedObject.FindProperty("type");
		SerializedProperty waitDuration = serializedObject.FindProperty("waitDuration");
		SerializedProperty rotateAngle = serializedObject.FindProperty("rotateAngle");
		SerializedProperty rotateDuration = serializedObject.FindProperty("rotateDuration");
		SerializedProperty rotationEase = serializedObject.FindProperty("rotationEase");
		SerializedProperty newPath = serializedObject.FindProperty("newPath");
		SerializedProperty probability = serializedObject.FindProperty("probability");

		EditorGUILayout.PropertyField(type);
		EditorGUILayout.Space();


		if ((PathActionPointType)type.enumValueIndex == PathActionPointType.GoToNext)
		{
			EditorGUILayout.LabelField("ACTION: Move to the next point position of the path.");
		}
		else if ((PathActionPointType)type.enumValueIndex == PathActionPointType.GoToFirst)
		{
			EditorGUILayout.LabelField("ACTION: Move to the first point position of the path.");
		}
		else if ((PathActionPointType)type.enumValueIndex == PathActionPointType.RestartPath)
		{
			EditorGUILayout.LabelField("ACTION: Restart the path sequence.");
		}
		else if ((PathActionPointType)type.enumValueIndex == PathActionPointType.Rotate)
		{
			EditorGUILayout.LabelField("ACTION: Rotate the enemy.");
			EditorGUILayout.PropertyField(rotateAngle);
			EditorGUILayout.PropertyField(rotateDuration);
			EditorGUILayout.PropertyField(rotationEase);
		}
		else if ((PathActionPointType)type.enumValueIndex == PathActionPointType.Wait)
		{
			EditorGUILayout.LabelField("ACTION: Make the enemy wait.");
			EditorGUILayout.PropertyField(waitDuration);
		}
		else if ((PathActionPointType)type.enumValueIndex == PathActionPointType.ChangePath)
		{
			EditorGUILayout.LabelField("ACTION: Change the current path with a probability.");
			EditorGUILayout.PropertyField(newPath);
			EditorGUILayout.PropertyField(probability);
		}

		serializedObject.ApplyModifiedProperties();
	}
}
#endif