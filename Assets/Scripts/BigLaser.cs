using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BigLaser : MonoBehaviour
{
    private Camera cam;
    private Rigidbody2D rb;
    private Transform shootPoint;
    private LineRenderer laser;
    private Vector2 dir;

    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        shootPoint = transform.GetChild(1);
        laser = transform.GetChild(2).GetComponent<LineRenderer>();
        gameObject.SetActive(true);
    }

    private void Update()
    {
        Vector2 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        dir = rb.position - mouse;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
        rb.rotation = angle;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireLaser(1f);
        }
    }

    private void FireLaser(float intensity)
    {
        laser.positionCount = 5;
        laser.widthMultiplier = 1f;
        laser.SetPosition(0, shootPoint.position);
        LayerMask bounds = LayerMask.GetMask("Boundary");
        LayerMask enemies = LayerMask.GetMask("Invader");
        Vector2 rayDir = -dir.normalized;
        for (int i = 0; i < 4; i++)
        {
            RaycastHit2D ray = Physics2D.Raycast(shootPoint.position, rayDir, Mathf.Infinity, bounds);
            if (ray.collider != null)
            {
                RaycastHit2D[] enemiesHit = Physics2D.CircleCastAll(shootPoint.position, 1f, rayDir, Mathf.Infinity, enemies);
                Debug.Log(rayDir);
                foreach (RaycastHit2D hit in enemiesHit)
                {
                    if (hit.collider != null && hit.collider.GetComponent<Invader>() != null)
                    {
                        GameManager.Instance.OnInvaderKilled(hit.collider.GetComponent<Invader>());
                        GameManager.Instance.ChangeScore(5);
                    }
                }
                laser.SetPosition(i + 1, ray.point);
                Vector2 newDir = Vector2.Reflect(rayDir, ray.normal);
                Debug.Log(newDir);
                rayDir = newDir;
            }
        }
        GameManager.Instance.pitch = Random.Range(0.8f, 1.1f);
        GameManager.Instance.scoreMult = 1f;
        GameManager.Instance.UpdateScoreUI();
    }
}
