using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackMove : UnitAI
{
    //might need an idle state, or a patrol state in the future
    public enum UnitState { Moving, Attacking };
    // Start is called before the first frame update
    public float moveTolerance = 0.5f;

    private UnitState unitState = UnitState.Moving;
    private Vector2 lastMoveGoal;

    protected override void Start()
    {
        base.Start();
        lastMoveGoal = new Vector2(transform.position.x, transform.position.z);
    }

    public override void MoveToCoordinate(Vector2 coord)
    {
        planner.enabled = true;
        base.MoveToCoordinate(coord);
        unitState = UnitState.Moving;
        lastMoveGoal = coord;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (unitState == UnitState.Moving)
        {
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
            // so for now


            target = FindNextTarget(); // change logic based on who exactly you track 
            var pos2d = new Vector2(transform.position.x, transform.position.z);
            if (target == null)
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
            else
            {
                planner.enabled = true;
                 var otherpos2d = new Vector2(target.transform.position.x, target.transform.position.z);
                 
                 //difference for ranged, if we are in range then stop moving(and just attack)
                if((otherpos2d-pos2d).magnitude > parameters.getAttackRange()){
                    planner.goal = otherpos2d;
                }
                else{
                    //if we are close enough, stop moving
                    planner.goal = pos2d;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    planner.enabled = false;
                }
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
