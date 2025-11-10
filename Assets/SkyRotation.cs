using UnityEngine;

public class SkyRotation : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles - new Vector3(0f, 0f, speed*Time.fixedDeltaTime));
    }
}
