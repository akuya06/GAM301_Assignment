using UnityEngine;
using UnityEngine.EventSystems;

// Gắn script này vào GameObject nút bắn Mobile (Image/Button)
public class MobileFireButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Weapon weapon;

    private void Reset()
    {
        if (weapon == null)
        {
            weapon = FindObjectOfType<Weapon>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (weapon != null)
        {
            weapon.OnMobileFireButtonDown();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (weapon != null)
        {
            weapon.OnMobileFireButtonUp();
        }
    }
}
