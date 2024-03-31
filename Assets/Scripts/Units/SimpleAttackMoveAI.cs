using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SimpleAttackMove : UnitAI
{
    //might need an idle state, or a patrol state in the future
    public enum UnitState { Moving, Attacking };
    // Start is called before the first frame update
    public float moveTolerance = 0.5f;
    public float moveLockTime = 1.0f; //this is the amount of time a unit is forced to obey a move command, before it can defend itself
    private bool moveLock = false;

    private UnitState unitState = UnitState.Moving;
    private Vector2 lastMoveGoal;

    protected override void Start()
    {
        base.Start();
        lastMoveGoal = new Vector2(transform.position.x, transform.position.z);
    }

    public override void MoveToCoordinate(Vector3 coord)
    {
        planner.enabled = true;
        base.MoveToCoordinate(coord);
        unitState = UnitState.Moving;
        lastMoveGoal = coord;
        moveLock = true;
        StartCoroutine(resetMoveLock());
    }
    
    IEnumerator resetMoveLock(){
        yield return new WaitForSeconds(moveLockTime);
        moveLock = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (unitState == UnitState.Moving)
        {
            if(!moveLock){
                target = FindNextTarget(); // change logic based on who exactly you track 
                if(target != null){
                    unitState = UnitState.Attacking;
                }
            }
            //should also have a timeout eventually 
            var pos2d = new Vector2(transform.position.x, transform.position.z);
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
            else
            {
                planner.enabled = true;
                var otherpos2d = new Vector2(target.transform.position.x, target.transform.position.z);
                //weapon system not triggering
                planner.goal = otherpos2d;
            }
        }
    }

    //copied from enemy AI, may end up tweaked in future
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

}
