using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearEnemiesObjective : MonoBehaviour {
    private float InitialEnemyCount;
    private float currentEnemyCount;
    public ulong currentObj = 0;

    void Start() {
        InitialEnemyCount = nestedChildCount(transform);
        currentEnemyCount = InitialEnemyCount;
        currentObj = ToastSystem.instance.SendObjective("Clear all enemies " + (InitialEnemyCount - nestedChildCount(transform)) + "/" + InitialEnemyCount);
    }

    private int nestedChildCount(Transform parent) {
        if (parent.childCount == 0) {
            return 1;
        }
        int count = 0;
        foreach (Transform child in parent) {
            //just doing on level, instead of full recursion
            count += child.childCount;
        }
        return count;
    }

    // Update is called once per frame
    void Update() {
        if (currentEnemyCount != nestedChildCount(transform)) {
            ToastSystem.instance.RemoveObjective(currentObj);
            currentObj = ToastSystem.instance.SendObjective("Clear all enemies " + (InitialEnemyCount - nestedChildCount(transform)) + "/" + InitialEnemyCount);
        }
        currentEnemyCount = nestedChildCount(transform);
        if (currentEnemyCount == 0) {
            ToastSystem.instance.CompleteObjective(currentObj);
        }
    }
}
