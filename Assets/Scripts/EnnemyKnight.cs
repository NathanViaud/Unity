using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnnemyKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 2.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    [SerializeField] Animator m_animator;


    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private bool                m_grounded = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;

    private static string playerState = "inactive";

    private static int ennemyHealth = 25;

    private bool isDead = false;
    private bool isBouncing = false;

    private int step = 0;
    private float inputX = 1;

    // Use this for initialization
    void Start ()
    {
        // m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update ()
    {
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle input and movement --
        // float inputX = Input.GetAxis("Horizontal");
        // float inputX = 1;

        this.step ++;
        // Debug.Log("step: " + this.step);

        if (this.step > 1000) {
            this.step = 0;
            inputX= -inputX;
        }

        // Swap direction of sprite depending on walk direction
        if (inputX > 0 && ennemyHealth > 0 && !isDead)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
            
        else if (inputX < 0 && !isDead)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (isBouncing == false && playerState != "attacking" && playerState != "hurt" && !isDead && m_grounded)
        {
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        }

        //Run
        if (Mathf.Abs(inputX) > Mathf.Epsilon && !isDead)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else if (!isDead)
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }
    }

    public void takeDamage(int damage, int direction)
    {
        float bounce = 1f;
        m_body2d.AddForce(transform.right * bounce * direction, ForceMode2D.Impulse);
        m_body2d.AddForce(transform.up * bounce, ForceMode2D.Impulse);
        isBouncing = true;
        Invoke("stopBouncing", 0.5f);
        ennemyHealth -= damage;
        if(ennemyHealth <= 0 && !isDead)
        {
            m_animator.SetTrigger("Death");
            Invoke("killEnnemy", 0.25f);
        } else
        {
            m_animator.SetTrigger("Hurt");
        }
    }

    void killEnnemy()
    {
        isDead = true;
        Debug.Log("Ennemy killed");
        Invoke("destroyEnnemy", 2f);
    }

    void destroyEnnemy()
    {
        Destroy(gameObject);
    }

    void stopBouncing()
    {
        isBouncing = false;
    }

    float attackTimer = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" && m_grounded && !isDead) {
            attackTimer = 0;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" && m_grounded && !isDead && m_timeSinceAttack >= 0.5f) {
            attackTimer += Time.deltaTime;
            if(attackTimer > 2 && m_grounded && !isDead && m_timeSinceAttack >= 0.5f) {
                attackTimer = 0;
                m_timeSinceAttack = 0;
                m_animator.SetTrigger("Attack1");
                other.gameObject.GetComponent<HeroKnight>().takeDamage(25, m_facingDirection);
            }
        }
    }
}
