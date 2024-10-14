using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OK
{
    [CreateAssetMenu(menuName = "A.I./Enemy Actions/Attack Action")]
    public class EnemyAttackAction : EnemyActions
    {
        public int attackScore = 3;
        public float recoveryTime = 2;

        public float maximumAttackAngle = 35;
        public float minimumAttackAngle = -35;

        public float minimumDistanceNeededToAttack = 0;
        public float maximumDistanceNeededToAttack = 1.5f;
    }
}

