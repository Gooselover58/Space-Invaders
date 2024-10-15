using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    public GameObject laserPrefab;
    GameObject laser;
    float speed = 5f;
    private bool isDying;
    private BoxCollider2D col;
    private ParticleSystem deathPart;
    private float time;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
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
            }
        }
        else
        {
            float y = -1;
            position.y += y * Time.deltaTime;
            
        }
        transform.position = position;
    }

    private IEnumerator DeathSequence()
    {
        col.enabled = false;
        isDying = true;
        transform.localRotation = Quaternion.Euler(0, 0, 45);
        deathPart.Play();
        yield return new WaitForSeconds(3);
        GameManager.Instance.OnPlayerKilled(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Missile") || collision.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            GameManager.Instance.ChangeLives(1);
            StartCoroutine(DeathSequence());
        }
    }
}
