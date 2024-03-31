using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerInput))]
public class ControlSystem : MonoBehaviour {
    public GameObject moveWaypointIndicator;
    public GameObject attackWaypointIndicator;
    public GameObject unitInfoPrefab;
    public GameObject selectedUnitsPanel;
    public GameObject selectedUnitsContainer;
    public GameObject canvas;
    public GameObject selBox;
    public GameObject selMenu;
    public string affiliation;
    public float doubleClickPeriod = 0.2f;
    public float continuousMovementPeriod = 0.1f;
    // public float moveCommandRepeatPeriod = 0.1f;

    GlobalUnitManager globalUnitManager;
    PlayerInput input;
    Camera cam;
    RectTransform selBoxTransform;
    RectTransform selMenuTransform;
    List<GameObject> controlledUnits;
    Plane groundPlane;
    LayerMask unitsLayerMask;
    LayerMask groundLayerMask;
    TMP_Dropdown selMenuDropdown;
    GraphicRaycaster grc;
    PointerEventData clickData;
    InputActionMap playerMap;
    InputActionMap uiMap;

    bool attackMode = false;
    bool queueMode = false;

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
        groundLayerMask = LayerMask.GetMask("Ground");
        selMenuDropdown = selMenu.GetComponentInChildren<TMP_Dropdown>();
        foreach (var unitType in globalUnitManager.GetUnitTypes()) {
            selMenuDropdown.options.Add(new TMP_Dropdown.OptionData(unitType));
        }
        grc = canvas.GetComponent<GraphicRaycaster>();
        clickData = new PointerEventData(EventSystem.current);
        controlledUnits = new List<GameObject>();
        playerMap = input.actions.FindActionMap("Player");
        uiMap = input.actions.FindActionMap("UI");
        uiMap.Disable();
        input.actions["Queue Mode"].Disable();
    }

    void OnMove() {
        StartCoroutine(MoveWhileHoldingInput());
    }

    UnityEvent attachedEvent;
    GameObject lastWaypointIndicator;

    private enum ActionType {
        Move,
        AttackMove,
    }

    private class Command {
        public ActionType type;
        public Vector3 goal;
    }

    Queue<Command> actionQueue = new();

    Command CommandToMouse(ActionType type) {
        var goalRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        float goalEnter;
        groundPlane.Raycast(goalRay, out goalEnter);
        var goal3 = goalRay.GetPoint(goalEnter);
        return new Command {
            type = type,
            goal = goal3
        };
    }

    void ExecuteCommand(Command command, bool attach = false) {
        GameObject waypointIndicator = null;
        switch (command.type) {
            case ActionType.Move:
                waypointIndicator = moveWaypointIndicator;
                break;
            case ActionType.AttackMove:
                waypointIndicator = attackWaypointIndicator;
                break;
        }
        var goal2 = new Vector2(command.goal.x, command.goal.z);
        DestroyWaypointIndicator();
        lastWaypointIndicator = Instantiate(waypointIndicator, command.goal, Quaternion.identity);
        GameObject chosenUnit = null;
        float chosenUnitDistance = 0f;
        foreach (GameObject obj in controlledUnits) {
            if (obj != null) {
                if (obj.TryGetComponent(out UnitAI ai)) {
                    switch (command.type) {
                        case ActionType.Move:
                            ai.MoveToCoordinate(goal2);
                            break;
                        case ActionType.AttackMove:
                            ai.AttackMoveToCoordinate(goal2);
                            break;
                    }
                    var distanceToGoal = (obj.transform.position - command.goal).magnitude;
                    if (distanceToGoal > chosenUnitDistance) {
                        chosenUnit = obj;
                        chosenUnitDistance = distanceToGoal;
                    }
                    // plan.changeWayPointXZ(goal);
                }
            }
        }
        if (chosenUnit != null && chosenUnit.TryGetComponent(out Planner planner)) {
            switch (command.type) {
                case ActionType.Move:
                    attachedEvent = planner.reachedGoalEvent;
                    break;
                case ActionType.AttackMove:
                    attachedEvent = planner.finishedAttackingEvent;
                    break;
            }
            attachedEvent.AddListener(DestroyWaypointIndicator);
            if (attach) {
                attachedActionEvent = attachedEvent;
                attachedActionEvent.AddListener(FinishExecutingAction);
            }
        }
    }

    void MoveToMouse() {
        if (controlledUnits.Count == 0) {
            return;
        }

        var command = CommandToMouse(ActionType.Move);
        if (queueMode) {
            actionQueue.Enqueue(command);
        } else {
            ExecuteCommand(command);
        }
    }

    void AttackMove() {
        if (controlledUnits.Count == 0) {
            return;
        }

        var command = CommandToMouse(ActionType.AttackMove);
        if (queueMode) {
            actionQueue.Enqueue(command);
        } else {
            ExecuteCommand(command);
            if (!input.actions["Activate Attack"].IsPressed()) {
                attackMode = false;
            }
        }
    }

    bool executingAction = false;
    UnityEvent attachedActionEvent;
    public void FinishExecutingAction() {
        executingAction = false;
        attachedActionEvent.RemoveListener(FinishExecutingAction);
        attachedActionEvent = null;
    }

    private void Update() {
        if (actionQueue.Count > 0 && !executingAction) {
            var command = actionQueue.Dequeue();
            ExecuteCommand(command, attach: true);
        }
        UpdateSelectionDisplay();
    }

    void DestroyWaypointIndicator() {
        if (lastWaypointIndicator != null) {
            Destroy(lastWaypointIndicator);
        }
        if (attachedEvent != null) {
            attachedEvent.RemoveListener(DestroyWaypointIndicator);
            attachedEvent = null;
        }
    }

    IEnumerator MoveWhileHoldingInput() {
        MoveToMouse();
        while (input.actions["Move"].IsPressed()) {
            yield return new WaitForSeconds(continuousMovementPeriod);
            // yield return null;
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
        if (attackMode) {
            AttackMove();
            return;
        }
        StartCoroutine(SelectUnits());
    }

    void OnActivateAttack() {
        if (controlledUnits.Count > 0) {
            attackMode = true;
        }
    }

    void OnQueueMode() {
        StartCoroutine(ManageQueueMode());
    }

    IEnumerator ManageQueueMode() {
        queueMode = true;
        while (input.actions["Queue Mode"].IsPressed()) {
            yield return null;
        }
        queueMode = false;
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
        unit.TryGetComponent(out UnitAffiliation unitAffiliation);
        var type = unitAffiliation.unit_type;
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

    void OnStop() {
        StartCoroutine(StopUnits());
    }

    IEnumerator StopUnits() {
        while (input.actions["Stop"].IsPressed()) {
            foreach (var unit in controlledUnits) {
                if (unit != null && unit.TryGetComponent(out UnitAI ai)) {
                    ai.MoveToCoordinate(new Vector2(unit.transform.position.x, unit.transform.position.z));
                }
            }
            DestroyWaypointIndicator();
            yield return null;
        }
    }

    void RegisterUnits() {
        foreach (var unit in controlledUnits) {
            RegisterUnit(unit);
        }
        if (controlledUnits.Count == 0) {
            attackMode = false;
        }
    }

    void UnregisterUnits() {
        foreach (var unit in controlledUnits) {
            UnregisterUnit(unit);
        }
        DestroyWaypointIndicator();
        controlledUnits.Clear();
    }

    void RegisterUnit(GameObject unit) {
        if (unit == null) return;
        unit.transform.GetChild(0).gameObject.SetActive(true);
    }

    void UnregisterUnit(GameObject unit) {
        if (unit == null) return;
        unit.transform.GetChild(0).gameObject.SetActive(false);
        if (controlledUnits.Count == 0) {
            attackMode = false;
        }
    }

    void UpdateSelectionDisplay() {
        if (controlledUnits.Count == 0) {
            selectedUnitsPanel.SetActive(false);
            return;
        }
        selectedUnitsPanel.SetActive(true);
        SortedDictionary<string, List<UnitParameters>> units = new();
        var foundUnit = false;
        foreach (var unit in controlledUnits) {
            if (unit != null && unit.TryGetComponent(out UnitAffiliation unitaff) && unit.TryGetComponent(out UnitParameters unitparams)) {
                if (units.ContainsKey(unitaff.unit_type)) {
                    units[unitaff.unit_type].Add(unitparams);
                } else {
                    units.Add(unitaff.unit_type, new List<UnitParameters> { unitparams });
                }
                foundUnit = true;
            }
        }
        if (!foundUnit) {
            selectedUnitsPanel.SetActive(false);
            return;
        }
        while (selectedUnitsContainer.transform.childCount > 0) {
            DestroyImmediate(selectedUnitsContainer.transform.GetChild(0).gameObject);
        }
        var counter = 0;
        foreach (var (_, typedUnits) in units) {
            var totalHealth = 0f;
            var maxHealth = 0f;
            foreach (var unitparams in typedUnits) {
                maxHealth += unitparams.maxHP;
                totalHealth += unitparams.getHP();
            }
            var sprite = typedUnits[0].gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite;
            var unitInfo = Instantiate(unitInfoPrefab, selectedUnitsContainer.transform);
            var image = unitInfo.GetComponentInChildren<Image>();
            image.sprite = sprite;
            var text = unitInfo.GetComponentInChildren<TMP_Text>();
            text.SetText("x" + typedUnits.Count.ToString());
            var slider = unitInfo.GetComponentInChildren<Slider>();
            slider.SetValueWithoutNotify(totalHealth / maxHealth);
            unitInfo.TryGetComponent(out RectTransform rectTransform);
            rectTransform.anchoredPosition = new Vector2(0, -175 * counter);
            counter++;
        }
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
