using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;

    [SerializeField]
    private Player _player;

    private Animator _anim;
    [SerializeField]

    private AudioClip _explosionAudio;

    private AudioSource _audioSource;

    [SerializeField]
    private GameObject _laserPrefab;

    private float _fireRate = 1.0f;
    private float _canFire = 0;


    private float _angle;
    private float _amplitude = 2f;
    private float _frequency = 2f;
    private float _startX;
    private float _angleForCircularMotion = 360;
    private float _radius = 7f;
    private Vector3 center = new Vector3(0, 0, 0);
    private bool _isAlive;

    private enum TypesOfMovement
    {
        Straight,
        Angular,
        SideToSide,
        Circling
    }
    private TypesOfMovement movementType;

    void Start()
    {
        _startX = transform.position.x;
        _angle = Random.Range(-45f, 45f);
        _isAlive = true;
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        if (_anim == null)
        {
            Debug.LogError("The animator is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Audio Source is NULL");
        }
        else
        {
            _audioSource.clip = _explosionAudio;
        }

        movementType = (TypesOfMovement)Random.Range(0, 4);
    }

    void Update()
    {
        switch (movementType) 
        {
            case TypesOfMovement.Angular:
                Angular();
                break;
            case TypesOfMovement.Circling:
                Circling();
                break;
            case TypesOfMovement.SideToSide:
                SideToSide();
                break;
            case TypesOfMovement.Straight:
                Straight();
                break;
            default:
                Debug.Log("Default");
                break;
        }

        if (Time.time > _canFire)
        {
            FireLaser();
        }

    }


    //Type of movements
    void Straight()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -5)
        {
            transform.position = new Vector3(Random.Range(-8f, 8f), 8f, 0);
        }
    }

    void SideToSide()
    {
        float x = _startX + Mathf.Sin(_frequency * Time.time) * _amplitude;
        float y = transform.position.y - _speed * Time.deltaTime;
        transform.position = new Vector3(x, y, 0);
        if (transform.position.y < -5)
        {
            transform.position = new Vector3(Random.Range(-8f, 8f), 8f, 0);
        }
    }

    void Circling()
    {
        _angleForCircularMotion += _speed * Time.deltaTime;
        float x = center.x + Mathf.Cos(_angleForCircularMotion) * _radius;
        float y = center.y + Mathf.Sin(_angleForCircularMotion) * _radius;
        transform.position = new Vector3(x, y, 0);
        if (transform.position.y < -5)
        {
            transform.position = new Vector3(Random.Range(-8f, 8f), 8f, 0);
        }
    }

    void Angular()
    {
        float radian = Mathf.Deg2Rad * _angle;
        float y = -Mathf.Cos(radian);
        float x = Mathf.Sin(radian);
        Vector3 direction = new Vector3(x, y, 0);
        transform.Translate(direction * Time.deltaTime * _speed);
        if (transform.position.y < -5)
        {
            transform.position = new Vector3(Random.Range(-8f, 8f), 8f, 0);
        }
    }

    //Choose type of movement


    private void FireLaser()
    {
        if(_isAlive == true)
        {
            _fireRate = Random.Range(3.0f, 7.0f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            Destroying();
            _isAlive = false;
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(10);
            }

            Destroying();
            _isAlive = false;

        }

    }

    private void Destroying()
    {
        _anim.SetTrigger("OnEnemyDeath");
        _speed = 0f;
        Destroy(this.gameObject, 2.6f);
        _audioSource.Play();
        Destroy(GetComponent<Collider2D>());
    }
}
