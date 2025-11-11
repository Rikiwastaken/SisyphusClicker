using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AutoClickerScript : MonoBehaviour
{

    public static AutoClickerScript instance;

    public float delaybetweenautoclicks;
    private float delaybetweenautoclickscounter;

    public float delaybetweenautofavorss;
    private float delaybetweenautofavorscounter;

    public float delaybetweenautocoli;
    private float delaybetweenautocolicounter;

    public List<int> AutoclickerTierUnlocks;
    public List<int> AutoFavorTierUnlocks;
    public List<int> AutoColiTierUnlocks;

    private int currenttierautoclickertier;
    private int currenttierautofavortier;
    private int currenttierautocolitier;

    public GameObject CyclopsGO;

    public GameObject MermaidGO;

    public ColyseumMovements ColiseumMovements;

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

                    float delaytiermultiplier = Mathf.Pow(0.5f, currenttierautoclickertier);

                    delaybetweenautoclickscounter = (int)(delaybetweenautoclicks * delaytiermultiplier / Time.deltaTime);
                    RollBoulder.instance.rotateBoulder();
                }
                else
                {
                    delaybetweenautoclickscounter--;
                }
                if(!CyclopsGO.activeSelf)
                {
                    CyclopsGO.SetActive(true);
                }
            }
            else
            {
                if (CyclopsGO.activeSelf)
                {
                    CyclopsGO.SetActive(false);
                }

            }

            if (TalentTreeScript.instance.allnodes[AutoFavorTierUnlocks[0]].unlocked)
            {
                if (delaybetweenautofavorscounter == 0)
                {

                    float delaytiermultiplier = Mathf.Pow(0.5f, currenttierautofavortier);

                    delaybetweenautofavorscounter = (int)(delaybetweenautofavorss * delaytiermultiplier / Time.deltaTime);
                    RollBoulder.instance.GainFavor();
                }
                else
                {
                    delaybetweenautofavorscounter--;
                }
                if (!MermaidGO.activeSelf)
                {
                    MermaidGO.SetActive(true);
                }
            }
            else
            {
                if (MermaidGO.activeSelf)
                {
                    MermaidGO.SetActive(false);
                }

            }
            if (TalentTreeScript.instance.allnodes[AutoColiTierUnlocks[0]].unlocked)
            {
                if (delaybetweenautocolicounter == 0)
                {

                    float delaytiermultiplier = Mathf.Pow(0.5f, currenttierautocolitier);

                    delaybetweenautocolicounter = (int)(delaybetweenautocoli * delaytiermultiplier / Time.deltaTime);
                    ColiseumMovements.distributeColiseumResult(true);
                }
                else
                {
                    delaybetweenautocolicounter--;
                }
            }
            
        }
        
    }

    public void UpdateAutoclickers()
    {
        UpdateACTier();
        UpdateAFTier();
        UpdateAColiTier();
    }

    private void UpdateACTier()
    {
        for (int i = 0;i<AutoclickerTierUnlocks.Count;i++)
        {

            if(TalentTreeScript.instance != null && TalentTreeScript.instance.allnodes!=null && AutoclickerTierUnlocks!=null && AutoclickerTierUnlocks.Count>i &&  TalentTreeScript.instance.allnodes.Count> AutoclickerTierUnlocks[i] && TalentTreeScript.instance.allnodes[AutoclickerTierUnlocks[i]].unlocked)
            {
                currenttierautofavortier =TalentTreeScript.instance.allnodes[AutoclickerTierUnlocks[i]].tier;
            }
        }
    }

    private void UpdateAFTier()
    {
        for (int i = 0; i < AutoFavorTierUnlocks.Count; i++)
        {

            if (TalentTreeScript.instance != null && TalentTreeScript.instance.allnodes != null && AutoFavorTierUnlocks != null && AutoFavorTierUnlocks.Count > i && TalentTreeScript.instance.allnodes.Count > AutoFavorTierUnlocks[i] && TalentTreeScript.instance.allnodes[AutoFavorTierUnlocks[i]].unlocked)
            {
                currenttierautofavortier = TalentTreeScript.instance.allnodes[AutoFavorTierUnlocks[i]].tier;
            }
        }
    }

    private void UpdateAColiTier()
    {
        for (int i = 0; i < AutoColiTierUnlocks.Count; i++)
        {

            if (TalentTreeScript.instance != null && TalentTreeScript.instance.allnodes != null && AutoColiTierUnlocks != null && AutoColiTierUnlocks.Count > i && TalentTreeScript.instance.allnodes.Count > AutoColiTierUnlocks[i] && TalentTreeScript.instance.allnodes[AutoColiTierUnlocks[i]].unlocked)
            {
                currenttierautocolitier = TalentTreeScript.instance.allnodes[AutoColiTierUnlocks[i]].tier;
            }
        }
    }
}
