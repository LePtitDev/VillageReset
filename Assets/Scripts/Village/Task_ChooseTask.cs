using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskName("Choose task")]
public class Task_ChooseTask : Task
{

    // Agents lists
    private AgentController[] agents;

    // Agents vaillge
    private Village _village;

	// Use this for initialization
	protected override void Start () {
        base.Start();
	    _village = GameObject.Find("Village").GetComponent<Village>();
	}

    protected override void Update()
    {
        agents = GameObject.Find("Village").GetComponentsInChildren<AgentController>();
        base.Update();
    }

    [ActionMethod]
    public void ChooseBuild()
    {
        gameObject.AddComponent<Task_Build>();
        Destroy(this);
    }

    [ActionMethod]
    public void ChooseChopWood()
    {
        gameObject.AddComponent<Task_ChopWood>();
        Destroy(this);
    }

    [ActionMethod]
    public void ChooseBreakStone()
    {
        gameObject.AddComponent<Task_BreakStone>();
        Destroy(this);
    }

    [ActionMethod]
    public void ChooseMineIron()
    {
        gameObject.AddComponent<Task_MineIron>();
        Destroy(this);
    }

    [ActionMethod]
    public void ChooseFarmCorn()
    {
        gameObject.AddComponent<Task_FarmCorn>();
        Destroy(this);
    }

    [ActionMethod]
    public void ChooseHunt()
    {
        gameObject.AddComponent<Task_Hunt>();
        Destroy(this);
    }

    [PerceptMethod]
    [ActionLink("ChooseBuild", 4f)]
    public bool NoBuilder()
    {
        foreach (AgentController a in agents)
        {
            if (a.GetComponent<Task_Build>() != null)
                return false;
        }
        return true;
    }

    [PerceptMethod]
    [ActionLink("ChooseChopWood", 2f)]
    public bool NoTimberman()
    {
        foreach (AgentController a in agents)
        {
            if (a.GetComponent<Task_ChopWood>() != null)
                return false;
        }
        return true;
    }

    [PerceptMethod]
    [ActionLink("ChooseBreakStone", 2f)]
    public bool NoCarrier()
    {
        foreach (AgentController a in agents)
        {
            if (a.GetComponent<Task_BreakStone>() != null)
                return false;
        }
        return true;
    }

    [PerceptMethod]
    [ActionLink("ChooseMineIron", 1.5f)]
    public bool NoMiner()
    {
        foreach (AgentController a in agents)
        {
            if (a.GetComponent<Task_MineIron>() != null)
                return false;
        }
        return true;
    }

    [PerceptMethod]
    [ActionLink("ChooseFarmCorn", 2f)]
    public bool NoFarmer()
    {
        foreach (AgentController a in agents)
        {
            if (a.GetComponent<Task_FarmCorn>() != null)
                return false;
        }
        return true;
    }

    [PerceptMethod]
    [ActionLink("ChooseBuild", 0f)]
    public bool NoBuildNeeded()
    {
        bool need = true;
        foreach (GameObject o in _village.Building)
        {
            House house = o.GetComponent<House>();
            if (house != null && house.Villagers.Length < house.MaxCount)
                need = false;
        }
        if (need)
            return false;
        int consomation = (int)(_village.Villagers.Length *
                                (
                                    (float) Manager.Instance.Properties.GetElement("Agent.Hunger").Value /
                                    (float) Manager.Instance.Properties.GetElement("Delay.Hungry").Value
                                ) * (float) Manager.Instance.Properties.GetElement("Delay.Season").Value * 5f);
        int count = _village.GetComponentsInChildren<Cornfield>().Length;
        int cornfieldProduction = (int)(float)Manager.Instance.Properties.GetElement("Harvest.Cornfield").Value;
        float cornfieldDuration = (float) Manager.Instance.Properties.GetElement("Delay.Cornfield.Seeding").Value +
                                  (float) Manager.Instance.Properties.GetElement("Delay.Cornfield.Growing").Value +
                                  (float) Manager.Instance.Properties.GetElement("Delay.Cornfield.Harvest").Value;
        int production = (int)(Manager.Instance.SeasonDuration * 3f / cornfieldDuration) * count * cornfieldProduction *
                         (int)(float)Manager.Instance.Properties.GetElement("FoodValue.Corn").Value;
        if (consomation > production)
            return false;
        float countFree = 0;
        foreach (GameObject o in _village.Building)
        {
            StockPile stocks = o.GetComponent<StockPile>();
            if (stocks != null)
            {
                Inventory inv = stocks.GetComponent<Inventory>();
                countFree += inv.MaxWeight - inv.Weight;
            }
        }
        return countFree > 100f;
    }

    [PerceptMethod]
    [ActionLink("ChooseFarmCorn", 0f)]
    public bool NoCornfield()
    {
        return GameObject.Find("Cornfield(Clone)") == null;
    }

    [PerceptMethod]
    [ActionLink("ChooseChopWood", 1.5f)]
    public bool WoodReserveLow()
    {
        int count = 0;
        foreach (Inventory inv in GameObject.Find("Village").GetComponentsInChildren<Inventory>())
        {
            if (inv.name == "StockPile(Clone)")
                count += inv.GetElement("Wood");
        }
        return count < 100;
    }

    [PerceptMethod]
    [ActionLink("ChooseBreakStone", 1.3f)]
    public bool StoneReserveLow()
    {
        int count = 0;
        foreach (Inventory inv in GameObject.Find("Village").GetComponentsInChildren<Inventory>())
        {
            if (inv.name == "StockPile(Clone)")
                count += inv.GetElement("Stone");
        }
        return count < 50;
    }

    [PerceptMethod]
    public bool Spring()
    {
        return Manager.Instance.CurrentSeason == 0;
    }

    [PerceptMethod]
    public bool Summer()
    {
        return Manager.Instance.CurrentSeason == 1;
    }

    [PerceptMethod]
    public bool Automn()
    {
        return Manager.Instance.CurrentSeason == 2;
    }

    [PerceptMethod]
    [ActionLink("ChooseChopWood", 1.5f)]
    [ActionLink("ChooseFarmCorn", 0f)]
    [ActionLink("ChooseBuild", 0f)]
    [ActionLink("ChooseHunt", 0f)]
    public bool Winter()
    {
        return Manager.Instance.CurrentSeason == 3;
    }

}
