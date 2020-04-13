using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    public enum FSMStates
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Dead
    }

    public FSMStates currentState;

    public float attackDistance = 2.5f;
    public float chaseDistance = 6;
    public GameObject player;

    public Transform enemyEyes;
    public float FOV; 
    
    private PlayerHealth playerHealth; 
	public float attackRate = 2;
	public GameObject deadVFX; 

    GameObject[] wanderPoints;
    Vector3 nextDestination;
    Animator anim;
    float distanceToPlayer;
	float elapsedTime = 0; 
	//EnemyHealth enemyHealth; 
	int health = 1; 
    int currentDestinationIndex = 0;
	Transform deadTransform; 
	bool isDead;

	private NavMeshAgent agent; 
	
	
    void Start()
    {
		wanderPoints = GameObject.FindGameObjectsWithTag("WanderPoint");
        
		anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player");

        playerHealth = player.GetComponent<PlayerHealth>(); 
		
		isDead = false;
		
        Initialize();

    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, 
            player.transform.position);
			
		//health = enemyHealth.currentHealth;

        switch (currentState)
        {
            case FSMStates.Patrol:
                UpdatePatrolState();
                break;
            case FSMStates.Chase:
                UpdateChaseState();
                break;
            case FSMStates.Attack:
                UpdateAttackState();
                break;
            case FSMStates.Dead:
                UpdateDeadState();
                break;
        }
                
        elapsedTime += Time.deltaTime; 
		
		if(health <=0) {
			currentState = FSMStates.Dead; 
		}
    }

    void Initialize()
    {
		currentState = FSMStates.Patrol;

        FindNextPoint();
    }

    void UpdatePatrolState()
    {
        print("Patrolling!");

        anim.SetInteger("animState", 1);

        agent.stoppingDistance = attackDistance;

        agent.speed = 2f;

        if(Vector3.Distance(transform.position, nextDestination) < 3)
        {
            FindNextPoint();
        }
        else if(InFOV())
        {
            currentState = FSMStates.Chase;
        }

        FaceTarget(nextDestination);

        agent.SetDestination(nextDestination); 

    }

    void UpdateChaseState()
    {
        anim.SetInteger("animState", 2);
        
        agent.stoppingDistance = attackDistance;

        agent.speed = 3f; 

        nextDestination = player.transform.position;

        if(distanceToPlayer <= attackDistance)
        {
            currentState = FSMStates.Attack;
        }
        else if(distanceToPlayer > chaseDistance)
        {
	        FindNextPoint();
            currentState = FSMStates.Patrol;
        }

        FaceTarget(nextDestination);
        
        agent.SetDestination(nextDestination); 
    }

    void UpdateAttackState()
    {
        print("attack");
		nextDestination = player.transform.position; 
		
		agent.stoppingDistance = attackDistance;
		
		if (distanceToPlayer <= attackDistance) {
			currentState = FSMStates.Attack;
		}
		else if (distanceToPlayer > attackDistance && distanceToPlayer <= chaseDistance) {
			currentState = FSMStates.Chase; 
		}
		else if(distanceToPlayer > chaseDistance) {
			currentState = FSMStates.Patrol;
		}
		FaceTarget(nextDestination); 
		anim.SetInteger("animState", 3);
		
		EnemyAttack(); 
    }

    void UpdateDeadState()
    {
	    agent.speed = 0; 
		isDead = true; 
		anim.SetInteger("animState", 4); 
		deadTransform = transform; 
		Destroy(gameObject, 3); 
    }

    void FindNextPoint()
    {
        nextDestination = wanderPoints[currentDestinationIndex].transform.position;

        // Promote random wandering vs. circular wandering
        int randomDes = Random.Range(0, wanderPoints.Length);

        if (randomDes == currentDestinationIndex)
        {
	        currentDestinationIndex = (currentDestinationIndex + 1)
	                                  % wanderPoints.Length;
        }
        else
        {
	        currentDestinationIndex = randomDes; 
        }

        agent.SetDestination(nextDestination);

    }

    void FaceTarget(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;
		directionToTarget.y = 0; 
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp
            (transform.rotation, lookRotation, 10 * Time.deltaTime);
    }
	
	void EnemyAttack() {
		if (elapsedTime >= attackRate && !isDead){
			var animDur = anim.GetCurrentAnimatorStateInfo(0).length; 
			Invoke(nameof(Attacking), animDur); 
			elapsedTime = 0.0f; 
		}
	}
	
	void Attacking() {
		playerHealth.dealDamage(Random.Range(5,15));
	}
	
	private void OnDestroy() {
		try
		{
			Instantiate(deadVFX, deadTransform.position, Quaternion.Euler(new Vector3(-90, 0, 0))); 
		}
		catch (Exception e)
		{
			// Clean up console log errors when an EnemyAI gets destroyed on scene change
			Debug.Log("Handled -> " + e.Message);
		}
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Vector3 frontRay = enemyEyes.position + (enemyEyes.forward * chaseDistance);
        Vector3 leftRay = Quaternion.Euler(0, FOV * 0.5f, 0) * frontRay; 
        Vector3 rightRay = Quaternion.Euler(0, -FOV * 0.5f, 0) * frontRay; 

		Debug.DrawLine(enemyEyes.position, frontRay, Color.cyan);
		Debug.DrawLine(enemyEyes.position, leftRay, Color.cyan);
		Debug.DrawLine(enemyEyes.position, rightRay, Color.cyan);
    }

    private bool InFOV()
    {
	    RaycastHit hit; 
	    
	    Vector3 directionToPlayer = player.transform.position - enemyEyes.position;

	    if (Vector3.Angle(directionToPlayer, enemyEyes.forward) <= FOV)
	    {
		    if (Physics.Raycast(enemyEyes.position, directionToPlayer, out hit, chaseDistance))
		    {
			    return hit.collider.CompareTag("Player");
		    }

		    return false; 
	    }

	    return false; 
    }

    public void Die()
    {
	    currentState = FSMStates.Dead; 
    }
}
