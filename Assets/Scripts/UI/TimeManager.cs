using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
	
	/// <summary>
	/// Speed levels
	/// </summary>
	public static readonly int[] SpeedLevels = new int[] { 1, 2, 5, 10 };
	
	// Speed level
	private int _speed;

	// Indicate if game is in pause
	private bool _pause;
	
	/// <summary>
	/// Speed level
	/// </summary>
	public int Speed { get { return _speed; } }
	
	/// <summary>
	/// Indicate if game is in pause
	/// </summary>
	public bool Paused { get { return _pause; } }

	// Slower button
	private Button _buttonSlower;

	// Pause button
	private Button _buttonPause;

	// Play button
	private Button _buttonPlay;

	// Faster button
	private Button _buttonFaster;

	// Speed text
	private Text _textSpeed;

	// Use this for initialization
	private void Start ()
	{
		_speed = 0;
		_pause = false;
		foreach (RectTransform t in GetComponentsInChildren<RectTransform>())
		{
			switch (t.name)
			{
				case "Slower":
					_buttonSlower = t.GetComponent<Button>();
					break;
				case "Pause":
					_buttonPause = t.GetComponent<Button>();
					break;
				case "Play":
					_buttonPlay = t.GetComponent<Button>();
					break;
				case "Faster":
					_buttonFaster = t.GetComponent<Button>();
					break;
				case "Speed":
					_textSpeed = t.GetComponentInChildren<Text>();
					break;
			}
		}
	}

    // Update is called once per frame
    private void Update()
    {
        Camera.main.GetComponent<CameraController>().SetUIFocus(false);
    }

    // Refresh speed
    private void Refresh()
	{
		Time.timeScale = _pause ? 0 : SpeedLevels[_speed];
		_buttonSlower.interactable = _speed > 0;
		_buttonPause.interactable = !_pause;
		_buttonPlay.interactable = _pause;
		_buttonFaster.interactable = _speed < SpeedLevels.Length - 1;
		_textSpeed.text = SpeedLevels[_speed].ToString() + "x";
	}

	/// <summary>
	/// Slow time
	/// </summary>
	public void Slower()
	{
		if (_speed > 0)
			_speed--;
		Refresh();
	}

	/// <summary>
	/// Slow time
	/// </summary>
	public void Faster()
	{
		if (_speed < SpeedLevels.Length - 1)
			_speed++;
		Refresh();
	}

	/// <summary>
	/// Slow time
	/// </summary>
	public void Pause()
	{
		_pause = true;
		Refresh();
	}

	/// <summary>
	/// Slow time
	/// </summary>
	public void Play()
	{
		_pause = false;
		Refresh();
	}
	
}
