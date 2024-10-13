using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class CombatController : MonoBehaviour
{
    private CharacterStats stats;
    private Animator animator;

    public float attackCooldown = 1f;
    private float lastAttackTime;

    [SerializeField]
    private bool startInCoverState = false;

    private bool isCovering;
    public float coverDamageReduction = 0.5f;

    // Animator parameter hashes
    private int coverTriggerHash;
    private int uncoverTriggerHash;

    private void Awake()
    {
        stats = GetComponent<CharacterStats>();
        animator = GetComponent<Animator>();

        // Cache the parameter hashes
        coverTriggerHash = Animator.StringToHash("CoverTrigger");
        uncoverTriggerHash = Animator.StringToHash("UncoverTrigger");
    }

    private void Start()
    {
        // Initialize the cover state
        isCovering = startInCoverState;
        if (isCovering)
        {
            animator.SetTrigger(coverTriggerHash);
        }
        else
        {
            animator.SetTrigger(uncoverTriggerHash);
        }

        // Force the Animator to update immediately
        animator.Update(0f);

        Debug.Log("Initial state - isCovering: " + isCovering);
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack(1);
        }
        else if (Input.GetMouseButtonDown(1) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack(2);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCover();
        }
    }

    private void Attack(int attackType)
    {
        lastAttackTime = Time.time;
        animator.SetTrigger(attackType == 1 ? "Attack1Trigger" : "Attack2Trigger");
        Debug.Log(gameObject.name + " performs Attack " + attackType);
    }

    private void ToggleCover()
    {
        isCovering = !isCovering;
        if (isCovering)
        {
            animator.SetTrigger(coverTriggerHash);
            Debug.Log(gameObject.name + " is covering. Animation should show cover.");
        }
        else
        {
            animator.SetTrigger(uncoverTriggerHash);
            Debug.Log(gameObject.name + " stopped covering. Animation should show normal stance.");
        }

        // Force the Animator to update immediately
        animator.Update(0f);

        Debug.Log("Current state - isCovering: " + isCovering);
    }

    public void TakeDamage(int damage)
    {
        if (isCovering)
        {
            damage = Mathf.FloorToInt(damage * (1 - coverDamageReduction));
        }
        stats.TakeDamage(damage);
    }

    public void ApplyDamage()
    {
        Debug.Log(gameObject.name + " applies damage!");
    }
}
