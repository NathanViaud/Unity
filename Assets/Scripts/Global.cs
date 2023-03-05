using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Global : MonoBehaviour
{
    private GameObject player = null;
    public static bool isDead = false;
    public GameObject healthBar;

    void Start()
    {
        player = GameObject.Find("HeroKnight");
    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.y < -10)
        {
            isDead = true;
        }

        if(isDead)
        {
            Debug.Log("You died");
        }
    }

    // public void attack() {
    //     Debug.Log(player.GetComponent<HeroKnight>().transform.position);
    // }

    // public void setHealth(int health)
    // {
    //     healthBar.GetComponent<Text>().text = "Health: " + health;
    // }

}
