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
    public GameObject waypointIndicator;
    public GameObject canvas;
    public GameObject selBox;
    public GameObject selMenu;
    public string affiliation;
    public float doubleClickPeriod = 0.2f;
    // public float moveCommandRepeatPeriod = 0.1f;

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
    InputActionMap playerMap;
    InputActionMap uiMap;

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
            selMenuDropdown.options.Add(new TMP_Dropdown.OptionData(unitType.Name));
        }
        grc = canvas.GetComponent<GraphicRaycaster>();
        clickData = new PointerEventData(EventSystem.current);
        controlledUnits = new List<GameObject>();
        playerMap = input.actions.FindActionMap("Player");
        uiMap = input.actions.FindActionMap("UI");
        uiMap.Disable();
    }

    void OnMove() {
        StartCoroutine(MoveWhileHoldingInput());
    }

    GameObject lastWaypointIndicator;

    void MoveToMouse() {
        if (controlledUnits.Count == 0) {
            return;
        }

        var goalRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        float goalEnter;
        groundPlane.Raycast(goalRay, out goalEnter);
        var goal3 = goalRay.GetPoint(goalEnter);
        var goal = new Vector2(goal3.x, goal3.z);
        if (lastWaypointIndicator != null) {
            Destroy(lastWaypointIndicator);
        }
        lastWaypointIndicator = Instantiate(waypointIndicator, goal3, Quaternion.identity);
        foreach (GameObject obj in controlledUnits) {
            if (obj != null) {
                if (obj.TryGetComponent(out UnitAI ai)) {
                    ai.MoveToCoordinate(goal);
                    // plan.changeWayPointXZ(goal);
                }
            }
        }
    }

    IEnumerator MoveWhileHoldingInput() {
        MoveToMouse();
        while (input.actions["Move"].IsPressed()) {
            // yield return new WaitForSeconds(moveCommandRepeatPeriod);
            yield return null;
            MoveToMouse();
        }
    }

    void OnSelect() {
        clickData.position = Mouse.current.position.ReadValue();
        var clickResults = new List<RaycastResult>();
        grc.Raycast(clickData, clickResults);
        if (clickResults.Count > 0) {
            return;
        }
        HideSelMenu();
        StartCoroutine(SelectUnits());
    }

    void OnSelectOne() {
        SelectOne(toggle: true);
    }

    bool SelectOne(bool toggle = false) {
        var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitsLayerMask)) {
            var obj = hit.transform.gameObject;
            if (obj.TryGetComponent(out UnitAffiliation aff)) {
                if (aff.affiliation == affiliation) {
                    if (controlledUnits.Contains(obj)) {
                        if (toggle) {
                            UnregisterUnit(obj);
                            controlledUnits.Remove(obj);
                        }
                    } else {
                        controlledUnits.Add(obj);
                        RegisterUnit(obj);
                    }
                    return true;
                }
            }
        }
        return false;
    }

    void OnSelectType() {
        UnregisterUnits();
        if (SelectOne() && controlledUnits.Count == 1) {
            SelectUnitsSharingType(controlledUnits.First());
        }
    }

    void SelectUnitsSharingType(GameObject unit) {
        unit.TryGetComponent(out UnitAI unitAI);
        var type = unitAI.GetType();
        SetControlledUnits(globalUnitManager.FindByType(type));
    }

    void OnSelectTypeMenu() {
        ShowSelMenu();
    }

    GameObject lastSelectedUnit;
    float lastSelectedTime = -100f;

    IEnumerator SelectUnits() {
        UnregisterUnits();
        var selectedOne = SelectOne();
        var selectedTime = Time.time;
        var mousePos = Mouse.current.position.ReadValue();
        var initialCamPos = cam.transform.position;
        var initialPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
        var finalPos = initialPos;
        UpdateSelections(initialPos, finalPos, extend: selectedOne);
        selBox.SetActive(true);
        while (input.actions["Select"].IsPressed()) {
            mousePos = Mouse.current.position.ReadValue();
            finalPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
            UpdateSelections(initialPos + (cam.transform.position - initialCamPos), finalPos, extend: selectedOne);
            yield return null;
        }
        selBox.SetActive(false);
        if (selectedOne && controlledUnits.Count == 1) {
            var selectedUnit = controlledUnits.First();
            if (Time.time - lastSelectedTime <= doubleClickPeriod && lastSelectedUnit == selectedUnit) {
                SelectUnitsSharingType(selectedUnit);
            }
            Debug.Log(Time.time - lastSelectedTime); ;
            lastSelectedUnit = selectedUnit;
            lastSelectedTime = selectedTime;
        }
    }

    void UpdateSelections(Vector3 initialPos, Vector3 finalPos, bool extend = false) {
        var initialPosViewport = cam.WorldToViewportPoint(initialPos);
        var finalPosViewport = cam.WorldToViewportPoint(finalPos);
        var bottomLeft = new Vector2(
            Mathf.Min(initialPosViewport.x, finalPosViewport.x),
            Mathf.Min(initialPosViewport.y, finalPosViewport.y)
        );
        var bottomRight = new Vector2(
            Mathf.Max(initialPosViewport.x, finalPosViewport.x),
            Mathf.Min(initialPosViewport.y, finalPosViewport.y)
        );
        var topLeft = new Vector2(
            Mathf.Min(initialPosViewport.x, finalPosViewport.x),
            Mathf.Max(initialPosViewport.y, finalPosViewport.y)
        );
        var topRight = new Vector2(
            Mathf.Max(initialPosViewport.x, finalPosViewport.x),
            Mathf.Max(initialPosViewport.y, finalPosViewport.y)
        );
        selBoxTransform.anchorMin = bottomLeft;
        selBoxTransform.anchorMax = topRight;
        var bottomLeftRay = cam.ViewportPointToRay(bottomLeft);
        var bottomRightRay = cam.ViewportPointToRay(bottomRight);
        var topLeftRay = cam.ViewportPointToRay(topLeft);
        var topRightRay = cam.ViewportPointToRay(topRight);
        float bottomLeftEnter, bottomRightEnter, topLeftEnter, topRightEnter;
        groundPlane.Raycast(bottomLeftRay, out bottomLeftEnter);
        groundPlane.Raycast(bottomRightRay, out bottomRightEnter);
        groundPlane.Raycast(topLeftRay, out topLeftEnter);
        groundPlane.Raycast(topRightRay, out topRightEnter);
        var bottomLeftHit = bottomLeftRay.GetPoint(bottomLeftEnter);
        var bottomRightHit = bottomRightRay.GetPoint(bottomRightEnter);
        var topLeftHit = topLeftRay.GetPoint(topLeftEnter);
        var topRightHit = topRightRay.GetPoint(topRightEnter);
        // SetControlledUnits(globalUnitManager.FindInBox(bottomLeftHit, topRightHit));
        SetControlledUnits(globalUnitManager.FindInTrapezoid(bottomLeftHit, bottomRightHit, topLeftHit, topRightHit), extend: extend);
    }

    void SetControlledUnits(List<GameObject> units, bool extend = false) {
        var filteredUnits = units.Where(unit => unit.TryGetComponent(out UnitAffiliation aff) && aff.affiliation == affiliation).ToList();
        if (extend) {
            controlledUnits.AddRange(filteredUnits);
            foreach (var unit in filteredUnits) {
                RegisterUnit(unit);
            }
        } else {
            UnregisterUnits();
            controlledUnits = filteredUnits;
            RegisterUnits();
        }
    }

    void RegisterUnits() {
        foreach (var unit in controlledUnits) {
            RegisterUnit(unit);
        }
    }

    void UnregisterUnits() {
        foreach (var unit in controlledUnits) {
            UnregisterUnit(unit);
        }
        controlledUnits.Clear();
    }

    void RegisterUnit(GameObject unit) {
        if (unit == null) return;
        unit.transform.GetChild(0).gameObject.SetActive(true);
    }

    void UnregisterUnit(GameObject unit) {
        if (unit == null) return;
        unit.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void SelectUnitType(int type) {
        if (type != 0) {
            SetControlledUnits(globalUnitManager.FindByTypeIdx(type - 1));
        }
        HideSelMenu();
    }

    void ShowSelMenu() {
        var pivot = cam.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        selMenuTransform.pivot = new Vector2(1 - pivot.x, 1 - pivot.y);
        selMenu.SetActive(true);
        uiMap.Enable();
    }

    void HideSelMenu() {
        selMenu.SetActive(false);
        selMenuDropdown.SetValueWithoutNotify(0);
        uiMap.Disable();
    }
}
