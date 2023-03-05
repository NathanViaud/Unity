using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onTriggerEnter2D(Collider2D other)
    {
        Debug.Log("in attack range");
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("in attack range collision");
    }
}
