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

	public int CurrentTime { get; set; }

	protected void Awake()
	{
		Instance = this;
		game.OnGameOver += StopTimer;
	}


	public void SetText(string text) => console.text = text;

	public void StartTimer(int value)
	{
		CurrentTime = value;
		timer = StartCoroutine(StartTimerCore());
	}

	private IEnumerator StartTimerCore()
	{
		while (true)
		{
			chrono.text = CurrentTime.ToString();

			if (CurrentTime == 10)
			{
				endChronoSoundInstance = FMODUnity.RuntimeManager.CreateInstance(endChronoSound);
				endChronoSoundInstance.start();
			}

			if (CurrentTime == 0)
			{
				chrono.text = "";
				game.ReloadLevel();
				break;
			}

			yield return new WaitForSeconds(1f);
			CurrentTime--;
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
	}
}
