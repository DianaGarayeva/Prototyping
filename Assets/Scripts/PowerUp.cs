using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField]
    private int _powerupID;
    [SerializeField]
    private AudioClip _clip;
    private GameObject player;
    private Transform target;
    private float rotationSpeed = 360f;


    private void Start()
    {
        player = GameObject.Find("Player");
        target = player.transform;

    }
    private void Update()
    {
      
        if (Input.GetKey(KeyCode.C))
        {
            Vector2 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, target.position, _speed * 3 * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            if (transform.position.y < -6f)
            {
                Destroy(this.gameObject);
            }
        }
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            AudioSource.PlayClipAtPoint(_clip, transform.position);
            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.Ammo();
                        break;
                    case 4:
                        player.HealthPowerUp();
                        break;
                    case 5:
                        player.SpecialShotActive();
                        break;
                    case 6:
                        player.NegativePowerUpActive();
                        break;  
                    default:
                        Debug.Log("Default");
                        break;

                }
                 
            }
            Destroy(this.gameObject);
        }
    }

}
