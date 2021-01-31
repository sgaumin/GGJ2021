using UnityEngine;

public class MusicHolder : MonoBehaviour
{
	public static MusicHolder Instance { get; private set; }

	[FMODUnity.EventRef, SerializeField] private string musicSound;

	private static FMOD.Studio.EventInstance musicSoundInstance;
	private bool hasBeenInialized;

	protected void Awake() => Instance = this;

	private void Init()
	{
		if (!hasBeenInialized)
		{
			hasBeenInialized = true;
			musicSoundInstance = FMODUnity.RuntimeManager.CreateInstance(musicSound);
			musicSoundInstance.start();
		}
	}

	public void SetFirstMusic()
	{
		Init();
		musicSoundInstance.setParameterByName("Music", 0.5f);
	}

	public void SetHideMusic()
	{
		Init();
		musicSoundInstance.setParameterByName("Music", 1.5f);
	}

	public void SetChaseMusic()
	{
		Init();
		musicSoundInstance.setParameterByName("Music", 2.5f);
	}

	public void SetVictoryMusic()
	{
		Init();
		musicSoundInstance.setParameterByName("Music", 3.5f);
	}

	public void SetLooseMusic()
	{
		Init();
		musicSoundInstance.setParameterByName("Music", 4.5f);
	}

	private void OnDestroy()
	{
		musicSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		Instance = null;
	}
}
