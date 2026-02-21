using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetection : MonoBehaviour
{
    private Enemy _enemy;
    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Laser")
        {
            _enemy.Dodging(); 
        }
    }

}
