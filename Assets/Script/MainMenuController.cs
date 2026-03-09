using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;
    private VisualElement mainMenuBackground;
    
    private Button newGameButton;
    private Button continueButton;
    private Button settingsButton;
    private Button quitButton;
    
    [SerializeField] private string gameSceneName = "GameScene"; // Tên scene game của bạn

    [Header("Background Sprites")]
    [SerializeField] private Sprite mainMenuBackgroundSprite;

    [Header("Pause Button Sprites")]
    [SerializeField] private Sprite pauseButtonNormalSprite;
    [SerializeField] private Sprite pauseButtonHoverSprite;
    [SerializeField] private Sprite pauseButtonPressedSprite;
    
    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // Nền chính
        mainMenuBackground = root.Q<VisualElement>("MainMenu");

        // Áp dụng safe area cho mobile (tai thỏ, bo góc...)
        root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        
        // Lấy các button từ UI
        newGameButton = root.Q<Button>("NewGameButton");
        continueButton = root.Q<Button>("ContinueButton");
        settingsButton = root.Q<Button>("SettingsButton");
        quitButton = root.Q<Button>("QuitButton");

        // Áp dụng ảnh nền
        ApplyBackgroundImage();

        // Áp dụng ảnh cho các nút kiểu pause
        SetupPauseButtonVisual(newGameButton);
        SetupPauseButtonVisual(continueButton);
        SetupPauseButtonVisual(settingsButton);
        SetupPauseButtonVisual(quitButton);
        
        // Đăng ký các sự kiện
        newGameButton?.RegisterCallback<ClickEvent>(OnNewGameClick);
        continueButton?.RegisterCallback<ClickEvent>(OnContinueClick);
        settingsButton?.RegisterCallback<ClickEvent>(OnSettingsClick);
        quitButton?.RegisterCallback<ClickEvent>(OnQuitClick);
        
        // Ẩn cursor và unlock
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        
        // Kiểm tra có save game không để enable/disable nút Continue
        CheckSaveGame();
    }
    
    void OnDisable()
    {
        // Hủy đăng ký các sự kiện
        newGameButton?.UnregisterCallback<ClickEvent>(OnNewGameClick);
        continueButton?.UnregisterCallback<ClickEvent>(OnContinueClick);
        settingsButton?.UnregisterCallback<ClickEvent>(OnSettingsClick);
        quitButton?.UnregisterCallback<ClickEvent>(OnQuitClick);

        if (root != null)
        {
            root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
    }
    
    private void OnNewGameClick(ClickEvent evt)
    {
        Debug.Log("Starting new game...");
        // Xóa save game cũ nếu có
        PlayerPrefs.DeleteKey("SavedLevel");
        PlayerPrefs.Save();
        
        // Load scene game
        SceneManager.LoadScene(gameSceneName);
    }
    
    private void OnContinueClick(ClickEvent evt)
    {
        Debug.Log("Continuing game...");
        
        // Load level đã lưu
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            string savedScene = PlayerPrefs.GetString("SavedLevel", gameSceneName);
            SceneManager.LoadScene(savedScene);
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
    
    private void OnSettingsClick(ClickEvent evt)
    {
        Debug.Log("Opening settings...");
        // TODO: Mở menu settings
        // Bạn có thể tạo một panel settings riêng
    }
    
    private void OnQuitClick(ClickEvent evt)
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    private void CheckSaveGame()
    {
        // Disable nút Continue nếu không có save game
        if (continueButton != null)
        {
            bool hasSave = PlayerPrefs.HasKey("SavedLevel");
            continueButton.SetEnabled(hasSave);
        }
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        ApplySafeArea();
    }

    private void ApplyBackgroundImage()
    {
        if (mainMenuBackground != null && mainMenuBackgroundSprite != null)
        {
            mainMenuBackground.style.backgroundImage = new StyleBackground(mainMenuBackgroundSprite);
        }
    }

    private void ApplySafeArea()
    {
        if (mainMenuBackground == null)
            return;

        Rect safe = Screen.safeArea;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float left = safe.xMin;
        float right = screenWidth - safe.xMax;
        float bottom = safe.yMin;
        float top = screenHeight - safe.yMax;

        mainMenuBackground.style.paddingLeft = left;
        mainMenuBackground.style.paddingRight = right;
        mainMenuBackground.style.paddingTop = top;
        mainMenuBackground.style.paddingBottom = bottom;
    }

    private void SetupPauseButtonVisual(Button button)
    {
        if (button == null || pauseButtonNormalSprite == null)
            return;

        bool isHovering = false;

        // Trạng thái mặc định
        button.style.backgroundImage = new StyleBackground(pauseButtonNormalSprite);

        // Hover
        button.RegisterCallback<MouseEnterEvent>(_ =>
        {
            isHovering = true;
            if (pauseButtonHoverSprite != null)
                button.style.backgroundImage = new StyleBackground(pauseButtonHoverSprite);
        });

        button.RegisterCallback<MouseLeaveEvent>(_ =>
        {
            isHovering = false;
            button.style.backgroundImage = new StyleBackground(pauseButtonNormalSprite);
        });

        // Pressed
        button.RegisterCallback<PointerDownEvent>(_ =>
        {
            if (pauseButtonPressedSprite != null)
                button.style.backgroundImage = new StyleBackground(pauseButtonPressedSprite);
        });

        button.RegisterCallback<PointerUpEvent>(_ =>
        {
            // Khi nhả chuột: nếu vẫn đang hover thì dùng hover, ngược lại dùng normal
            if (pauseButtonHoverSprite != null && isHovering)
            {
                button.style.backgroundImage = new StyleBackground(pauseButtonHoverSprite);
            }
            else
            {
                button.style.backgroundImage = new StyleBackground(pauseButtonNormalSprite);
            }
        });
    }
}
