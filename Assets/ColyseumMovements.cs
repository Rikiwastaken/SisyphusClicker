using UnityEngine;
using UnityEngine.InputSystem;

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

    private void OnEnable()
    {
        HP = 100;
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
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>(); 
        if(touchingground && moveInput.y>0)
        {
            rb.AddForceY(moveforce,ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
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
            if(SR.flipX)
            {
                SR.flipX = false;
            }
            if (!animator.GetBool("Walking"))
            {
                animator.SetBool("Walking", true);
            }
        }
        else if(rb.linearVelocityX < -0.01f)
        {
            if (!SR.flipX)
            {
                SR.flipX = true;
            }
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
        if(transform.localPosition.x<-8.5f)
        {
            transform.localPosition = new Vector3(-8.5f, transform.localPosition.y, transform.localPosition.z);
        }
        if (transform.localPosition.x > 8.5f)
        {
            transform.localPosition = new Vector3(8.5f, transform.localPosition.y, transform.localPosition.z);
        }
    }
}
