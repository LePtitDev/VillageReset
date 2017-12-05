using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryWatcher : MonoBehaviour {

	// Main camera
	private CameraController _camera;

	// Text for name
	private Text _textTitle;

	// Text error
	private GameObject _textError;

	// Text entry
	private GameObject _textHeader;

	// Text header
	private GameObject _textEntry;

	// ScrollRect viewport
	private RectTransform _viewport;

	// ScrollRect content
	private RectTransform _content;

	// ScrollRect basic size
	private Vector2 _basicSize;

	// Request string
	private string _request;

	// Additionnal texts for names
	private List<Text> _additionalHeader;

	// Additionnal texts for quantities
	private List<List<Text>> _additionalEntry;

	// Use this for initialization
	private void Start ()
	{
		_camera = Camera.main.GetComponent<CameraController>();
		foreach (Text t in GetComponentsInChildren<Text>(true))
		{
			if (t.name == "Title")
				_textTitle = t;
			else if (t.name == "Error")
				_textError = t.gameObject;
			else if (t.name == "Header")
				_textHeader = t.gameObject;
			else if (t.name == "Entry")
				_textEntry = t.gameObject;
		}
		_viewport = GetComponentInChildren<ScrollRect>().GetComponentInChildren<Mask>().GetComponent<RectTransform>();
		foreach (RectTransform t in _viewport.GetComponentsInChildren<RectTransform>(true))
		{
			if (t.name == "Content")
				_content = t;
		}
		_basicSize = _content.sizeDelta;
		_additionalHeader = new List<Text>();
		_additionalEntry = new List<List<Text>>();
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if (_camera.Target == null)
		{
			gameObject.SetActive(false);
			return;
		}
		_textTitle.text = "Mémoire de " + _camera.Target.GetComponent<AgentController>().FirstName;
		Memory memory = _camera.Target.GetComponent<Memory>();
		if (memory == null)
		{
			gameObject.SetActive(false);
			return;
		}
		Memory.Answer answer = memory.Request(_request);
		if (answer == null)
		{
            Error();
			return;
		}
		_textError.SetActive(false);
		for (int i = 0; i < answer.Names.Length; i++)
		{
			if (i >= _additionalHeader.Count)
			{
				GameObject aName = Instantiate(_textHeader, _content.transform);
				aName.SetActive(true);
				RectTransform tr = aName.GetComponent<RectTransform>();
				tr.position = new Vector3(tr.position.x + i * tr.sizeDelta.x, tr.position.y);
				_additionalHeader.Add(aName.GetComponent<Text>());
				_additionalEntry.Add(new List<Text>());
			}
			_additionalHeader[i].text = answer.Names[i][1];
		}
		if (answer.Columns.Length > 0)
		{
			for (int i = 0; i < answer.Columns.Length; i++)
			{
				if (i >= _additionalEntry[0].Count)
				{
					for (int j = 0; j < answer.Columns[0].Length; j++)
					{
						GameObject aName = Instantiate(_textEntry, _content.transform);
						aName.SetActive(true);
						RectTransform tr = aName.GetComponent<RectTransform>();
						tr.position = new Vector3(tr.position.x + j * tr.sizeDelta.x, tr.position.y - i * tr.sizeDelta.y);
						_additionalEntry[j].Add(aName.GetComponent<Text>());
					}
				}
				for (int j = 0; j < answer.Columns[0].Length; j++)
					_additionalEntry[j][i].text = answer.Columns[i][j].ToString();
			}
			if (answer.Columns.Length < _additionalEntry[0].Count)
			{
				for (int i = answer.Columns.Length; i < _additionalEntry[0].Count; i++)
				{
                    for (int j = 0; j < _additionalEntry.Count; j++)
                        Destroy(_additionalEntry[j][i].gameObject);
				}
				for (int j = 0; j < answer.Columns[0].Length; j++)
					_additionalEntry[j].RemoveRange(answer.Columns.Length, _additionalEntry[j].Count - answer.Columns.Length);
			}
		}
		else
		{
			for (int i = 0; i < _additionalEntry.Count; i++)
			{
				foreach (Text text in _additionalEntry[i])
					Destroy(text.gameObject);
                _additionalEntry[i] = new List<Text>();
            }
        }
        if (answer.Names.Length < _additionalHeader.Count)
        {
            for (int i = answer.Names.Length; i < _additionalHeader.Count; i++)
                Destroy(_additionalHeader[i].gameObject);
            _additionalHeader.RemoveRange(answer.Names.Length, _additionalHeader.Count - answer.Names.Length);
            _additionalEntry.RemoveRange(answer.Names.Length, _additionalEntry.Count - answer.Names.Length);
        }
        RectTransform header = _textHeader.GetComponent<RectTransform>();
		RectTransform entry = _textEntry.GetComponent<RectTransform>();
		if (_additionalEntry.Count > 0)
			_content.sizeDelta = new Vector2(header.position.x + header.sizeDelta.x * _additionalHeader.Count,
				entry.position.y + entry.sizeDelta.y * _additionalEntry[0].Count);
		else
			_content.sizeDelta = _basicSize;
	}

    /// <summary>
    /// Remove entries and display error
    /// </summary>
    private void Error()
    {
        _textError.SetActive(true);
        for (int i = 0; i < _additionalEntry.Count; i++)
            Destroy(_additionalHeader[i].gameObject);
        _additionalHeader = new List<Text>();
        for (int i = 0; i < _additionalEntry.Count; i++)
        {
            foreach (Text text in _additionalEntry[i])
                Destroy(text.gameObject);
        }
        _additionalEntry = new List<List<Text>>();
    }

	/// <summary>
	/// Close panel
	/// </summary>
	public void Close()
	{
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Send a request to update
	/// </summary>
	/// <param name="request"></param>
	public void Request(string request)
	{
		gameObject.SetActive(true);
		_request = request;
	}
}
