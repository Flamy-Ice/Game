using UnityEngine;

public class DamageFloor : MonoBehaviour
{
    [Header("Ustawienia Obrażeń")]
    [SerializeField] private float damageAmount = 15f;       // Ile obrażeń zadaje podłoga
    [SerializeField] private float damageInterval = 0.5f;    // Co ile sekund zadaje obrażenia (np. co pół sekundy)

    private float nextDamageTime;

    private void OnTriggerEnter(Collider other)
    {
        // Sprawdzamy, czy obiekt, który wszedł na podłogę, ma komponent PlayerHealth
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            // Zadaj obrażenia natychmiast przy pierwszym kontakcie
            player.TakeDamage(damageAmount);

            // Wyznacz czas, kiedy gracz może otrzymać kolejne obrażenia
            nextDamageTime = Time.time + damageInterval;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            // Jeśli gracz wciąż stoi na podłodze i minął czas cooldownu
            if (Time.time >= nextDamageTime)
            {
                player.TakeDamage(damageAmount);

                // Przesuwamy czas kolejnego ataku w przyszłość
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }
}