using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private int maxHealth;
    [HideInInspector] public int health;
    [SerializeField] private float damageCooldown;
    private float damageTimer;
    [SerializeField] private float shootCooldown;
    private float shootTimer;
    [SerializeField] private AudioClip damageClip;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletLifetime;

    [SerializeField] private GameObject face;
    [SerializeField] private Sprite neutralFace;
    [SerializeField] private Sprite hurtFace;
    [SerializeField] private Sprite deathFace;
    [SerializeField] private Sprite confusedFace;

    private void Start()
    {
        health = maxHealth;
        healthBar.value = 1;
    }

    private void Update()
    {
        if (health <= 0) {
            face.GetComponent<SpriteRenderer>().sprite = deathFace;
            this.GetComponent<PlayerMovement>().enabled = false;
            this.enabled = false;
            return;
        }
        if(damageTimer <= 0) {
            face.GetComponent<SpriteRenderer>().sprite = neutralFace;
        } else {
            face.GetComponent<SpriteRenderer>().sprite = hurtFace;
            damageTimer -= Time.deltaTime;
        }
        if (Input.GetButton("Fire1") && shootTimer <= 0) {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var b = bullet.GetComponent<Bullet>();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            float ySign = (mousePos.y < transform.position.y) ? -1f : 1f;
            b.rotation = Vector2.Angle(Vector2.right, mousePos - transform.position) * ySign;
            b.speed = bulletSpeed;
            Destroy(bullet, bulletLifetime);
            shootTimer = shootCooldown;
        } else {
            shootTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Bullet" && damageTimer <= 0) {
            AudioSource ASHit = gameObject.AddComponent<AudioSource>();
            ASHit.clip = damageClip;
            ASHit.volume = 0.5f;
            ASHit.Play();
            health -= 1;
            healthBar.value = (float)health / (float)maxHealth;
            damageTimer = damageCooldown;
            Destroy(collision.gameObject);
        }
    }

    public void FullHeal() {
        health = maxHealth;
        healthBar.value = 1;
    }
}
