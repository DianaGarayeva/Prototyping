using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    [SerializeField]
    private float _fireRate = 0.5f;
    [SerializeField]
    private float _canFire = -1;

    [SerializeField]
    private GameObject _Laser_prefab;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            Instantiate(_Laser_prefab, transform.position+new Vector3(0f, 0.8f, 0f), Quaternion.identity);
        }
        Motion();
    }

    void Motion()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * _speed * Time.deltaTime);
        if (transform.position.y > 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y < -3f)
        {
            transform.position = new Vector3(transform.position.x, -3f, 0);

        }

        if (transform.position.x > 11.0f)
        {
            transform.position = new Vector3(-11.0f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.0f)
        {
            transform.position = new Vector3(11.0f, transform.position.y, 0);

        }

    }
}
