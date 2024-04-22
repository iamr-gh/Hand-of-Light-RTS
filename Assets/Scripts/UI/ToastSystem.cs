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

    public UnityEvent onDialogueAdvanced;

    private class DialogueRequest {
        public string message;
        public bool autoDismiss = true;
        public float autoDismissTime = 3f;
        public Sprite portrait = null;
        public Nullable<Color> portraitColor = Color.white;
        public string portraitLabel;
        public bool blur = false;
        public AudioClip audioClip = null;
        public float audioVolume = 1f;
    }

    private class NotificationRequest {
        public string message;
        public bool autoDismiss = true;
        public float autoDismissTime = 3f;
        public Nullable<Color> boxColor = null;
        public Nullable<Color> textColor = null;
        public Nullable<Color> textOutlineColor = null;
        public AudioClip audioClip = null;
        public float audioVolume = 1f;
    }

    Queue<DialogueRequest> dialogueQueue;
    Queue<Tuple<ulong, NotificationRequest>> notificationQueue;

    DialogueRequest currentDialogue;
    Dictionary<ulong, (NotificationRequest, bool, AudioClip, float)> currentNotifications;
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
    }

    public void SendDialogue(string message, bool autoDismiss = true, float autoDismissTime = 3f, Sprite portrait = null, Nullable<Color> portraitColor = null, string portraitLabel = null, bool blur = false, AudioClip audioClip = null, float audioVolume = 1f) {
        dialogueQueue.Enqueue(new DialogueRequest {
            message = message,
            autoDismiss = autoDismiss,
            autoDismissTime = autoDismissTime,
            portrait = portrait,
            portraitColor = portraitColor,
            portraitLabel = portraitLabel,
            blur = blur,
            audioClip = audioClip,
            audioVolume = audioVolume,
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
        if (dialogue.audioClip != null) {
            AudioManager.instance.PlayDialogue(dialogue.audioClip, dialogue.audioVolume);
        }
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
        onDialogueAdvanced.Invoke();
        AudioManager.instance.StopDialogue();
    }

    public ulong SendNotification(string message, bool autoDismiss = true, float autoDismissTime = 3f, Nullable<Color> boxColor = null, Nullable<Color> textColor = null, Nullable<Color> textOutlineColor = null, AudioClip audioClip = null, float audioVolume = 1f) {
        var id = notificationCounter++;
        notificationQueue.Enqueue(Tuple.Create(id, new NotificationRequest {
            message = message,
            autoDismiss = autoDismiss,
            autoDismissTime = autoDismissTime,
            boxColor = boxColor,
            textColor = textColor,
            textOutlineColor = textOutlineColor,
            audioClip = audioClip,
            audioVolume = audioVolume,
        }));
        return id;
    }

    public void DismissNotification(ulong id, AudioClip audioClip = null, float audioVolume = 1f) {
        if (!currentNotifications.ContainsKey(id)) {
            return;
        }
        var tuple = currentNotifications[id];
        tuple.Item2 = true;
        tuple.Item3 = audioClip;
        tuple.Item4 = audioVolume;
        currentNotifications[id] = tuple;
    }

    private IEnumerator DisplayNotification(Tuple<ulong, NotificationRequest> request) {
        var (id, notification) = request;
        HudUI.instance.AddNotification(id, notification.message, notification.boxColor, notification.textColor, notification.textOutlineColor);
        currentNotifications[id] = (notification, false, null, 1f);
        // yield return new WaitForSecondsRealtime(HudUI.instance.tweenDuration);
        if (notification.audioClip != null) {
            AudioManager.instance.PlayAudioClip(notification.audioClip, notification.audioVolume);
        }
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
        var (_, _, audioClip, audioVolume) = currentNotifications[id];
        if (audioClip != null) {
            AudioManager.instance.PlayAudioClip(audioClip, audioVolume);
        }
        HudUI.instance.RemoveNotification(id);
        currentNotifications.Remove(id);
    }

    public ulong SendObjective(string message, AudioClip audioClip = null, float audioVolume = 1f) {
        var id = objectiveCounter++;
        HudUI.instance.AddObjective(id, message);
        if (audioClip != null) {
            AudioManager.instance.PlayAudioClip(audioClip, audioVolume);
        }
        currentObjectives.Add(id);
        return id;
    }

    public void CompleteObjective(ulong id, AudioClip audioClip = null, float audioVolume = 1f) {
        HudUI.instance.SetObjectiveComplete(id, true);
        if (audioClip != null) {
            AudioManager.instance.PlayAudioClip(audioClip, audioVolume);
        }
    }

    public void UncompleteObjective(ulong id, AudioClip audioClip = null, float audioVolume = 1f) {
        HudUI.instance.SetObjectiveComplete(id, false);
        if (audioClip != null) {
            AudioManager.instance.PlayAudioClip(audioClip, audioVolume);
        }
    }

    public void FailObjective(ulong id, AudioClip audioClip = null, float audioVolume = 1f) {
        HudUI.instance.SetObjectiveFailed(id);
        if (audioClip != null) {
            AudioManager.instance.PlayAudioClip(audioClip, audioVolume);
        }
    }

    public void RemoveObjective(ulong id, AudioClip audioClip = null, float audioVolume = 1f) {
        HudUI.instance.RemoveObjective(id);
        currentObjectives.Remove(id);
        if (audioClip != null) {
            AudioManager.instance.PlayAudioClip(audioClip, audioVolume);
        }
    }
}
