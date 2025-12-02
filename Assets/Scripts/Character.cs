using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    private CapsuleCollider2D gameObjectCollider;
    private SpriteRenderer gameObjectSprite;

    [Header("Side")]
    [SerializeField] private bool isLeft;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private int enemyLayerIndex;

    [Header("Base Stats")]
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float baseAttackSpeed = 1f;

    [Header("Current Stats")]
    [SerializeField] private float characterSpeed;
    [SerializeField] private float damage;
    [SerializeField] private float health;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float hitDistance = 1f;
    [SerializeField] private float attackTimer;
    [SerializeField] private int characterType;
    [SerializeField] private string myTowerTag;

    [Header("Multiplier Info")]
    [SerializeField] private float appliedMultiplier = 1f;
    [SerializeField] private bool isBuffed = false;

    private float currentCharacterSpeed;
    private bool isStopping;
    private bool isAttacing;
    private bool isDeath;
    private float maxHealth;

    [Header("Audio")]
    [SerializeField] private MMF_Player attackSound;
    [SerializeField] private MMF_Player castleHit;
    [SerializeField] private MMF_Player deathEffects;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;

    private void Awake()
    {
        // Base deðerleri current deðerlere kopyala
        characterSpeed = baseSpeed;
        damage = baseDamage;
        health = baseHealth;
        attackSpeed = baseAttackSpeed;
    }

    private void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponent<Animator>();
        gameObjectCollider = GetComponent<CapsuleCollider2D>();
        gameObjectSprite = GetComponent<SpriteRenderer>();
        attackSound = GameObject.Find("AttackSound").gameObject.GetComponent<MMF_Player>();
        castleHit = GameObject.Find("CastleHitSound").gameObject.GetComponent<MMF_Player>();
        deathEffects = GameObject.Find("DeathSounds").gameObject.GetComponent<MMF_Player>();

        attackTimer = attackSpeed;

        maxHealth = health;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    /// <summary>
    /// Brew sisteminden gelen multiplier'ý uygula
    /// </summary>
    public void ApplyMultiplier(float multiplier)
    {
        appliedMultiplier = multiplier;
        isBuffed = multiplier > 1.05f;

        // Statlarý multiplier ile çarp
        damage = baseDamage * multiplier;
        health = baseHealth * multiplier;
        maxHealth = health;
        characterSpeed = baseSpeed * Mathf.Sqrt(multiplier);
        attackSpeed = baseAttackSpeed / Mathf.Sqrt(multiplier); // Daha hýzlý atak için bölüyoruz

        // UI güncelle
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }

        // Buffed ise scale biraz büyüt
        if (isBuffed)
        {
            float scaleBonus = 1f + (multiplier - 1f) * 0.15f;
            transform.localScale *= scaleBonus;
        }

        Debug.Log($"[{gameObject.name}] Multiplier applied: x{multiplier:F2} | DMG:{damage:F1} HP:{health:F1} SPD:{characterSpeed:F2}");
    }

    private void Update()
    {
        if (isDeath) return;

        Movement();
        Attack();
        AnimatorController();
        HealthCheck();
        UpdateHealthUI();
    }

    private void Movement()
    {
        if (!isStopping)
        {
            float dir = isLeft ? 1f : -1f;
            currentCharacterSpeed = dir * characterSpeed;
        }
        else
        {
            currentCharacterSpeed = 0f;
        }

        rb.linearVelocity = new Vector2(currentCharacterSpeed, rb.linearVelocity.y);

        Vector3 scale = transform.localScale;
        scale.z = isLeft ? 1f : -1f;
        transform.localScale = scale;
    }

    private void Attack()
    {
        Vector2 rayDir = isLeft ? Vector2.right : Vector2.left;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDir, hitDistance, hitLayers);

        RaycastHit2D hit = default;
        bool found = false;

        foreach (var h in hits)
        {
            if (h.collider == null) continue;

            if (h.collider.gameObject == gameObject)
                continue;


            hit = h;
            found = true;
            break;
        }

        Debug.DrawLine(transform.position, transform.position + (Vector3)rayDir * hitDistance, Color.red);

        if (found)
        {
            if (hit.collider.gameObject.tag != myTowerTag)
            {
                isStopping = true;
            }

            if (hit.collider.gameObject.layer == enemyLayerIndex)
            {
                isAttacing = true;
                attackTimer -= Time.deltaTime;

                if (attackTimer <= 0f)
                {
                    Character enemy = hit.collider.GetComponent<Character>();
                    if (enemy != null)
                    {
                        enemy.Damage(damage, characterType);
                        attackSound?.PlayFeedbacks();
                    }

                    BaseScripts baseSc = hit.collider.GetComponent<BaseScripts>();
                    if (baseSc != null)
                    {
                        baseSc.BaseDamage(damage);
                        castleHit?.PlayFeedbacks();
                    }

                    attackTimer = attackSpeed;
                }
            }
            else
            {
                isAttacing = false;
                attackTimer = attackSpeed;
            }
        }
        else
        {
            isStopping = false;
            isAttacing = false;
            attackTimer = attackSpeed;
        }
    }

    private void HealthCheck()
    {
        if (health <= 0f && !isDeath)
        {
            isDeath = true;
            deathEffects?.PlayFeedbacks();
            gameObjectSprite.enabled = false;
            gameObject.layer = 0;
            rb.gravityScale = 0;
            gameObjectCollider.enabled = false;
            Destroy(gameObject, 1f);
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }
    }

    public void Damage(float damageAmount, int enemyCharacterType)
    {
        float baseDamageAmount = damageAmount;
        float multipliedDamage = baseDamageAmount;

        switch (characterType)
        {
            case 0: // Default
                switch (enemyCharacterType)
                {
                    case 0: multipliedDamage = baseDamageAmount * 1f; break;
                    case 1: multipliedDamage = baseDamageAmount * 2f; break;
                    case 2: multipliedDamage = baseDamageAmount * 2f; break;
                    case 3: multipliedDamage = baseDamageAmount * 2f; break;
                    case 4: multipliedDamage = baseDamageAmount * 2f; break;
                    case 5: multipliedDamage = baseDamageAmount * 2f; break;
                }
                break;
            case 1: // Ates
                switch (enemyCharacterType)
                {
                    case 0: multipliedDamage = baseDamageAmount * 0.25f; break;
                    case 1: multipliedDamage = baseDamageAmount * 1f; break;
                    case 2: multipliedDamage = baseDamageAmount * 0.5f; break;
                    case 3: multipliedDamage = baseDamageAmount * 2f; break;
                    case 4: multipliedDamage = baseDamageAmount * 1.5f; break;
                    case 5: multipliedDamage = baseDamageAmount * 0.75f; break;
                }
                break;
            case 2: // Su
                switch (enemyCharacterType)
                {
                    case 0: multipliedDamage = baseDamageAmount * 0.25f; break;
                    case 1: multipliedDamage = baseDamageAmount * 2f; break;
                    case 2: multipliedDamage = baseDamageAmount * 1f; break;
                    case 3: multipliedDamage = baseDamageAmount * 1.5f; break;
                    case 4: multipliedDamage = baseDamageAmount * 0.75f; break;
                    case 5: multipliedDamage = baseDamageAmount * 0.5f; break;
                }
                break;
            case 3: // Toprak
                switch (enemyCharacterType)
                {
                    case 0: multipliedDamage = baseDamageAmount * 1f; break;
                    case 1: multipliedDamage = baseDamageAmount * 1.5f; break;
                    case 2: multipliedDamage = baseDamageAmount * 0.5f; break;
                    case 3: multipliedDamage = baseDamageAmount * 1f; break;
                    case 4: multipliedDamage = baseDamageAmount * 0.75f; break;
                    case 5: multipliedDamage = baseDamageAmount * 2f; break;
                }
                break;
            case 4: // Hava
                switch (enemyCharacterType)
                {
                    case 0: multipliedDamage = baseDamageAmount * 0.25f; break;
                    case 1: multipliedDamage = baseDamageAmount * 0.75f; break;
                    case 2: multipliedDamage = baseDamageAmount * 2f; break;
                    case 3: multipliedDamage = baseDamageAmount * 0.5f; break;
                    case 4: multipliedDamage = baseDamageAmount * 1f; break;
                    case 5: multipliedDamage = baseDamageAmount * 1.5f; break;
                }
                break;
            case 5: // Elektrik
                switch (enemyCharacterType)
                {
                    case 0: multipliedDamage = baseDamageAmount * 0.25f; break;
                    case 1: multipliedDamage = baseDamageAmount * 0.75f; break;
                    case 2: multipliedDamage = baseDamageAmount * 1.5f; break;
                    case 3: multipliedDamage = baseDamageAmount * 0.5f; break;
                    case 4: multipliedDamage = baseDamageAmount * 2f; break;
                    case 5: multipliedDamage = baseDamageAmount * 1f; break;
                }
                break;
            default:
                multipliedDamage = baseDamageAmount * 1f;
                break;
        }

        health -= multipliedDamage;
    }

    private void AnimatorController()
    {
        animator.SetBool("Attack", isAttacing);
    }

    // Getter methods
    public float GetDamage() => damage;
    public float GetHealth() => health;
    public float GetMaxHealth() => maxHealth;
    public float GetSpeed() => characterSpeed;
    public float GetMultiplier() => appliedMultiplier;
    public bool IsBuffed() => isBuffed;
}