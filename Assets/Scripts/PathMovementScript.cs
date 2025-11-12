using TMPro;
using UnityEngine;

public class PathMovementScript : MonoBehaviour
{

    public bool movepath;

    public float movespeed;

    public Transform path0;
    public Transform path1;

    private Transform mainpath;

    private RollBoulder RollBoulder;

    public TextMeshProUGUI TitleTMP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainpath = path0;
        RollBoulder = FindAnyObjectByType<RollBoulder>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if(movepath)
        {
            float newmovespeed = movespeed * Mathf.Pow(2, Mathf.Min(RollBoulder.currentSave.currentDistanceBonusTier,8f));
            path0.transform.localPosition -= new Vector3(newmovespeed * Time.deltaTime, 0, 0);
            path1.transform.localPosition -= new Vector3(newmovespeed * Time.deltaTime, 0, 0);

            if (mainpath.transform.localPosition.x <= -30)
            {
                if(mainpath == path0)
                {
                    path0.transform.localPosition+= new Vector3(60f, 0, 0);
                    mainpath = path1;
                    if(path0.childCount>0)
                    {
                        Destroy(path0.GetChild(0).gameObject);
                    }
                }
                else
                {
                    path1.transform.localPosition += new Vector3(60f, 0, 0);
                    mainpath = path0;
                }
            }
            if(TitleTMP!=null && TitleTMP.transform.localPosition.x>-3000)
            {
                TitleTMP.transform.localPosition -= new Vector3(1f, 0.3f, 0f);
            }
            else if(TitleTMP != null)
            {
                Destroy(TitleTMP.gameObject);
            }
            

        }
    }
}
