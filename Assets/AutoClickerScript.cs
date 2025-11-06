using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AutoClickerScript : MonoBehaviour
{

    public static AutoClickerScript instance;

    public float delaybetweenautoclicks;
    private float delaybetweenautoclickscounter;

    public List<int> AutoclickerTierUnlocks;

    private int currenttier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(TalentTreeScript.instance != null)
        {
            if (TalentTreeScript.instance.allnodes[AutoclickerTierUnlocks[0]].unlocked)
            {
                if (delaybetweenautoclickscounter == 0)
                {

                    float delaytiermultiplier = Mathf.Pow(0.5f, currenttier);

                    delaybetweenautoclickscounter = (int)(delaybetweenautoclicks * delaytiermultiplier / Time.deltaTime);
                    RollBoulder.instance.rotateBoulder();
                }
                else
                {
                    delaybetweenautoclickscounter--;
                }
            }
        }
        
    }

    public void UpdateACTier()
    {
        for (int i = 0;i<AutoclickerTierUnlocks.Count;i++)
        {
            if(TalentTreeScript.instance.allnodes[AutoclickerTierUnlocks[i]].unlocked)
            {
                currenttier =TalentTreeScript.instance.allnodes[AutoclickerTierUnlocks[i]].tier;
            }
        }
    }
}
