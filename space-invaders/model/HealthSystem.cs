namespace CapstoneProg3.model;

public class HealthSystem
{
    private int maxHealth;
    public int currentHealth;

    public event Action? OnHealthChanged;

    public HealthSystem(int maxHealth = 3)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
    }
    
    public void TakeDamage()
    {
        if (currentHealth > 0)
        {
            currentHealth--;
            OnHealthChanged?.Invoke(); // Notifica a ViewModel para atualizar a UI
        }
    }

    public void Heal()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++;
            OnHealthChanged?.Invoke();
        }
    }
}