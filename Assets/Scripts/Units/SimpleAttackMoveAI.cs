using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SimpleAttackMove : UnitAI
{
    //might need an idle state, or a patrol state in the future
    public enum UnitState { Moving, Attacking };
    // Start is called before the first frame update
    public float moveTolerance = 0.5f;
    // public float moveLockTime = 1.0f; //this is the amount of time a unit is forced to obey a move command, before it can defend itself
    // private bool moveLock = false;

    protected UnitState unitState = UnitState.Moving;
    protected Vector3 lastMoveGoal;

    protected override void Start()
    {
        base.Start();
        lastMoveGoal = new Vector2(transform.position.x, transform.position.z);
    }

    public override void MoveToCoordinate(Vector3 coord)
    {
        // planner.enabled = true;
        base.MoveToCoordinate(coord);
        lastMoveGoal = coord;
        unitState = UnitState.Moving;
    }

    public override void AttackMoveToCoordinate(Vector3 coord)
    {
        base.AttackMoveToCoordinate(coord);
        lastMoveGoal = coord;
        unitState = UnitState.Moving;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (unitState == UnitState.Moving)
        {
            //should also have a timeout eventually 
            var pos2d = new Vector2(transform.position.x, transform.position.z);
            if(navAgent != null){
                if((navAgent.destination - transform.position).magnitude< moveTolerance){
                    unitState = UnitState.Attacking;
                    return;
                }
            }
            if (planner == null)
            {
                return;
            }
            if ((pos2d - planner.goal).magnitude < moveTolerance)
            {
                unitState = UnitState.Attacking;
            }
        }
        else if (unitState == UnitState.Attacking)
        {
            //eventually have a tie in to dmg system/weapon 
            target = FindNextTarget(); // change logic based on who exactly you track 
            if (target == null)
            {

                var pos2d = new Vector2(transform.position.x, transform.position.z);
                if (navAgent != null)
                {
                    navAgent.SetDestination(lastMoveGoal);
                }
                else
                {
                    planner.goal = lastMoveGoal;

                    //turn off planner if we're fine
                    if ((pos2d - planner.goal).magnitude < moveTolerance)
                    {
                        // planner.goal = pos2d;
                        // there's a jitter issue I don't understand
                        planner.enabled = false;
                        GetComponent<Rigidbody>().velocity = Vector3.zero;
                    }
                }
            }
            else
            {
                //stop if close enough to target
                if(weaponSystem.TargetInRange()){
                    Debug.Log("Trying to stop");
                    if(navAgent != null){
                        navAgent.SetDestination(transform.position);
                    }
                }
                else{
                    var otherpos2d = new Vector2(target.transform.position.x, target.transform.position.z);
                    if (navAgent != null){
                        Debug.Log("Setting attack to enemy");
                        navAgent.SetDestination(target.transform.position);
                    }
                    else{

                        planner.enabled = true;
                        //weapon system not triggering
                        planner.goal = otherpos2d;
                    }
                }
            }
        }
    }

    //copied from enemy AI, may end up tweaked in future
    protected GameObject FindNextTarget()
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

}
