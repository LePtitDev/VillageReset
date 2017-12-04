using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseWatcher : MonoBehaviour
{

	// Current instance
	public static PauseWatcher Instance;

	// Use this for initialization
	private void Start ()
	{
		Instance = this;
		gameObject.SetActive(false);
	}
	
	/// <summary>
	/// Toggle pause panel activation
	/// </summary>
	public void TogglePause()
	{
		bool next = !gameObject.activeSelf;
		gameObject.SetActive(next);
		if (next)
			Time.timeScale = 0f;
		else
			GameObject.Find("Timing").GetComponent<TimeManager>().Refresh();
	}

	/// <summary>
	/// Return to game
	/// </summary>
	public void Continue()
	{
		TogglePause();
	}

	/// <summary>
	/// Return to menu
	/// </summary>
	public void ReturnToMenu()
	{
		Camera.main.gameObject.SetActive(false);
		SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenu"));
		Launcher.Instance.LauncherCamera.gameObject.SetActive(true);
		Launcher.Instance.gameObject.SetActive(true);
		SceneManager.UnloadSceneAsync("MainGame");
		Time.timeScale = 1f;
	}

	/// <summary>
	/// Quit game
	/// </summary>
	public void Quit()
	{
		Application.Quit();
	}
	
}
