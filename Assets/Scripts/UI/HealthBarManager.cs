using UnityEngine;
using System.Collections.Generic;

public class HealthBarManager : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform worldCanvas;
    
    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 10;
    [SerializeField] private bool autoFindWorldCanvas = true;
    
    // Singleton pattern for easy access
    public static HealthBarManager Instance { get; private set; }
    
    // Object pooling for performance
    private Queue<EnemyHealthBar> healthBarPool = new Queue<EnemyHealthBar>();
    private List<EnemyHealthBar> activeHealthBars = new List<EnemyHealthBar>();
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeManager()
    {
        // Auto-find WorldCanvas if not assigned
        if (autoFindWorldCanvas && worldCanvas == null)
        {
            GameObject worldCanvasGO = GameObject.Find("WorldCanvas");
            if (worldCanvasGO != null)
            {
                worldCanvas = worldCanvasGO.transform;
                Debug.Log("Auto-found WorldCanvas: " + worldCanvasGO.name);
            }
            else
            {
                Debug.LogWarning("WorldCanvas not found! Create a Canvas with name 'WorldCanvas'");
            }
        }
        
        // Create health bar prefab if not assigned
        if (healthBarPrefab == null)
        {
            CreateDefaultHealthBarPrefab();
        }
        
        // Initialize object pool
        InitializePool();
    }
    
    private void CreateDefaultHealthBarPrefab()
    {
        Debug.Log("Creating default health bar prefab...");
        
        // Create root GameObject
        GameObject healthBarGO = new GameObject("EnemyHealthBar");
        
        // Add Canvas component for World Space
        Canvas canvas = healthBarGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 10;
        
        // Set small scale for world space
        healthBarGO.transform.localScale = Vector3.one * 0.01f;
        
        // Add RectTransform setup
        RectTransform rectTransform = healthBarGO.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 10);
        
        // Create background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(healthBarGO.transform);
        
        UnityEngine.UI.Image backgroundImage = background.AddComponent<UnityEngine.UI.Image>();
        backgroundImage.color = new Color(0, 0, 0, 0.8f);
        backgroundImage.raycastTarget = false;
        
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // Create health slider
        GameObject sliderGO = new GameObject("HealthSlider");
        sliderGO.transform.SetParent(healthBarGO.transform);
        
        UnityEngine.UI.Slider slider = sliderGO.AddComponent<UnityEngine.UI.Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        slider.interactable = false;
        
        RectTransform sliderRect = sliderGO.GetComponent<RectTransform>();
        sliderRect.anchorMin = Vector2.zero;
        sliderRect.anchorMax = Vector2.one;
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;
        
        // Create fill area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGO.transform);
        
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;
        
        // Create fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform);
        
        UnityEngine.UI.Image fillImage = fill.AddComponent<UnityEngine.UI.Image>();
        fillImage.color = Color.red;
        fillImage.raycastTarget = false;
        
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        // Setup slider references
        slider.fillRect = fillRect;
        
        // Add EnemyHealthBar script
        EnemyHealthBar healthBar = healthBarGO.AddComponent<EnemyHealthBar>();
        
        // Store as prefab reference
        healthBarPrefab = healthBarGO;
        
        Debug.Log("Default health bar prefab created successfully!");
    }
    
    private void InitializePool()
    {
        if (healthBarPrefab == null || worldCanvas == null) return;
        
        for (int i = 0; i < poolSize; i++)
        {
            GameObject healthBarGO = Instantiate(healthBarPrefab, worldCanvas);
            EnemyHealthBar healthBar = healthBarGO.GetComponent<EnemyHealthBar>();
            
            if (healthBar != null)
            {
                healthBarGO.SetActive(false);
                healthBarPool.Enqueue(healthBar);
            }
        }
        
        Debug.Log($"Health bar pool initialized with {poolSize} objects");
    }
    
    public EnemyHealthBar RequestHealthBar(Transform enemy, EnemyAI enemyAI)
    {
        EnemyHealthBar healthBar = null;
        
        // Try to get from pool first
        if (healthBarPool.Count > 0)
        {
            healthBar = healthBarPool.Dequeue();
        }
        else
        {
            // Create new one if pool is empty
            if (healthBarPrefab != null && worldCanvas != null)
            {
                GameObject healthBarGO = Instantiate(healthBarPrefab, worldCanvas);
                healthBar = healthBarGO.GetComponent<EnemyHealthBar>();
            }
        }
        
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.Initialize(enemy, enemyAI);
            activeHealthBars.Add(healthBar);
        }
        
        return healthBar;
    }
    
    public void ReturnHealthBar(EnemyHealthBar healthBar)
    {
        if (healthBar == null) return;
        
        activeHealthBars.Remove(healthBar);
        healthBar.gameObject.SetActive(false);
        healthBarPool.Enqueue(healthBar);
        
        // Reset to pool position
        if (worldCanvas != null)
        {
            healthBar.transform.SetParent(worldCanvas);
        }
    }
    
    public void ReturnAllHealthBars()
    {
        for (int i = activeHealthBars.Count - 1; i >= 0; i--)
        {
            if (activeHealthBars[i] != null)
            {
                ReturnHealthBar(activeHealthBars[i]);
            }
        }
    }
    
    // Public properties for external access
    public int ActiveHealthBars => activeHealthBars.Count;
    public int PooledHealthBars => healthBarPool.Count;
    public Transform WorldCanvas => worldCanvas;
    
    void OnApplicationQuit()
    {
        Instance = null;
    }
}
