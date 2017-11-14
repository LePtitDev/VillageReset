using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

	// Current instangce of Manager
	public static Manager Instance;

	[Header("Generation")]

	// Random seed
	[Range(1000000, 1000000000)]
	public Int32 Seed;

	// Width and height of map
	public int Width, Height;

	[Header("Village")]

	// Minimum distance between the village center and water
	[Range(0, 10)]
	public int MinimumWaterDistance;

	// Minimum distance between the village center and trees
	[Range(0, 10)]
	public int MinimumForestDistance;

	// Minimum distance between the village center and stone
	[Range(0, 10)]
	public int MinimumStoneDistance;

	// Minimum distance between the village center and iron
	[Range(0, 10)]
	public int MinimumIronDistance;

	[Header("Displaying")]

	// Display patches density
	public bool DisplayDensity = false;

	// Randomizer
	[HideInInspector]
	public System.Random Randomizer;

	// Patches map
	[HideInInspector]
	public GameObject[,] Patches;
	
	// Properties
	[HideInInspector]
	public YamlLoader Properties;
	
	// Use this for initialization
	private void Awake () {
		Instance = this;
		Randomizer = new System.Random(Seed);
		Patches = new GameObject[Width, Height];
		BoxCollider boxcollider = gameObject.AddComponent<BoxCollider> ();
		boxcollider.center = new Vector3 ((float)Width / 2.0f - 0.5f, 0.5f, (float)Height / 2.0f - 0.5f);
		boxcollider.size = new Vector3 ((float)Width, 2, (float)Height);
		boxcollider.isTrigger = true;
		Properties = System.IO.File.Exists(@"properties.yml") ? new YamlLoader(@"properties.yml") : CreateProperties();
	}

	/// <summary>
	/// Create properties
	/// </summary>
	/// <returns>Properties</returns>
	private static YamlLoader CreateProperties()
	{
		YamlLoader loader = new YamlLoader();
		// POIDS DES ELEMENTS
		loader.AddElement(new YamlLoader.PropertyElement("RessourcesWeight",
			new YamlLoader.PropertyElement[]
			{
				new YamlLoader.PropertyElement("Wood", 1.0f),
				new YamlLoader.PropertyElement("Stone", 1.5f),
				new YamlLoader.PropertyElement("Iron", 2.0f)
			}
		));
		// COUT DE CONSTRUCTION
		loader.AddElement(new YamlLoader.PropertyElement("BuildingCost",
			new YamlLoader.PropertyElement[]
			{
				new YamlLoader.PropertyElement("StockPile", new YamlLoader.PropertyElement[]
				{
					new YamlLoader.PropertyElement("Wood", 20.0f),
					new YamlLoader.PropertyElement("Duration", 10.0f)
				}),
				new YamlLoader.PropertyElement("House", new YamlLoader.PropertyElement[]
				{
					new YamlLoader.PropertyElement("Wood", 100.0f),
					new YamlLoader.PropertyElement("Stone", 50.0f),
					new YamlLoader.PropertyElement("Duration", 20.0f)
				}),
				new YamlLoader.PropertyElement("Cornfield", new YamlLoader.PropertyElement[]
				{
					new YamlLoader.PropertyElement("Duration", 10.0f)
				}),
				new YamlLoader.PropertyElement("StonePit", new YamlLoader.PropertyElement[]
				{
					new YamlLoader.PropertyElement("Duration", 10.0f)
				}),
				new YamlLoader.PropertyElement("FishermanHut", new YamlLoader.PropertyElement[]
				{
					new YamlLoader.PropertyElement("Wood", 50.0f),
					new YamlLoader.PropertyElement("Stone", 10.0f),
					new YamlLoader.PropertyElement("Duration", 20.0f)
				}),
				new YamlLoader.PropertyElement("LoggerHut", new YamlLoader.PropertyElement[]
				{
					new YamlLoader.PropertyElement("Wood", 50.0f),
					new YamlLoader.PropertyElement("Stone", 10.0f),
					new YamlLoader.PropertyElement("Duration", 20.0f)
				})
			}
		));
		loader.Save(@"properties.yml");
		return loader;
	}

}
