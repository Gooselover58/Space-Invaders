using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Missile : Projectile
{
    public GameObject Explosion;
    private void Awake()
    { 
        //Explosion = FindAnyObjectByType(Missile)
        direction = Vector3.down;
    }
   
    void Update()
    {
        transform.position += speed * Time.deltaTime * direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.Instance.PlayCollisionParts(transform.position);
        GameObject explosionInstance = Instantiate(Explosion, transform.position, Quaternion.identity);

        // Destroy the explosion object after 1 second
        Destroy(explosionInstance, 0.5f);
        Destroy(gameObject); //s� fort den krockar med n�got s� ska den f�rsvinna.
    }
   
}
