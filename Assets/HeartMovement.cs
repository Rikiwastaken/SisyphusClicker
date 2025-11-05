using UnityEngine;

public class HeartMovement : MonoBehaviour
{

    public float timebeforenewdirection;
    private int timebeforenewdirectioncnt;

    public Vector2 direction;

    public float speed;

    public float timebeforedisappear;
    private int timebeforedisappearcnt;

    private SpriteRenderer SR;

    private int framesbeforedeath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        timebeforedisappearcnt = (int)((timebeforedisappear+ UnityEngine.Random.Range(0,2f)) / Time.deltaTime);
        framesbeforedeath = timebeforedisappearcnt;
        recalculatedirection();
    }

    // Update is called once per frame
    void Update()
    {
        ManageHeartMovement();
        ManageFade();


    }


    private void ManageFade()
    {
        if(timebeforedisappearcnt == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            timebeforedisappearcnt--;

            if(timebeforedisappearcnt < framesbeforedeath / 2)
            {
                Color newcolor = SR.color;
                newcolor.a = timebeforedisappearcnt / ((float)framesbeforedeath / 2f);
                SR.color = newcolor;
            }
        }
    }


    private void ManageHeartMovement()
    {
        if (timebeforenewdirectioncnt == 0)
        {
            timebeforenewdirectioncnt = (int)(timebeforenewdirection / Time.deltaTime);
            recalculatedirection();
        }
        else
        {
            timebeforenewdirectioncnt--;

            transform.position += (Vector3)direction * speed * Time.deltaTime;

        }
    }

    private void recalculatedirection()
    {
        direction = new Vector2(0, 1);

        direction += new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-0.3f, 0.3f));

    }




}
