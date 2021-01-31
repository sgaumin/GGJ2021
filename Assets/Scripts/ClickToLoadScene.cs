﻿using System;
using System.Collections;
using Tools;
using UnityEngine;

public class ClickToLoadScene : MonoBehaviour
{
	[SerializeField] private string nameScene;

	[Header("References")]
	[SerializeField] private FadScreen fader;

	private Coroutine loadingLevel;

	protected void Start()
	{
		fader.FadIn();
	}

	private void Update()
	{
		if (Input.anyKeyDown)
		{
			LoadScene();
		}
	}

	public void LoadScene()
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.LoadLevelByName(nameScene);
			}));
		}
	}

	private IEnumerator LoadLevelCore(Action content = null)
	{
		Time.timeScale = 1f;
		yield return fader.FadOutCore();
		content?.Invoke();
	}
}