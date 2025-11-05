using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;
using static TalentTreeScript;

public class TreeNodeScript : MonoBehaviour
{

    //public class TreeNode
    //{
    //    public int id;
    //    public string name;
    //    public List<int> parentIDs;
    //    public int tier;
    //    public int basecost;
    //    public Sprite picture;
    //    public GameObject GameObject;
    //    public bool unlocked;
    //}

    public int NodeId;

    private TreeNode ScriptNode;

    private TextMeshProUGUI buttonNameTMP;

    private List<TreeNode> parents = new List<TreeNode>();

    private double necessary_favors;

    public float thickness;

    public Sprite LineSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        buttonNameTMP = GetComponentInChildren<TextMeshProUGUI>();

        foreach(TreeNode node in TalentTreeScript.instance.allnodes)
        {
            if(node.id == NodeId)
            {
                ScriptNode = node;
                break;
            }
        }
        ScriptNode.GameObject = gameObject;
        foreach (TreeNode node in TalentTreeScript.instance.allnodes)
        {
            if (ScriptNode.parentIDs.Contains(node.id))
            {
                parents.Add(node);
            }
        }

        CalculateNecessaryFavors();
        CreateLines();
        buttonNameTMP.text = ScriptNode.NodeName + " " + IntToRoman(ScriptNode.tier);
    }

    private void CalculateNecessaryFavors()
    {
        necessary_favors = ScriptNode.basecost * 10 ^ (ScriptNode.tier * 3);
    }

    private bool CheckIfTalentCanBeUnlocked()
    {

        foreach (TreeNode node in parents)
        {
            if (!node.unlocked)
            {
                return false;
            }
        }

        if (RollBoulder.instance.favors < necessary_favors)
        {
            return false;
        }



        return true;
    }

    private void CreateLines()
    {
        foreach (TreeNode parent in parents)
        {
            GameObject square = new GameObject();
            square.AddComponent<Image>();
            square.GetComponent<Image>().sprite = LineSprite;
            Vector2 center = (transform.position + parent.GameObject.transform.position) / 2f;
            square.transform.position = center;

            Vector2 dir = parent.GameObject.transform.position - transform.position;
            float length = dir.magnitude;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            square.transform.rotation = Quaternion.Euler(0, 0, angle);
            square.transform.localScale = new Vector3(length, thickness, 1);

        }
    }

    private string IntToRoman(int value)
    {
        switch (value)
        {
            case 1: return "I";
            case 2: return "II";
            case 3: return "III";
            case 4: return "IV";
            case 5: return "V";
            case 6: return "VI";
            case 7: return "VII";
            case 8: return "VIII";
            case 9: return "IX";
            case 10: return "X";
            case 11: return "XI";
            case 12: return "XII";
            case 13: return "XIII";
            case 14: return "XIV";
            case 15: return "XV";
            case 16: return "XVI";
            case 17: return "XVII";
            case 18: return "XVIII";
            case 19: return "XIX";
            case 20: return "XX";
            default: return "";
        }
    }

    public void TryToUnlock()
    {
        if (!ScriptNode.unlocked && CheckIfTalentCanBeUnlocked())
        {
            RollBoulder.instance.favors -= necessary_favors;
            ScriptNode.unlocked = true;
        }
    }
}
