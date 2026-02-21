    
using UnityEngine;

public class Homing_Projectile : MonoBehaviour
{
    private GameObject[] _enemies;
    private GameObject _target; 
    private float _speed = 15f;

    void Start()
    {
        FindClosestTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if(_target == null)
        {
            FindClosestTarget();
            //transform.Translate(Vector3.up * _speed * Time.deltaTime);
            if(_target == null)
            {
                return;
            }
        }

        Vector2 direction = (_target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 300f * Time.deltaTime);
        transform.position += transform.up * _speed * Time.deltaTime;

    }  
    void FindClosestTarget()
    {
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform closestTarget;
        foreach (var enemy in _enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = enemy.transform;
            }
            _target = enemy;
        }

    }



}
