using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTextRotate : MonoBehaviour
{
    private Quaternion startRot;
    private Quaternion endRot;
    private float time;
    private float currentZ;
    [SerializeField] float rotateTime;

    private void Awake()
    {
        startRot = transform.rotation;
        currentZ = -30f;
        endRot = Quaternion.Euler(0, 0, currentZ);
        time = 0.5f;
    }

    private void Update()
    {
        time += Time.deltaTime / rotateTime;
        transform.rotation = Quaternion.Slerp(startRot, endRot, time);
        if (time >= 1f)
        {
            startRot = transform.rotation;
            currentZ = -currentZ;
            endRot = Quaternion.Euler(0, 0, currentZ);
            time = 0f;
        }
    }
}
