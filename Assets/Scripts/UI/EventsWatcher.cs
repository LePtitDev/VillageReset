using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventsWatcher : MonoBehaviour
{

	// Scrollview
	private ScrollRect _scrollview;

	// Events content
	private Text _textContent;

	// Vertical scrollbar
	private Scrollbar _scrollbar;
	
	// Gui style
	private GUIStyle _style;

	// Indicate if need an update
	private int _needUpdate;

	// Last yDelta
	private float _yDelta;

	// Use this for initialization
	private void Start ()
	{
		_scrollview = GetComponentInChildren<ScrollRect>();
		_textContent = _scrollview.GetComponentInChildren<Text>();
		_scrollbar = _scrollview.verticalScrollbar;
		_style = new GUIStyle();
		_style.font = _textContent.font;
		_style.fontStyle = _textContent.fontStyle;
		_style.fontSize = _textContent.fontSize;
		_needUpdate = 1;
		foreach (RectTransform t in GetComponentsInChildren<RectTransform>())
		{
			if (t.name == "Title")
			{
				_yDelta = t.rect.height - t.offsetMax.y * 2;
				break;
			}
		}
		ToogleReduce();
	}

	// Update is called once per frame
	private void Update()
	{
		if (_needUpdate > 0)
		{
			_scrollbar.value = 0f;
			_needUpdate--;
		}
	}

	/// <summary>
	/// Toggle reduce area
	/// </summary>
	public void ToogleReduce()
	{
		float tmp = _yDelta;
		RectTransform tr = GetComponent<RectTransform>();
		_yDelta = tr.sizeDelta.y;
		tr.sizeDelta = new Vector2(tr.sizeDelta.x, tmp);
		_scrollview.gameObject.SetActive(!_scrollview.gameObject.activeSelf);
	}

	/// <summary>
	/// Add an event message
	/// </summary>
	/// <param name="msg">Message</param>
	public void SendEvent(string msg)
	{
		string str = "\n>";
		string[] amsg = msg.Split(' ');
		for (int i = 0; i < amsg.Length; i++)
		{
			str += " ";
			Vector2 v = _style.CalcSize(new GUIContent(str + amsg[i]));
			if (v.x > _textContent.rectTransform.rect.width)
				str += '\n';
			str += amsg[i];
		}
		_textContent.text += str;
		_textContent.rectTransform.sizeDelta = new Vector2(_textContent.rectTransform.sizeDelta.x, _style.CalcSize(new GUIContent(_textContent.text)).y);
		_needUpdate = 2;
	}
	
}
