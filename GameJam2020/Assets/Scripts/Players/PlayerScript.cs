using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public PlayerID ID = PlayerID.Player1;
    public Statistics Statistics { get; set; }
    public float invincibilityDuration;
    public PlayerControls playerControls;
    public Weapon weapon;
    public ColorList color;

    // Vie actuelle (Statistics.Life -> Maximum)
    public int life;

    // Controles
    private Vector2 movement;
    private bool isShooting;

    private Vector3 axisUp = new Vector3(1, 0, 1).normalized;
    private Vector3 axisRight = new Vector3(1, 0, -1).normalized;

    // Invincibilité et tir
    private float invincibility = 0;
    private float nextShot = 0;

    // Initialisation du joueur
    private void Awake()
    {
        Statistics = new Statistics { Damages = 4, FireRate = 1f, Life = 10, Speed = 7.5f };
        life = Statistics.Life;

        playerControls = new PlayerControls();

        if (ID == PlayerID.Player1)
        {
            playerControls.Player1.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
            playerControls.Player1.Move.canceled += ctx => movement = ctx.ReadValue<Vector2>();
            playerControls.Player1.Shoot.performed += ctx => isShooting = true;
            playerControls.Player1.Shoot.canceled += ctx => isShooting = false;
            playerControls.Player1.Interact.performed += ctx => OnInteract();
        }
        else if (ID == PlayerID.Player2)
        {
            playerControls.Player2.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
            playerControls.Player2.Move.canceled += ctx => movement = ctx.ReadValue<Vector2>();
            playerControls.Player2.Shoot.performed += ctx => isShooting = true;
            playerControls.Player2.Shoot.canceled += ctx => isShooting = false;
            playerControls.Player2.Interact.performed += ctx => OnInteract();
        }

        weapon = new Blaster(this, GameManager.Instance.weaponsProjectiles[0]);
    }

    // Gestion des contrôles
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Gestion des touches
    private void CheckShoot()
    {
        if (Time.time >= nextShot)
        {
            nextShot = Time.time + weapon.Shoot();
        }
    }

    private void OnInteract()
    {

    }

    private void OnHurt()
    {

    }

    private void OnDie()
    {

    }

    // Déplacements
    private void FixedUpdate()
    {
        if (isShooting) { CheckShoot(); }

        GetComponent<CharacterController>().Move((axisUp * movement.y + axisRight * movement.x) * Statistics.Speed * Time.fixedDeltaTime);
        transform.forward = Vector3.Lerp(transform.forward, GetComponent<CharacterController>().velocity, 0.25f);
    }

    public void Damage(int damages)
    {
        if (Time.time > invincibility)
        {
            if (life < damages)
            {
                OnDie();
            }
            else
            {
                OnHurt();
            }

            invincibility = Time.time + invincibilityDuration;
            life = Mathf.Max(0, life - damages);
        }
    }
}