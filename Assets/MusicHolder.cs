using UnityEngine;

public class MusicHolder : MonoBehaviour
{
	public static MusicHolder Instance { get; private set; }

	[FMODUnity.EventRef, SerializeField] private string musicSound;

	private static FMOD.Studio.EventInstance musicSoundInstance;

	protected void Awake() => Instance = this;

	public void SetFirstMusic()
	{
		musicSoundInstance = FMODUnity.RuntimeManager.CreateInstance(musicSound);
		musicSoundInstance.start();
		musicSoundInstance.setParameterByName("Music", 0.5f);
	}

	public void SetHideMusic()
	{
		musicSoundInstance = FMODUnity.RuntimeManager.CreateInstance(musicSound);
		musicSoundInstance.start();
		musicSoundInstance.setParameterByName("Music", 1.5f);
	}

	public void SetChaseMusic()
	{
		musicSoundInstance = FMODUnity.RuntimeManager.CreateInstance(musicSound);
		musicSoundInstance.start();
		musicSoundInstance.setParameterByName("Music", 2.5f);
	}

	public void SetVictoryMusic()
	{
		musicSoundInstance = FMODUnity.RuntimeManager.CreateInstance(musicSound);
		musicSoundInstance.start();
		musicSoundInstance.setParameterByName("Music", 3.5f);
	}

	public void SetLooseMusic()
	{
		musicSoundInstance = FMODUnity.RuntimeManager.CreateInstance(musicSound);
		musicSoundInstance.start();
		musicSoundInstance.setParameterByName("Music", 4.5f);
	}

	private void OnDestroy()
	{
		musicSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		Instance = null;
	}
}
