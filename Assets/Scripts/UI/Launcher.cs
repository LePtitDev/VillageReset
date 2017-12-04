using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{

	// Instance courante
	public static Launcher Instance;
	
	// Launcher camera
	[HideInInspector]
	public Camera LauncherCamera;

	// Values dictionnary
	public Dictionary<string, float> Values;

    // Indicate if brigands attacks on winter
    public bool WinterAttack;

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
		LauncherCamera = Camera.main;
		foreach (Slider slider in GetComponentsInChildren<Slider>())
		{
			if (!Values.ContainsKey(slider.name))
				Values.Add(slider.name, 0f);
			Values[slider.name] = slider.value;
            GameObject.Find(slider.name + "Value").GetComponent<Text>().text = slider.value.ToString();
		}
        WinterAttack = GameObject.Find("WinterAttack").GetComponent<Toggle>().isOn;
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
