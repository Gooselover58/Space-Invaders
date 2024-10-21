using System.Collections;
using System.Collections.Generic;
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            FireLaser(1f);
        }
    }

    private void FireLaser(float intensity)
    {
        laser.positionCount = 2;
        laser.widthMultiplier = 1;
        laser.SetPosition(0, shootPoint.position);
        LayerMask bounds = LayerMask.GetMask("Boundary");
        RaycastHit2D ray = Physics2D.Raycast(shootPoint.position, -dir.normalized, Mathf.Infinity, bounds);
        if (ray.collider != null)
        {
            laser.SetPosition(1, ray.point);
        }
    }
}
