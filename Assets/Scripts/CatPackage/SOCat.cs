using System.Collections;
using System.Collections.Generic;
using CatPackage;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName="new Cat", menuName="Custom/Cat")]
public class SOCat : ScriptableObject
{
    [Header("Display info")] [SerializeField]
    private Sprite catSprite;
    [SerializeField] private string catName;
    [SerializeField] private string abilityDescription;
    [SerializeField] private CatAnimation attackAnimation;
    [SerializeField] private Color catColor;
    [SerializeField] private Color catEyeColor;
    [SerializeField] private Color catNoseColor;
    [SerializeField] private ECatBreed catBreed;
    [Space]
    [Header("Cat settings")]
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private int damage;
    [SerializeField] private float cooldown;
    [SerializeField] private int health;

    
    public void SpawnAttackPrefab(Vector3 shootPos, Transform self, int catLevel)
    {
        var attackObject = Instantiate(attackPrefab, shootPos, Quaternion.identity);
        UtilsMethods.LookAtMouse(attackObject.transform);
        var attackScript = attackObject.GetComponent<AttackObject>();
        attackScript.Attack(self, Mathf.CeilToInt(
            damage + (damage / 10f) * catLevel));
        Destroy(attackObject, 0.1f);
    }

    public void SetMaterialColor(SpriteRenderer spriteRenderer)
    {
        // red -> nos
        // blue -> fur
        // green -> eyes
        spriteRenderer.sprite = catSprite;
        var material = spriteRenderer.material;
        material.SetColor("_Red", catNoseColor);
        material.SetColor("_Blue", catColor);
        material.SetColor("_Green", catEyeColor);
    }
    public void SetMaterialColor(Image image)
    {
        // red -> nos
        // blue -> fur
        // green -> eyes
        image.sprite = catSprite;
        var material = Instantiate(image.material);
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

        material.SetColor("_Red", catNoseColor);
        material.SetColor("_Blue", catColor);
        material.SetColor("_Green", catEyeColor);
        image.material = material;
    }

    public SCatDisplayInfo GetDisplayInfo()
    {
        return new SCatDisplayInfo()
        {
            catBreed = catBreed,
            catName = catName,
            catSprite = catSprite,
            catTier = (ECatTier)Mathf.CeilToInt((int)catBreed / 2f),
            catColor = catColor,
            catEyeColor = catEyeColor,
            catNoseColor = catNoseColor,
        };
    }

    public SCatSpecificInfo GetSpecificInfo(int catLevel)
    {
        float getCooldown()
        {
            var cd = cooldown;
            for (var i = 0; i < catLevel; i++)
            {
                cd -= (cd / 100f) * 5f;
            }

            return cd;
        }
        
        return new SCatSpecificInfo()
        {
            cooldown = getCooldown(),
            damage = Mathf.CeilToInt(damage + (damage / 100f * catLevel)),
            maxHealth = Mathf.CeilToInt(health + (health / 100f * catLevel)),
        };
    }
}

public struct SCatDisplayInfo
{
    public Sprite catSprite;
    public ECatBreed catBreed;
    public ECatTier catTier;
    public string catName;
    public Color catColor;
    public Color catEyeColor;
    public Color catNoseColor;
}

public struct SCatSpecificInfo
{
    public int damage;
    public float cooldown;
    public int maxHealth;
}
