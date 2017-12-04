using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulationWatcher : MonoBehaviour
{

    // Villagers text
    private Text villagers;

    // Brigands text
    private Text brigands;

    // Wolfs text
    private Text wolfs;

    // Sheeps text
    private Text sheeps;

	// Use this for initialization
	void Start () {
		foreach (Text t in GetComponentsInChildren<Text>())
        {
            if (t.name == "Villagers")
                villagers = t;
            else if (t.name == "Brigands")
                brigands = t;
            else if (t.name == "Wolfs")
                wolfs = t;
            else if (t.name == "Sheeps")
                sheeps = t;
        }
	}
	
	// Update is called once per frame
	void Update () {
        villagers.text = GameObject.FindObjectsOfType<AgentController>().Length.ToString();
        brigands.text = GameObject.FindObjectsOfType<Brigand>().Length.ToString();
        wolfs.text = GameObject.FindObjectsOfType<Wolf>().Length.ToString();
        sheeps.text = GameObject.FindObjectsOfType<Sheep>().Length.ToString();
    }
}
