using TMPro;
using UnityEngine;

public class RollBoulder : MonoBehaviour
{

    public static RollBoulder instance;

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


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

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

        string distance_text = "Favors : ";

        distance_text += CalculateNumberString(meterswalked);
        if (distanceTMP != null)
        {
            distanceTMP.text = distance_text+"m";
        }
    }

    public void ManageFavorsText()
    {
        string favors_text = "Favors : ";

        favors_text += CalculateNumberString(favors);
        if (favorsTMP != null)
        {
            favorsTMP.text = favors_text;
        }
    }

    public string CalculateNumberString(double number)
    {
        double numbertomodify = number;
        string[] suffixes = { "", "k", "M", "G", "T", "P", "E", "Z", "Y", "R", "Q" };

        int idx = 0;
        while (numbertomodify >= 1000.0 && idx < suffixes.Length - 1)
        {
            numbertomodify /= 1000.0;
            idx++;
        }

        string numberstr = "";
        if (idx == 0)
            numberstr += ((double)numbertomodify).ToString() + " " + suffixes[idx];
        else
            numberstr += numbertomodify.ToString("0.###") + " " + suffixes[idx];

        return numberstr;
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
