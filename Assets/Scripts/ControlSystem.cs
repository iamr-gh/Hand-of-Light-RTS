using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerInput))]
public class ControlSystem : MonoBehaviour {
    public GameObject canvas;
    public GameObject selBox;
    public GameObject selMenu;
    public string affiliation;

    GlobalUnitManager globalUnitManager;
    PlayerInput input;
    Camera cam;
    RectTransform selBoxTransform;
    RectTransform selMenuTransform;
    List<GameObject> controlledUnits;
    Plane groundPlane;
    LayerMask unitsLayerMask;
    TMP_Dropdown selMenuDropdown;
    GraphicRaycaster grc;
    PointerEventData clickData;

    // Start is called before the first frame update
    void Start() {
        globalUnitManager = GlobalUnitManager.singleton;
        TryGetComponent(out input);
        cam = Camera.main;
        selBox.TryGetComponent(out selBoxTransform);
        selBox.SetActive(false);
        selMenu.TryGetComponent(out selMenuTransform);
        selMenu.SetActive(false);
        groundPlane = new Plane(Vector3.up, 0);
        unitsLayerMask = LayerMask.GetMask("Units");
        selMenuDropdown = selMenu.GetComponentInChildren<TMP_Dropdown>();
        foreach (var unitType in globalUnitManager.GetUnitTypes()) {
            selMenuDropdown.options.Add(new TMP_Dropdown.OptionData(unitType.Name.Replace("Controller", "")));
        }
        grc = canvas.GetComponent<GraphicRaycaster>();
        clickData = new PointerEventData(EventSystem.current);
    }

    void OnMove() {
        var goalRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        float goalEnter;
        groundPlane.Raycast(goalRay, out goalEnter);
        var goal3 = goalRay.GetPoint(goalEnter);
        var goal = new Vector2(goal3.x, goal3.z);
        foreach (GameObject obj in controlledUnits) {
            if (obj != null) {
                if (obj.TryGetComponent(out Planner plan)) {
                    plan.changeWayPointXZ(goal);
                }
            }
        }
    }

    void OnSelect() {
        clickData.position = Mouse.current.position.ReadValue();
        var clickResults = new List<RaycastResult>();
        grc.Raycast(clickData, clickResults);
        if (clickResults.Count > 0) {
            return;
        }
        StartCoroutine(SelectUnits());
    }

    void OnSelectOne() {
        var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitsLayerMask)) {
            var obj = hit.transform.gameObject;
            if (obj.TryGetComponent(out UnitAffiliation aff)) {
                if (aff.affiliation == affiliation) {
                    if (controlledUnits.Contains(obj)) {
                        UnregisterUnit(obj);
                        controlledUnits.Remove(obj);
                    } else {
                        controlledUnits.Add(obj);
                        RegisterUnit(obj);
                    }
                }
            }
        }
    }

    void OnSelectType() {
        var pivot = cam.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        selMenuTransform.pivot = new Vector2(1 - pivot.x, 1 - pivot.y);
        selMenu.SetActive(true);
    }

    IEnumerator SelectUnits() {
        var mousePos = Mouse.current.position.ReadValue();
        var initialCamPos = cam.transform.position;
        var initialPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
        var finalPos = initialPos;
        UpdateSelections(initialPos, finalPos);
        selBox.SetActive(true);
        while (input.actions["Select"].IsPressed()) {
            mousePos = Mouse.current.position.ReadValue();
            finalPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
            UpdateSelections(initialPos + (cam.transform.position - initialCamPos), finalPos);
            yield return null;
        }
        selBox.SetActive(false);
    }

    void UpdateSelections(Vector3 initialPos, Vector3 finalPos) {
        var initialPosViewport = cam.WorldToViewportPoint(initialPos);
        var finalPosViewport = cam.WorldToViewportPoint(finalPos);
        selBoxTransform.anchorMin = new Vector2(
            Mathf.Min(initialPosViewport.x, finalPosViewport.x),
            Mathf.Min(initialPosViewport.y, finalPosViewport.y)
        );
        selBoxTransform.anchorMax = new Vector2(
            Mathf.Max(initialPosViewport.x, finalPosViewport.x),
            Mathf.Max(initialPosViewport.y, finalPosViewport.y)
        );
        var lowerLeftRay = cam.ViewportPointToRay(selBoxTransform.anchorMin);
        var topRightRay = cam.ViewportPointToRay(selBoxTransform.anchorMax);
        float lowerLeftEnter, topRightEnter;
        groundPlane.Raycast(lowerLeftRay, out lowerLeftEnter);
        groundPlane.Raycast(topRightRay, out topRightEnter);
        var lowerLeftHit = lowerLeftRay.GetPoint(lowerLeftEnter);
        var topRightHit = topRightRay.GetPoint(topRightEnter);
        SetControlledUnits(globalUnitManager.FindInBox(lowerLeftHit, topRightHit));
    }

    void SetControlledUnits(List<GameObject> units) {
        UnregisterUnits();
        controlledUnits = units.Where(unit => unit.TryGetComponent(out UnitAffiliation aff) && aff.affiliation == affiliation).ToList();
        RegisterUnits();
    }

    void RegisterUnits() {
        if (controlledUnits != null) {
            foreach (var unit in controlledUnits) {
                RegisterUnit(unit);
            }
        }
    }

    void UnregisterUnits() {
        if (controlledUnits != null) {
            foreach (var unit in controlledUnits) {
                UnregisterUnit(unit);
            }
        }
    }

    void RegisterUnit(GameObject unit) {
        unit.transform.GetChild(0).gameObject.SetActive(true);
    }

    void UnregisterUnit(GameObject unit) {
        unit.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void SelectUnitType(int type) {
        selMenu.SetActive(false);
        if (type != 0) {
            SetControlledUnits(globalUnitManager.FindByType(type - 1));
        }
        selMenuDropdown.SetValueWithoutNotify(0);
    }
}
