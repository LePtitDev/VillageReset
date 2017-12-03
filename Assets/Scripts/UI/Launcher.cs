using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{

	// Instance courante
	public static Launcher Instance;

	// Values dictionnary
	public Dictionary<string, float> Values;

	// Use this for initialization
	private void Start()
	{
		Instance = this;
		Values = new Dictionary<string, float>();
		Refresh();
	}

	/// <summary>
	/// Refresh values
	/// </summary>
	public void Refresh()
	{
		foreach (Slider slider in GetComponentsInChildren<Slider>())
		{
			if (!Values.ContainsKey(slider.name))
				Values.Add(slider.name, 0f);
			Values[slider.name] = slider.value;
		}
	}

	/// <summary>
	/// Load main game scene
	/// </summary>
	public void StartGame()
	{
		//Debug.Log("Scene2 loading: " + scenePaths[0]);
		SceneManager.LoadScene("MainGame", LoadSceneMode.Additive);
		gameObject.SetActive(false);
	}
	
}
