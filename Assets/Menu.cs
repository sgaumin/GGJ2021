using UnityEngine;

public class Menu : MonoBehaviour
{
	[FMODUnity.EventRef, SerializeField] private string clicSound;
	[FMODUnity.EventRef, SerializeField] private string musicSound;
	public Game game;

	private bool hasClicked;
	private FMOD.Studio.EventInstance clickInstance;
	private FMOD.Studio.EventInstance musicSoundInstance;

	protected void Start()
	{
		musicSoundInstance = FMODUnity.RuntimeManager.CreateInstance(musicSound);
		musicSoundInstance.setParameterByName("Music", 0);
		musicSoundInstance.start();
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
