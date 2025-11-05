using TMPro;
using UnityEngine;

public class RollBoulder : MonoBehaviour
{
    public int framewhererotate;
    public int rotationperframe;
    public Transform Boulder;
    public Transform HeartContainer;

    public double meterswalked;

    public Vector3 targetrotation;
    public TextMeshProUGUI distanceTMP;
    public TextMeshProUGUI favorsTMP;

    public float FavorPoints;

    public GameObject HeartPrefab;
    public Vector2 heartspawnpoint;

    public float pointsnecessaryforheart;
    public double favors;
    

    private void Start()
    {
        targetrotation = Boulder.transform.localRotation.eulerAngles;
    }

    void Update()
    {
        if (framewhererotate > 0)
        {
            framewhererotate--;

            targetrotation += new Vector3(0, 0, rotationperframe);

            
            
        }

        Boulder.transform.localRotation = Quaternion.Lerp(
            Boulder.transform.localRotation,
            Quaternion.Euler(targetrotation),
            Time.deltaTime
        );

        ManageDistanceText();
    }

    public void ManageDistanceText()
    {
        double meters = meterswalked; 
        string[] suffixes = { "m", "km", "Mm", "Gm", "Tm", "Pm", "Em", "Zm", "Ym", "Rm", "Qm" }; 

        int idx = 0;
        while (meters >= 1000.0 && idx < suffixes.Length - 1)
        {
            meters /= 1000.0;
            idx++;
        }

        string distance_text = "Distance walked : ";
        if (idx == 0)
            distance_text += ((double)meters).ToString() + " " + suffixes[idx];
        else
            distance_text += meters.ToString("0.###") + " " + suffixes[idx];

        if (distanceTMP != null)
            distanceTMP.text = distance_text;
    }

    public void ManageFavorsText()
    {
        double favorsearned = favors;
        string[] suffixes = { "", "k", "M", "G", "T", "P", "E", "Z", "Y", "R", "Q" };

        int idx = 0;
        while (favorsearned >= 1000.0 && idx < suffixes.Length - 1)
        {
            favorsearned /= 1000.0;
            idx++;
        }

        string favors_text = "Favors : ";
        if (idx == 0)
            favors_text += ((double)favorsearned).ToString() + " " + suffixes[idx];
        else
            favors_text += favorsearned.ToString("0.###") + " " + suffixes[idx];

        if (favorsTMP != null)
            favorsTMP.text = favors_text;
    }

    public void rotateBoulder()
    {
        framewhererotate++;
        FavorPoints++;
        meterswalked++;


        if(FavorPoints > pointsnecessaryforheart)
        { 
            FavorPoints -= pointsnecessaryforheart;
            GameObject newheart =Instantiate(HeartPrefab);
            newheart.transform.position = heartspawnpoint + new Vector2(UnityEngine.Random.Range(-1f, 1f),0f);
            newheart.transform.parent = HeartContainer;
            favors++;
            ManageFavorsText();
        }

    }
}
