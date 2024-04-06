using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
    public bool stopped = true;
    public float jitter_max_time = 0.5f;
    public float jitter_magnitude = 0.1f;
    private float initial_nav_agent_speed;

    public float goal_leash = 5f; //if not within this distance from goal, will not stop with anti jitter
    public float goal_leash_move_cmd = 5f; //if not within this distance from goal, will not stop with anti jitter
    public float goal_leash_a_move = 1f; //if not within this distance from goal, will not stop with anti jitter

    private bool commanded = false;

    public float commandDelay = 1;

    private bool attacking = true; // the state of idling and willing to attack anyway in aggro range
    private Vector3 last_goal;

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
            target = FindNextTarget();
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
                navAgent.SetDestination(target.transform.position);
                //if within range, stop and attack -- HANDLED BY COROUTINE
            }

        }
        else
        {
            //weapon system uses target
            target = null;
        }
    }

    private IEnumerator WeaponSystemManager()
    {
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
                Debug.Log("Jitter Magnitude:" + ((before - after).magnitude / jitter_max_time).ToString());
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
