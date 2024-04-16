using AbilityPackage;
using CatPackage;
using UnityEngine;
using UnityEngine.UI;
using Utils;

[CreateAssetMenu(fileName="new Cat", menuName="Custom/Cat")]
public class SOCat : ScriptableObject
{
    [Header("Display info")] [SerializeField]
    private Sprite catSprite;
    [SerializeField] private string catName;
    [SerializeField, TextArea] private string abilityDescription;
    [SerializeField] private CatAnimation attackAnimation;
    [SerializeField] private Color catColor;
    [SerializeField] private Color catEyeColor;
    [SerializeField] private Color catNoseColor;
    [SerializeField] private ECatBreed catBreed;
    [Space]
    [Header("Cat settings")]
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private int damage;
    [SerializeField] private float attacksPerSecond;
    [SerializeField] private float cooldown;
    [SerializeField] private int health;

    
    public void SpawnAttackPrefab(Vector3 shootPos, Transform self, int catLevel)
    {
        var attackObject = Instantiate(attackPrefab, shootPos, Quaternion.identity);
        UtilsMethods.LookAtMouse(attackObject.transform);
        var attackScript = attackObject.GetComponent<Ability>();
        attackScript.UseAbility(self, Mathf.CeilToInt(
            damage + (damage / 10f) * catLevel));
        Destroy(attackObject, 0.1f);
    }

    public SCatDisplayInfo GetDisplayInfo()
    {
        return new SCatDisplayInfo()
        {
            CatBreed = catBreed,
            CatName = catName,
            CatSprite = catSprite,
            CatTier = (ECatTier)Mathf.CeilToInt((int)catBreed / 2f),
            CatColor = catColor,
            CatEyeColor = catEyeColor,
            CatNoseColor = catNoseColor,
            AbilityDescription = abilityDescription,
        };
    }

    public SCatSpecificInfo GetSpecificInfo(int catLevel)
    {
        float getCooldown()
        {
            var cd = cooldown;
            for (var i = 1; i < catLevel; i++)
            {
                cd -= (cd / 100f) * 5f;
            }

            return cd;
        }
        
        return new SCatSpecificInfo()
        {
            Cooldown = getCooldown(),
            Damage = Mathf.CeilToInt(damage + damage / 100f * (catLevel - 1)),
            MaxHealth = Mathf.CeilToInt(health + health / 100f * (catLevel - 1)),
            AttacksPerSecond = Mathf.CeilToInt(attacksPerSecond + attacksPerSecond / 100f * (catLevel - 1)),
        };
    }
}

public struct SCatDisplayInfo
{
    public Sprite CatSprite;
    public ECatBreed CatBreed;
    public ECatTier CatTier;
    public string CatName;
    public string AbilityDescription;
    public Color CatColor;
    public Color CatEyeColor;
    public Color CatNoseColor;
}

public struct SCatSpecificInfo
{
    public int Damage;
    public int AttacksPerSecond;
    public float Cooldown;
    public int MaxHealth;
}
