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

    private SpawnManager _spawnManager;

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
    private GameObject _laserPrefab;

    [SerializeField]
    private int _lives = 3;


    [SerializeField]    
    private bool _isTripleShotActive = false;
    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _tripleShot;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private int _maxShieldStrength = 4;
    private int _currentShieldStrength;
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

    //Ammo
    private int _maxShots = 15;
    private int _currentShot;

    //Special shot power-up
    private bool _isSpecialShotActive = false;

    //Negative Power-up
    private bool _isNegativePowerUpActive = false;

    //Camera
    private Camera_Shake cameraShake;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);

         _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI manager is NULL");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Audio Sourse is NULL");
        }
        else
        {
            _audioSource.clip = _LaserSoundClip; 
        }

        cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera_Shake>();
        _currentShot = _maxShots;

    }



    //Update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
        Movement();


    }

    //Movement
    void Movement()
    {
        float _currentSpeed = _speed;

        if (_isNegativePowerUpActive == true)
        {
            _currentSpeed = 0;
        }
        //else
        //{
        //    _currentSpeed = _speed;
        //}

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

    //Shooting 
    public void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            _currentShot--;
            Instantiate(_tripleShot, transform.position, Quaternion.identity);
        }else if (_isSpecialShotActive == true)
        {
            _currentShot--;
            Vector3 angle = new Vector3(0, 0, 15f);
            for(int i = 0; i < 24; i++)
            {
                Instantiate(_laserPrefab, transform.position, Quaternion.Euler(angle*i));
            }

        }
        else
        {
            _currentShot--;
            Instantiate(_laserPrefab, transform.position + new Vector3(0f, 0.8f, 0f), Quaternion.identity);
        }
        _uiManager.UpdateAmmo(_currentShot);
        _audioSource.Play();
    }

    //Damage
    public void Damage()
    {
        if (_isShieldActive == true && _currentShieldStrength > 0)
        {
            _currentShieldStrength--;
            float shieldDeg = (float)_currentShieldStrength / _maxShieldStrength;
            _uiManager.UpdateShield(shieldDeg);

            if (_isShieldActive && _currentShieldStrength <= 0)
            {
                _uiManager.UpdateShield(0);
                _isShieldActive = false;
                _shieldVisualizer.SetActive(false);
            }

            return;
        }

        else
        {
            _lives--;

            CheckLives();
            _uiManager.UpdateLives(_lives);

            if (_lives < 1)
            {
                _spawnManager.OnPlayerDeath();
                Instantiate(_explosion, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }
        cameraShake.Shake();

    }

    private void CheckLives()
    {
        if (_lives == 3)
        {
            _rightEngine.SetActive(false);
            _leftEngine.SetActive(false);

        }
        if (_lives == 2)
        {
            _leftEngine.SetActive(false);
            _rightEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
            _leftEngine.SetActive(true);
        }
    }

    //Power-ups
    public void HealthPowerUp()
    {
        if (_lives < 3)
        {
            _lives++;
            CheckLives();
            _uiManager.UpdateLives(_lives);
        }
    }

    public void Ammo()
    {
        _currentShot = _maxShots;
        _uiManager.UpdateAmmo(_currentShot);
    }

    public void ShieldActive()
    {
        _currentShieldStrength = _maxShieldStrength;
        _uiManager.UpdateShield(1);
        Debug.Log(_currentShieldStrength);
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
    }

    public void SpecialShotActive()
    {
        _isSpecialShotActive = true;
        StartCoroutine(SpecialShotRoutine());
    }

    IEnumerator SpecialShotRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isSpecialShotActive = false;
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

    public void NegativePowerUpActive()
    {
        _isNegativePowerUpActive = true;
        StartCoroutine(NegativePowerUpRoutine());
    }
    IEnumerator NegativePowerUpRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isNegativePowerUpActive = false;
    }
    //Add score 
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
