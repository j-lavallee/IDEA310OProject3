using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrolling,
    Following,
    Attacking
}

public class EnemyController : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Renderer[] enemyRenderers;

    [Header("Settings")]
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float stopAtDistance = 0.5f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float viewAngle = 180f;
    [SerializeField] private float losePlayerTime = 3f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] public float enemyHealth = 100f;
    [SerializeField] public float playerDamage = 10f;
    [SerializeField] public float damageMult = 1f;
    [SerializeField] private float force = 1f;

    private NavMeshAgent agent;
    private Animator animator;
    private EnemyState state = EnemyState.Patrolling;
    private int currentPatrolIndex;
    private bool isWaiting;
    private float timeSinceLostPlayer;
    private bool isAttacking;
    private float attackTimeout = 1.5f;
    private float attackTimer;

    public healthBar healthBar;
    public enemyCounter enemyCounter;
    public GameObject ragdoll;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (enemyRenderers == null || enemyRenderers.Length == 0)
        {
            enemyRenderers = GetComponentsInChildren<Renderer>();
        }
    }

    private void Start()
    {
        GoToNextPatrolPoint();
    }

    private void Update()
    {
        var distanceToPlayer = Vector3.Distance(player.position, transform.position);

        switch (state)
        {
            case EnemyState.Patrolling:
                Patrol();

                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    state = EnemyState.Following;
                }

                break;

            case EnemyState.Following:
                FollowPlayer();

                if (distanceToPlayer <= attackRange)
                {
                    state = EnemyState.Attacking;
                    StartAttack();
                }

                if (!CanSeePlayer())
                {
                    timeSinceLostPlayer += Time.deltaTime;

                    if (timeSinceLostPlayer >= losePlayerTime)
                    {
                        state = EnemyState.Patrolling;
                        GoToClosestPatrolPoint();
                    }
                }
                else
                {
                    timeSinceLostPlayer = 0f;
                }

                break;

            case EnemyState.Attacking:
                Attack();
                attackTimer += Time.deltaTime;

                if (attackTimer >= attackTimeout)
                {
                    EndAttack();
                }

                if (!isAttacking && distanceToPlayer > attackRange)
                {
                    state = EnemyState.Following;
                    agent.isStopped = false;
                }

                break;

        }

        UpdateAnimations();
    }

    private void StartAttack()
    {
        agent.isStopped = true;
        isAttacking = true;
        attackTimer = 0f;
        animator.SetTrigger("Attack");
    }

    private void Attack()
    {
        agent.isStopped = true;
        var direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        agent.isStopped = false;
        attackTimer = 0f;
        var distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= attackRange)
        {
            StartAttack();
        }
        else
        {
            state = EnemyState.Following;
        }
    }

    private void FollowPlayer()
    {
        agent.SetDestination(player.position);
    }

    private void Patrol()
    {
        if (isWaiting) return;
        if (!agent.pathPending && agent.remainingDistance <= stopAtDistance)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(patrolWaitTime);

        agent.isStopped = false;
        GoToNextPatrolPoint();
        isWaiting = false;
    }

    private void GoToClosestPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        var closestIndex = 0;
        var closestDistance = float.MaxValue;

        for (var i = 0; i < patrolPoints.Length; i++)
        {
            var distance = Vector3.Distance(transform.position, patrolPoints[i].position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        currentPatrolIndex = closestIndex;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;

    }

    private void UpdateAnimations()
    {
        var isWalking = agent.velocity.sqrMagnitude > 0.01f;
        animator.SetBool(IsWalking, isWalking);
    }

    private bool CanSeePlayer()
    {
        return IsFacingPlayer() && HasClearPathToPlayer();
    }

    private bool IsFacingPlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        var angle = Vector3.Angle(transform.forward, dirToPlayer);

        return angle <= viewAngle / 2f;
    }

    private bool HasClearPathToPlayer()
    {
        var dirToPlayer = player.position - transform.position;

        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, dirToPlayer.magnitude))
        {
            return hit.transform == player;
        }

        return true;
    }

    private void damagePlayer()
    {
        var distanceToPlayer = Vector3.Distance(player.position,  transform.position);

        if (distanceToPlayer <= attackRange)
        {
            healthBar.health -= 15f;
        }

    }

    public void setMaterial(Material newMaterial)
    {
        if (enemyRenderers == null) return;

        foreach (Renderer renderer in enemyRenderers)
        {
            if (renderer == null) continue;

            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = newMaterial;
            }

            renderer.materials = materials;
        }
    }

    public void takeDamage()
    {
        enemyHealth -= playerDamage * damageMult;

        if (enemyHealth <= 0)
        {
            dead();
        }
    }

    private void dead()
    {
        enemyCounter.addOne();
        Vector3 pushDirection = (transform.position - player.position).normalized;
        Destroy(gameObject);
        var rg = Instantiate(ragdoll, transform.position, transform.rotation);

        foreach (Rigidbody bone in rg.GetComponentsInChildren<Rigidbody>())
        {
            bone.AddForce(pushDirection * force, ForceMode.VelocityChange);
        }
        
    }
}
