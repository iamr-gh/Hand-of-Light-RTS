using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class UnitPortraitData {
    public string type;
    public Sprite sprite;
    public Color color;
}

public class GlobalUnitManager : MonoBehaviour {

    //we're doing singleton
    public static GlobalUnitManager singleton;

    public List<UnitPortraitData> portraits;

    //maps affiliation to unit type
    public Dictionary<string, List<GameObject>> units = new();

    private List<GameObject> visibleEnemies = new();
    private List<GameObject> hiddenEnemies = new();

    public GameObject[] allManaged;

    private List<string> unitTypes = new();

    private LayerMask groundLayerMask;

    private int unitLayer;
    private int hiddenUnitLayer;

    public Plane groundPlane;

    void Awake() {
        // there will be exactly one of these
        if (singleton == null) {
            singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start() {

        groundLayerMask = LayerMask.GetMask("Ground");
        groundPlane = new Plane(Vector3.up, 3);

        unitLayer = LayerMask.NameToLayer("Units");
        hiddenUnitLayer = LayerMask.NameToLayer("Hidden Units");



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

                    if (controlSystem != null) {
                        if (unitaff.affiliation == controlSystem.affiliation) {
                            var type = unitaff.unit_type;
                            if (!unitTypes.Contains(type)) {
                                unitTypes.Add(type);
                            }
                        } else {
                            HideUnit(obj);
                            hiddenEnemies.Add(obj);
                        }
                    }
                }
            }
        }
    }

    void Update() {
        if (FogOfWarManager.instance != null) {
            UpdateFogOfWar();
        }
    }

    public void Reindex() {
        allManaged = GameObject.FindGameObjectsWithTag("Managed");
        TryGetComponent(out ControlSystem controlSystem);
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

                    if (controlSystem != null && unitaff.affiliation == controlSystem.affiliation) {
                        var type = unitaff.unit_type;
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

    public bool CanFriendlySee(Vector3 pos) {
        foreach (GameObject obj in units[ControlSystem.instance.affiliation]) {
            if (obj == null) continue;
            if (obj.TryGetComponent(out UnitParameters unitParams)) {
                if ((obj.transform.position - pos).magnitude <= unitParams.getSightRange()) {
                    return true;
                }
            }
        }
        return false;
    }

    public List<GameObject> FindNearMouse(float radius) {
        var objs = new List<GameObject>();
        Vector3 pos;
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask)) {
            pos = hit.point;
        } else {
            float goalEnter;
            groundPlane.Raycast(ray, out goalEnter);
            pos = ray.GetPoint(goalEnter);
        }
        //eventually convert into a spatial hash to not be N^2
        foreach (GameObject obj in allManaged) {
            if (obj == null) continue;
            //2d distance
            var pos2 = new Vector2(pos.x, pos.z);
            var objPos2 = new Vector2(obj.transform.position.x, obj.transform.position.z);
            if (Vector2.Distance(pos2, objPos2) <= radius) {
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

    public List<GameObject> FindByType(String type) {
        var objs = new List<GameObject>();
        foreach (GameObject obj in allManaged) {
            if (obj == null) continue;
            if (obj.TryGetComponent(out UnitAffiliation unitAffiliation) && unitAffiliation.unit_type == type) {
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
            if (obj.TryGetComponent(out UnitAffiliation unitaff) && unitaff.unit_type == type) {
                objs.Add(obj);
            }
        }
        return objs;
    }

    public List<string> GetUnitTypes() {
        return unitTypes;
    }

    public (Sprite, Color) GetPortrait(string type) {
        var idx = portraits.FindIndex(portraitData => portraitData.type == type);
        if (idx == -1) return (null, Color.white);
        return (portraits[idx].sprite, portraits[idx].color);
    }

    void UpdateFogOfWar() {
        visibleEnemies = visibleEnemies.Where(unit => unit != null).ToList();
        hiddenEnemies = hiddenEnemies.Where(unit => unit != null).ToList();
        var toHide = new HashSet<int>();
        var i = 0;
        foreach (var unit in visibleEnemies) {
            if (!CanFriendlySee(unit.transform.position)) {
                toHide.Add(i);
            }
            i++;
        }
        foreach (var unit in units[ControlSystem.instance.affiliation]) {
            if (unit != null && unit.TryGetComponent(out UnitParameters unitParams)) {
                var pos = unit.transform.position;
                var sightRange = unitParams.getSightRange();
                i = 0;
                var toShow = new HashSet<int>();
                foreach (var enemy in hiddenEnemies) {
                    if (Vector3.Distance(enemy.transform.position, pos) <= sightRange) {
                        toShow.Add(i);
                    }
                    i++;
                }
                foreach (var idx in toShow) {
                    var enemy = hiddenEnemies[idx];
                    ShowUnit(enemy);
                    visibleEnemies.Add(enemy);
                }
                i = 0;
                hiddenEnemies.RemoveAll(_ => toShow.Contains(i++));
            }
        }
        foreach (var idx in toHide) {
            var enemy = visibleEnemies[idx];
            HideUnit(enemy);
            hiddenEnemies.Add(enemy);
        }
        i = 0;
        visibleEnemies.RemoveAll(_ => toHide.Contains(i++));
        FogOfWarManager.instance.UpdateFog();
    }

    void HideUnit(GameObject unit) {
        SetLayerRecursively(unit, hiddenUnitLayer);
    }

    void ShowUnit(GameObject unit) {
        SetLayerRecursively(unit, unitLayer);
    }

    void SetLayerRecursively(GameObject obj, int layer) {
        if (obj == null) {
            return;
        }
        obj.layer = layer;
        foreach (Transform child in obj.transform) {
            if (child == null) {
                continue;
            }
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    // void Update()
    // {
    //eventually use a spatial hash    
    // }
}
