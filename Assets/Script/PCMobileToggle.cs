using UnityEngine;
using UnityEngine.UI;

// Gắn script này vào GameObject chứa Toggle "PC / Mobile UI"
public class PCMobileToggle : MonoBehaviour
{
    [Header("UI Toggle")] 
    [Tooltip("Toggle PC / Mobile UI (ON = Mobile UI bật, OFF = tắt)")]
    public Toggle pcMobileToggle;

    private void Reset()
    {
        // Tự tìm Toggle trên cùng GameObject nếu để trống
        if (pcMobileToggle == null)
        {
            pcMobileToggle = GetComponent<Toggle>();
        }
    }

    private void Awake()
    {
        if (pcMobileToggle == null)
        {
            pcMobileToggle = GetComponent<Toggle>();
        }
    }

    private void OnEnable()
    {
        if (pcMobileToggle != null)
        {
            pcMobileToggle.onValueChanged.AddListener(OnToggleChanged);

            // Đồng bộ trạng thái toggle với mode hiện tại (nếu có InputModeManager)
            if (InputModeManager.Instance != null)
            {
                bool isMobile = InputModeManager.Instance.IsMobile;
                pcMobileToggle.isOn = isMobile; // ON = Mobile UI bật
            }
        }
    }

    private void OnDisable()
    {
        if (pcMobileToggle != null)
        {
            pcMobileToggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        // ON = bật Mobile UI, OFF = tắt Mobile UI
        if (InputModeManager.Instance == null)
            return;

        if (isOn)
        {
            InputModeManager.Instance.SetMobileMode();
        }
        else
        {
            InputModeManager.Instance.SetPCMode();
        }
    }
}
