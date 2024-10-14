using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OK
{
    public class PlayerStats : CharacterStats
    {
        public HealthBar healthBar;
        //public StaminaBar staminaBar;

        AnimatorHandler animatorHandler;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }
        
        public void TakeDamage(int damage, string damageAnimation = "Damage_01")
        {
            if (isDead)
                return;
            currentHealth = currentHealth - damage;

            healthBar.SetCurrentHealth(currentHealth);

            animatorHandler.PlayTargetAnimation(damageAnimation, true);

            if(currentHealth <= 0)
            {
                // DEATH
                currentHealth = 0;
                animatorHandler.PlayTargetAnimation("Dead_01", true);
                isDead = true;
            }
        }
    }    
}
