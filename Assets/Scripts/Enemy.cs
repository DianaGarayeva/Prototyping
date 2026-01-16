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
    
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        if (_anim == null)
        {
            Debug.LogError("The animator is NULL");
        }

        if (_audioSource==null)
        {
            Debug.LogError("Audio Source is NULL");
        }
        else
        {
            _audioSource.clip = _explosionAudio;
        }
    }


    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire)
        {
            FireLaser();
        }

    }

    private void FireLaser()
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

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -5)
        {
            transform.position = new Vector3(Random.Range(-8f, 8f), 8f, 0);
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
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(10);
            }

            Destroying();

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
