using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _Speed = 8.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _Speed * Time.deltaTime);

        if(transform.position.y > 11f)
        {
            Destroy(this.gameObject);
        }
    }
}
