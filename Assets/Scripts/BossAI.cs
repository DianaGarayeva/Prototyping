using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    private Vector3 _initialPosition = new Vector3(0, 8, 0);
    private Vector3 _position = new Vector3(0, 4, 0);
    private float _speed = 3f;
    private int _maxHealth = 20;
    private int _currentHealth;
    private UIManager _ui;
    private Transform _target;
    private float _rotationSpeed = 180f;
    private float _rotationRate = 3f;
    private float _rotate = 1f;     
    private float _fireRate = 3f;
    private float _canFire = 1f;
    private Quaternion _targetRotation;
    private bool _start = false;
    private int shots = 0; 
    [SerializeField]
    private GameObject _laserPrefab;
    private Animator _anim;
    [SerializeField]
    private AudioClip _explosionAudio;
    private AudioSource _audioSource;
    private SpawnManager _spawnManager;
    private bool _canShoot = true;
    private GameManager _gameManager;
    private Player _player;
    public void Start()
    {
        transform.position = _initialPosition;
        _currentHealth = _maxHealth;
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
            Debug.Log("player is NULL");

        _ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_ui == null)
        {
            Debug.LogError("UI manager is NULL");
        }
        _target = GameObject.Find("Player").GetComponent<Transform>();
        if(_target == null)
        {
            Debug.LogError("Target is NULL");
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

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
    }

    public void Update()    
    {
        if (!_start)
        {
            transform.position = Vector3.MoveTowards(transform.position, _position, _speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _position) < 0.01f)
            {
                _start = true;
            }

            return;
        }

        if (Time.time >= _rotate)
        {
            _rotate = Time.time + _rotationRate;

            Vector2 direction = _target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 270f;

            _targetRotation = Quaternion.Euler(0, 0, angle);
        }

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            _targetRotation,
            _rotationSpeed * Time.deltaTime
        );

        if (Time.time >= _canFire && _canShoot)
        {
            _canFire = Time.time + _fireRate;
            if (shots % 5 == 0)
            {
                ConsistentAttack();
            }
        }
        if(_currentHealth<= 0)
        {
            OnBossDeath();
            _spawnManager.OnPlayerVictory();
            _gameManager.GameOver();
            _ui.VictorySequence();
        }
    }
    private void ConsistentAttack()
    {
        GameObject laser = Instantiate(_laserPrefab, transform.position - transform.up * 2f, transform.rotation);
        Laser[] lasers = laser.GetComponentsInChildren<Laser>();
        foreach (var l in lasers)
        {
            l.AssignEnemyLaser();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.tag == "Laser")
        {
            _currentHealth--;
            float lifePercentage =(float) _currentHealth / _maxHealth;
            Destroy(other.gameObject);
            _ui.UpdateBossLives(lifePercentage);
            _player.AddScore(15);
        }
    }

    void OnBossDeath()
    {
        _anim.SetTrigger("OnEnemyDeath");
        Destroy(this.gameObject, 2.6f);
        Destroy(GetComponent<Collider2D>());
        _audioSource.Play();
        _spawnManager.OnPlayerVictory();
        _canShoot = false;
    }

}


    // Bigger + 
    // (Opt) Zoom out
    // Doesn't move+
    // 20 hp -> Health bar +
    // if hp 10 -> more angry 
    // Consistent attacks +


