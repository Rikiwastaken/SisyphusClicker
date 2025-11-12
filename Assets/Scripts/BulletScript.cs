using UnityEngine;

public class BulletScript : MonoBehaviour
{

    public float speed;

    private Vector2 Direction;
    private GameObject Sender;

    public float maxX;
    public float maxY;

    private ColyseumMovements colyseumMovements;
    private RollBoulder rollBoulder;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject!=Sender)
        {
            if(collision.GetComponent<ColyseumMovements>() != null && !colyseumMovements.matchover)
            {
                int damage = 10 / (rollBoulder.numberofMoreDef + 1);
                if (Sender.GetComponent<ColyseumEnemyMovements>() && Sender.GetComponent<ColyseumEnemyMovements>().isZeus)
                {
                    damage = 100 / (rollBoulder.numberofMoreDef + 1);
                }
                

                collision.GetComponent<ColyseumMovements>().HP -= damage;
                if(collision.GetComponent<ColyseumMovements>().HP<=0)
                {
                    colyseumMovements.matchover = true;
                    colyseumMovements.Victory = false;
                }
            }
            if (collision.GetComponent<ColyseumEnemyMovements>() != null && !colyseumMovements.matchover && Sender.GetComponent<ColyseumEnemyMovements>()==null)
            {
                int damage = 25 * (rollBoulder.numberofBetterGun + 1);
                collision.GetComponent<ColyseumEnemyMovements>().HP -= 25;
            }
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Direction.Normalize();
        transform.localPosition += (Vector3)(Direction * speed * Time.fixedDeltaTime);
        if(Mathf.Abs(transform.localPosition.x)>maxX || Mathf.Abs(transform.localPosition.y) > maxY)
        {
            Destroy(gameObject) ;
        }
    }


    public void InitializeBullet(GameObject sender, Vector2 direction)
    {
        rollBoulder = FindAnyObjectByType<RollBoulder>();
        Sender = sender;
        Direction = direction;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        colyseumMovements = FindAnyObjectByType<ColyseumMovements>();
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        transform.rotation = rot;
    }

}
