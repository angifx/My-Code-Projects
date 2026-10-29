using UnityEngine;

public class MouseFollowUI : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRect;  
    [SerializeField] private RectTransform platform;   
    [SerializeField] private float speed = 500f;
    [SerializeField] private float edgePadding = 20f;

    [Header("Optional: Animator für Lauf-Animation")]
    [SerializeField] private Animator animator;

    private RectTransform rect;
    private float minX, maxX;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        float halfWidth = platform.rect.width / 2f;
        minX = -halfWidth + edgePadding;
        maxX = halfWidth - edgePadding;
    }

    void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, Input.mousePosition, null, out Vector2 localPoint);

        float targetX = Mathf.Clamp(localPoint.x, minX, maxX);

        Vector2 pos = rect.anchoredPosition;
        float diff = targetX - pos.x;

        if (Mathf.Abs(diff) > 1f)
        {
            Vector3 s = rect.localScale;
            s.x = diff < 0 ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
            rect.localScale = s;

            if (animator != null) animator.SetBool("IsMoving", true);
        }
        else if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }

        pos.x = Mathf.MoveTowards(pos.x, targetX, speed * Time.deltaTime);
        rect.anchoredPosition = pos;
    }
}