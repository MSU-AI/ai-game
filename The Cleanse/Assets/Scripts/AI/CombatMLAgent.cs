
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using OK;

public class CombatMLAgent : Agent
{
    [Header("References")]
    public EnemyManager enemyManager;
    public EnemyStats enemyStats;
    public EnemyAnimatorManager animatorManager;
    public Transform opponent;

    [Header("Combat Settings")]
    public float maxAttackRange = 2f;
    public float optimalRange = 1.5f;
    public float attackCooldown = 1f;

    [Header("Movement Settings")]
    public float movementSpeed = 5f;

    [Header("Reward Settings")]
    public float positioningRewardScale = 0.01f;
    public float facingRewardScale = 0.005f;
    public float movementPenaltyScale = 0.001f;
    public float stalemateTimeThreshold = 5f;
    public float stalemateMultiplier = 0.1f;

    private float lastAttackTime;
    private bool isInCombat;
    private Rigidbody rb;
    private float timeSinceLastAction;
    private Vector3 lastPosition;
    private float episodeTime;
    private float episodeTimer = 0f;
    private float maxEpisodeTime = 30f;

    private void Awake()
    {
        if (enemyManager == null) enemyManager = GetComponent<EnemyManager>();
        if (enemyStats == null) enemyStats = GetComponent<EnemyStats>();
        if (animatorManager == null) animatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        rb = GetComponent<Rigidbody>();

        lastAttackTime = -attackCooldown;
        lastPosition = transform.position;
        timeSinceLastAction = 0f;
        episodeTime = 0f;
    }

    public override void OnEpisodeBegin()
    {

        episodeTimer = 0f;
        if (enemyStats != null)
        {
            enemyStats.currentHealth = enemyStats.maxHealth;
        }

        lastAttackTime = -attackCooldown;
        isInCombat = false;
        timeSinceLastAction = 0f;
        episodeTime = 0f;
        lastPosition = transform.position;

        if (transform.localPosition.y < 0 && rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.localPosition = new Vector3(0, 0.5f, 0);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Always collect position (3 values)
        sensor.AddObservation(transform.localPosition);

        // Direction to opponent and distance (4 values total)
        Vector3 directionToOpponent = Vector3.zero;
        float distanceToOpponent = 0f;

        if (opponent != null)
        {
            directionToOpponent = (opponent.position - transform.position).normalized;
            distanceToOpponent = Vector3.Distance(transform.position, opponent.position);
        }

        // Add direction (3 values) and distance (1 value)
        sensor.AddObservation(directionToOpponent);
        sensor.AddObservation(distanceToOpponent);

        // Health ratio (1 value)
        float healthRatio = 0f;
        if (enemyStats != null)
        {
            healthRatio = enemyStats.currentHealth / (float)enemyStats.maxHealth;
        }
        sensor.AddObservation(healthRatio);

        // Debug log to verify observations
        Debug.Log($"[{gameObject.name}] Observations collected:" +
                  $"\nPosition: {transform.localPosition} (3 values)" +
                  $"\nDirection: {directionToOpponent} (3 values)" +
                  $"\nDistance: {distanceToOpponent} (1 value)" +
                  $"\nHealth: {healthRatio} (1 value)" +
                  $"\nTotal: 8 values");
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        episodeTimer += Time.deltaTime;
        if (episodeTimer >= maxEpisodeTime)
        {
            AddReward(-0.5f); // Penalty for timeout
            EndEpisode();
            return;
        }

        if (!isInCombat || opponent == null) return;

        episodeTime += Time.deltaTime;
        timeSinceLastAction += Time.deltaTime;

        var continuousActions = actionBuffers.ContinuousActions;
        var discreteActions = actionBuffers.DiscreteActions;

        Vector3 beforeMovePosition = transform.position;

        Vector3 move = new Vector3(continuousActions[0], 0, continuousActions[1]);
        move = transform.TransformDirection(move).normalized;

        if (rb != null)
        {
            rb.MovePosition(rb.position + move * movementSpeed * Time.deltaTime);
        }

        float distanceMoved = Vector3.Distance(beforeMovePosition, transform.position);
        AddReward(-distanceMoved * movementPenaltyScale);

        float rotation = continuousActions[2] * enemyManager.rotationSpeed;
        transform.Rotate(0, rotation * Time.deltaTime, 0);

        if (CanAttack())
        {
            switch (discreteActions[0])
            {
                case 1:
                    PerformBasicAttack();
                    timeSinceLastAction = 0f;
                    break;
                case 2:
                    PerformHeavyAttack();
                    timeSinceLastAction = 0f;
                    break;
            }
        }

        CalculateRewards();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        var discreteActions = actionsOut.DiscreteActions;

        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
        continuousActions[2] = Input.GetAxis("Mouse X");

        if (Input.GetMouseButtonDown(0))
            discreteActions[0] = 1;
        else if (Input.GetMouseButtonDown(1))
            discreteActions[0] = 2;
        else
            discreteActions[0] = 0;
    }

    private bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown && !enemyManager.isPerformingAction;
    }

    private void PerformBasicAttack()
    {
        lastAttackTime = Time.time;
        animatorManager.PlayTargetAnimation("Attack1", true);
        enemyManager.isPerformingAction = true;
    }

    private void PerformHeavyAttack()
    {
        lastAttackTime = Time.time;
        animatorManager.PlayTargetAnimation("Attack2", true);
        enemyManager.isPerformingAction = true;
    }

    private void CalculateRewards()
    {
        if (opponent == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Cannot calculate rewards - no opponent");
            return;
        }

        float distanceToOpponent = Vector3.Distance(transform.position, opponent.position);

        // Original positioning reward
        float positioningReward = Mathf.Exp(-Mathf.Pow(distanceToOpponent - optimalRange, 2) / 2);
        AddReward(positioningReward * positioningRewardScale);

        // New proximity reward
        float proximityReward = 1.0f - (distanceToOpponent / maxAttackRange);
        AddReward(proximityReward * 0.1f);

        // Extra reward for getting into attack range
        if (distanceToOpponent <= maxAttackRange)
        {
            AddReward(0.2f);
        }

        // Original facing reward
        Vector3 directionToOpponent = (opponent.position - transform.position).normalized;
        float facingReward = Vector3.Dot(transform.forward, directionToOpponent);
        AddReward(facingReward * facingRewardScale);

        // Original stalemate penalty
        if (timeSinceLastAction > stalemateTimeThreshold)
        {
            float stalemateTime = timeSinceLastAction - stalemateTimeThreshold;
            float stalematePenalty = stalemateTime * stalemateMultiplier;
            AddReward(-stalematePenalty);
        }

        // Original out of bounds penalty
        if (transform.position.y < 0)
        {
            Debug.Log($"[{gameObject.name}] Out of bounds - ending episode with penalty");
            AddReward(-1f);
            EndEpisode();
        }
    }

    public void OnDamageDealt(float damage)
    {
        float reward = damage * 0.2f;
        AddReward(reward);
        timeSinceLastAction = 0f;
    }

    public void OnDamageTaken(float damage)
    {
        float penalty = damage * 0.05f;
        AddReward(-penalty);

        if (enemyStats != null && enemyStats.currentHealth <= 0)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    public void SetCombatState(bool inCombat)
    {
        isInCombat = inCombat;
    }
}
