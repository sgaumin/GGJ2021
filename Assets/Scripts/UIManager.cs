using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }

	[Header("Audio")]
	[FMODUnity.EventRef, SerializeField] private string endChronoSound;

	[Header("References")]
	[SerializeField] private TextMeshProUGUI chrono;
	[SerializeField] private TextMeshProUGUI console;
	[SerializeField] private Game game;

	private int currenTime;
	private Coroutine timer;
	private FMOD.Studio.EventInstance endChronoSoundInstance;

	protected void Awake() => Instance = this;

	public void SetText(string text) => console.text = text;

	public void StartTimer(int value)
	{
		currenTime = value;
		timer = StartCoroutine(StartTimerCore());
	}

	private IEnumerator StartTimerCore()
	{
		while (true)
		{
			chrono.text = currenTime.ToString();

			if (currenTime == 10)
			{
				endChronoSoundInstance = FMODUnity.RuntimeManager.CreateInstance(endChronoSound);
				endChronoSoundInstance.start();
			}

			if (currenTime == 0)
			{
				chrono.text = "";
				game.ReloadLevel();
				break;
			}

			yield return new WaitForSeconds(1f);
			currenTime--;
		}
	}

	public void StopTimer()
	{
		if (timer != null)
		{
			StopCoroutine(timer);
		}
	}
}
