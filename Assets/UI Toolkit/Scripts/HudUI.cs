using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class NotificationUI {
    public VisualElement root;
    private Label notificationText;
    private VisualElement notificationBox;

    public NotificationUI(VisualElement e) {
        root = e;
        notificationText = root.Q<Label>("NotificationText");
        notificationBox = root.Q("NotificationBox");
    }

    public void SetText(string text) {
        notificationText.text = text;
    }

    public void SetTextColor(Color color, Nullable<Color> outlineColor = null) {
        notificationText.style.color = color;
        if (outlineColor != null) {
            notificationText.style.unityTextOutlineColor = outlineColor.Value;
        }
    }

    public void SetBoxColor(Color color) {
        notificationBox.style.unityBackgroundImageTintColor = color;
    }
}

public class ObjectiveUI {
    public VisualElement root;
    private Label objectiveText;
    private VisualElement objectiveCheckbox;

    public ObjectiveUI(VisualElement e) {
        root = e;
        objectiveText = root.Q<Label>("ObjectiveText");
        objectiveCheckbox = root.Q("ObjectiveCheckbox");
    }

    public void SetText(string text) {
        objectiveText.text = text;
    }

    public void SetChecked(bool val) {
        if (val) {
            objectiveCheckbox.style.backgroundImage = HudUI.instance.checkmarkTexture;
        } else {
            objectiveCheckbox.style.backgroundImage = StyleKeyword.None;
        }
    }
}

public class SelectedUnitsUI {
    public VisualElement root;
    private Label unitTypeLabel;
    private Label unitCountLabel;
    private VisualElement unitPortrait;
    private ProgressBar unitsHealthBar;

    public SelectedUnitsUI(VisualElement e) {
        root = e;
        unitTypeLabel = root.Q<Label>("UnitType");
        unitCountLabel = root.Q<Label>("UnitCount");
        unitPortrait = root.Q("UnitPortrait");
        unitsHealthBar = root.Q<ProgressBar>("UnitHealthBar");
    }

    public void SetHealth(float val) {
        unitsHealthBar.value = val * 100f;
        var bar = unitsHealthBar.Q(className: "unity-progress-bar__progress");
        if (val > 0.5f) {
            bar.style.backgroundColor = Color.green;
        } else if (val > 0.25f) {
            bar.style.backgroundColor = Color.yellow;
        } else {
            bar.style.backgroundColor = Color.red;
        }
    }

    public void SetCount(int val) {
        unitCountLabel.text = "x" + val.ToString();
    }

    public void SetTypeName(string type) {
        unitTypeLabel.text = type;
    }

    public void SetPortrait(Sprite portrait, Nullable<Color> color = null) {
        unitPortrait.style.backgroundImage = new StyleBackground(portrait);
        if (color != null) {
            unitPortrait.style.unityBackgroundImageTintColor = color.Value;
        }
    }
}

public class AbilityUI {
    private VisualElement root;
    private Label abilityName;
    private Label abilityNumber;
    private VisualElement abilityIcon;
    private VisualElement abilityCooldownIndicator;
    private VisualElement abilitySelectionIndicator;

    public AbilityUI(VisualElement e) {
        root = e;
        Hide();
        abilityName = root.Q<Label>("AbilityName");
        abilityNumber = root.Q<Label>("AbilityNumber");
        abilityIcon = root.Q("AbilityIcon");
        abilityCooldownIndicator = root.Q("AbilityCooldownIndicator");
        abilitySelectionIndicator = root.Q("AbilitySelectionIndicator");
        SetCooldownProgress(1);
    }

    public void SetName(string name) {
        abilityName.text = name;
    }

    public void SetNumber(int number) {
        abilityNumber.text = number.ToString();
    }

    public void SetIcon(Sprite icon) {
        abilityIcon.style.backgroundImage = new StyleBackground(icon);
    }

    public void SetCooldownProgress(float value) {
        abilityCooldownIndicator.style.width = Length.Percent(100f - value * 100f);
    }

    public void ShowSelectionIndicator() {
        abilitySelectionIndicator.style.display = DisplayStyle.Flex;
    }

    public void HideSelectionIndicator() {
        abilitySelectionIndicator.style.display = DisplayStyle.None;
    }

    public void Hide() {
        root.style.display = DisplayStyle.None;
    }

    public void Show() {
        root.style.display = DisplayStyle.Flex;
    }
}

public class HudUI : MonoBehaviour {
    public static HudUI instance;

    public float tweenDuration = 0.5f;

    public VisualTreeAsset notificationTemplate;
    public VisualTreeAsset objectiveTemplate;
    public VisualTreeAsset selectedUnitsTemplate;
    public Texture2D checkmarkTexture;

    private VisualElement notificationsContainer;
    private VisualElement objectivesContainer;
    private VisualElement objectivesList;
    private VisualElement selectedUnitsContainer;
    private VisualElement selectedUnitsList;
    private VisualElement abilitiesContainer;
    private VisualElement abilitiesList;
    private VisualElement dialogueContainer;
    private VisualElement dialogueBlur;
    private Label dialogueText;
    private VisualElement dialoguePortrait;
    private Label dialoguePortraitLabel;

    private Dictionary<ulong, NotificationUI> notifications = new();
    private Dictionary<ulong, ObjectiveUI> objectives = new();
    private Dictionary<string, SelectedUnitsUI> selectedUnits = new();
    private List<AbilityUI> abilities = new();

    private void OnEnable() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        var uiDocument = GetComponent<UIDocument>();
        notificationsContainer = uiDocument.rootVisualElement.Q("Notifications");
        objectivesContainer = uiDocument.rootVisualElement.Q("Objectives");
        objectivesList = uiDocument.rootVisualElement.Q("ObjectivesList");
        selectedUnitsContainer = uiDocument.rootVisualElement.Q("SelectedUnits");
        selectedUnitsList = uiDocument.rootVisualElement.Q("SelectedUnitsList");
        abilitiesContainer = uiDocument.rootVisualElement.Q("Abilities");
        abilitiesList = abilitiesContainer.Q("AbilitiesList");
        dialogueContainer = uiDocument.rootVisualElement.Q("DialogueContainer");
        dialogueBlur = dialogueContainer.Q("DialogueBlur");
        dialogueText = dialogueContainer.Q<Label>("DialogueText");
        dialoguePortrait = dialogueContainer.Q("DialoguePortrait");
        dialoguePortraitLabel = dialogueContainer.Q<Label>("DialoguePortraitLabel");
        for (var i = 0; i < ControlSystem.instance.numSupportedAbilities; i++) {
            var ability = new AbilityUI(abilitiesList.Children().ElementAt(i));
            ability.SetNumber(i + 1);
            abilities.Add(ability);
        }
        ClearNotifications();
        ClearObjectives();
        UpdateSelectedUnits(new());
        HideAbilities();
    }

    public void AddNotification(ulong id, string text, Nullable<Color> boxColor = null, Nullable<Color> textColor = null, Nullable<Color> textOutlineColor = null) {
        StartCoroutine(AddNotificationCoroutine(id, text, boxColor, textColor, textOutlineColor));
    }

    public void RemoveNotification(ulong id) {
        StartCoroutine(RemoveNotificationCoroutine(id));
    }

    public void ClearNotifications() {
        foreach (var id in notifications.Keys) {
            RemoveNotification(id);
        }
    }

    private IEnumerator AddNotificationCoroutine(ulong id, string text, Nullable<Color> boxColor = null, Nullable<Color> textColor = null, Nullable<Color> textOutlineColor = null) {
        var e = notificationTemplate.Instantiate();
        notificationsContainer.Add(e);
        e.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue>{new TimeValue(tweenDuration)});
        e.style.right = Length.Percent(-125f);
        var notification = new NotificationUI(e);
        notification.SetText(text);
        if (boxColor != null) {
            notification.SetBoxColor(boxColor.Value);
        }
        if (textColor != null) {
            notification.SetTextColor(textColor.Value, textOutlineColor);
        }
        notifications[id] = notification;
        yield return null;
        e.style.right = Length.Percent(0f);
    }

    private IEnumerator RemoveNotificationCoroutine(ulong id) {
        notifications[id].root.style.right = Length.Percent(-125f);
        yield return new WaitForSecondsRealtime(tweenDuration);
        notificationsContainer.Remove(notifications[id].root);
        notifications.Remove(id);
    }

    public void AddObjective(ulong id, string text) {
        var e = objectiveTemplate.Instantiate();
        objectivesList.Add(e);
        var objective = new ObjectiveUI(e);
        objective.SetText(text);
        objectives[id] = objective;
        if (objectives.Count == 1) {
            ShowObjectives();
        }
    }

    public void RemoveObjective(ulong id) {
        objectivesList.Remove(objectives[id].root);
        objectives.Remove(id);
        if (objectives.Count == 0) {
            HideObjectives();
        }
    }

    public void ClearObjectives() {
        objectivesList.Clear();
        objectives.Clear();
        HideObjectives();
    }

    private void ShowObjectives() {
        objectivesContainer.style.left = Length.Percent(0f);
    }

    private void HideObjectives() {
        objectivesContainer.style.left = Length.Percent(-100f);
    }

    public void SetObjectiveComplete(ulong id, bool val) {
        objectives[id].SetChecked(val);
    }

    public void UpdateSelectedUnits(SortedDictionary<string, List<UnitParameters>> unitInfo) {
        var startingCount = selectedUnits.Count;
        var typesToRemove = new List<string>();
        foreach (var type in selectedUnits.Keys) {
            if (!unitInfo.ContainsKey(type)) {
                typesToRemove.Add(type);
            }
        }
        foreach (var type in typesToRemove) {
            selectedUnitsList.Remove(selectedUnits[type].root);
            selectedUnits.Remove(type);
        }
        foreach (var item in unitInfo) {
            var type = item.Key;
            var units = item.Value;
            var totalHealth = 0f;
            var maxHealth = 0f;
            foreach (var unitparams in units) {
                maxHealth += unitparams.maxHP;
                totalHealth += unitparams.getHP();
            }
            var healthPercent = totalHealth / maxHealth;
            if (selectedUnits.ContainsKey(type)) {
                var selectedUnitsInfo = selectedUnits[type];
                selectedUnitsInfo.SetHealth(healthPercent);
                selectedUnitsInfo.SetCount(units.Count);
            } else {
                var e = selectedUnitsTemplate.Instantiate();
                selectedUnitsList.Add(e);
                var selectedUnitsInfo = new SelectedUnitsUI(e);
                selectedUnitsInfo.SetTypeName(type);
                var (sprite, color) = GlobalUnitManager.singleton.GetPortrait(type);
                selectedUnitsInfo.SetPortrait(sprite, color);
                selectedUnitsInfo.SetHealth(healthPercent);
                selectedUnitsInfo.SetCount(units.Count);
                selectedUnits[type] = selectedUnitsInfo;
            }
        }
        if (selectedUnits.Count == 0) {
            selectedUnitsContainer.style.bottom = Length.Percent(-100f);
        } else if (startingCount == 0) {
            selectedUnitsContainer.style.bottom = Length.Percent(0f);
        }
    }

    public void SetAbilityInfo(int number, string name, Sprite icon, float cooldown) {
        var ability = abilities[number];
        ability.SetName(name);
        ability.SetIcon(icon);
        ability.SetCooldownProgress(cooldown);
    }

    public void SelectAbility(int number) {
        abilities[number].ShowSelectionIndicator();
    }

    public void DeselectAbility(int number) {
        abilities[number].HideSelectionIndicator();
    }

    public void ShowAbilityInfo(int number) {
        abilities[number].Show();
    }

    public void HideAbilityInfo(int number) {
        abilities[number].Hide();
    }

    public void ShowAbilities() {
        abilitiesContainer.style.bottom = Length.Percent(0f);
    }

    public void HideAbilities() {
        abilitiesContainer.style.bottom = Length.Percent(-125f);
    }

    public void SetDialogueText(string text) {
        dialogueText.text = text;
    }

    public void SetDialoguePortrait(Sprite portrait, Nullable<Color> color = null) {
        if (portrait != null) {
            dialoguePortrait.style.backgroundImage = new StyleBackground(portrait);
            if (color != null) {
                dialoguePortrait.style.unityBackgroundImageTintColor = color.Value;
            } else {
                dialoguePortrait.style.unityBackgroundImageTintColor = Color.white;
            }
            dialoguePortrait.style.display = DisplayStyle.Flex;
        } else {
            dialoguePortrait.style.display = DisplayStyle.None;
        }
    }

    public void SetDialogueBlur(bool val) {
        if (val) {
            dialogueBlur.style.display = DisplayStyle.Flex;
        } else {
            dialogueBlur.style.display = DisplayStyle.None;
        }
    }

    public void SetDialoguePortraitLabel(string label) {
        if (label != null) {
            dialoguePortraitLabel.text = label;
            dialoguePortraitLabel.style.display = DisplayStyle.Flex;
        } else {
            dialoguePortraitLabel.style.display = DisplayStyle.None;
        }
    }

    public void ShowDialogue() {
        dialogueContainer.style.top = Length.Percent(0f);
    }

    public void HideDialogue() {
        dialogueContainer.style.top = Length.Percent(100f);
    }
}
