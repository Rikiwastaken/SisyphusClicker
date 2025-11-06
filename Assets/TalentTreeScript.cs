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

    // --- Add these ---
    private bool isDragging = false;
    private Vector3 offset;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        HandleDrag();
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                isDragging = true;
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, hit.distance)
                );
                offset = transform.position - mouseWorld;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist)
            );

            transform.position = mouseWorld + offset;
        }
    }
}
