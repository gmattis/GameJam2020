using UnityEngine;

public enum ProjectileType
{
    Player,
    Enemy
}

public class ProjectileScript : MonoBehaviour
{
    public int damages = 1; // Dégats du projectiles
    public float fireRate = 1f; // Cadence de tir
    public float velocity = 1f; // Vitesse du projectile
    public ProjectileType type; // Type de projectile
    public Color color;
    public GameObject[] objectsEffects;
    public WeaponType weaponType;

    // Start is called before the first frame update
    void Start()
    {
        if (type == ProjectileType.Player)
        {
            if (weaponType == WeaponType.Blaster)
            {
                var renderer = objectsEffects[0].GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>();
                renderer.material.SetColor("_EmissionColor", color);
            }
        }
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * velocity * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 9 && type == ProjectileType.Player)
        {
            collision.collider.GetComponent<EnemyScript>().Damage(damages);
            Destroy(gameObject);
        }
        else if (collision.collider.gameObject.layer == 8 && type == ProjectileType.Enemy)
        {
            collision.collider.GetComponent<PlayerScript>().Damage(damages);
            Destroy(gameObject);
        }
        else if (collision.collider.gameObject.layer == 10)
        {
            GameObject.Instantiate(GameManager.Instance.impact, transform.position + transform.forward * velocity * Time.fixedDeltaTime, Quaternion.Euler(transform.rotation.eulerAngles[0], 180 + transform.rotation.eulerAngles[1], transform.rotation.eulerAngles[2]));
            Destroy(gameObject);
        }
    }
}
