using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour {
    public static FogOfWarManager instance;

    public GameObject cloudPrefab;
    public float cloudYOffset = 0.5f;
    public float gridUnitSize = 1f;
    public Vector2 gridXLimits = new Vector2(-100, 100);
    public Vector2 gridYLimits = new Vector2(-100, 100);
    public float gridHeight = 100f;

    List<GameObject> visibleClouds = new();
    List<GameObject> hiddenClouds = new();

    LayerMask groundLayerMask;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        groundLayerMask = LayerMask.GetMask("Ground");
    }

    // Start is called before the first frame update
    void Start() {
        for (var x = gridXLimits.x; x < gridXLimits.y; x += gridUnitSize) {
            for (var y = gridYLimits.x; y < gridYLimits.y; y += gridUnitSize) {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(x, gridHeight, y), Vector3.down, out hit, Mathf.Infinity, groundLayerMask)) {
                    var cloud = Instantiate(cloudPrefab, new Vector3(hit.point.x, hit.point.y + cloudYOffset, hit.point.z), Quaternion.identity);
                    visibleClouds.Add(cloud);
                }
            }
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
