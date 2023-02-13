using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyInteractions : MonoBehaviour
{
    public float movementSpeed = 3f;
    public int ennemyHealth = 100;
    public int damage = 10;
    public int step = 0;
    public static bool isBouncing = false;

    private GameObject player = null;

    void Start()
    {
        player = GameObject.Find("HeroKnight");
    }

    void Update()
    {
        if(step > 1000) {
            step = 0;
            this.movementSpeed = -movementSpeed;
        }
        step++;
        transform.Translate(Vector2.right * movementSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(HeroKnight.getPlayerState() == "blocking") {
                Debug.Log("blocked");
            } else {
                Debug.Log("emotional damage");
                float bounce = 7f;
                int direction = movementSpeed > 0 ? 1 : -1;
                player.GetComponent<Rigidbody2D>().AddForce(transform.right * bounce * direction, ForceMode2D.Impulse);
                player.GetComponent<Rigidbody2D>().AddForce(transform.up * bounce, ForceMode2D.Impulse);
                isBouncing = true;
                Invoke("stopBouncing", 0.5f);

                HeroKnight.takeDamage(damage);
            }
        }
    }

    void stopBouncing() {
        isBouncing = false;
    }
}
