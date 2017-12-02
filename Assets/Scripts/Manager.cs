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

    // Villagers count on start
    [Range(1, 20)]
    public int VillagersCount;

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

    [Header("Winter")]

    // Season duration
    public float SeasonDuration;

    // Time when game start
    private float _gameStart;

    // Current season
    // (0: Printemps, 1: Ete, 2: Automne, 3: Hiver)
    public int CurrentSeason { get { return (int)((Time.time - _gameStart) / SeasonDuration) % 4; } }

    // Passed year count
    public int YearCount { get { return (int)((Time.time - _gameStart) / (SeasonDuration * 4f)); } }

    // Grass material
    public Material GrassMaterial;

    // Grass color
    public Color GrassColorNormal, GrassColorWinter;

    // Leaves material
    public Material LeavesMaterial;

    // Leaves color
    public Color LeavesColorNormal, LeavesColorWinter;

    // Use this for initialization
    private void Awake () {
		Instance = this;
        _gameStart = Time.time;
        LeavesMaterial.color = LeavesColorNormal;
        Randomizer = new System.Random(Seed);
		Patches = new GameObject[Width, Height];
		BoxCollider boxcollider = gameObject.AddComponent<BoxCollider> ();
		boxcollider.center = new Vector3 ((float)Width / 2.0f - 0.5f, 0.5f, (float)Height / 2.0f - 0.5f);
		boxcollider.size = new Vector3 ((float)Width, 2, (float)Height);
		boxcollider.isTrigger = true;
		Properties = System.IO.File.Exists(@"properties.yml") ? new YamlLoader(@"properties.yml") : CreateProperties();
	    SeasonDuration = (float)Properties.GetElement("Delay.Season").Value;
    }

    // Update is called once per frame
    private void Update()
    {
        if (CurrentSeason != 3)
        {
            if (GrassColorNormal != GrassMaterial.color || LeavesColorNormal != LeavesMaterial.color)
            {
                float seasonBegin = (Time.time - _gameStart) / SeasonDuration;
                seasonBegin = Mathf.Min(1f, (seasonBegin - (int)seasonBegin) * 5f);
                if (GrassColorNormal != GrassMaterial.color)
                    GrassMaterial.color = Color.Lerp(GrassColorWinter, GrassColorNormal, seasonBegin);
                if (LeavesColorNormal != LeavesMaterial.color)
                    LeavesMaterial.color = Color.Lerp(LeavesColorWinter, LeavesColorNormal, seasonBegin);
            }
        }
        else
        {
            if (GrassColorWinter != GrassMaterial.color || LeavesColorWinter != LeavesMaterial.color)
            {
                float seasonBegin = (Time.time - _gameStart) / SeasonDuration;
                seasonBegin = Mathf.Min(1f, (seasonBegin - (int)seasonBegin) * 5f);
                if (GrassColorWinter != GrassMaterial.color)
                    GrassMaterial.color = Color.Lerp(GrassColorNormal, GrassColorWinter, seasonBegin);
                if (LeavesColorWinter != LeavesMaterial.color)
                    LeavesMaterial.color = Color.Lerp(LeavesColorNormal, LeavesColorWinter, seasonBegin);
            }
        }
    }

    /// <summary>
    /// Create properties
    /// </summary>
    /// <returns>Properties</returns>
    private static YamlLoader CreateProperties()
	{
		YamlLoader loader = new YamlLoader();
		// VALEURS DE DEPART
		loader.AddElement(new YamlLoader.PropertyElement("Start",
			new YamlLoader.PropertyElement[]
			{
				// Nombre de villageois au départ
				new YamlLoader.PropertyElement("Villagers", 4f),
				// Ressources dans la pile de stockage de départ
				new YamlLoader.PropertyElement("Ressources",
					new YamlLoader.PropertyElement[]
					{
						// Quantité de blé de départ
						new YamlLoader.PropertyElement("Corn", 200f),
						// Quantité de vêtements de départ
						new YamlLoader.PropertyElement("Clothes", 20f)
					})
			}
		));
		// VALEURS DE NOURRITURE
		loader.AddElement(new YamlLoader.PropertyElement("FoodValue",
			new YamlLoader.PropertyElement[]
			{
				// Valeur nutritionnelle du blé
				new YamlLoader.PropertyElement("Corn", 8f),
				// Valeur nutritionnelle du poisson
				new YamlLoader.PropertyElement("Fish", 10f),
				// Valeur nutritionnelle de la viande
				new YamlLoader.PropertyElement("Meat", 50f)
			}
		));
		// VALEURS DES RECOLTES
		loader.AddElement(new YamlLoader.PropertyElement("Harvest",
			new YamlLoader.PropertyElement[]
			{
				// Quantité totale de ressources sur un arbre
				new YamlLoader.PropertyElement("Tree", 50f),
				// Quantité totale de ressources sur un rocher
				new YamlLoader.PropertyElement("Stone", 100f),
				// Quantité totale de ressources sur un minerai de fer
				new YamlLoader.PropertyElement("Iron", 20f),
				// Quantité totale de ressources sur un champs de blé
				new YamlLoader.PropertyElement("Cornfield", 200f)
			}
		));
		// VALEURS DE DIVERS DELAIS (EN SECONDES)
		loader.AddElement(new YamlLoader.PropertyElement("Delay",
			new YamlLoader.PropertyElement[]
			{
				// Durée d'une saison
				new YamlLoader.PropertyElement("Season", 60f),
				// Durée de coupe complète d'un arbre
				new YamlLoader.PropertyElement("Tree", 30f),
				// Durée de minage complet d'un rocher
				new YamlLoader.PropertyElement("Stone", 120f),
				// Durée de minage complet d'un minerai de fer
				new YamlLoader.PropertyElement("Iron", 60f),
				// Durées relatives à un champs de blé
				new YamlLoader.PropertyElement("Cornfield", 
					new YamlLoader.PropertyElement[]
					{
						// Durée d'ensemencement
						new YamlLoader.PropertyElement("Seeding", 20f),
						// Durée de croissance
						new YamlLoader.PropertyElement("Growing", 100f),
						// Durée de récolte
						new YamlLoader.PropertyElement("Harvest", 30f)
					}),
				// Durée de pêche d'un poisson
				new YamlLoader.PropertyElement("Fishing", 2f),
				// Durée de plantage d'un arbre
				new YamlLoader.PropertyElement("Log", 50f),
				// Durée nécessaire pour que le niveau de faim tombe à zero
				new YamlLoader.PropertyElement("Hungry", 50f),
				// Durée nécessaire pour que le niveau de vie tombe à zero à cause de la faim
				new YamlLoader.PropertyElement("Starving", 50f),
				// Durée nécessaire pour que le niveau de vie tombe à zero à cause du froid
				new YamlLoader.PropertyElement("Freezing", 50f),
				// Durée nécessaire pour que le niveau de vie remonte entièrement
				new YamlLoader.PropertyElement("Heal", 50f),
				// Durée nécessaire pour que des vêtements équipés soit détruits
				new YamlLoader.PropertyElement("Clothes", 80f)
			}
		));
		// VALEURS RELATIVES A L'AGENT
		loader.AddElement(new YamlLoader.PropertyElement("Agent",
			new YamlLoader.PropertyElement[]
			{
				// Niveau de vie
				new YamlLoader.PropertyElement("Health", 50f),
				// Niveau de faim
				new YamlLoader.PropertyElement("Hunger", 50f),
				// Age maximum avant le décès naturel
				new YamlLoader.PropertyElement("Life", 60f)
			}
		));
		// POIDS DES ELEMENTS
		loader.AddElement(new YamlLoader.PropertyElement("RessourcesWeight",
			new YamlLoader.PropertyElement[]
			{
				new YamlLoader.PropertyElement("Wood", 1f),
				new YamlLoader.PropertyElement("Stone", 1.5f),
				new YamlLoader.PropertyElement("Iron", 2f),
				new YamlLoader.PropertyElement("Corn", 0.1f),
				new YamlLoader.PropertyElement("Clothes", 0.5f)
			}
		));
		// COUT DE CONSTRUCTION
		loader.AddElement(new YamlLoader.PropertyElement("BuildingCost",
			new YamlLoader.PropertyElement[]
			{
				new YamlLoader.PropertyElement("StockPile", new YamlLoader.PropertyElement[]
				{
					new YamlLoader.PropertyElement("Wood", 20f),
					new YamlLoader.PropertyElement("Duration", 10f)
				}),
				new YamlLoader.PropertyElement("House", new YamlLoader.PropertyElement[]
				{
					new YamlLoader.PropertyElement("Wood", 100f),
					new YamlLoader.PropertyElement("Stone", 50f),
					new YamlLoader.PropertyElement("Duration", 20f)
				}),
				new YamlLoader.PropertyElement("Cornfield", new YamlLoader.PropertyElement[]
				{
					new YamlLoader.PropertyElement("Duration", 10f)
				}),
				new YamlLoader.PropertyElement("StonePit", new YamlLoader.PropertyElement[]
				{
					new YamlLoader.PropertyElement("Duration", 10f)
				}),
				new YamlLoader.PropertyElement("FishermanHut", new YamlLoader.PropertyElement[]
				{
					new YamlLoader.PropertyElement("Wood", 50f),
					new YamlLoader.PropertyElement("Stone", 10f),
					new YamlLoader.PropertyElement("Duration", 20f)
				}),
				new YamlLoader.PropertyElement("LoggerHut", new YamlLoader.PropertyElement[]
				{
					new YamlLoader.PropertyElement("Wood", 50f),
					new YamlLoader.PropertyElement("Stone", 10f),
					new YamlLoader.PropertyElement("Duration", 20f)
				})
			}
		));
		loader.Save(@"properties.yml");
		return loader;
	}

}
