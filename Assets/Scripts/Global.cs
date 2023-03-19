using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour
{
    private GameObject player = null;
    public static bool isDead = false;
    public static bool win = false;

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
        
        if(player.transform.position.x >= 40) {
            win = true;
        }

        if(isDead)
        {
            Debug.Log("You died");
            Invoke("returnToMenu", 0.5f);

        }

        if(win) {
            Debug.Log("You win");
            Invoke("returnToMenu", 0.5f);

        }
    }

    public void returnToMenu()
    {
        SceneManager.LoadScene(1);
    }
}
