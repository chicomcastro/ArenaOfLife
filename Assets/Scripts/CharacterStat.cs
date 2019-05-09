[System.Serializable]
public class CharacterStat
{
    public int agility;
    public int speed;
    public float HP;
    public float damage;
    public float attackCooldown;
    public float attackRange;
    public float attackKnockback;
}

[System.Serializable]
public class EnemyStats : CharacterStat
{
    public float searchingRange = 5f;
    public float habitatRadius = 10f;
}