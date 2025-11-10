using UnityEngine;

public class PathMovementScript : MonoBehaviour
{

    public bool movepath;

    public float movespeed;

    public Transform path0;
    public Transform path1;

    private Transform mainpath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainpath = path0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(movepath)
        {

            path0.transform.localPosition -= new Vector3(movespeed*Time.deltaTime, 0, 0);
            path1.transform.localPosition -= new Vector3(movespeed * Time.deltaTime, 0, 0);

            if (mainpath.transform.localPosition.x <= -30)
            {
                if(mainpath == path0)
                {
                    path0.transform.localPosition+= new Vector3(60f, 0, 0);
                    mainpath = path1;
                }
                else
                {
                    path1.transform.localPosition += new Vector3(60f, 0, 0);
                    mainpath = path0;
                }
            }
        }
    }
}
