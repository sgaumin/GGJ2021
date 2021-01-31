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

	private Coroutine timer;
	private FMOD.Studio.EventInstance endChronoSoundInstance;
	private int currenTime;

	public int CurrenTime
	{
		get => currenTime;
		set
		{
			currenTime = Mathf.Max(value, 0);
			if (chrono)
			{
				chrono.text = currenTime.ToString();
			}
		}
	}

	protected void Awake()
	{
		Instance = this;
		game.OnGameOver += StopTimer;
	}

	public void SetText(string text) => console.text = text;

	public void StartTimer(int value)
	{
		CurrenTime = value;

		if (chrono != null)
		{
			timer = StartCoroutine(StartTimerCore());
		}
	}

	private IEnumerator StartTimerCore()
	{
		while (true)
		{
			if (CurrenTime == 10)
			{
				endChronoSoundInstance = FMODUnity.RuntimeManager.CreateInstance(endChronoSound);
				endChronoSoundInstance.start();
			}

			if (CurrenTime == 0)
			{
				chrono.gameObject.FadOut();
				game.ReloadLevel();
				break;
			}

			yield return new WaitForSeconds(1f);
			CurrenTime--;
		}
	}

	public void StopTimer()
	{
		if (timer != null)
		{
			StopCoroutine(timer);
		}

		endChronoSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	private void OnDestroy()
	{
		game.OnGameOver -= StopTimer;

		endChronoSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}
}
