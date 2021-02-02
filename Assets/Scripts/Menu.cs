using UnityEngine;

public class Menu : MonoBehaviour
{
	[FMODUnity.EventRef, SerializeField] private string clicSound;
	public Game game;

	[Header("References")]
	[SerializeField] private MusicHolder musicHolderPrefab;

	private bool hasClicked;
	private FMOD.Studio.EventInstance clickInstance;

	private void Awake()
	{
		if (MusicHolder.Instance == null)
		{
			Instantiate(musicHolderPrefab, null);
		}
	}

	protected void Start()
	{
		MusicHolder.Instance.SetMenuMusic();
	}

	void Update()
	{
		if (Input.anyKeyDown && !hasClicked)
		{
			clickInstance = FMODUnity.RuntimeManager.CreateInstance(clicSound);
			clickInstance.start();

			hasClicked = true;
			game.LoadNextLevel();
		}
	}
}
