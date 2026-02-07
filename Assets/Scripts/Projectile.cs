using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 1;
    public float lifetime = 3f;

    private Vector2 direction;
    private Rigidbody2D rb;
    private float spawnTime;
    private bool canDamage = false; // Защита от мгновенного попадания

    void Start()
    {
        Debug.Log($"=== СНАРЯД START: {name} ===");

        spawnTime = Time.time;

        // 1. Rigidbody
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        // 2. Визуализация
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("НЕТ SPRITE RENDERER! Создаю красный квадрат...");
            sr = gameObject.AddComponent<SpriteRenderer>();

            // БОЛЬШОЙ ЯРКИЙ КРАСНЫЙ КВАДРАТ
            Texture2D tex = new Texture2D(64, 64);
            for (int x = 0; x < 64; x++)
                for (int y = 0; y < 64; y++)
                    tex.SetPixel(x, y, Color.red);
            tex.Apply();

            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
            sr.color = new Color(1, 0, 0, 1); // 100% непрозрачный
            transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        }

        sr.sortingOrder = 999; // Поверх всего

        // 3. Коллайдер
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
        }
        col.isTrigger = true;
        col.radius = 0.2f;

        // 4. ВРЕМЕННО ОТКЛЮЧАЕМ КОЛЛАЙДЕР на 0.1 секунду!
        col.enabled = false;
        Invoke("EnableCollider", 0.1f);

        // 5. Уничтожение через время
        Destroy(gameObject, lifetime);

        Debug.Log($"Снаряд создан в {transform.position}");
    }

    void EnableCollider()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
            canDamage = true;
            Debug.Log("Коллайдер включен, можно наносить урон");
        }
    }

    void Update()
    {
        // Визуализация в Scene окне
        Debug.DrawLine(transform.position, transform.position + (Vector3)direction * 1f, Color.yellow);

        // Лог позиции каждую секунду
        if (Time.time - spawnTime > 1f && Time.time - spawnTime < 1.01f)
            Debug.Log($"Снаряд {name}: pos={transform.position}");
    }

    void FixedUpdate()
    {
        if (rb != null && direction != Vector2.zero)
        {
            rb.velocity = direction * speed;
        }
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
        Debug.Log($"Направление: {direction}, speed={speed}");

        if (direction.x < 0)
            transform.localScale = new Vector3(-0.5f, 0.5f, 1f);
        else
            transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!canDamage)
        {
            Debug.Log($"Игнорируем столкновение (коллайдер еще отключен): {other.name}");
            return;
        }

        Debug.Log($"=== СНАРЯД ПОПАЛ В: {other.name} (Tag: {other.tag}, Layer: {other.gameObject.layer}) ===");

        // Проверяем ВСЕ возможные теги
        if (other.CompareTag("Enemy") || other.CompareTag("Projectile"))
        {
            Debug.Log("Игнорируем врага/снаряд");
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("*** ПОПАЛ В ИГРОКА! ***");
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Урон нанесен!");
            }
            Destroy(gameObject);
            return;
        }

        // Проверяем ВСЕ возможные теги для земли
        if (other.CompareTag("Ground") || other.CompareTag("Platform") ||
            other.CompareTag("Wall") || other.CompareTag("Untagged"))
        {
            Debug.Log($"Попал в {other.tag}: {other.name}");
            Destroy(gameObject);
            return;
        }

        // Любой другой объект
        Debug.Log($"Попал в неизвестный объект: {other.name}, уничтожаю");
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        Debug.Log($"XXX Снаряд {name} УНИЧТОЖЕН. Прожил: {Time.time - spawnTime:F2} сек XXX");
    }
}