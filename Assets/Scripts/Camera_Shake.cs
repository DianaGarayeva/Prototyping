
using UnityEngine;

public class Camera_Shake : MonoBehaviour
{
    public float _duration = 0.2f;   
    public float _magnitude = 0.1f;   
    private Vector3 _initialPosition;
    private float _shakeTime = 0f;

    void Start()
    {
        _initialPosition = transform.position;
    }

    void Update()
    {
        if (_shakeTime > 0)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * _magnitude;
            transform.position = _initialPosition + new Vector3(shakeOffset.x, shakeOffset.y, 0);
            _shakeTime -= Time.deltaTime;
        }
        else
        {
            transform.position = _initialPosition;
        }   
    }

    public void Shake()
    {
        _shakeTime = _duration;
    }
}
