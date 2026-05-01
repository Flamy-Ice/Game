using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private CharacterData _baseStats; // Reference to our ScriptableObject

    // Current runtime values
    private float _currentHealth;
    private float _currentShield;
    private float _regenTimer;

    // Properties to access stats (can be modified by items later)
    public float MaxHealth => _baseStats.maxHealth;
    public float Armor => _baseStats.armor;

    private void Start()
    {
        // Initialize stats at game start
        _currentHealth = _baseStats.maxHealth;
        _currentShield = _baseStats.maxShield;
    }

    private void Update()
    {
        HandleRegeneration();
    }

    private void HandleRegeneration()
    {
        if (_currentHealth < MaxHealth)
        {
            // Apply healing over time
            _currentHealth += _baseStats.healthRegen * Time.deltaTime;
            _currentHealth = Mathf.Min(_currentHealth, MaxHealth);
        }
    }

    public void TakeDamage(float rawDamage, GameObject attacker)
    {
        // 1. Check for Dodge
        if (Random.value < _baseStats.dodgeChance)
        {
            Debug.Log("Dodged!");
            return;
        }

        // 2. Calculate Damage Reduction (Armor formula)
        // Standard formula: Damage = Raw / (1 + (Armor / 100))
        float damageAfterArmor = rawDamage * (100f / (100f + Armor));

        // 3. Apply Thorns (Deal damage back to attacker)
        if (attacker != null && _baseStats.thorns > 0)
        {
            float reflectDamage = damageAfterArmor * _baseStats.thorns;
            // Assuming enemy has a similar TakeDamage method
            // attacker.GetComponent<EnemyStats>()?.TakeDamage(reflectDamage);
        }

        // 4. Subtract from Shield first
        if (_currentShield > 0)
        {
            if (_currentShield >= damageAfterArmor)
            {
                _currentShield -= damageAfterArmor;
                damageAfterArmor = 0;
            }
            else
            {
                damageAfterArmor -= _currentShield;
                _currentShield = 0;
            }
        }

        // 5. Subtract remaining damage from Health
        _currentHealth -= damageAfterArmor;

        if (_currentHealth <= 0) Die();
    }

    public void OnDealDamage(float damageDealt)
    {
        // 6. Life Steal logic
        if (_baseStats.lifeSteal > 0)
        {
            float healAmount = damageDealt * _baseStats.lifeSteal;
            _currentHealth = Mathf.Min(_currentHealth + healAmount, MaxHealth);
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // Add death logic or restart game
    }
}