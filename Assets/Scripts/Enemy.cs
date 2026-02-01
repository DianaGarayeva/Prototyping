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


    //For firing
    [SerializeField]
    private GameObject _laserPrefab;
    private float _fireRate = 1.0f;
    private float _canFire = 0;

    //For types of movement
    private float _angle;
    private float _amplitude = 2f;
    private float _frequency = 2f;
    private float _startX;
    private float _angleForCircularMotion = 360;
    private float _radius = 7f;
    private Vector3 center = new Vector3(0, 0, 0);
    private bool _isAlive;

    //Choosing type of movement
    private enum TypesOfMovement
    {
        Straight,
        Angular,
        SideToSide,
        Circling
    }
    private TypesOfMovement movementType;
    
    //Enemy shield
    [SerializeField]
    private GameObject _shield;
    private bool _isShieldActive;


    void Start()
    {
        _isAlive = true;

        _startX = transform.position.x;
        _angle = Random.Range(-45f, 45f);

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("The player is NULL");
        }

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("The animator is NULL");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Audio Source is NULL");
        }
        else
        {
            _audioSource.clip = _explosionAudio;
        }

        float chance = Random.Range(0f, 1f);
        //Debug.Log(chance);
        if (chance <= 0.5)
        {
            _shield.SetActive(true);
            _isShieldActive = true;
        }
        else
        {
            _shield.SetActive(false);
            _isShieldActive = false;
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

    // Fire for Enemy
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
            //Shield for enemy
            if(_isShieldActive == true)
            {
                _shield.SetActive(false);
                _isShieldActive = false;
                return;
            }
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
