using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Missile : Projectile
{
    GameObject Explosion;
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
        Destroy(gameObject); //så fort den krockar med något så ska den försvinna.
    }
   
}
