using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("UI References")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float handleLimit = 1.0f; // 1 = edge of bg radius

    public Vector2 Direction { get; private set; }

    private Canvas canvas;
    private Camera uiCamera;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            uiCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
        }

        if (background == null)
        {
            background = transform as RectTransform;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (background == null || handle == null)
            return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, uiCamera, out localPoint))
        {
            Vector2 radius = background.sizeDelta / 2f;
            Vector2 normalized = new Vector2(localPoint.x / radius.x, localPoint.y / radius.y);

            Vector2 clamped = Vector2.ClampMagnitude(normalized, 1f);
            Direction = clamped;

            handle.anchoredPosition = clamped * radius * handleLimit;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Direction = Vector2.zero;
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }
}
