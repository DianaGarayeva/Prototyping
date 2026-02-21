using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _Speed = 8.0f;
    private bool _isEnemyLaser = false;
    private bool _isEnemyLaserMovesUp = false;
    void Update()
    {
        if (_isEnemyLaser == false)
        {
            MoveUp();
        }
        else if(_isEnemyLaser && _isEnemyLaserMovesUp)
        {
            MoveUp();
        }else if(_isEnemyLaser && !_isEnemyLaserMovesUp)
        {
            MoveDown();
        }
    }
    void MoveUp()
    {
        transform.Translate(Vector3.up * _Speed * Time.deltaTime);
        if (transform.position.y > 11f || transform.position.y < -11f || transform.position.x>10f || transform.position.x<-10f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
    void MoveDown()
    {
        transform.Translate(Vector3.down * _Speed * Time.deltaTime);
        if (transform.position.y < -11f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }
    public void EnemyLaserMovesUp()
    {
        _isEnemyLaserMovesUp = true;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && _isEnemyLaser==true)
        {
            Player player = collision.GetComponent<Player>();
            if(player != null)
            {
                player.Damage();
            }
            Destroy(this.gameObject);
        }else if(collision.tag == "PowerUp")
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }
}
