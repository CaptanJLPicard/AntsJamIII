using UnityEngine;
using MoreMountains.Feedbacks;

public class Character : MonoBehaviour
{
    [Header("Refenrences")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform enemyBase;  
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

    private float currentCharacterSpeed;
    private bool isStopping;
    private bool isAttacing;
    private bool isDeath;

    [Header("Audio")]
    [SerializeField] private MMF_Player attackSound;   
    [SerializeField] private MMF_Player deathEffects;

    private void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponent<Animator>();
        gameObjectCollider = GetComponent<CapsuleCollider2D>();
        gameObjectSprite = GetComponent<SpriteRenderer>();

        attackTimer = attackSpeed;
    }

    private void Update()
    {
        if (isDeath) return;

        Movement();
        Attack();
        AnimatorController();
        HealthCheck();
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
                        enemy.Damage(damage);
                        attackSound?.PlayFeedbacks();
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
            Destroy(gameObject , 3f);
        }
    }

    public void Damage(float damageAmount)
    {
        health -= damageAmount;
    }

    private void AnimatorController()
    {
        animator.SetBool("Attack", isAttacing);
    }
}
