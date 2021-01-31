using UnityEngine;

public class MusicHolder : MonoBehaviour
{
	public static MusicHolder Instance { get; private set; }

	[FMODUnity.EventRef, SerializeField] private string musicSound;

	private static FMOD.Studio.EventInstance musicSoundInstance;

	protected void Awake() => Instance = this;

	protected void Start()
	{
		musicSoundInstance = FMODUnity.RuntimeManager.CreateInstance(musicSound);

		musicSoundInstance.start();
	}

	public void SetFirstMusic()
	{
		musicSoundInstance.setParameterByName("Music", 0.5f);
	}

	public void SetHideMusic()
	{
		musicSoundInstance.setParameterByName("Music", 1.5f);
	}

	public void SetChaseMusic()
	{
		musicSoundInstance.setParameterByName("Music", 2.5f);
	}

	public void SetVictoryMusic()
	{
		musicSoundInstance.setParameterByName("Music", 3.5f);
	}

	public void SetLooseMusic()
	{
		musicSoundInstance.setParameterByName("Music", 4.5f);
	}

	private void OnDestroy()
	{
		Instance = null;
		musicSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}
}
