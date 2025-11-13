using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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

    private int maxhp;

    public bool isfinalBattle;

    public bool waittingforbattlestart;

    public UnityEngine.UI.Image EndBlackScreen;

    public TextMeshProUGUI EndText;

    public UnityEngine.UI.Image TrueEnd;

    public Button endbutton;

    private int endcounter;

    public void Setup()
    {
        matchover = false;
        foreach (BulletScript bullet in FindObjectsByType<BulletScript>(FindObjectsSortMode.None))
        {
            Destroy(bullet.gameObject);
        }

        transform.position = new Vector2(-enemystartpos.x,enemystartpos.y);
        matchovercounter = 0;

        maxhp = 100 * (RollBoulder.instance.numberofMoreHealth + 1);
        HP = maxhp;
        currentclip = maxbulletperclip;
        reloadcounter = 0;
        Victory = false;
        matchover = false;
        allenemies = FindObjectsByType<ColyseumEnemyMovements>(FindObjectsSortMode.None);
        if(MatchEndButton != null)
        {
            MatchEndButton.gameObject.SetActive(false);
        }
        
        UpdateColTier();
        if(allenemies==null || allenemies.Length ==0 || !allenemies[0].isZeus || isfinalBattle)
        {
            if (allenemies.Length < currentColyseumtier + 1)
            {
                for (int i = allenemies.Length; i <= currentColyseumtier + 1; i++)
                {
                    GameObject newenemy = Instantiate(EnemyPrefab);
                    allenemies.Append(newenemy.GetComponent<ColyseumEnemyMovements>());
                }
            }
            foreach (ColyseumEnemyMovements enemy in allenemies)
            {
                enemy.GetComponent<BoxCollider2D>().isTrigger = false;
                enemy.HP = 100;
                enemy.currentclip = enemy.maxbulletperclip;
                enemy.transform.position = enemystartpos + new Vector2(UnityEngine.Random.Range(-1f, 2f), UnityEngine.Random.Range(0, 2f));
            }
        }
        else
        {
            allenemies[0].HP = 5000;
            allenemies[0].currentclip = allenemies[0].maxbulletperclip;
            allenemies[0].transform.position = enemystartpos + new Vector2(UnityEngine.Random.Range(-1f, 2f), UnityEngine.Random.Range(0, 2f));
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
        foreach(ColyseumEnemyMovements enemy  in FindObjectsByType<ColyseumEnemyMovements>(FindObjectsSortMode.None))
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
        LifeBar.fillAmount = (float)((float)HP / (float)maxhp);
        CheckIfAllEnemiesdied();
        if(waittingforbattlestart)
        {
            return;
        }
        if (matchover)
        {
            FinishMatch();
            return;
        }


        

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
                delaybetweenbulletscnt = (int)(delaybetweenbullets/ (RollBoulder.instance.numberofBetterGun / 4f + 1f) / Time.fixedDeltaTime);
                SpawnBullet(direction);
            }
        }

        BulletText.text = currentclip+"/"+maxbulletperclip;
    }

    private void ManageTrueEnd()
    {

        if(EndBlackScreen.color.a<1f)
        {
            Color oldcolor = EndBlackScreen.color;
            oldcolor.a += Time.deltaTime/3f;
            EndBlackScreen.color = oldcolor;
        }
        else if(EndText.color.a<1f)
        {
            Color oldcolor = EndText.color;
            oldcolor.a += Time.deltaTime / 3f;
            EndText.color = oldcolor;
            if (HP<=0)
            {
                oldcolor.a -= Time.deltaTime / 6f;
                EndText.text = "Sisyphus was beaten by Zeus, but it was not the end.\nHe can improve and beat him.\nHe will reach Olympus again.";
                if(EndText.color.a >= 1f)
                {
                    transform.parent.gameObject.SetActive(false);
                    RollBoulder.instance.BaseLayer.SetActive(true);
                    Color oldcolorA = EndBlackScreen.color;
                    oldcolorA.a =0f;
                    EndBlackScreen.color = oldcolorA;
                    Color oldcolorB = EndText.color;
                    oldcolorB.a = 0f;
                    EndText.color = oldcolorB;
                    HP = 1;
                }
                
            }
            else
            {
                EndText.text = "And so with Zeus' death Sisyphus was freed from his curse and lived the rest of his days peacefully";
            }

                
        }
        else if (TrueEnd.color.a < 1f)
        {
            Color oldcolor = TrueEnd.color;
            oldcolor.a += Time.deltaTime / 5f;
            TrueEnd.color = oldcolor;
        }
        else if(endcounter< 2f/Time.fixedDeltaTime)
        {
            endcounter++;
        }
        else if(endbutton!=null)
        {
            endbutton.gameObject.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SpawnBullet(Vector2 direction)
    {
        if(currentclip>0)
        {
            currentclip--;

            if(currentclip==0)
            {
                reloadcounter = (int)(reloadtime/(RollBoulder.instance.numberofBetterGun/2f+1f) / Time.fixedDeltaTime);
                lastmaxcounterreloadcounter = reloadcounter;
            }
            MusicPlayer.instance.PlayGunSound();
            GameObject newbullet = Instantiate(BulletPrefab);
            newbullet.transform.SetParent(GunTransform.GetChild(0));
            newbullet.transform.localPosition = BulletSpawnOffset;
            newbullet.transform.parent = null;
            newbullet.GetComponent<BulletScript>().InitializeBullet(gameObject, direction);
        }


    }


    private void FinishMatch()
    {
        if(!isfinalBattle)
        {
            if (!MatchEndButton.gameObject.activeSelf)
            {
                MatchEndButton.gameObject.SetActive(true);
                TextMeshProUGUI buttontmp = MatchEndButton.GetComponentInChildren<TextMeshProUGUI>();
                if (Victory)
                {
                    buttontmp.text = "Victory !\nFavor multiplied by " + (currentColyseumtier+1) * 2;
                }
                else
                {
                    buttontmp.text = "Defeat !\nFavor divided by " + (currentColyseumtier + 1) * 2;
                }
            }
            else if (matchovercounter < 2.5f / Time.fixedDeltaTime)
            {
                matchovercounter++;
            }
            else
            {
                foreach (ColyseumEnemyMovements enemy in FindObjectsByType<ColyseumEnemyMovements>(FindObjectsSortMode.None))
                {
                    Destroy(enemy.gameObject);
                }
                MatchEndButton.onClick.Invoke();
            }
        }
        else
        {
            if(HP>0)
            {
                MusicPlayer.instance.PlayEndMusic();
            }
            else
            {
                MusicPlayer.instance.PlayMapMusic();
            }
            ManageTrueEnd();
        }
    }



    public void CloseColyseumButton()
    {
        distributeColiseumResult(Victory);
    }

    public void distributeColiseumResult(bool iswinner)
    {
        if (iswinner)
        {
            RollBoulder.instance.currentSave.favors *= (currentColyseumtier+1f) * 2;
        }
        else
        {
            RollBoulder.instance.currentSave.favors /= (currentColyseumtier + 1f) * 2;
        }
        RollBoulder.instance.ManageFavorsText();
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
