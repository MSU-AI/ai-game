using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OK
{
    public class EnemyStats : CharacterStats
    {
        EnemyAnimatorManager enemyAnimatorManager;
        public UIEnemyHealthBar enemyHealthBar;
        InputHandler inputHandler;
        CameraHandler cameraHandler;

        public GameObject enemy;

        private void Awake()
        {
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            inputHandler = FindObjectOfType<InputHandler>();
            cameraHandler = FindObjectOfType<CameraHandler>();
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            enemyHealthBar.SetMaxHealth(maxHealth);
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
            enemyHealthBar.SetHealth(currentHealth);

            enemyAnimatorManager.PlayTargetAnimation(damageAnimation, true);

            if(currentHealth <= 0)
            {
                // DEATH
                currentHealth = 0;
                enemyAnimatorManager.PlayTargetAnimation("Dead_01", true);
                isDead = true;
                Destroy(enemy, 1.2f);
                if (inputHandler.lockOnFlag != false)
                {
                    cameraHandler.ClearLockOnTargets();
                }
            }
        }
    }    
}
