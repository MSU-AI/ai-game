using UnityEngine;
using UnityEngine.AI;

namespace OK
{
    public class AttackState : State
    {
        public CombatStanceState combatStanceState;
        public EnemyAttackAction[] enemyAttacks;
        public EnemyAttackAction currentAttack;
        private CombatMLAgent mlAgent;
        private bool isMLControlled = false;

        private void Start()
        {
            // Try to get ML Agent right at start
            mlAgent = GetComponent<CombatMLAgent>();
            if (mlAgent != null)
            {
                isMLControlled = true;
                Debug.Log($"[{gameObject.name}] ML Agent found in AttackState");
            }
        }

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            // If ML Agent exists but hasn't been initialized
            if (mlAgent == null)
            {
                mlAgent = enemyManager.GetComponent<CombatMLAgent>();
                if (mlAgent != null)
                {
                    isMLControlled = true;
                    Debug.Log($"[{gameObject.name}] ML Agent found during Tick");
                }
            }

            // Set combat state as soon as we enter attack state
            if (isMLControlled && enemyManager.currentTarget != null)
            {
                mlAgent.opponent = enemyManager.currentTarget.transform;
                mlAgent.SetCombatState(true);
                Debug.Log($"[{gameObject.name}] Combat state set to true, opponent assigned");
            }

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

            HandleRotateTowardsTarget(enemyManager);

            // If using ML Agent and in combat range
            if (isMLControlled && IsInCombatRange(distanceFromTarget))
            {
                return this;
            }
            // Fallback to original logic if ML Agent not present or not in range
            else if (!isMLControlled)
            {
                return HandleOriginalAttackLogic(enemyManager, enemyAnimatorManager, distanceFromTarget, viewableAngle);
            }

            return combatStanceState;
        }

        private bool IsInCombatRange(float distance)
        {
            return distance <= enemyAttacks[0].maximumDistanceNeededToAttack &&
                   distance >= enemyAttacks[0].minimumDistanceNeededToAttack;
        }

        private State HandleOriginalAttackLogic(EnemyManager enemyManager, EnemyAnimatorManager enemyAnimatorManager, float distanceFromTarget, float viewableAngle)
        {
            if (enemyManager.isPerformingAction)
                return combatStanceState;

            if (currentAttack != null)
            {
                if (distanceFromTarget < currentAttack.minimumDistanceNeededToAttack)
                {
                    return this;
                }
                else if (distanceFromTarget < currentAttack.maximumDistanceNeededToAttack)
                {
                    if (viewableAngle <= currentAttack.maximumAttackAngle
                        && viewableAngle >= currentAttack.minimumAttackAngle)
                    {
                        if (enemyManager.currentRecoveryTime <= 0 && enemyManager.isPerformingAction == false)
                        {
                            enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                            enemyAnimatorManager.anim.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
                            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
                            enemyManager.isPerformingAction = true;
                            enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
                            currentAttack = null;
                            return combatStanceState;
                        }
                    }
                }
            }
            else
            {
                GetNewAttack(enemyManager);
            }

            return combatStanceState;
        }

        private void HandleRotateTowardsTarget(EnemyManager enemyManager)
        {
            // Rotate Manually
            if (enemyManager.isPerformingAction)
            {
                Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                {
                    direction = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed * Time.deltaTime);
            }
            // Rotate with NavMesh
            else
            {
                Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navmeshAgent.desiredVelocity);
                Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;

                enemyManager.navmeshAgent.enabled = true;
                enemyManager.navmeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
                enemyManager.enemyRigidBody.velocity = targetVelocity;
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navmeshAgent.transform.rotation, enemyManager.rotationSpeed * Time.deltaTime);
            }
        }

        private void GetNewAttack(EnemyManager enemyManager)
        {
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);

            int maxScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                && distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle
                    && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                    {
                        maxScore += enemyAttackAction.attackScore;
                    }
                }
            }

            int randomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                && distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle
                    && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                    {
                        if (currentAttack != null)
                            return;

                        temporaryScore += enemyAttackAction.attackScore;

                        if (temporaryScore > randomValue)
                        {
                            currentAttack = enemyAttackAction;
                        }
                    }
                }
            }
        }
    }
}