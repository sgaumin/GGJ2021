using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }

	[Header("References")]
	[SerializeField] private TextMeshProUGUI console;

	protected void Awake() => Instance = this;

	public void SetText(string text) => console.text = text;
}
