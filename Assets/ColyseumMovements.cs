using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ColyseumMovements : MonoBehaviour
{
    public float maxspeed;
    public float moveforce;

    private Rigidbody2D rb;

    private Vector2 moveInput;

    public GameObject GroundGO;

    public bool touchingground;

    private SpriteRenderer SR;

    private Animator animator;

    public UnityEngine.UI.Image LifeBar;

    public int HP;

    public Camera Camera;

    public Transform GunTransform;

    private SpriteRenderer GunSR;

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

    public TextMeshProUGUI BulletText;

    public bool matchover;

    private ColyseumEnemyMovements[] allenemies;

    public bool Victory;

    public Button MatchEndButton;

    public List<int> ColyseumTierUnlocks;

    public TalentTreeScript TalentTreeScript;

    private int currentColyseumtier;

    public GameObject EnemyPrefab;

    public Vector2 enemystartpos;

    private int matchovercounter;

    private void OnEnable()
    {
        transform.position = new Vector2(-enemystartpos.x,enemystartpos.y);
        matchovercounter = 0;
        HP = 100;
        currentclip = maxbulletperclip;
        reloadcounter = 0;
        Victory = false;
        matchover = false;
        allenemies = FindObjectsByType<ColyseumEnemyMovements>(FindObjectsSortMode.None);
        MatchEndButton.gameObject.SetActive(false);
        UpdateColTier();

        if(allenemies.Length < currentColyseumtier+1)
        {
            for(int i = allenemies.Length;i<= currentColyseumtier + 1;i++)
            {
                GameObject newenemy = Instantiate(EnemyPrefab);
                allenemies.Append(newenemy.GetComponent<ColyseumEnemyMovements>());
            }
        }
        foreach(ColyseumEnemyMovements enemy in allenemies)
        {
            enemy.HP = 100;
            enemy.currentclip = enemy.maxbulletperclip;
            enemy.transform.position = enemystartpos + new Vector2(UnityEngine.Random.Range(-1f, 2f), UnityEngine.Random.Range(0, 2f));
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject== GroundGO || collision.otherCollider.gameObject == GroundGO)
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
        animator.SetBool("HoldingGun", true);
        GunSR = GunTransform.GetComponentInChildren<SpriteRenderer>();
        allenemies = FindObjectsByType<ColyseumEnemyMovements>(FindObjectsSortMode.None);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>(); 

    }

    private void CheckIfAllEnemiesdied()
    {
        bool alldead = true;
        foreach(ColyseumEnemyMovements enemy  in allenemies)
        {
            if(enemy.HP > 0)
            {
                alldead = false; break;
            }
        }

        if(alldead)
        {
            matchover = true;
            Victory = true;
        }
    }

    public void OnJump()
    {
        if (touchingground)
        {
            rb.AddForceY(moveforce, ForceMode2D.Impulse);
        }
    }

    public void OnReload()
    {
        if (currentclip<maxbulletperclip && reloadcounter == 0)
        {
            reloadcounter = (int)(reloadtime / Time.fixedDeltaTime);
            lastmaxcounterreloadcounter = reloadcounter;
        }
    }

    void FixedUpdate()
    {
        CheckIfAllEnemiesdied();
        if (matchover)
        {
            FinishMatch();
            return;
        }


        LifeBar.fillAmount = (float)(HP / 100f);

        float x = moveInput.x;

        if (x < 0 && rb.linearVelocity.x > -maxspeed)
        {
            rb.linearVelocityX =-maxspeed;
        }
        else if (x > 0 && rb.linearVelocity.x < maxspeed)
        {
            rb.linearVelocityX = maxspeed;
        }

        if(rb.linearVelocityX > 0.01f)
        {

            if (!animator.GetBool("Walking"))
            {
                animator.SetBool("Walking", true);
            }
        }
        else if(rb.linearVelocityX < -0.01f)
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


        // Convert mouse position to world coordinates
        Vector3 mouseWorld = Camera.ScreenToWorldPoint(Input.mousePosition);

        // Compute direction from sprite to mouse
        Vector2 direction = mouseWorld - GunTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (reloadcounter == 0)
        {
            GunTransform.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            GunTransform.transform.rotation = Quaternion.Euler(GunTransform.transform.rotation.eulerAngles + new Vector3(0f, 0f, (360f / reloadtime) * Time.fixedDeltaTime));
        }

        if (mouseWorld.x> transform.position.x)
        {
            if (SR.flipX)
            {
                SR.flipX = false;
                GunTransform.localPosition = new Vector3(Mathf.Abs(GunTransform.localPosition.x), GunTransform.localPosition.y, GunTransform.localPosition.z);
                GunSR.flipY = false;
            }
        }
        else if(mouseWorld.x < transform.position.x)
        {
            if (!SR.flipX)
            {
                SR.flipX = true;
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

    private void ManageGunShoot(Vector2 direction)
    {

        if(reloadcounter>0)
        {
            if(!ReloadBar.transform.parent.gameObject.activeSelf)
            {
                ReloadBar.transform.parent.gameObject.SetActive(true);
            }
            ReloadBar.fillAmount = (float)(lastmaxcounterreloadcounter -  reloadcounter)/ (float)lastmaxcounterreloadcounter;
            reloadcounter --;
            if(reloadcounter==0)
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
            if (Input.GetMouseButton(0) && currentclip > 0 && reloadcounter == 0)
            {
                delaybetweenbulletscnt = (int)(delaybetweenbullets / Time.fixedDeltaTime);
                SpawnBullet(direction);
            }
        }

        BulletText.text = currentclip+"/"+maxbulletperclip;
    }

    private void SpawnBullet(Vector2 direction)
    {
        if(currentclip>0)
        {
            currentclip--;

            if(currentclip==0)
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


    private void FinishMatch()
    {
        if(!MatchEndButton.gameObject.activeSelf)
        {
            MatchEndButton.gameObject.SetActive(true);
            TextMeshProUGUI buttontmp = MatchEndButton.GetComponentInChildren<TextMeshProUGUI>();
            if (Victory)
            {
                buttontmp.text = "Victory !\nFavor multiplied by " + currentColyseumtier * 2;
            }
            else
            {
                buttontmp.text = "Defeat !\nFavor divided by " + currentColyseumtier * 2;
            }
        }
        else if(matchovercounter < 2.5f/Time.fixedDeltaTime)
        {
            matchovercounter++;
        }
        else
        {
            MatchEndButton.onClick.Invoke();
        }
        
    }

    public void CloseColyseumButton()
    {
        if (Victory)
        {
            RollBoulder.instance.currentSave.favors *= currentColyseumtier * 2;
        }
        else
        {
            RollBoulder.instance.currentSave.favors /= currentColyseumtier * 2;
        }
    }

    public void UpdateColTier()
    {
        for (int i = 0; i < ColyseumTierUnlocks.Count; i++)
        {

            if (TalentTreeScript != null && TalentTreeScript.allnodes != null && ColyseumTierUnlocks != null && ColyseumTierUnlocks.Count > i && TalentTreeScript.allnodes.Count > ColyseumTierUnlocks[i] && TalentTreeScript.allnodes[ColyseumTierUnlocks[i]].unlocked)
            {
                currentColyseumtier = TalentTreeScript.allnodes[ColyseumTierUnlocks[i]].tier;
            }
        }
    }

}
