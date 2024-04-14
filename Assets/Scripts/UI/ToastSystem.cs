using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TMPro;
using Unity.VisualScripting;


//using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class ToastSystem : MonoBehaviour {
    public static ToastSystem instance;
    private class DialogueRequest {
        public string message;
        public bool autoDismiss = true;
        public float autoDismissTime = 3f;
        public Sprite portrait = null;
        public Nullable<Color> portraitColor = Color.white;
        public string portraitLabel;
        public bool blur = false;
    }

    private class NotificationRequest {
        public string message;
        public bool autoDismiss = true;
        public float autoDismissTime = 3f;
        public Nullable<Color> boxColor = null;
        public Nullable<Color> textColor = null;
        public Nullable<Color> textOutlineColor = null;
    }

    Queue<DialogueRequest> dialogueQueue;
    Queue<Tuple<ulong, NotificationRequest>> notificationQueue;
    Queue<Tuple<ulong, string>> objectiveQueue;

    DialogueRequest currentDialogue;
    Dictionary<ulong, (NotificationRequest, bool)> currentNotifications;
    HashSet<ulong> currentObjectives;

    ulong notificationCounter = 0;
    ulong objectiveCounter = 0;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        dialogueQueue = new();
        notificationQueue = new();
        objectiveQueue = new();
        currentDialogue = null;
        currentNotifications = new();
        currentObjectives = new();
    }

    private void Update() {
        if (dialogueQueue.Count > 0 && currentDialogue == null) {
            StartCoroutine(DisplayDialogue(dialogueQueue.Dequeue()));
        }
        while (notificationQueue.Count > 0) {
            StartCoroutine(DisplayNotification(notificationQueue.Dequeue()));
        }
        while (objectiveQueue.Count > 0) {
            DisplayObjective(objectiveQueue.Dequeue());
        }
    }

    public void SendDialogue(string message, bool autoDismiss = true, float autoDismissTime = 3f, Sprite portrait = null, Nullable<Color> portraitColor = null, string portraitLabel = null, bool blur = false) {
        dialogueQueue.Enqueue(new DialogueRequest{
            message = message,
            autoDismiss = autoDismiss,
            autoDismissTime = autoDismissTime,
            portrait = portrait,
            portraitColor = portraitColor,
            portraitLabel = portraitLabel,
            blur = blur,
        });
    }

    bool dialogueFullySent = false;
    bool dialogueAdvanced = false;
    public void AdvanceDialogue() {
        if (!dialogueFullySent) {
            dialogueFullySent = true;
            return;
        }
        dialogueAdvanced = true;
    }

    private IEnumerator DisplayDialogue(DialogueRequest dialogue) {
        var message = "";
        HudUI.instance.SetDialogueText(message);
        HudUI.instance.SetDialoguePortrait(dialogue.portrait, dialogue.portraitColor);
        HudUI.instance.SetDialoguePortraitLabel(dialogue.portraitLabel);
        HudUI.instance.SetDialogueBlur(dialogue.blur);
        HudUI.instance.ShowDialogue();
        currentDialogue = dialogue;
        yield return new WaitForSecondsRealtime(HudUI.instance.tweenDuration);
        dialogueFullySent = false;
        dialogueAdvanced = false;
        var messageIdx = 0;
        if (dialogue.autoDismiss) {
            var elapsedTime = 0f;
            while (!dialogueAdvanced && elapsedTime < dialogue.autoDismissTime) {
                yield return null;
                elapsedTime += Time.unscaledDeltaTime;
                if (!dialogueFullySent) {
                    message += dialogue.message[messageIdx++];
                    HudUI.instance.SetDialogueText(message);
                    if (messageIdx == dialogue.message.Length) {
                        dialogueFullySent = true;
                    }
                } else {
                    HudUI.instance.SetDialogueText(dialogue.message);
                }
            }
        } else {
            while (!dialogueAdvanced) {
                yield return null;
                if (!dialogueFullySent) {
                    message += dialogue.message[messageIdx++];
                    HudUI.instance.SetDialogueText(message);
                    if (messageIdx == dialogue.message.Length) {
                        dialogueFullySent = true;
                    }
                } else {
                    HudUI.instance.SetDialogueText(dialogue.message);
                }
            }
        }
        currentDialogue = null;
        if (dialogueQueue.Count == 0) {
            HudUI.instance.HideDialogue();
        }
    }

    public ulong SendNotification(string message, bool autoDismiss = true, float autoDismissTime = 3f, Nullable<Color> boxColor = null, Nullable<Color> textColor = null, Nullable<Color> textOutlineColor = null) {
        var id = notificationCounter++;
        notificationQueue.Enqueue(Tuple.Create(id, new NotificationRequest {
            message = message,
            autoDismiss = autoDismiss,
            autoDismissTime = autoDismissTime,
            boxColor = boxColor,
            textColor = textColor,
            textOutlineColor = textOutlineColor,
        }));
        return id;
    }

    public void DismissNotification(ulong id) {
        var tuple = currentNotifications[id];
        tuple.Item2 = true;
        currentNotifications[id] = tuple;
    }

    private IEnumerator DisplayNotification(Tuple<ulong, NotificationRequest> request) {
        var (id, notification) = request;
        HudUI.instance.AddNotification(id, notification.message, notification.boxColor, notification.textColor, notification.textOutlineColor);
        currentNotifications[id] = (notification, false);
        if (notification.autoDismiss) {
            var elapsedTime = 0f;
            while (!currentNotifications[id].Item2 && elapsedTime < notification.autoDismissTime) {
                yield return null;
                elapsedTime += Time.unscaledDeltaTime;
            }
        } else {
            while (!currentNotifications[id].Item2) {
                yield return null;
            }
        }
        HudUI.instance.RemoveNotification(id);
        currentNotifications.Remove(id);
    }

    public ulong SendObjective(string objective) {
        var id = objectiveCounter++;
        objectiveQueue.Enqueue(Tuple.Create(id, objective));
        return id;
    }

    public void CompleteObjective(ulong id) {
        HudUI.instance.SetObjectiveComplete(id, true);
    }

    public void UncompleteObjective(ulong id) {
        HudUI.instance.SetObjectiveComplete(id, false);
    }

    public void RemoveObjective(ulong id) {
        HudUI.instance.RemoveObjective(id);
        currentObjectives.Remove(id);
    }

    private void DisplayObjective(Tuple<ulong, string> objective) {
        var (id, message) = objective;
        HudUI.instance.AddObjective(id, message);
        currentObjectives.Add(id);
    }
}
