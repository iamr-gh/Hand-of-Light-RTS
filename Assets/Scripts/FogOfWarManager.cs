using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour {
    public static FogOfWarManager instance;

    public GameObject cloudPrefab;
    public float cloudYOffset = 0.5f;

    List<GameObject> visibleClouds = new();
    List<GameObject> hiddenClouds = new();

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start() {
        AddClouds(transform.GetChild(0).GetComponentsInChildren<Transform>());
        AddClouds(transform.GetChild(1).GetComponentsInChildren<Transform>());
    }

    void AddClouds(Transform[] blocks) {
        foreach (var block in blocks) {
            var cloud = Instantiate(cloudPrefab, new Vector3(block.position.x, block.position.y + cloudYOffset, block.position.z), Quaternion.identity, block);
            visibleClouds.Add(cloud);
        }
    }

    public void UpdateFog() {
        var toShow = new HashSet<int>();
        var i = 0;
        foreach (var cloud in hiddenClouds) {
            if (!GlobalUnitManager.singleton.CanFriendlySee(cloud.transform.position)) {
                toShow.Add(i);
            }
            i++;
        }
        foreach (var unit in GlobalUnitManager.singleton.units[ControlSystem.instance.affiliation]) {
            if (unit != null && unit.TryGetComponent(out UnitParameters unitParams)) {
                var pos = unit.transform.position;
                var sightRange = unitParams.getSightRange();
                i = 0;
                var toHide = new HashSet<int>();
                foreach (var cloud in visibleClouds) {
                    if (Vector3.Distance(cloud.transform.position, pos) <= sightRange) {
                        toHide.Add(i);
                    }
                    i++;
                }
                foreach (var idx in toHide) {
                    var cloud = visibleClouds[idx];
                    cloud.SetActive(false);
                    hiddenClouds.Add(cloud);
                }
                i = 0;
                visibleClouds.RemoveAll(_ => toHide.Contains(i++));
            }
        }
        foreach (var idx in toShow) {
            var cloud = hiddenClouds[idx];
            cloud.SetActive(true);
            visibleClouds.Add(cloud);
        }
        i = 0;
        hiddenClouds.RemoveAll(unit => toShow.Contains(i++));
    }
}
