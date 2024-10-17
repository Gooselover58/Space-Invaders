using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    public GameObject laserPrefab;
    Animator animator;
    GameObject laser;
    float speed = 5f;
    private bool isDying;
    private SpriteRenderer sr;
    private BoxCollider2D col;
    private ParticleSystem deathPart;
    private float time;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        isDying = false;
        deathPart = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        time += Time.deltaTime;
        if (!isDying)
        {
            float x = Input.GetAxisRaw("Horizontal");
            Quaternion rot = Quaternion.Euler(0, 0, 0 * 30f);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, time);
            position.x += x * speed * Time.deltaTime;

            Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
            Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

            position.x = Mathf.Clamp(position.x, leftEdge.x, rightEdge.x);


            if (Input.GetKeyDown(KeyCode.Space) && laser == null)
            {
                
                GameManager.Instance.pitch = Random.Range(0.8f, 1.1f);
                GameManager.Instance.PlayShootSound();
                laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
                //animator.SetTrigger("Shoot");
            }
            animator.SetBool("MoveLeft", Input.GetKey(KeyCode.A));
            animator.SetBool("MoveRight", Input.GetKey(KeyCode.D));
        }
        else
        {
            float y = -1;
            position.y += y * Time.deltaTime;
            
        }
        transform.position = position;
    }

    private IEnumerator invicFrames()
    {
        col.enabled = false;
        bool state = false;
        for (int i = 0; i < 20; i++)
        {
            sr.enabled = state;
            state = !state;
            yield return new WaitForSeconds(0.1f);
        }
        col.enabled = true;
    }

    private IEnumerator DeathSequence()
    {
        col.enabled = false;
        isDying = true;
        transform.localRotation = Quaternion.Euler(0, 0, 45);
        deathPart.Play();
        yield return new WaitForSeconds(3);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        deathPart.Stop();
        col.enabled = true;
        isDying = false;
        GameManager.Instance.OnPlayerKilled(this);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Missile") || collision.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            GameManager.Instance.ChangeLives(1);
            if (GameManager.Instance.playerDead)
            {
                StartCoroutine(DeathSequence());
            }
            else
            {
                StartCoroutine(invicFrames());
            }
        }
    }
}
