using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    private float _speedMultiplier = 5f;

    //For thrusters
    [SerializeField]
    private float _maxCharge = 20f; // Max charge of thrusters 
    [SerializeField]
    private float _currentCharge = 0; // Charge that changes 
    [SerializeField]
    private float _drainRate = 5f; //drain rate in m/s
    [SerializeField]
    private float _rechargeRate = 5f; //recharge rate in m/s
    [SerializeField]
    private bool _isUsingThrusters = false; // that is whether the Left Shift key is pressed
    [SerializeField]
    private float _coolDownTime = 2f; // Time before recharging starts 
    [SerializeField]
    private float _coolDownTimer = 0; // Timer for cooldown
    [SerializeField]
    private bool _isInCoolDown = false; // if coolDown has started; 

    [SerializeField]
    private float _fireRate = 1f;
    [SerializeField]
    private float _canFire = -1;

    [SerializeField]
    private GameObject _Laser_prefab;

    [SerializeField]
    private int _lives = 3;

    private SpawnManager _spawnManager;

    [SerializeField]    
    private bool _isTripleShotActive = false;


    [SerializeField]
    private bool _isShieldActive = false;


    [SerializeField]
    private GameObject _tripleShot;

    [SerializeField]
    private GameObject _shieldVisualizer;


    [SerializeField]
    private int _score = 0;

    [SerializeField]
    private UIManager _uiManager;


    [SerializeField]
    private GameObject _rightEngine;

    [SerializeField]
    private GameObject _leftEngine;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private AudioClip _LaserSoundClip;
    private AudioSource _audioSource;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
         _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        if (_uiManager == null)
        {
            Debug.LogError("UI manager is NULL");
        }

        if(_audioSource == null)
        {
            Debug.LogError("Audio Sourse is NULL");
        }
        else
        {
            _audioSource.clip = _LaserSoundClip; 
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
        Motion();


    }

    void Motion()
    {
        float _currentSpeed = _speed;

        if (Input.GetKey(KeyCode.LeftShift) && _currentCharge > 0 && _isInCoolDown == false)
        {
            _currentSpeed = _speed * _speedMultiplier;
            _isUsingThrusters = true;
            _currentCharge -= _drainRate * Time.deltaTime;
        }
        else
        {
            _isUsingThrusters = false;
        }

        if (_currentCharge <= 0 && _isInCoolDown == false)
        {
            _currentCharge = 0;
            _isInCoolDown = true;
            _coolDownTimer = _coolDownTime;
        }

        if (_isInCoolDown == true)
        {
            _coolDownTimer -= Time.deltaTime;
            if (_coolDownTimer <= 0)
            {
                _isInCoolDown = false;
            }
        }

        if (_isInCoolDown == false && _isUsingThrusters == false)
        {
            _currentCharge += _rechargeRate * Time.deltaTime;
            if (_currentCharge >= _maxCharge)
            {
                _currentCharge = _maxCharge;
            }
        }
        float chargeDeg = _currentCharge / _maxCharge;
        _uiManager.UpdateCharge(chargeDeg);

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
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * _currentSpeed * Time.deltaTime);
    }



    public void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShot, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_Laser_prefab, transform.position + new Vector3(0f, 0.8f, 0f), Quaternion.identity);
        }
        _audioSource.Play();
    }

    public void Damage()
    {
        if(_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }
        else
        {
            _lives--;

            if(_lives == 2)
            {
                _rightEngine.SetActive(true);
            }
            else if(_lives == 1)
            {
                _leftEngine.SetActive(true);
            }
                _uiManager.UpdateLives(_lives);

            if (_lives < 1)
            {
                _spawnManager.OnPlayerDeath();
                Instantiate(_explosion, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }

    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
    }

    public void TripleShotActive() 
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostRoutine());
    }

    IEnumerator SpeedBoostRoutine()
    {
        yield return new WaitForSeconds(5);
        _speed /= _speedMultiplier;
    }


    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
