using UnityEngine;

public enum WeaponType
{
    Blaster,
    Ionizer,
    Railgun,
    Other
}

public class Weapon
{
    protected GameObject projectile;
    protected PlayerScript player;
    protected WeaponType weaponType;

    public Weapon(PlayerScript playerScript, GameObject projectileObject)
    {
        player = playerScript;
        projectile = projectileObject;
    }

    public float Shoot()
    {
        GameObject projectileInstance = GameObject.Instantiate(projectile, player.transform.position, player.transform.rotation);
        ProjectileScript projectileScript = projectileInstance.GetComponent<ProjectileScript>();
        projectileScript.damages *= player.Statistics.Damages;
        projectileScript.color = GameColors.ToColor(player.color);
        projectileScript.weaponType = weaponType;
        return 1f / (player.Statistics.FireRate * projectileScript.fireRate);
    }
}
public class Blaster : Weapon
{
    protected new GameObject projectile = GameManager.Instance.weaponsProjectiles[0];
    protected new WeaponType weaponType = WeaponType.Blaster;

    public Blaster(PlayerScript p, GameObject g) : base(p, g) { }
}

public class Ionizer : Weapon
{
    protected new GameObject projectile = GameManager.Instance.weaponsProjectiles[1];
    protected new WeaponType weaponType = WeaponType.Ionizer;

    public Ionizer(PlayerScript p, GameObject g) : base(p, g) { }
}

public class Railgun : Weapon
{
    protected new GameObject projectile = GameManager.Instance.weaponsProjectiles[2];
    protected new WeaponType weaponType = WeaponType.Railgun;

    public Railgun(PlayerScript p, GameObject g) : base(p, g) { }
}