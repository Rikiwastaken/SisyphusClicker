using UnityEngine;

public class ColyseumEnemyMovements : MonoBehaviour
{
    public float maxspeed;
    public float moveforce;

    private Rigidbody2D rb;

    public GameObject GroundGO;

    public bool touchingground;

    private SpriteRenderer SR;

    private Animator animator;

    public UnityEngine.UI.Image LifeBar;

    public int HP;

    public Transform GunTransform;

    private SpriteRenderer GunSR;
    public SpriteRenderer HelmetSR;

    public Vector2 BulletSpawnOffset;

    public GameObject BulletPrefab;

    public float delaybetweenbullets;

    private int delaybetweenbulletscnt;

    public int maxbulletperclip;
    public int currentclip;

    public float reloadtime;
    public int reloadcounter;

    private int lastmaxcounterreloadcounter;

    public UnityEngine.UI.Image ReloadBar;

    private GameObject Player;


    public float DirectionMovementduration;

    private int DirectionMovementdurationcnt;

    private int randommovement;

    public float jumptrydelay;

    public int jumptrydelaycnt;

    public bool isZeus;

    public bool waittingforbattlestart;

    private void OnEnable()
    {
        HP = 100;
        if(isZeus)
        {
            HP = 5000;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject == GroundGO || collision.otherCollider.gameObject == GroundGO)
        {
            touchingground = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject == GroundGO || collision.otherCollider.gameObject == GroundGO)
        {
            touchingground = false;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        if(!isZeus)
        {
            animator.SetBool("HoldingGun", true);
        }
        GunSR = GunTransform.GetComponentInChildren<SpriteRenderer>();
        Player = FindAnyObjectByType<ColyseumMovements>().gameObject;
    }

    void FixedUpdate()
    {
       
        float maxhp = 100f;
        if(isZeus)
        {
            maxhp = 5000f;
        }
        LifeBar.fillAmount = (float)(HP / maxhp);
        
        if (Player.GetComponent<ColyseumMovements>().matchover || HP<=0 || waittingforbattlestart)
        {
            return;
        }
        ManageMovement();

        


        if (rb.linearVelocityX > 0.01f)
        {

            if (!animator.GetBool("Walking"))
            {
                animator.SetBool("Walking", true);
            }
        }
        else if (rb.linearVelocityX < -0.01f)
        {

            if (!animator.GetBool("Walking"))
            {
                animator.SetBool("Walking", true);
            }
        }
        else
        {
            if (animator.GetBool("Walking"))
            {
                animator.SetBool("Walking", false);
            }

        }


        Vector3 target = Player.transform.position;

        // Compute direction from sprite to mouse
        Vector2 direction = target - GunTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if(reloadcounter==0)
        {
            GunTransform.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            GunTransform.transform.rotation = Quaternion.Euler(GunTransform.transform.rotation.eulerAngles + new Vector3(0f,0f,(360f / reloadtime) * Time.fixedDeltaTime));
        }


        if (target.x > transform.position.x)
        {
            if (SR.flipX)
            {
                SR.flipX = false;
                HelmetSR.flipX = false;
                GunTransform.localPosition = new Vector3(Mathf.Abs(GunTransform.localPosition.x), GunTransform.localPosition.y, GunTransform.localPosition.z);
                GunSR.flipY = false;
            }
        }
        else if (target.x < transform.position.x)
        {
            if (!SR.flipX)
            {
                SR.flipX = true;
                HelmetSR.flipX = true;
                GunTransform.localPosition = new Vector3(-Mathf.Abs(GunTransform.localPosition.x), GunTransform.localPosition.y, GunTransform.localPosition.z);
                GunSR.flipY = true;
            }
        }


        if (transform.localPosition.x < -8.5f)
        {
            transform.localPosition = new Vector3(-8.5f, transform.localPosition.y, transform.localPosition.z);
        }
        if (transform.localPosition.x > 8.5f)
        {
            transform.localPosition = new Vector3(8.5f, transform.localPosition.y, transform.localPosition.z);
        }


        ManageGunShoot(direction);

    }

    private void ManageMovement()
    {
        if(DirectionMovementdurationcnt == 0)
        {
            randommovement = UnityEngine.Random.Range(-1, 2);
            DirectionMovementdurationcnt = (int)(DirectionMovementduration / Time.fixedDeltaTime);
        }
        else
        {
            DirectionMovementdurationcnt--;
        }

        if (jumptrydelaycnt == 0)
        {

            if(UnityEngine.Random.Range(-1, 2)>0)
            {
                if (touchingground)
                {
                    rb.AddForceY(moveforce, ForceMode2D.Impulse);
                }
            }

            jumptrydelaycnt = (int)(jumptrydelay / Time.fixedDeltaTime);
        }
        else
        {
            jumptrydelaycnt--;
        }

        if (randommovement < 0 && rb.linearVelocity.x > -maxspeed)
        {
            rb.linearVelocityX = -maxspeed;
        }
        else if (randommovement > 0 && rb.linearVelocity.x < maxspeed)
        {
            rb.linearVelocityX = maxspeed;
        }

    }

    private void ManageGunShoot(Vector2 direction)
    {

        if (reloadcounter > 0)
        {
            if (!ReloadBar.transform.parent.gameObject.activeSelf)
            {
                ReloadBar.transform.parent.gameObject.SetActive(true);
            }
            ReloadBar.fillAmount = (float)(lastmaxcounterreloadcounter - reloadcounter) / (float)lastmaxcounterreloadcounter;
            reloadcounter--;
            if (reloadcounter == 0)
            {
                currentclip = maxbulletperclip;

            }
        }
        else
        {
            if (ReloadBar.transform.parent.gameObject.activeSelf)
            {
                ReloadBar.transform.parent.gameObject.SetActive(false);
            }
        }

        if (delaybetweenbulletscnt > 0)
        {
            delaybetweenbulletscnt--;
        }
        else
        {
            if (currentclip > 0 && reloadcounter == 0)
            {
                delaybetweenbulletscnt = (int)(delaybetweenbullets / Time.fixedDeltaTime);
                SpawnBullet(direction);
            }
        }
    }




    private void SpawnBullet(Vector2 direction)
    {
        if (currentclip > 0)
        {
            currentclip--;

            if (currentclip == 0)
            { 
                reloadcounter = (int)(reloadtime / Time.fixedDeltaTime);
                lastmaxcounterreloadcounter = reloadcounter;
            }

            GameObject newbullet = Instantiate(BulletPrefab);
            newbullet.transform.SetParent(GunTransform.GetChild(0));
            newbullet.transform.localPosition = BulletSpawnOffset;
            newbullet.transform.parent = null;
            newbullet.GetComponent<BulletScript>().InitializeBullet(gameObject, direction);
        }
    }
}
