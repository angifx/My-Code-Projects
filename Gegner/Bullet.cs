using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public int bulletDamage = 1; 

    private Vector2 direction; 

    public void SetDirection(Vector2 dir) 
    {
        direction = dir;
        Destroy(gameObject, lifeTime); 
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") return;

        GegnerMovement gegner = collision.gameObject.GetComponent<GegnerMovement>();
        if (gegner)
        {
            gegner.TakeDamage(bulletDamage);

        }
        BossControllerr Boss = collision.gameObject.GetComponent<BossControllerr>();
        if (Boss)
        {
            Boss.TakeDamage(bulletDamage);

        }

        Destroy(gameObject);
    }
}