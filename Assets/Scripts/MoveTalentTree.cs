using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIDragMove : MonoBehaviour
{
    public float speedperframe;

    public float finalpos;

    private Vector3 Initialpos;

    private void Start()
    {
        Initialpos = transform.position;
    }

    private void OnEnable()
    {
        if(Initialpos!=Vector3.zero)
        {
            transform.position = Initialpos;
        }
        
    }

    private void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel")>0)
        {
            transform.position += new Vector3(0f,speedperframe, 0f) / Time.deltaTime;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.position -= new Vector3(0f, speedperframe, 0f) / Time.deltaTime;
        }

        if(transform.position.y > Initialpos.y)
        {
            transform.position = new Vector3(transform.position.x, Initialpos.y, transform.position.z);
        }
        if (transform.localPosition.y < finalpos)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, finalpos, transform.localPosition.z);
        }

    }

}
