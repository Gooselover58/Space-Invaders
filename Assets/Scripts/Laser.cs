using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Laser : Projectile
{
    private bool isOnBeat;

    private void Awake()
    {
        speed = 12f;
        isOnBeat = false;
        direction = Vector3.up;
        if (GameManager.Instance.onBeat)
        {
            GetComponent<SpriteRenderer>().color = Color.yellow;
            transform.localScale *= 1.5f;
            speed *= 1.5f;
            GameManager.Instance.ScreenThrust(-2, 0.1f);
            isOnBeat = true;
        }
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollision(collision);
    }

    void CheckCollision(Collider2D collision)
    {
        Bunker bunker = collision.gameObject.GetComponent<Bunker>();

        if (collision.gameObject.layer == LayerMask.NameToLayer("Invader") && isOnBeat)
        {

        }
        else if(bunker == null) //Om det inte är en bunker vi träffat så ska skottet försvinna.
        {
            if (collision.gameObject.layer != LayerMask.NameToLayer("Invader"))
            {
                GameManager.Instance.PlayCollisionParts(transform.position);
            }
            Destroy(gameObject);
        }
    }
}
