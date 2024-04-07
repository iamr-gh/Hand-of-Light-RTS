using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

// rewriting to be cleaner, and change behaviors
// this will initially be based around normal unit behavior, enemies can have additional controllers
[RequireComponent(typeof(UnitAffiliation))]
[RequireComponent(typeof(UnitParameters))]
[RequireComponent(typeof(NavMeshAgent))] //nav agent will be stopped by setting its max speed to 0
// rigidbody should not be required
//weapon system may be changed to be controlled by AI
[RequireComponent(typeof(WeaponSystem))] // use of the weapon system needs to freeze target

public class UnitAIV2 : UnitAI
{
    // protected UnitAffiliation unitAffiliation;
    // protected UnitParameters unitParameters;
    // protected NavMeshAgent navAgent;
    // protected WeaponSystem weaponSystem;
    // protected GameObject target = null; //either friendly or enemy

    public UnityEvent reachedGoalEvent;
    public UnityEvent finishedAttackingEvent;

    public bool stopped = true;
    public float jitter_max_time = 1f;
    public float jitter_magnitude = 2.5f;
    protected float initial_nav_agent_speed;

    public float goal_leash = 3.5f; //if not within this distance from goal, will not stop with anti jitter
    public float goal_leash_move_cmd = 5f; //if not within this distance from goal, will not stop with anti jitter
    public float goal_leash_a_move = 1f; //if not within this distance from goal, will not stop with anti jitter
    private bool commanded = false;
    public float commandDelay = 1;

    public bool attacking = true; // the state of idling and willing to attack anyway in aggro range
    protected Vector3 last_goal;

    protected override void Start()
    {
        base.Start();
        initial_nav_agent_speed = navAgent.speed;
        last_goal = transform.position;
        StartCoroutine(antiJitterChecker());
        StartCoroutine(WeaponSystemManager());
    }

    // Update is called once per frame
    protected override void Update()
    {
        //provide a direct and real way to stop it
        if (stopped)
        {
            navAgent.speed = 0;
        }
        else
        {
            navAgent.speed = initial_nav_agent_speed;
        }

        //search for a target inside aggro range, go to it within weapon range, and while weapon active you are stopped
        if (attacking)
        {
            //only using enemy target rn
            //can easily be expanded with attack moving on a unit
            var old_target = target;
            target = FindNextTarget();

            //allow movement on target change
            if (old_target != target)
            {
                stopped = false;
            }

            if (target == null)
            {
                //no enemy, go back to leash spot
                //attack move triggers move command -- need to do it without leashing?
                // AttackMoveToCoordinate(last_goal);
                //might get killed by anti jitter
                navAgent.SetDestination(last_goal);
            }
            else
            {
                //if within weapon range, stop charging at opponent
                if (Vector3.Distance(target.transform.position, transform.position) <= parameters.getAttackRange())
                {
                    navAgent.SetDestination(transform.position);
                }
                else
                {
                    navAgent.SetDestination(target.transform.position);
                }
            }

        }
        else
        {
            //weapon system uses target
            target = null;
        }

        if (Vector3.Distance(last_goal, transform.position) <= 1.5f) {
            reachedGoalEvent.Invoke();
        }
    }

    //this could be refactored to be more efficient
    protected IEnumerator WeaponSystemManager()
    {
        yield return null;
        //this needs to be controlled in a stronger manner
        while (true)
        {
            yield return null;
            if (attacking && target != null && weaponSystem.isAttacking)
            {
                stopped = true;
                //wait for them to finish attacking
                while (weaponSystem.isAttacking)
                {
                    yield return null;
                }
                stopped = false;
            }
        }

    }


    //eventually might want different attack priorities, inside affiliation
    private GameObject FindNextTarget()
    {
        List<GameObject> enemies = new List<GameObject>(); // List of enemies within our aggro
                                                           // Get all objects within our aggro range
        List<GameObject> potentialEnemies = GlobalUnitManager.singleton.FindNearby(transform.position, parameters.getAggroRange());
        foreach (GameObject obj in potentialEnemies)
        {
            UnitAffiliation otherAff = obj.GetComponent<UnitAffiliation>();
            UnitParameters otherUnitParameters = obj.GetComponent<UnitParameters>();
            //If the other object does not have parameters or damage handling, do nothing
            if (otherAff == null || otherUnitParameters == null) { continue; }
            if (affiliation.affiliation != otherAff.affiliation) { enemies.Add(obj); }
        }

        float minDistance = float.MaxValue;
        GameObject priorityEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float enemyDistance = (enemy.transform.position - transform.position).magnitude;
            if (enemyDistance < minDistance)
            {
                minDistance = enemyDistance;
                priorityEnemy = enemy;
            }
        }
        return priorityEnemy;
    }

    //may need increased tiers of anti jitter checkers, but we'll start with one
    IEnumerator antiJitterChecker()
    {
        while (true)
        {
            // if we don't move for a bit, stop unit 
            var before = transform.position;
            yield return new WaitForSeconds(jitter_max_time);
            var after = transform.position;


            if (((before - after).magnitude / jitter_max_time) <= jitter_magnitude
                && !commanded
                && (navAgent.destination - transform.position).magnitude <= goal_leash)
            {
                stopped = true;
            }
            else
            {
                // Debug.Log("Jitter Magnitude:" + ((before - after).magnitude / jitter_max_time).ToString());
            }
        }
    }

    IEnumerator resetCommandLock()
    {
        yield return new WaitForSeconds(commandDelay);
        commanded = false;
    }


    //equivalent to a move command shift a command
    public override void MoveToCoordinate(Vector3 coord)
    {
        last_goal = coord;
        commanded = true;
        StartCoroutine(resetCommandLock());
        stopped = false;
        attacking = false;
        navAgent.SetDestination(coord);
        goal_leash = goal_leash_move_cmd; //move cmd goal leash is more charitable due to certain factors

        //switch to attack state after this
        //coroutine that waits until stoppage?
        //and target handling
        StartCoroutine(attackOnStop());
    }

    private IEnumerator attackOnStop()
    {
        while (!stopped)
        {
            yield return null;
        }
        attacking = true;
        goal_leash = goal_leash_a_move;
    }

    public override void AttackMoveToCoordinate(Vector3 coord)
    {
        last_goal = coord;
        commanded = true;
        StartCoroutine(resetCommandLock());
        stopped = false;
        attacking = true;

        navAgent.SetDestination(coord);
        goal_leash = goal_leash_a_move;
    }

    public override void Stop()
    {
        stopped = true;
    }


}
