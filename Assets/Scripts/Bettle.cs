using UnityEngine;
using System.Collections;

public class Bettle : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public float patrolDistance = 3f;
    public bool startMovingRight = true;

    [Header("Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public int contactDamage = 1;
    public float knockbackForce = 15f;
    public float knockbackDuration = 0.3f;

    [Header("Health")]
    public int health = 50;

    private Vector2 startPos;
    private bool movingRight;
    private SpriteRenderer sr;
    private Transform player;
    private float attackTimer;
    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        startPos = transform.position;
        movingRight = startMovingRight;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Найти игрока
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Не найден игрок!");

        // Начинаем с полным таймером
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        attackTimer -= Time.deltaTime;

        if (distanceToPlayer <= attackRange)
        {
            // Останавливаемся
            rb.velocity = Vector2.zero;

            // Поворачиваемся к игроку
            bool playerIsRight = player.position.x > transform.position.x;
            sr.flipX = playerIsRight;

            // Анимация
            if (anim != null)
                anim.SetBool("isAttacking", true);

            // Стреляем
            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
                Debug.Log("Жук выстрелил!");
            }
        }
        else
        {
            // Патрулируем
            Patrol();

            if (anim != null)
            {
                anim.SetBool("isAttacking", false);
                anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
            }
        }
    }

    void Patrol()
    {
        if (movingRight)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            sr.flipX = true;

            if (transform.position.x >= startPos.x + patrolDistance)
                movingRight = false;
        }
        else
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            sr.flipX = false;

            if (transform.position.x <= startPos.x - patrolDistance)
                movingRight = true;
        }
    }

    void Attack()
    {
        Debug.Log($"Жук {name} атакует!");

        if (anim != null)
            anim.SetTrigger("attack");

        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            projectile.name = "Bullet_" + Time.time;

            Projectile projScript = projectile.GetComponent<Projectile>();
            if (projScript != null)
            {
                // Направление: куда смотрит жук
                Vector2 direction = sr.flipX ? Vector2.right : Vector2.left;
                projScript.SetDirection(direction);

                Debug.Log($"Направление: {direction}");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Урон
            collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(contactDamage);

            // Отключить управление
            Player_Controller playerCtrl = collision.gameObject.GetComponent<Player_Controller>();
            if (playerCtrl != null)
            {
                playerCtrl.isKnockedBack = true;
                StartCoroutine(EnableControl(playerCtrl));
            }

            // Отбрасывание
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                float direction = (collision.transform.position.x > transform.position.x) ? 1f : -1f;

                // ЧИСТО ГОРИЗОНТАЛЬНО с МИНИМУМОМ вертикали
                playerRb.velocity = new Vector2(
                    direction * 12f,     // Горизонталь (12 - нормально)
                    Mathf.Min(playerRb.velocity.y + 1f, 5f) // Очень маленький подскок
                );

                Debug.Log($"Отбрасывание: {direction * 12f}, {playerRb.velocity.y}");
            }
        }
    }

    IEnumerator EnableControl(Player_Controller playerCtrl)
    {
        yield return new WaitForSeconds(knockbackDuration);
        playerCtrl.isKnockedBack = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
        else StartCoroutine(DamageFlash());
    }

    IEnumerator DamageFlash()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}