using UnityEngine;

public class MouseFollowWorld : MonoBehaviour
{
    [SerializeField] private Camera cam;           
    [SerializeField] private float minX = -20f;       
    [SerializeField] private float maxX = 20f;        
    [SerializeField] private float fixedY = -3f;     
    [SerializeField] private float speed = 5f;

    [Header("Optional: Animator für Lauf-Animation")]
    [SerializeField] private Animator animator; 

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        float targetX = Mathf.Clamp(mouseWorld.x, minX, maxX);

        float diff = targetX - transform.position.x;

        // Sprite in Laufrichtung spiegeln
        if (Mathf.Abs(diff) > 0.05f)
        {
            sr.flipX = diff < 0;
            if (animator != null) animator.SetBool("IsMoving", true);
        }
        else if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }

        float newX = Mathf.MoveTowards(transform.position.x, targetX, speed * Time.deltaTime);
        transform.position = new Vector3(newX, fixedY, transform.position.z);
    }
}