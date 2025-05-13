using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            print("hit " + collision.gameObject.name + " !");
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            print("hit a wall and boom");
            Destroy(gameObject);
        }
    }

}
