using System.Collections;
using System.Collections.Generic;
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

    private bool commanded = false;
    public float commandDelay = 1;

    protected override void Start()
    {
        base.Start();
        initial_nav_agent_speed = navAgent.speed;
        StartCoroutine(antiJitterChecker());
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

    public override void MoveToCoordinate(Vector3 coord)
    {
        commanded = true;
        StartCoroutine(resetCommandLock());
        stopped = false;
        navAgent.SetDestination(coord);
        //switch to attack state after this
        //and target handling
    }

    public override void AttackMoveToCoordinate(Vector3 coord)
    {
        commanded = true;
        StartCoroutine(resetCommandLock());
        stopped = false;
    }

    public override void Stop()
    {
        stopped = true;
    }


}
