using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("Refenrences")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    private CapsuleCollider2D gameObjectCollider;
    private SpriteRenderer gameObjectSprite;

    [Header("Side")]
    [SerializeField] private bool isLeft;           
    [SerializeField] private LayerMask hitLayers;   
    [SerializeField] private int enemyLayerIndex;  

    [Header("Variables")]
    [SerializeField] private float characterSpeed = 2f;
    [SerializeField] private float hitDistance = 1f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackTimer;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float health = 100f;
    [SerializeField] private int characterType; // Default = 0 //Ates = 1 //Su = 2 // Toprak = 3 // Hava = 4 //Elektrik
    [SerializeField] private string myTowerTag;

    private float currentCharacterSpeed;
    private bool isStopping;
    private bool isAttacing;
    private bool isDeath;

    [Header("Audio")]
    [SerializeField] private MMF_Player attackSound;   
    [SerializeField] private MMF_Player deathEffects;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;
    private float maxHealth;

    private void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponent<Animator>();
        gameObjectCollider = GetComponent<CapsuleCollider2D>();
        gameObjectSprite = GetComponent<SpriteRenderer>();

        attackTimer = attackSpeed;

        maxHealth = health;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
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
            if (h.collider.gameObject.tag == myTowerTag)
                continue;

            hit = h;
            found = true;
            break;
        }

        Debug.DrawLine(transform.position, transform.position + (Vector3)rayDir * hitDistance, Color.red);

        if (found)
        {
            isStopping = true;

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
                    if(baseSc != null)
                    {
                        baseSc.BaseDamage(damage);
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
            Destroy(gameObject , 1f);
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
        float baseDamage = health -= damageAmount;
        switch (characterType)
         {
            case 0: // Default
                switch (enemyCharacterType)
                {
                    case 0:
                        health -= baseDamage * 1f;
                        break;
                    case 1:
                        health -= baseDamage * 2f;
                        break;
                    case 2:
                        health -= baseDamage * 2f;
                        break;
                    case 3:
                        health -= baseDamage * 2f;
                        break;
                    case 4:
                        health -= baseDamage * 2f;
                        break;
                    case 5:
                        health -= baseDamage * 2f;
                        break;
                }
                break;
            case 1: //Ates
                switch (enemyCharacterType)
                {
                    case 0:
                        health -= baseDamage * 0.25f;
                        break;
                    case 1:
                        health -= baseDamage * 1f;
                        break;
                    case 2:
                        health -= baseDamage * 0.5f;
                        break;
                    case 3:
                        health -= baseDamage * 2f;
                        break;
                    case 4:
                        health -= baseDamage * 1.5f;
                        break;
                    case 5:
                        health -= baseDamage * 0.75f;
                        break;
                }
                break;
            case 2: //Su
                switch (enemyCharacterType)
                {
                    case 0:
                        health -= baseDamage * 0.25f;
                        break;
                    case 1:
                        health -= baseDamage * 2f;
                        break;
                    case 2:
                        health -= baseDamage * 1f;
                        break;
                    case 3:
                        health -= baseDamage * 1.5f;
                        break;
                    case 4:
                        health -= baseDamage * 0.75f;
                        break;
                    case 5:
                        health -= baseDamage * 0.5f;
                        break;
                }
                break;
            case 3: //Toprak
                switch (enemyCharacterType)
                {
                    case 0:
                        health -= baseDamage * 1f;
                        break;
                    case 1:
                        health -= baseDamage * 1.5f;
                        break;
                    case 2:
                        health -= baseDamage * 0.5f;
                        break;
                    case 3:
                        health -= baseDamage * 1f;
                        break;
                    case 4:
                        health -= baseDamage * 0.75f;
                        break;
                    case 5:
                        health -= baseDamage * 2f;
                        break;
                }
                break;
            case 4: //Hava
                switch (enemyCharacterType)
                {
                    case 0:
                        health -= baseDamage * 0.25f;
                        break;
                    case 1:
                        health -= baseDamage * 0.75f;
                        break;
                    case 2:
                        health -= baseDamage * 2f;
                        break;
                    case 3:
                        health -= baseDamage * 0.5f;
                        break;
                    case 4:
                        health -= baseDamage * 1f;
                        break;
                    case 5:
                        health -= baseDamage * 1.5f;
                        break;
                }
                break;
            case 5: //Elektrik
                switch (enemyCharacterType)
                {
                    case 0:
                        health -= baseDamage * 0.25f;
                        break;
                    case 1:
                        health -= baseDamage * 0.75f;
                        break;
                    case 2:
                        health -= baseDamage * 1.5f;
                        break;
                    case 3:
                        health -= baseDamage * 0.5f;
                        break;
                    case 4:
                        health -= baseDamage * 2f;
                        break;
                    case 5:
                        health -= baseDamage * 1f;
                        break;
                }
                break;
            default:
                health -= baseDamage * 1f;
                break;
        }
    }

    private void AnimatorController()
    {
        animator.SetBool("Attack", isAttacing);
    }
}
