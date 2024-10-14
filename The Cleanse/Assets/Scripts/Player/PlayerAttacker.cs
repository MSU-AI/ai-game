using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OK
{
    public class PlayerAttacker : MonoBehaviour
    {
        AnimatorHandler animatorHandler;
        InputHandler inputHandler;
        WeaponSlotManager weaponSlotManager;
        PlayerManager playerManager;
        PlayerEquipmentManager playerEquipmentManager;
        PlayerInventory playerInventory;
        public string lastAttack;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            inputHandler = GetComponent<InputHandler>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            playerManager = GetComponent<PlayerManager>();
            playerEquipmentManager = GetComponentInChildren<PlayerEquipmentManager>();
            playerInventory = GetComponent<PlayerInventory>();
        }

        public void HandleWeaponCombo(WeaponItem weapon)
        {
            if(inputHandler.comboFlag)
            {
                animatorHandler.anim.SetBool("canDoCombo", false);

                if (lastAttack == weapon.OH_Light_Attack_01)
                {
                    animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_02, true);
                }
                else if (lastAttack == weapon.TH_Light_Attack_01)
                {
                    animatorHandler.PlayTargetAnimation(weapon.TH_Light_Attack_02, true);
                }
            }
            
        }

        public void HandleLightAttack(WeaponItem weapon)
        {
            weaponSlotManager.attackingWeapon = weapon;
            if (inputHandler.twoHandFlag)
            {
                animatorHandler.PlayTargetAnimation(weapon.TH_Light_Attack_01, true);
                lastAttack = weapon.TH_Light_Attack_01;
            }
            else
            {
                animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_01, true);
                lastAttack = weapon.OH_Light_Attack_01;
            }   
        }

        public void HandleHeavyAttack(WeaponItem weapon)
        {
            weaponSlotManager.attackingWeapon = weapon;
            if (inputHandler.twoHandFlag)
            {
                animatorHandler.PlayTargetAnimation(weapon.TH_Heavy_Attack_01, true);
                lastAttack = weapon.TH_Heavy_Attack_01;
            }
            else
            {
                animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_01, true);
                lastAttack = weapon.OH_Heavy_Attack_01;
            }
        }

        #region Defense actions
        public void HandleParryAction()
        {
            if (playerInventory.leftWeapon.isShieldWeapon)
            {
                PerformLeftWeaponArt(inputHandler.twoHandFlag);
            }
            else if (playerInventory.leftWeapon.isMeleeWeapon)
            {
                //perform light attack
            }
        }
        private void HandleBlockAction()
        {
            if (playerManager.isInteracting)
                return;

            if (playerManager.isBlocking)
                return;

            animatorHandler.PlayTargetAnimation("Block Start", false);
            playerEquipmentManager.OpenBlockingCollider();
            playerManager.isBlocking = true;
        }

        public void HandleQAction()
        {
            HandleBlockAction();
        }
        #endregion

        #region Attack actions
        private void PerformLeftWeaponArt(bool isTwoHanding)
        {
            if (playerManager.isInteracting)
                return;
            
            if (isTwoHanding)
            {
                //if we do be 2 handing, right hand art               
            }
            else
            {
                animatorHandler.PlayTargetAnimation(playerInventory.leftWeapon.weapon_Art, true);
            }
        }
        #endregion
    }
}
