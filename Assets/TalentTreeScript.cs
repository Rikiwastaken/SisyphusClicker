using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TalentTreeScript : MonoBehaviour
{
    public static TalentTreeScript instance;

    [Serializable]
    public class TreeNode
    {
        public int id;
        public string NodeName;
        public List<int> parentIDs;
        public int tier;
        public double basecost;
        public Sprite picture;
        public GameObject GameObject;
        public bool unlocked;
        public string type; // AC (autoclicker), MD (More Distance), ACol (AutoCOLYSEUM), Col (COLYSEUM), MF (More Favors), FD (Favor Delays), AF (Auto Favors), RE (Random Events)
    }

    public bool triggervisualchange;

    public List<TreeNode> allnodes;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        InvertIndexes();
        transform.parent.gameObject.SetActive(false);
    }

    public void Update()
    {
        if(triggervisualchange)
        {
            triggervisualchange = false;
            for(int i = 0; i < transform.childCount; i++)
            {
                if(transform.GetChild(i).GetComponent<TreeNodeScript>())
                {
                    transform.GetChild(i).GetComponent<TreeNodeScript>().OnEnable();
                }
            }
        }
    }

    private void InvertIndexes()
    {
        List<Transform> children = new List<Transform>();
        for(int i = 0; i<transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }

        for(int i = 0;i<children.Count; i++)
        {
            children[children.Count-1-i].SetSiblingIndex(i);
        }

    }

    public void ApplyNodeChangesAndSave()
    {
#if UNITY_EDITOR

        foreach(TreeNode node in allnodes)
        {
            switch(node.type)
            {
                case "AC":
                    node.basecost = 10;
                    node.NodeName = "Auto Clicker";
                    break;
                case "MD":
                    node.basecost = 15;
                    node.NodeName = "More Distance";
                    break;
                case "Col":
                    node.basecost = 20;
                    node.NodeName = "Colyseum";
                    break;
                case "MF":
                    node.basecost = 25;
                    node.NodeName = "More Favors";
                    break;
                case "FD":
                    node.basecost = 30;
                    node.NodeName = "Favor Delays";
                    break;
                case "AF":
                    node.basecost = 30000000;
                    node.NodeName = "Auto Favors";
                    break;
                case "RE":
                    node.basecost = 40000;
                    node.NodeName = "Random Events";
                    break;
                case "ACol":
                    node.basecost = 50000000000;
                    node.NodeName = "Auto Colyseum";
                    break;

                    
            }
        }


        // Mark this object as dirty so Unity knows it changed
        EditorUtility.SetDirty(this);

        // Mark the scene dirty so it gets saved
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);


        Debug.Log("TalentTreeScript: All node changes have been saved to the scene.");
#else
        Debug.LogWarning("ApplyNodeChangesAndSave can only be used in the Unity Editor.");
#endif
    }

}
