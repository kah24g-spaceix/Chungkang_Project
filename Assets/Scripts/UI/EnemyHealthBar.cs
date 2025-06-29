using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject healthBarCanvas;
    
    [Header("Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2.5f, 0);
    [SerializeField] private bool alwaysFaceCamera = true;
    [SerializeField] private float hideDistance = 15f;
    [SerializeField] private bool hideWhenFullHealth = true;
    
    // References
    private Transform targetEnemy;
    private Camera playerCamera;
    private EnemyAI enemyAI;
    private float maxHealth;
    private float currentHealth;
    
    // Visibility control
    private bool isVisible = true;
    private CanvasGroup canvasGroup;
    
    void Awake()
    {
        // Get components
        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>();
            
        if (healthBarCanvas == null)
            healthBarCanvas = gameObject;
            
        // Add CanvasGroup for smooth fade effects
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    
    void Start()
    {
        // Find player camera
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            playerCamera = FindFirstObjectByType<Camera>();
        }
        
        // Initialize health bar
        if (healthSlider != null)
        {
            healthSlider.value = 1f;
        }
    }
    
    void Update()
    {
        if (targetEnemy == null || playerCamera == null) return;
        
        // Update position
        UpdatePosition();
        
        // Update rotation (billboard effect)
        if (alwaysFaceCamera)
        {
            UpdateRotation();
        }
        
        // Update visibility
        UpdateVisibility();
    }
    
    public void Initialize(Transform enemy, EnemyAI enemyController)
    {
        targetEnemy = enemy;
        enemyAI = enemyController;
        
        // Get max health from enemy config
        if (enemyAI != null && enemyAI.config != null)
        {
            maxHealth = enemyAI.config.maxHealth;
            currentHealth = maxHealth;
        }
        
        // Set initial position
        UpdatePosition();
        UpdateHealthBar(1f);
    }
    
    public void UpdateHealthBar(float healthPercentage)
    {
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Clamp01(healthPercentage);
        }
        
        // Update visibility based on health
        if (hideWhenFullHealth)
        {
            bool shouldShow = healthPercentage < 1f;
            SetVisibility(shouldShow);
        }
        
        // Change color based on health
        UpdateHealthBarColor(healthPercentage);
    }
    
    public void UpdateHealth(float newHealth)
    {
        currentHealth = newHealth;
        float healthPercentage = maxHealth > 0 ? currentHealth / maxHealth : 0;
        UpdateHealthBar(healthPercentage);
    }
    
    private void UpdatePosition()
    {
        if (targetEnemy != null)
        {
            // Position above enemy's head
            Vector3 worldPosition = targetEnemy.position + offset;
            transform.position = worldPosition;
        }
    }
    
    private void UpdateRotation()
    {
        if (playerCamera != null)
        {
            // Always face the camera (billboard effect)
            Vector3 directionToCamera = playerCamera.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }
    
    private void UpdateVisibility()
    {
        if (playerCamera == null || targetEnemy == null) return;
        
        float distanceToCamera = Vector3.Distance(playerCamera.transform.position, targetEnemy.position);
        bool shouldBeVisible = distanceToCamera <= hideDistance;
        
        if (shouldBeVisible != isVisible)
        {
            SetVisibility(shouldBeVisible);
        }
    }
    
    private void SetVisibility(bool visible)
    {
        isVisible = visible;
        
        if (canvasGroup != null)
        {
            // Smooth fade in/out
            canvasGroup.alpha = visible ? 1f : 0f;
        }
        else
        {
            // Immediate show/hide
            healthBarCanvas.SetActive(visible);
        }
    }
    
    private void UpdateHealthBarColor(float healthPercentage)
    {
        if (healthSlider == null) return;
        
        Image fillImage = healthSlider.fillRect?.GetComponent<Image>();
        if (fillImage != null)
        {
            // Green to Red gradient based on health
            Color healthColor = Color.Lerp(Color.red, Color.green, healthPercentage);
            fillImage.color = healthColor;
        }
    }
    
    // Called when enemy is destroyed
    public void OnEnemyDestroyed()
    {
        // Return to pool instead of destroying
        if (HealthBarManager.Instance != null)
        {
            HealthBarManager.Instance.ReturnHealthBar(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void OnValidate()
    {
        // Editor preview
        if (Application.isPlaying && healthSlider != null)
        {
            UpdateHealthBar(healthSlider.value);
        }
    }
}
