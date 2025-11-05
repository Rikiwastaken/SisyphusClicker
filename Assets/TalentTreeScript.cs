using System;
using System.Collections.Generic;
using UnityEngine;

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
        public int basecost;
        public Sprite picture;
        public GameObject GameObject;
        public bool unlocked;
    }

    public List<TreeNode> allnodes;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
