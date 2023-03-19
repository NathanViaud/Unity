using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    private static Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private bool                m_grounded = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;

    private static string playerState = "inactive";

    private int playerHealth = 100;

    private bool isBouncing = false;

    public GameObject healtText;

    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
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

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0 && !Global.isDead)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
            
        else if (inputX < 0 && !Global.isDead)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (isBouncing == false && playerState != "attacking" && playerState != "hurt" && playerState != "dead" && !Global.isDead)
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        //Death
        if (Input.GetKeyDown("e")) {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
            playerHealth = 0;
            playerState = "dead";
            Global.isDead = true;
        }
            
        //Hurt
        else if (Input.GetKeyDown("a"))
            m_animator.SetTrigger("Hurt");

        //Attack
        else if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            playerState = "inactive";
            // Global.attack();
            playerAttack();

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1))
        {
            playerState = "blocking";
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
        {
            playerState = "inactive";
            m_animator.SetBool("IdleBlock", false);
        }

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon && !Global.isDead)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else if (!Global.isDead)
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }
    }

    void playerAttack()
    {
        playerState = "attacking";
        Invoke("playerAttackEnd", 0.25f);
    }

    void playerAttackEnd()
    {
        playerState = "inactive";
    }

    public static string getPlayerState()
    {
        return playerState;
    }

    public void takeDamage(int damage, int direction)
    {
        if (playerState == "blocking")
        {
            Debug.Log("Attack blocked");
        }
        else
        {
            float bounce = 2f;
            m_body2d.AddForce(transform.right * bounce * direction, ForceMode2D.Impulse);
            m_body2d.AddForce(transform.up * bounce, ForceMode2D.Impulse);
            isBouncing = true;
            Invoke("stopBouncing", 0.5f);
            playerHealth -= damage;
            // globalState.setHealth(playerHealth);
            healtText.GetComponent<Text>().text = "Health: " + playerHealth;
            if(playerHealth <= 0)
            {
                m_animator.SetTrigger("Death");
                Global.isDead = true;
            } else
            {
                m_animator.SetTrigger("Hurt");
            }
        }
    }

    
    void stopBouncing()
    {
        isBouncing = false;
    }
    

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy") {
            if(playerState == "attacking")
                other.gameObject.GetComponent<EnnemyKnight>().takeDamage(25, m_facingDirection);
        }
    }
}
