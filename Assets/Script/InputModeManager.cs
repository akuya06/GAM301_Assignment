using UnityEngine;

public class InputModeManager : MonoBehaviour
{
    public enum InputMode
    {
        PC,
        Mobile
    }

    public static InputModeManager Instance { get; private set; }

    [Header("Current Mode")]
    [SerializeField] private InputMode currentMode = InputMode.PC;

    [Header("UI Roots")]
    [Tooltip("Parent GameObject chứa toàn bộ UI Mobile: nút bắn, nhảy, ném bom, joystick...")]
    [SerializeField] private GameObject mobileUIRoot;

    [Tooltip("(Tuỳ chọn) UI riêng cho PC nếu có")] 
    [SerializeField] private GameObject pcUIRoot;

    public InputMode CurrentMode => currentMode;
    public bool IsMobile => currentMode == InputMode.Mobile;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ApplyMode(currentMode);
    }

    public void SetMode(InputMode mode)
    {
        currentMode = mode;
        ApplyMode(mode);
    }

    public void SetPCMode()
    {
        SetMode(InputMode.PC);
    }

    public void SetMobileMode()
    {
        SetMode(InputMode.Mobile);
    }

    private void ApplyMode(InputMode mode)
    {
        if (mobileUIRoot != null)
        {
            mobileUIRoot.SetActive(mode == InputMode.Mobile);
        }

        if (pcUIRoot != null)
        {
            pcUIRoot.SetActive(mode == InputMode.PC);
        }

        Debug.Log($"Switched input mode to: {mode}");
    }
}
