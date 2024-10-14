using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OK
{
    [CreateAssetMenu(menuName = "Items/Weapons")]
    public class WeaponItem : Item
    {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("Damage")]
        public int baseDamage = 25;
        public int criticalDamageMultiplier = 4;

        [Header("Absorption")]
        public float physicalDamageAbsorption;

        [Header("Idle Animations")]
        public string right_Hand_Idle;
        public string left_Hand_Idle;
        public string th_Idle;
        
        [Header("One Handed Attack Animations")]
        public string OH_Light_Attack_01;
        public string OH_Light_Attack_02;
        public string OH_Heavy_Attack_01;
        public string TH_Light_Attack_01;
        public string TH_Light_Attack_02;
        public string TH_Heavy_Attack_01;

        [Header("Weapon Art")]
        public string weapon_Art;

        [Header("Weapon Type")]
        public bool isMeleeWeapon;
        public bool isShieldWeapon;
    }

}
