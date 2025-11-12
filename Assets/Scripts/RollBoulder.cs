using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TalentTreeScript;

public class RollBoulder : MonoBehaviour
{

    public static RollBoulder instance;

    public int framewhererotate;
    public int rotationperframe;
    public Transform Boulder;
    public Transform HeartContainer;

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

    public ColyseumMovements FinalBattleMovement;

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
        public int ZeusCounter;
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
    public TextMeshProUGUI FinaleImageText;
    public UnityEngine.UI.Image TrueEndImage;

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
        if (currentSave.meterswalked >(double)Mathf.Pow(10,20))
        {
            if(!BaseLayer.activeSelf)
            {
                BaseLayer.SetActive(true);
            }
            if (TalentTreeScript.instance.transform.parent.gameObject.activeSelf)
            {
                TalentTreeScript.instance.transform.parent.gameObject.SetActive(false);
            }
            if (ColiseumLayer.activeSelf)
            {
                ColiseumLayer.SetActive(false);
            }
            if (BaseHUD.activeSelf)
            {
                MusicPlayer.instance.PlayBossMusic();
                BaseHUD.SetActive(false);
            }
            if (!FinaleImage.gameObject.activeSelf)
            {
                FinaleImage.gameObject.SetActive(true);
                if(currentSave.ZeusCounter > 0)
                {
                    switch(currentSave.ZeusCounter)
                    {
                        case 1:
                            FinaleImageText.text = "\"Hold on, you Again ?\"";
                            break;
                        case 2:
                            FinaleImageText.text = "\"You came all the way to get humiliated.\"";
                            break;
                        case 3:
                            FinaleImageText.text = "\"Don't you have anything better to do ?\"";
                            break;
                        case 4:
                            FinaleImageText.text = "\"This is embarassing.\"";
                            break;
                        case 5:
                            FinaleImageText.text = "\"Give me a break man...\"";
                            break;
                        case 6:
                            FinaleImageText.text = "\"You know what ? I'll just kill myself.\"";
                            break;
                        case 7:
                            FinaleImageText.text = "\"Enough !\"";
                            break;
                        case 8:
                            FinaleImageText.text = "\"I swear, I will just lift the curse.\"";
                            break;
                        case 9:
                            FinaleImageText.text = "\"Is this Soulslike ?\"";
                            break;
                        case 10:
                            FinaleImageText.text = "\"I'm not talking anymore.\"";
                            break;
                        default:
                            FinaleImageText.text = "\"...\"";
                            break;
                    }
                }
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
        currentSave.ZeusCounter ++;
        FinalBattleMovement.Setup();
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
        currentSave.ZeusCounter++;
        MusicPlayer.instance.PlayMapMusic();
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

        if(numberToModify >= 1000.0)
        {
            numberToModify = double.PositiveInfinity;
        }

        return numberToModify.ToString("0") + " " + suffixes[idx];
    }


    public void rotateBoulder()
    {
        if(!FinalBattleLayer.activeSelf)
        {
            double rotationbonus = 1;
            if (currentSave.currentDistanceBonusTier > 0)
            {
                rotationbonus += (double)Mathf.Pow(10, currentSave.currentDistanceBonusTier * 1.5f);
            }

            framewhererotate = (int)(0.5f / Time.fixedDeltaTime);

            currentSave.meterswalked += rotationbonus;

            GainFavor();
        }

        if(TrueEndImage.gameObject.activeSelf && TrueEndImage.color.a>=1f)
        {
            Application.Quit();
            Debug.Log("quitting");
        }
        
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
        for (int i = 0; i < Mathf.Min(TalentTreeScript.instance.allnodes.Count, currentSave.unlockedTree.Count); i++)
        {
            TalentTreeScript.instance.allnodes[i].unlocked = currentSave.unlockedTree[i];
        }
        
        AutoClickerScript.instance.UpdateAutoclickers();
        UpdateGun();
        UpdateUpgradeTiers();
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
