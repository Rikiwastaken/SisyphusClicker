using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TalentTreeScript;
using static TreeNodeScript;

public class RollBoulder : MonoBehaviour
{

    public static RollBoulder instance;

    public int framewhererotate;
    public int rotationperframe;
    public Transform Boulder;
    public Transform HeartContainer;

    private string savePath;

    public Vector3 targetrotation;
    public TextMeshProUGUI distanceTMP;
    public TextMeshProUGUI favorsTMP;



    public GameObject HeartPrefab;
    public Vector2 heartspawnpoint;

    public float pointsnecessaryforheart;

    public TalentTreeScript treescript;

    public Animator animator;

    private int framewherewalks;

    private PathMovementScript pathMovement;

    public bool reachedheaven;

    [Serializable]

    public class SaveClass
    {
        public double meterswalked;
        public int currentFavorAddTier;
        public int currentFavorDelayTier;
        public int currentDistanceBonusTier;
        public double favors;
        public double FavorPoints;
        public List<bool> unlockedTree;
        public bool AlreadyFacedZeus;
    }

    public SaveClass currentSave;

    public List<int> MoreHealthUnlocks;
    public List<int> MoreDefUnlocks;
    public List<int> BetterGunUnlocks;

    public int numberofMoreHealth;
    public int numberofMoreDef;
    public int numberofBetterGun;

    public GameObject BaseLayer;
    public GameObject ColiseumLayer;
    public GameObject FinalBattleLayer;

    public UnityEngine.UI.Image FinaleImage;

    public GameObject BaseHUD;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        savePath = System.IO.Path.Combine(Application.persistentDataPath, "savefile.json");
        pathMovement = FindAnyObjectByType<PathMovementScript>();
        LoadSave();
        targetrotation = Boulder.transform.localRotation.eulerAngles;
        ManageFavorsText();
        UpdateGun();
        UpdateUpgradeTiers();
    }

    void Update()
    {
        treescript.CheckIfColiseumUnlocked();
        if (currentSave.meterswalked >Mathf.Pow(10,19))
        {
            if(!BaseLayer.activeSelf)
            {
                BaseLayer.SetActive(true);
            }
            if(ColiseumLayer.activeSelf)
            {
                ColiseumLayer.SetActive(false);
            }
            if (BaseHUD.activeSelf)
            {
                BaseHUD.SetActive(false);
            }
            if (!FinaleImage.gameObject.activeSelf)
            {
                FinaleImage.gameObject.SetActive(true);
            }
            Color oldcolor = FinaleImage.color;
            Color newcolor = new Color(oldcolor.r + 0.5f * Time.deltaTime, oldcolor.g + 0.5f * Time.deltaTime, oldcolor.b + 0.5f * Time.deltaTime);
            FinaleImage.color = newcolor;
            return;
        }

        if(FinaleImage.gameObject.activeSelf)
        {
            return;
        }

        if(!BaseHUD.activeSelf)
        {
            BaseHUD.SetActive(true);
        }

        if (framewhererotate > 0)
        {

            

            framewhererotate--;

            targetrotation += new Vector3(0, 0, rotationperframe* Time.deltaTime);

            framewherewalks = (int)(2f/Time.deltaTime);

        }
        
        Debug.Log(targetrotation);
            Boulder.transform.localRotation = Quaternion.Lerp(
                Boulder.transform.localRotation,
                Quaternion.Euler(targetrotation),
                Time.deltaTime * Mathf.Pow(2, currentSave.currentDistanceBonusTier)
            );


        if(framewherewalks>0)
        {
            pathMovement.movepath = true;
            framewherewalks--;
            if (!animator.GetBool("Walking"))
            {
                animator.SetBool("Walking", true);
            }
        }
        else
        {
            pathMovement.movepath = false;
            if (animator.GetBool("Walking"))
            {
                animator.SetBool("Walking", false);
            }
        }

        ManageDistanceText();
    }

    public void FightButton()
    {
        FinaleImage.gameObject.SetActive(false);
        BaseLayer.SetActive(false);
        ColiseumLayer.SetActive(false);
        FinalBattleLayer.SetActive(true);
        FinaleImage.color = Color.black;
        currentSave.favors = 0;
        currentSave.meterswalked = 0;
        currentSave.AlreadyFacedZeus = true;
    }

    public void Submit()
    {
        FinaleImage.gameObject.SetActive(false);
        BaseLayer.SetActive(true);
        ColiseumLayer.SetActive(false);
        FinalBattleLayer.SetActive(false);
        FinaleImage.color = Color.black;
        currentSave.favors = 0;
        currentSave.meterswalked = 0;
        currentSave.AlreadyFacedZeus = true;
    }

    public void ManageDistanceText()
    {

        string distance_text = "Distance : ";

        distance_text += CalculateNumberString(currentSave.meterswalked);
        if (distanceTMP != null)
        {
            distanceTMP.text = distance_text + "m";
        }
    }

    public void ManageFavorsText()
    {
        string favors_text = "Favors : ";

        favors_text += CalculateNumberString(currentSave.favors);
        if (favorsTMP != null)
        {
            favorsTMP.text = favors_text;
        }
    }

    public string CalculateNumberString(double number)
    {
        double numberToModify = number;
        string[] suffixes = { "", "k", "M", "G", "T", "P", "E", "Z", "Y", "R", "Q" };

        int idx = 0;
        while (numberToModify >= 1000.0 && idx < suffixes.Length - 1)
        {
            numberToModify /= 1000.0;
            idx++;
        }

        return numberToModify.ToString("0") + " " + suffixes[idx];
    }


    public void rotateBoulder()
    {

        double rotationbonus = 1;
        if (currentSave.currentDistanceBonusTier > 0)
        {
            rotationbonus += (double)Mathf.Pow(10, currentSave.currentDistanceBonusTier * 1.5f);
        }

        framewhererotate = (int)(0.5f/Time.fixedDeltaTime);
        
        currentSave.meterswalked += rotationbonus;

        GainFavor();


        Save();
    }

    public void GainFavor()
    {
        currentSave.FavorPoints++;
        if (currentSave.FavorPoints > pointsnecessaryforheart * Mathf.Pow(0.5f, currentSave.currentFavorDelayTier))
        {
            currentSave.FavorPoints -= pointsnecessaryforheart;
            GameObject newheart = Instantiate(HeartPrefab);
            newheart.GetComponent<HeartMovement>().UpdateColor(currentSave.currentFavorAddTier);
            newheart.transform.position = heartspawnpoint + new Vector2(UnityEngine.Random.Range(-1f, 1f), 0f);
            newheart.transform.parent = HeartContainer;
            currentSave.favors += Mathf.Max(Mathf.Pow(5, currentSave.currentFavorAddTier) / 2f, 1f);
            ManageFavorsText();
        }
    }

    private void LoadSave()
    {
        try
        {
            if (System.IO.File.Exists(savePath))
            {
                string json = System.IO.File.ReadAllText(savePath);
                currentSave = JsonUtility.FromJson<SaveClass>(json);
                Debug.Log("Save loaded from: " + savePath);
            }
            else
            {
                currentSave = new SaveClass()
                {
                    meterswalked = 0,
                    currentFavorAddTier = 0,
                    currentFavorDelayTier = 0,
                    currentDistanceBonusTier = 0,
                    favors = 0,
                    FavorPoints = 0,
                    unlockedTree = new List<bool>()
                };
                foreach (TreeNode node in TalentTreeScript.instance.allnodes)
                {
                    currentSave.unlockedTree.Add(false);
                }

                Save();
                Debug.Log("No save found. New save created.");
            }

            for (int i = 0; i < Mathf.Min(TalentTreeScript.instance.allnodes.Count, currentSave.unlockedTree.Count); i++)
            {
                TalentTreeScript.instance.allnodes[i].unlocked = currentSave.unlockedTree[i];
            }

        }
        catch (Exception e)
        {
            Debug.LogError("Load failed: " + e.Message);

            // fallback
            if (currentSave == null)
            {
                currentSave = new SaveClass()
                {
                    meterswalked = 0,
                    currentFavorAddTier = 0,
                    currentFavorDelayTier = 0,
                    currentDistanceBonusTier = 0,
                    favors = 0,
                    FavorPoints = 0,
                    unlockedTree = new List<bool>()
                };
                foreach (TreeNode node in TalentTreeScript.instance.allnodes)
                {
                    currentSave.unlockedTree.Add(false);
                }
                for (int i = 0; i < TalentTreeScript.instance.allnodes.Count; i++)
                {
                    TalentTreeScript.instance.allnodes[i].unlocked = currentSave.unlockedTree[i];
                }
            }

        }
        AutoClickerScript.instance.UpdateAutoclickers();
        UpdateGun();
        UpdateUpgradeTiers();
    }


    public void Save()
    {
        try
        {

            List<bool> list = new List<bool>();
            foreach (TreeNode node in TalentTreeScript.instance.allnodes)
            {
                list.Add(node.unlocked);
            }
            currentSave.unlockedTree = list;
            string json = JsonUtility.ToJson(currentSave, true);
            System.IO.File.WriteAllText(savePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError("Save failed: " + e.Message);
        }
    }

    public void ResetSave()
    {
        currentSave = new SaveClass()
        {
            meterswalked = 0,
            currentFavorAddTier = 0,
            currentFavorDelayTier = 0,
            currentDistanceBonusTier = 0,
            favors = 0,
            FavorPoints = 0,
            unlockedTree = new List<bool>()
        };
        foreach (TreeNode node in TalentTreeScript.instance.allnodes)
        {
            currentSave.unlockedTree.Add(false);
        }
        Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateUpgradeTiers()
    {
        int favorAddTier = 0;
        int favorDelayTier = 0;
        int DistanceTier = 0;
        foreach (TreeNode node in TalentTreeScript.instance.allnodes)
        {
            if (node.type == "MF" && node.unlocked)
            {
                favorAddTier++;
            }
            if (node.type == "FD" && node.unlocked)
            {
                favorDelayTier++;
            }
            if (node.type == "MD" && node.unlocked)
            {
                DistanceTier++;
            }
        }

        favorAddTier *= DistanceTier;

        currentSave.currentFavorAddTier = favorAddTier;
        currentSave.currentFavorDelayTier = favorDelayTier;
        currentSave.currentDistanceBonusTier = DistanceTier;
        Save();
    }



    public void UpdateGun()
    {
        UpdateHealthTier();
        UpdatedefTier();
        UpdateGunTier();
    }

    private void UpdateHealthTier()
    {
        numberofMoreHealth = 0;
        for (int i = 0; i < MoreHealthUnlocks.Count; i++)
        {

            if (TalentTreeScript.instance != null && TalentTreeScript.instance.allnodes != null && MoreHealthUnlocks != null && MoreHealthUnlocks.Count > i && TalentTreeScript.instance.allnodes.Count > MoreHealthUnlocks[i] && TalentTreeScript.instance.allnodes[MoreHealthUnlocks[i]].unlocked)
            {
                numberofMoreHealth ++;
            }
        }
    }

    private void UpdatedefTier()
    {
        numberofMoreDef = 0;
        for (int i = 0; i < MoreDefUnlocks.Count; i++)
        {

            if (TalentTreeScript.instance != null && TalentTreeScript.instance.allnodes != null && MoreDefUnlocks != null && MoreDefUnlocks.Count > i && TalentTreeScript.instance.allnodes.Count > MoreDefUnlocks[i] && TalentTreeScript.instance.allnodes[MoreDefUnlocks[i]].unlocked)
            {
                numberofMoreDef++;
            }
        }
    }

    private void UpdateGunTier()
    {
        numberofBetterGun = 0;
        for (int i = 0; i < BetterGunUnlocks.Count; i++)
        {

            if (TalentTreeScript.instance != null && TalentTreeScript.instance.allnodes != null && BetterGunUnlocks != null && BetterGunUnlocks.Count > i && TalentTreeScript.instance.allnodes.Count > BetterGunUnlocks[i] && TalentTreeScript.instance.allnodes[BetterGunUnlocks[i]].unlocked)
            {
                numberofBetterGun++;
            }
        }
    }
}
