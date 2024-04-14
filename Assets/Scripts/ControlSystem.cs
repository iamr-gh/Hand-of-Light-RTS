using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

[System.Serializable]
public enum ControlState {
    NormalMode,
    SingleSelect,
    SelectingUnits,
    MovingUnits,
    SelectingTypeFromMenu,
    InMenu,
    StoppingUnits,
    AttackMode,
    QueueMode,
    QueueAttackMode,
    AbilityMode,
    Transient,
}

// Discrete input events
[System.Serializable]
public enum ControlActions {
    Select,
    StartDragging,
    StopSelecting,
    Move,
    StopMoving,
    SelectType,
    OpenSelectTypeMenu,
    CloseSelectTypeMenu,
    SelectOne,
    ActivateAttack,
    DeactivateAttack,
    Stop,
    StopStopping,
    ActivateQueue,
    DeactivateQueue,
    UseAbility,
    StopUsingAbility,
    Escape,
    OpenMenu,
    CloseMenu,
    AdvanceDialogue,
}

[RequireComponent(typeof(PlayerInput))]
public class ControlSystem : MonoBehaviour {
    public static ControlSystem instance;

    public GameObject moveWaypointIndicator;
    public GameObject attackWaypointIndicator;
    public GameObject aoeIndicator;
    public GameObject linePrefab;
    public Texture2D defaultCursor;
    public Texture2D attackCursor;
    public GameObject canvas;
    public GameObject selBox;
    public GameObject selMenu;
    public GameObject menu;
    public string affiliation;
    public int numSupportedAbilities = 4;
    public float minDragDistance = 0.5f;
    public float doubleClickPeriod = 0.2f;
    public float continuousMovementPeriod = 0.1f;
    public float selectVoiceLineCooldown = 1f;
    public float moveVoiceLineCooldown = 1f;
    public float attackMoveVoiceLineCooldown = 1f;

    bool selectVoiceLineEnabled = true;
    bool moveVoiceLineEnabled = true;
    bool attackMoveVoiceLineEnabled = true;

    void EnableSelectVoiceLine() {
        selectVoiceLineEnabled = true;
    }

    void EnableMoveVoiceLine() {
        moveVoiceLineEnabled = true;
    }

    void EnableAttackMoveVoiceLine() {
        attackMoveVoiceLineEnabled = true;
    }

    GlobalUnitManager globalUnitManager;
    PlayerInput input;
    Camera cam;
    RectTransform selBoxTransform;
    RectTransform selMenuTransform;
    public List<GameObject> controlledUnits;
    Plane groundPlane;
    LayerMask unitsLayerMask;
    LayerMask groundLayerMask;
    TMP_Dropdown selMenuDropdown;
    GraphicRaycaster grc;
    PointerEventData clickData;
    InputActionMap playerMap;
    InputActionMap cameraMap;
    InputActionMap uiMap;
    InputActionMap universalMap;

    ControlState controlState;

    UnityEvent attachedEvent;
    GameObject lastWaypointIndicator;

    cameraMovement camMovement;

    // Left click action types only
    private enum ActionType {
        Move,
        AttackMove,
    }

    private class Command {
        public ActionType type;
        public Vector3 goal;
    }

    Queue<(Command, GameObject)> actionQueue = new();
    Queue<GameObject> queueLines = new();
    UnityEvent attachedActionEvent;

    List<Ability> selectedAbilities;

    Ability currentAbility;
    GameObject currentCaster;
    Ability nextAbility;
    GameObject nextCaster;

    public ControlState GetControlState() {
        return controlState;
    }

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
        globalUnitManager = GlobalUnitManager.singleton;
        TryGetComponent(out input);
        cam = Camera.main;
        cam.TryGetComponent(out camMovement);
        selBox.TryGetComponent(out selBoxTransform);
        selBox.SetActive(false);
        selMenu.TryGetComponent(out selMenuTransform);
        selMenu.SetActive(false);
        groundPlane = globalUnitManager.groundPlane;
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
        cameraMap = input.actions.FindActionMap("Camera");
        uiMap = input.actions.FindActionMap("UI");
        universalMap = input.actions.FindActionMap("Universal");
        cameraMap.Enable();
        uiMap.Disable();
        universalMap.Enable();
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        controlState = ControlState.NormalMode;
        selectedAbilities = new(numSupportedAbilities);
        for (var i = 0; i < numSupportedAbilities; i++) {
            selectedAbilities.Add(null);
        }
    }

    private void Update() {
        UpdateSelectionDisplay();
        if (controlState == ControlState.AttackMode && controlledUnits.Count == 0) {
            ProcessInput(ControlActions.DeactivateAttack);
        }
    }

    void OnDestroy() {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    void ProcessInput(ControlActions action, Ability ability = null, GameObject caster = null) {
        switch (controlState) {
            case ControlState.NormalMode:
                switch (action) {
                    case ControlActions.Select:
                        controlState = ControlState.SingleSelect;
                        StartCoroutine(Select());
                        break;
                    case ControlActions.Move:
                        controlState = ControlState.MovingUnits;
                        StartCoroutine(MoveWhileHoldingInput());
                        PlayMoveVoiceLine();
                        break;
                    case ControlActions.SelectType:
                        SelectType();
                        break;
                    case ControlActions.OpenSelectTypeMenu:
                        controlState = ControlState.SelectingTypeFromMenu;
                        ShowSelMenu();
                        break;
                    case ControlActions.SelectOne:
                        SelectOne(toggle: true);
                        break;
                    case ControlActions.ActivateAttack:
                        if (controlledUnits.Count > 0) {
                            controlState = ControlState.AttackMode;
                            Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
                        }
                        break;
                    case ControlActions.Stop:
                        controlState = ControlState.StoppingUnits;
                        StartCoroutine(StopUnits());
                        break;
                    case ControlActions.ActivateQueue:
                        controlState = ControlState.QueueMode;
                        StartCoroutine(ManageQueueMode());
                        break;
                    case ControlActions.UseAbility:
                        controlState = ControlState.AbilityMode;
                        StartCoroutine(UseAbility(ability, caster));
                        break;
                    case ControlActions.AdvanceDialogue:
                        ToastSystem.instance.AdvanceDialogue();
                        break;
                    case ControlActions.Escape:
                        controlState = ControlState.InMenu;
                        uiMap.Enable();
                        playerMap.Disable();
                        cameraMap.Disable();
                        camMovement.enabled = false;
                        menu.SetActive(true);
                        break;
                }
                break;
            case ControlState.SingleSelect:
                switch (action) {
                    case ControlActions.StartDragging:
                        controlState = ControlState.SelectingUnits;
                        break;
                    case ControlActions.StopSelecting:
                        controlState = ControlState.NormalMode;
                        break;
                }
                break;
            case ControlState.SelectingUnits:
                switch (action) {
                    case ControlActions.StopSelecting:
                        controlState = ControlState.NormalMode;
                        break;
                }
                break;
            case ControlState.MovingUnits:
                switch (action) {
                    case ControlActions.StopMoving:
                        controlState = ControlState.NormalMode;
                        break;
                }
                break;
            case ControlState.SelectingTypeFromMenu:
                switch (action) {
                    case ControlActions.Select:
                        controlState = ControlState.SingleSelect;
                        HideSelMenu();
                        StartCoroutine(Select());
                        break;
                    case ControlActions.OpenSelectTypeMenu:
                        controlState = ControlState.SelectingTypeFromMenu;
                        ShowSelMenu();
                        break;
                    case ControlActions.CloseSelectTypeMenu:
                        controlState = ControlState.NormalMode;
                        HideSelMenu();
                        break;
                    case ControlActions.Escape:
                        controlState = ControlState.NormalMode;
                        HideSelMenu();
                        break;
                }
                break;
            case ControlState.InMenu:
                switch (action) {
                    case ControlActions.Escape:
                        controlState = ControlState.NormalMode;
                        uiMap.Disable();
                        playerMap.Enable();
                        cameraMap.Enable();
                        camMovement.enabled = true;
                        menu.SetActive(false);
                        break;
                }
                break;
            case ControlState.StoppingUnits:
                switch (action) {
                    case ControlActions.StopStopping:
                        controlState = ControlState.NormalMode;
                        break;
                }
                break;
            case ControlState.AttackMode:
                switch (action) {
                    case ControlActions.Select:
                        if (!input.actions["Activate Attack"].IsPressed()) {
                            controlState = ControlState.NormalMode;
                            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                        }
                        AttackMove();
                        PlayAttackMoveVoiceLine();
                        break;
                    case ControlActions.Move:
                        controlState = ControlState.MovingUnits;
                        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                        StartCoroutine(MoveWhileHoldingInput());
                        break;
                    case ControlActions.SelectType:
                        controlState = ControlState.NormalMode;
                        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                        SelectType();
                        break;
                    case ControlActions.OpenSelectTypeMenu:
                        controlState = ControlState.SelectingTypeFromMenu;
                        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                        ShowSelMenu();
                        break;
                    case ControlActions.SelectOne:
                        controlState = ControlState.NormalMode;
                        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                        SelectOne(toggle: true);
                        break;
                    case ControlActions.Stop:
                        controlState = ControlState.StoppingUnits;
                        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                        StartCoroutine(StopUnits());
                        break;
                    case ControlActions.ActivateQueue:
                        controlState = ControlState.QueueAttackMode;
                        StartCoroutine(ManageQueueMode());
                        break;
                    case ControlActions.DeactivateAttack:
                        controlState = ControlState.NormalMode;
                        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                        break;
                    case ControlActions.Escape:
                        controlState = ControlState.NormalMode;
                        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                        break;
                }
                break;
            case ControlState.QueueMode:
                switch (action) {
                    case ControlActions.Move:
                        AddMoveToQueue();
                        if (actionQueue.Count == 1) {
                            PlayMoveVoiceLine();
                        }
                        break;
                    case ControlActions.OpenSelectTypeMenu:
                        controlState = ControlState.SelectingTypeFromMenu;
                        ShowSelMenu();
                        break;
                    case ControlActions.SelectOne:
                        controlState = ControlState.NormalMode;
                        SelectOne(toggle: true);
                        break;
                    case ControlActions.ActivateAttack:
                        controlState = ControlState.QueueAttackMode;
                        Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
                        break;
                    case ControlActions.DeactivateQueue:
                        controlState = ControlState.NormalMode;
                        break;
                }
                break;
            case ControlState.QueueAttackMode:
                switch (action) {
                    case ControlActions.Move:
                        AddMoveToQueue();
                        if (actionQueue.Count == 1) {
                            PlayMoveVoiceLine();
                        }
                        break;
                    case ControlActions.OpenSelectTypeMenu:
                        controlState = ControlState.SelectingTypeFromMenu;
                        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                        ShowSelMenu();
                        break;
                    case ControlActions.SelectOne:
                        AddAttackMoveToQueue();
                        if (actionQueue.Count == 1) {
                            PlayAttackMoveVoiceLine();
                        }
                        break;
                    case ControlActions.DeactivateQueue:
                        controlState = ControlState.NormalMode;
                        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                        break;
                }
                break;
            case ControlState.AbilityMode:
                switch (action) {
                    case ControlActions.Escape:
                        controlState = ControlState.NormalMode;
                        break;
                    case ControlActions.Move:
                        controlState = ControlState.NormalMode;
                        break;
                    case ControlActions.UseAbility:
                        controlState = ControlState.Transient;
                        nextAbility = ability;
                        nextCaster = caster;
                        break;
                    case ControlActions.StopUsingAbility:
                        controlState = ControlState.NormalMode;
                        break;
                }
                break;
        }
    }

    void OnSelect() {
        clickData.position = Mouse.current.position.ReadValue();
        var clickResults = new List<RaycastResult>();
        grc.Raycast(clickData, clickResults);
        if (clickResults.Count > 0) {
            return;
        }
        ProcessInput(ControlActions.Select);
    }

    void OnMove() {
        ProcessInput(ControlActions.Move);
    }

    void OnSelectType() {
        ProcessInput(ControlActions.SelectType);
    }

    void OnSelectTypeMenu() {
        ProcessInput(ControlActions.OpenSelectTypeMenu);
    }

    void OnSelectOne() {
        ProcessInput(ControlActions.SelectOne);
    }

    void OnActivateAttack() {
        ProcessInput(ControlActions.ActivateAttack);
    }

    void OnStop() {
        ProcessInput(ControlActions.Stop);
    }

    void OnQueueMode() {
        ProcessInput(ControlActions.ActivateQueue);
    }

    void OnAbility1() {
        if (selectedAbilities[0] != null && selectedAbilities[0].CanCast()) {
            ProcessInput(ControlActions.UseAbility, ability: selectedAbilities[0], caster: selectedAbilities[0].gameObject);
        }
    }
    void OnAbility2() {
        if (selectedAbilities[1] != null && selectedAbilities[1].CanCast()) {
            ProcessInput(ControlActions.UseAbility, ability: selectedAbilities[1], caster: selectedAbilities[1].gameObject);
        }
    }

    void OnAbility3() {
        if (selectedAbilities[2] != null && selectedAbilities[2].CanCast()) {
            ProcessInput(ControlActions.UseAbility, ability: selectedAbilities[2], caster: selectedAbilities[2].gameObject);
        }
    }

    void OnAbility4() {
        if (selectedAbilities[3] != null && selectedAbilities[3].CanCast()) {
            ProcessInput(ControlActions.UseAbility, ability: selectedAbilities[3], caster: selectedAbilities[3].gameObject);
        }
    }

    void OnAdvanceDialogue() {
        ProcessInput(ControlActions.AdvanceDialogue);
    }

    void OnEscape() {
        ProcessInput(ControlActions.Escape);
    }

    (Vector3, bool) RaycastToGround(Ray ray) {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask)) {
            return (hit.point, true);
        } else {
            float goalEnter;
            groundPlane.Raycast(ray, out goalEnter);
            return (ray.GetPoint(goalEnter), false);
        }
    }

    Vector3 MouseToGround() {
        var goalRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        return RaycastToGround(goalRay).Item1;
    }

    Command CommandToMouse(ActionType type) {
        var goal3 = MouseToGround();
        return new Command {
            type = type,
            goal = goal3
        };
    }

    void ExecuteCommand(Command command, bool fromQueue = false) {
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
        if (!fromQueue) {
            DestroyWaypointIndicator();
            lastWaypointIndicator = Instantiate(waypointIndicator, command.goal, Quaternion.identity);
        }
        GameObject chosenUnit = null;
        float chosenUnitDistance = 0f;
        foreach (GameObject obj in controlledUnits) {
            if (obj != null) {
                if (obj.TryGetComponent(out UnitAIV2 ai)) {
                    switch (command.type) {
                        case ActionType.Move:
                            ai.MoveToCoordinate(command.goal);
                            break;
                        case ActionType.AttackMove:
                            ai.AttackMoveToCoordinate(command.goal);
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
        if (chosenUnit != null && chosenUnit.TryGetComponent(out UnitAIV2 unitAI)) {
            UnityEvent e;
            switch (command.type) {
                case ActionType.Move:
                    e = unitAI.reachedGoalEvent;
                    break;
                case ActionType.AttackMove:
                    e = unitAI.finishedAttackingEvent;
                    break;
                default:
                    e = null;
                    break;
            }
            if (fromQueue) {
                attachedActionEvent = e;
                attachedActionEvent.AddListener(FinishExecutingAction);
            } else {
                attachedEvent = e;
                attachedEvent.AddListener(DestroyWaypointIndicator);
            }
        }
    }

    void MoveToMouse() {
        if (controlledUnits.Count == 0) {
            return;
        }
        ClearActionQueue();
        ExecuteCommand(CommandToMouse(ActionType.Move));
    }

    void AttackMove() {
        if (controlledUnits.Count == 0) {
            return;
        }
        ClearActionQueue();
        ExecuteCommand(CommandToMouse(ActionType.AttackMove));
    }

    void AddMoveToQueue() {
        if (controlledUnits.Count == 0) {
            return;
        }
        if (attachedEvent != null) {
            attachedEvent.RemoveListener(DestroyWaypointIndicator);
            attachedEvent.AddListener(FinishExecutingAction);
            attachedActionEvent = attachedEvent;
            attachedEvent = null;
            actionQueue.Enqueue((null, lastWaypointIndicator));
            lastWaypointIndicator = null;
        }
        var command = CommandToMouse(ActionType.Move);
        var indicator = Instantiate(moveWaypointIndicator, command.goal, Quaternion.identity);
        actionQueue.Enqueue((command, indicator));
        if (actionQueue.Count > 1) {
            AddQueueLine(actionQueue.ElementAt(actionQueue.Count - 2).Item2, indicator);
        }
        if (attachedActionEvent == null) {
            ExecuteCommand(command, fromQueue: true);
        }
    }

    void AddAttackMoveToQueue() {
        if (controlledUnits.Count == 0) {
            return;
        }
        if (attachedEvent != null) {
            attachedEvent.RemoveListener(DestroyWaypointIndicator);
            attachedEvent.AddListener(FinishExecutingAction);
            attachedActionEvent = attachedEvent;
            attachedEvent = null;
            actionQueue.Enqueue((null, lastWaypointIndicator));
            lastWaypointIndicator = null;
        }
        var command = CommandToMouse(ActionType.AttackMove);
        var indicator = Instantiate(attackWaypointIndicator, command.goal, Quaternion.identity);
        actionQueue.Enqueue((command, indicator));
        if (actionQueue.Count > 1) {
            AddQueueLine(actionQueue.ElementAt(actionQueue.Count - 2).Item2, indicator);
        }
        if (attachedActionEvent == null) {
            ExecuteCommand(command, fromQueue: true);
        }
    }

    void AddQueueLine(GameObject obj1, GameObject obj2) {
        var line = Instantiate(linePrefab);
        line.TryGetComponent(out LineRenderer lineRenderer);
        lineRenderer.SetPosition(0, new Vector3(obj1.transform.position.x, obj1.transform.position.y + 0.05f, obj1.transform.position.z));
        lineRenderer.SetPosition(1, new Vector3(obj2.transform.position.x, obj2.transform.position.y + 0.05f, obj2.transform.position.z));
        queueLines.Enqueue(line);
    }

    IEnumerator UseAbility(Ability ability, GameObject caster) {
        nextAbility = null;
        nextCaster = null;
        currentAbility = ability;
        currentCaster = caster;
        switch (ability.type) {
            case AbilityTypes.GroundTargetedAOE:
                var indicator = Instantiate(aoeIndicator);
                indicator.transform.localScale = Vector3.one * ability.aoeRadius * 2;
                var cast = false;
                while (controlState == ControlState.AbilityMode) {
                    if (input.actions["Select"].WasPerformedThisFrame()) {
                        ProcessInput(ControlActions.StopUsingAbility);
                        cast = true;
                        break;
                    }
                    indicator.transform.position = MouseToGround();
                    yield return null;
                }
                Destroy(indicator);
                if (cast) {
                    var units = globalUnitManager.FindNearMouse(ability.aoeRadius);
                    ability.OnCastWrapper(new AbilityCastData {
                        caster = caster,
                        friendlyUnitsHit = units.Where(unit => unit.TryGetComponent(out UnitAffiliation aff) && aff.affiliation == affiliation).ToList(),
                        enemyUnitsHit = units.Where(unit => !unit.TryGetComponent(out UnitAffiliation aff) || aff.affiliation != affiliation).ToList(),
                        targetPosition = MouseToGround(),
                    });
                }
                break;
        }
        if (controlState == ControlState.Transient) {
            controlState = ControlState.AbilityMode;
            StartCoroutine(UseAbility(nextAbility, nextCaster));
        } else {
            currentAbility = null;
            currentCaster = null;
        }
    }

    public void FinishExecutingAction() {
        if (attachedActionEvent != null) {
            attachedActionEvent.RemoveListener(FinishExecutingAction);
            attachedActionEvent = null;
            if (actionQueue.Count > 0) {
                var (command, obj) = actionQueue.Dequeue();
                Destroy(obj);
                if (actionQueue.Count > 0) {
                    var line = queueLines.Dequeue();
                    Destroy(line);
                    var (newCommand, newObj) = actionQueue.Peek();
                    ExecuteCommand(newCommand, fromQueue: true);
                }
            }
        }
    }

    void ClearActionQueue() {
        if (attachedActionEvent != null) {
            attachedActionEvent.RemoveListener(FinishExecutingAction);
            attachedActionEvent = null;
        }
        while (actionQueue.Count > 0) {
            var (command, obj) = actionQueue.Dequeue();
            Destroy(obj);
        }
        while (queueLines.Count > 0) {
            var obj = queueLines.Dequeue();
            Destroy(obj);
        }
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
            MoveToMouse();
        }
        ProcessInput(ControlActions.StopMoving);
    }

    IEnumerator ManageQueueMode() {
        while (input.actions["Queue Mode"].IsPressed()) {
            yield return null;
        }
        ProcessInput(ControlActions.DeactivateQueue);
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

    void SelectType() {
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

    GameObject lastSelectedUnit;
    float lastSelectedTime = -100f;

    IEnumerator Select() {
        UnregisterUnits();
        var selectedOne = SelectOne();
        var selectedTime = Time.time;
        var mousePos = Mouse.current.position.ReadValue();
        var initialMousePos = mousePos;
        var initialCamPos = cam.transform.position;
        var initialPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
        var finalPos = initialPos;
        UpdateSelections(initialPos, finalPos, extend: selectedOne);
        while (input.actions["Select"].IsPressed()) {
            mousePos = Mouse.current.position.ReadValue();
            if (controlState == ControlState.SelectingUnits) {
                finalPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
                UpdateSelections(initialPos + (cam.transform.position - initialCamPos), finalPos, extend: selectedOne);
            } else if (Vector2.Distance(mousePos, initialMousePos) >= minDragDistance) {
                selBox.SetActive(true);
                ProcessInput(ControlActions.StartDragging);
            }
            yield return null;
        }
        selBox.SetActive(false);
        if (selectedOne && controlledUnits.Count == 1 && controlState != ControlState.SelectingUnits) {
            var selectedUnit = controlledUnits.First();
            if (Time.time - lastSelectedTime <= doubleClickPeriod && lastSelectedUnit == selectedUnit) {
                SelectUnitsSharingType(selectedUnit);
            }
            lastSelectedUnit = selectedUnit;
            lastSelectedTime = selectedTime;
        }
        ProcessInput(ControlActions.StopSelecting);
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
        var (bottomLeftHit, _) = RaycastToGround(bottomLeftRay);
        var (bottomRightHit, _) = RaycastToGround(bottomRightRay);
        var (topLeftHit, _) = RaycastToGround(topLeftRay);
        var (topRightHit, _) = RaycastToGround(topRightRay);
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

    IEnumerator StopUnits() {
        while (input.actions["Stop"].IsPressed()) {
            foreach (var unit in controlledUnits) {
                if (unit != null && unit.TryGetComponent(out UnitAIV2 ai)) {
                    ai.Stop();
                    // ai.MoveToCoordinate(unit.transform.position);
                }
            }
            DestroyWaypointIndicator();
            ClearActionQueue();
            yield return null;
        }
        ProcessInput(ControlActions.StopStopping);
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
        DestroyWaypointIndicator();
        ClearActionQueue();
        controlledUnits.Clear();
    }

    void RegisterUnit(GameObject unit) {
        if (unit == null) return;
        unit.transform.GetChild(0).gameObject.SetActive(true);
        PlaySelectVoiceLine(unit);
    }

    void UnregisterUnit(GameObject unit) {
        if (unit == null) return;
        unit.transform.GetChild(0).gameObject.SetActive(false);
    }

    void UpdateSelectionDisplay() {
        SortedDictionary<string, List<UnitParameters>> units = new();
        if (controlledUnits.Count == 0) {
            ClearActionQueue();
            HudUI.instance.UpdateSelectedUnits(units);
            HudUI.instance.HideAbilities();
            for (var i = 0; i < numSupportedAbilities; i++) {
                selectedAbilities[i] = null;
            }
            return;
        }
        List<bool> setAbilities = new(numSupportedAbilities);
        for (var i = 0; i < numSupportedAbilities; i++) {
            setAbilities.Add(false);
        }
        bool foundUnit = false;
        foreach (var unit in controlledUnits) {
            if (unit != null) {
                foundUnit = true;
                if (unit.TryGetComponent(out UnitAffiliation unitaff) && unit.TryGetComponent(out UnitParameters unitparams)) {
                    if (units.ContainsKey(unitaff.unit_type)) {
                        units[unitaff.unit_type].Add(unitparams);
                    } else {
                        units.Add(unitaff.unit_type, new List<UnitParameters> { unitparams });
                    }
                }
                var abilities = unit.GetComponents<Ability>();
                foreach (var ability in abilities) {
                    if (ability.abilitySlot <= numSupportedAbilities && ability.abilitySlot > 0) {
                        selectedAbilities[ability.abilitySlot - 1] = ability;
                        setAbilities[ability.abilitySlot - 1] = true;
                    }
                }
            }
        }
        HudUI.instance.UpdateSelectedUnits(units);
        bool setAnyAbility = false;
        for (var i = 0; i < numSupportedAbilities; i++) {
            if (!setAbilities[i]) {
                selectedAbilities[i] = null;
                HudUI.instance.DeselectAbility(i);
                HudUI.instance.HideAbilityInfo(i);
            } else {
                setAnyAbility = true;
                HudUI.instance.SetAbilityInfo(i, selectedAbilities[i].abilityName, selectedAbilities[i].abilityIcon, selectedAbilities[i].GetCooldownProgress());
                if (currentAbility != null && currentAbility.abilitySlot - 1 == i) {
                    HudUI.instance.SelectAbility(i);
                } else {
                    HudUI.instance.DeselectAbility(i);
                }
                HudUI.instance.ShowAbilityInfo(i);
            }
        }
        if (!setAnyAbility) {
            HudUI.instance.HideAbilities();
        } else {
            HudUI.instance.ShowAbilities();
        }
        if (!foundUnit) {
            ClearActionQueue();
        }
    }

    public void SelectUnitType(int type) {
        if (type != 0) {
            SetControlledUnits(globalUnitManager.FindByTypeIdx(type - 1));
        }
        ProcessInput(ControlActions.CloseSelectTypeMenu);
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

    void PlaySelectVoiceLine(GameObject unit) {
        if (selectVoiceLineEnabled && unit.TryGetComponent(out UnitParameters unitParams)) {
            var selectVoiceLine = unitParams.getSelectVoiceLine();
            if (selectVoiceLine != null) {
                AudioManager.instance.PlayAudioClip(selectVoiceLine, unitParams.getSelectVoiceLineVolume());
                selectVoiceLineEnabled = false;
                Invoke("EnableSelectVoiceLine", selectVoiceLineCooldown);
            }
        }
    }

    void PlayMoveVoiceLine() {
        if (controlledUnits.Count == 0) {
            return;
        }
        var unit = controlledUnits[Random.Range(0, controlledUnits.Count)];
        if (moveVoiceLineEnabled && unit.TryGetComponent(out UnitParameters unitParams)) {
            var moveVoiceLine = unitParams.getMoveVoiceLine();
            if (moveVoiceLine != null) {
                AudioManager.instance.PlayAudioClip(moveVoiceLine, unitParams.getMoveVoiceLineVolume());
                moveVoiceLineEnabled = false;
                Invoke("EnableMoveVoiceLine", moveVoiceLineCooldown);
            }
        }
    }

    void PlayAttackMoveVoiceLine() {
        if (controlledUnits.Count == 0) {
            return;
        }
        var unit = controlledUnits[Random.Range(0, controlledUnits.Count)];
        if (attackMoveVoiceLineEnabled && unit.TryGetComponent(out UnitParameters unitParams)) {
            var attackMoveVoiceLine = unitParams.getSelectVoiceLine();
            if (attackMoveVoiceLine != null) {
                AudioManager.instance.PlayAudioClip(attackMoveVoiceLine, unitParams.getAttackMoveVoiceLineVolume());
                attackMoveVoiceLineEnabled = false;
                Invoke("EnableAttackMoveVoiceLine", attackMoveVoiceLineCooldown);
            }
        }
    }
}
