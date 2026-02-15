using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 3.0f;
    [SerializeField]
    private GameObject _explosion;
    [SerializeField]
    private SpawnManager _spawnManager;
    [SerializeField]
    private AudioClip _explosionAudio;
    private AudioSource _audioSource;

    void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
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
            transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Laser")
            {
                Instantiate(_explosion, transform.position, Quaternion.identity);
                Destroy(other.gameObject);
                _spawnManager.StartSpawning();
                Destroy(this.gameObject, 0.25f);
                _audioSource.Play();
            }
        }
}
