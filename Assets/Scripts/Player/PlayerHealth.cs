using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private CharacterData _stats; // Your ScriptableObject with base stats

    // Current runtime values
    private float _currentHealth;
    private float _currentShield;
    private float _regenTimer;

    // Public properties to read HP/Shield (e.g., for UI)
    public float CurrentHealth => _currentHealth;
    public float CurrentShield => _currentShield;
    public float MaxHealth => _stats.maxHealth;

    private void Start()
    {
        // Initialize health and shield at start
        _currentHealth = _stats.maxHealth;
        _currentShield = _stats.maxShield;
    }

    private void Update()
    {
        HandleRegen();
        HandleShieldRegen();
    }

    // Logic for HP Regeneration
    private void HandleRegen()
    {
        if (_currentHealth < _stats.maxHealth)
        {
            _currentHealth += _stats.healthRegen * Time.deltaTime;
            _currentHealth = Mathf.Min(_currentHealth, _stats.maxHealth);
        }
    }

    // Optional: Shield slowly comes back if you want it to be renewable
    private void HandleShieldRegen()
    {
        if (_currentShield < _stats.maxShield)
        {
            // Simple shield regen (e.g., 10% of max per second)
            _currentShield += (_stats.maxShield * 0.1f) * Time.deltaTime;
            _currentShield = Mathf.Min(_currentShield, _stats.maxShield);
        }
    }

    public void TakeDamage(float rawDamage, GameObject attacker = null)
    {
        // 1. Check for Dodge (Random.value is 0.0 to 1.0)
        if (Random.value < _stats.dodgeChance)
        {
            Debug.Log("Dodged the attack!");
            return;
        }

        // 2. Apply Armor reduction
        // Formula: Damage = Raw * (100 / (100 + Armor))
        float damageAfterArmor = rawDamage * (100f / (100f + _stats.armor));

        // 3. Thorns (Return damage to attacker)
        if (attacker != null && _stats.thorns > 0)
        {
            float reflectDamage = damageAfterArmor * _stats.thorns;
            // attacker.GetComponent<Health>()?.TakeDamage(reflectDamage);
        }

        // 4. Damage Shield first
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

        // 5. Apply remaining damage to Health
        _currentHealth -= damageAfterArmor;

        if (_currentHealth <= 0) Die();
    }

    public void OnDealtDamage(float damageDealt)
    {
        // Life Steal: Heal based on damage you deal to enemies
        if (_stats.lifeSteal > 0)
        {
            float healAmount = damageDealt * _stats.lifeSteal;
            Heal(healAmount);
        }
    }

    public void Heal(float amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, _stats.maxHealth);
    }

    private void Die()
    {
        Debug.Log("Character has died!");
        // Disable movement, play animation, or show Game Over screen
    }
}