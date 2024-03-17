using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GlobalUnitManager : MonoBehaviour
{
    
    //we're doing singleton
    public static GlobalUnitManager singleton;
    //maps affiliation to unit type
    private Dictionary<String,List<GameObject>> units = new();
    
    private GameObject [] allManaged;

    private List<Type> unitTypes = new();

    // Start is called before the first frame update
    void Start()
    {
        //there will be exactly one of these
        if(singleton == null){
            singleton = this;
        }
        else{
            Destroy(gameObject);
        }
        
        

       //goal is to index once at start, be smart about reindexing
       //could maybe reindex with a different method
        allManaged =  GameObject.FindGameObjectsWithTag("Managed");
        
        //parse into specific types
        foreach (GameObject obj in allManaged){
            if(obj != null){
                if(obj.TryGetComponent(out UnitAffiliation unitaff)){
                    // units.Add()
                    if(units.TryGetValue(unitaff.affiliation, out List<GameObject> lst)){
                        lst.Add(obj);
                    }
                    else{
                        units.Add(unitaff.affiliation,new List<GameObject>{obj});
                    }
                }
                if (obj.TryGetComponent(out UnitController unitController)) {
                    var type = unitController.GetType();
                    if (!unitTypes.Contains(type)) {
                        unitTypes.Add(type);
                    }
                }
            }
        }
    }

    public List<GameObject> FindNearby(Vector3 pos, float radius){
        var objs = new List<GameObject>();
        //eventually convert into a spatial hash to not be N^2
        foreach(GameObject obj in allManaged){
            //3d distance
            if((obj.transform.position - pos).magnitude <= radius){
               objs.Add(obj);
            }
        }

        return objs;
    }

    public List<GameObject> FindInBox(Vector3 bottomLeft, Vector3 topRight) {
        var objs = new List<GameObject>();
        foreach (GameObject obj in allManaged) {
            var pos = obj.transform.position;
            if (pos.x > bottomLeft.x && pos.x < topRight.x && pos.z > bottomLeft.z && pos.z < topRight.z) {
                objs.Add(obj);
            }
        }
        return objs;
    }

    public List<GameObject> FindByType(int idx) {
        var objs = new List<GameObject>();
        var type = unitTypes[idx];
        foreach (GameObject obj in allManaged) {
            if (obj.TryGetComponent(out UnitController unitController) && unitController.GetType() == type) {
                objs.Add(obj);
            }
        }
        return objs;
    }

    public List<Type> GetUnitTypes() {
        return unitTypes;
    }
    // void Update()
    // {
    //eventually use a spatial hash    
    // }
}
