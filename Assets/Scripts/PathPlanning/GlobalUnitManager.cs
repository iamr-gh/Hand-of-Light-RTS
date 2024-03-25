using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUnitManager : MonoBehaviour {

    //we're doing singleton
    public static GlobalUnitManager singleton;
    //maps affiliation to unit type
    private Dictionary<String, List<GameObject>> units = new();

    public GameObject[] allManaged;

    private List<Type> unitTypes = new();

    // Start is called before the first frame update
    void Start() {
        //there will be exactly one of these
        if (singleton == null) {
            singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }



        //goal is to index once at start, be smart about reindexing
        //could maybe reindex with a different method
        allManaged = GameObject.FindGameObjectsWithTag("Managed");

        TryGetComponent(out ControlSystem controlSystem);

        //parse into specific types
        foreach (GameObject obj in allManaged) {
            //might have null refs, but that's ok
            if (obj != null) {
                if (obj.TryGetComponent(out UnitAffiliation unitaff)) {
                    // units.Add()
                    if (units.TryGetValue(unitaff.affiliation, out List<GameObject> lst)) {
                        lst.Add(obj);
                    } else {
                        units.Add(unitaff.affiliation, new List<GameObject> { obj });
                    }

                    if (controlSystem != null && unitaff.affiliation == controlSystem.affiliation && obj.TryGetComponent(out UnitAI unitAI)) {
                        var type = unitAI.GetType();
                        if (!unitTypes.Contains(type)) {
                            unitTypes.Add(type);
                        }
                    }
                }
            }
        }
    }

    public List<GameObject> FindNearby(Vector3 pos, float radius) {
        var objs = new List<GameObject>();
        //eventually convert into a spatial hash to not be N^2
        foreach (GameObject obj in allManaged) {
            if (obj == null) continue;
            //3d distance
            if ((obj.transform.position - pos).magnitude <= radius) {
                objs.Add(obj);
            }
        }

        return objs;
    }

    public List<GameObject> FindInBox(Vector3 bottomLeft, Vector3 topRight) {
        var objs = new List<GameObject>();
        foreach (GameObject obj in allManaged) {
            if (obj == null) continue;
            var pos = obj.transform.position;
            if (pos.x >= bottomLeft.x && pos.x <= topRight.x && pos.z >= bottomLeft.z && pos.z <= topRight.z) {
                objs.Add(obj);
            }
        }
        return objs;
    }

    public List<GameObject> FindInTrapezoid(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight) {
        var objs = new List<GameObject>();
        foreach (GameObject obj in allManaged) {
            if (obj == null) continue;
            var pos = obj.transform.position;
            Func<float, float> leftLineF = y => ((topLeft - bottomLeft).normalized * (y - bottomLeft.z) + bottomLeft).x;
            Func<float, float> rightLineF = y => ((topRight - bottomRight).normalized * (y - bottomRight.z) + bottomRight).x;
            if (pos.z >= bottomLeft.z && pos.z <= topRight.z && pos.x >= leftLineF(pos.z) && pos.x <= rightLineF(pos.z)) {
                objs.Add(obj);
            }
        }
        return objs;
    }

    public List<GameObject> FindByType(Type type) {
        var objs = new List<GameObject>();
        foreach (GameObject obj in allManaged) {
            if (obj == null) continue;
            if (obj.TryGetComponent(out UnitAI unitAI) && unitAI.GetType() == type) {
                objs.Add(obj);
            }
        }
        return objs;
    }

    public List<GameObject> FindByTypeIdx(int idx) {
        var objs = new List<GameObject>();
        var type = unitTypes[idx];
        foreach (GameObject obj in allManaged) {
            if (obj == null) continue;
            if (obj.TryGetComponent(out UnitAI unitAI) && unitAI.GetType() == type) {
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
