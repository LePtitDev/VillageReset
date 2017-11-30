using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskName("Choose task")]
public class Task_ChooseTask : Task
{

    // Agents lists
    private AgentController[] agents;

	// Use this for initialization
	protected override void Start () {
        base.Start();
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
    [ActionLink("ChooseMineIron", 2f)]
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

}
