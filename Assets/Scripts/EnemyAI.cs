using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    //public float enemySpeed = 5;
    public GameObject player;

    private PlayerHealth playerHealth; 
	//public GameObject[] spellProjectiles; 
//	public GameObject wandTip; 
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
		
		//wandTip = GameObject.FindGameObjectWithTag("WandTip");
		
		//enemyHealth = GetComponent<EnemyHealth>();
		
		//health = enemyHealth.currentHealth; 
		
		isDead = false;
		
        Initialize();

    }

    // Update is called once per frame
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

        agent.speed = 1.5f;

        if(Vector3.Distance(transform.position, nextDestination) < 2)
        {
            FindNextPoint();
        }
        else if(distanceToPlayer <= chaseDistance)
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

        agent.speed = 3; 

        nextDestination = player.transform.position;

        if(distanceToPlayer <= attackDistance)
        {
            currentState = FSMStates.Attack;
        }
        else if(distanceToPlayer > chaseDistance)
        {
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

        currentDestinationIndex = (currentDestinationIndex + 1) 
            % wanderPoints.Length;

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
		Instantiate(deadVFX, deadTransform.position, Quaternion.Euler(new Vector3(-90, 0, 0))); 
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

    }

    public void Die()
    {
	    currentState = FSMStates.Dead; 
    }
}
