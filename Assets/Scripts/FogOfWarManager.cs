using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour {
    public static FogOfWarManager instance;

    public GameObject cloudPrefab;
    public float cloudYOffset = 0.5f;

    List<GameObject> clouds = new();
    // Start is called before the first frame update
    void Start() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        AddClouds(transform.GetChild(0).GetComponentsInChildren<Transform>());
        AddClouds(transform.GetChild(1).GetComponentsInChildren<Transform>());
    }

    void AddClouds(Transform[] blocks) {
        foreach (var block in blocks) {
            var cloud = Instantiate(cloudPrefab, new Vector3(block.position.x, block.position.y + cloudYOffset, block.position.z), Quaternion.identity, block);
            clouds.Add(cloud);
        }
    }

    public void ResetFog() {
        foreach (var cloud in clouds) {
            cloud.SetActive(true);
        }
    }

    public void UpdateFog(Vector3 pos, float radius) {
        foreach (var cloud in clouds) {
            if (Vector3.Distance(cloud.transform.position, pos) <= radius) {
                cloud.SetActive(false);
            }
        }
    }
}
