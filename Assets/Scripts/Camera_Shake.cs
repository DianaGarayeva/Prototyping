using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Shake : MonoBehaviour
{
    public float duration = 0.2f;    // как долго трясется
    public float magnitude = 0.1f;   // сила тряски

    private Vector3 initialPosition;
    private float shakeTime = 0f;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (shakeTime > 0)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * magnitude;
            transform.localPosition = initialPosition + new Vector3(shakeOffset.x, shakeOffset.y, 0);
            shakeTime -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = initialPosition;
        }
    }

    public void Shake()
    {
        shakeTime = duration;
    }
}
