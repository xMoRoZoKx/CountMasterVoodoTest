using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public float cooldown = 0.5f;
    public float projectileSpeed = 10f;
    public float projectileRange = 50f;
    public int damage = 10;
    public GameObject shootEffect;
}