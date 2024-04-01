using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public enum NotificationPriority { High, Medium, Low }

public class ToastSystem : MonoBehaviour {
    public GameObject dialoguePrefab;
    public GameObject notificationPrefab;

    public UnityEvent onRequest;

    public static ToastSystem Instance;
    private class DialogueRequest {
        public string message;
        public bool autoDismiss = true;
        public float autoDismissTime = 3f;
    }

    private class NotificationRequest {
        public string message;
        public NotificationPriority priority;
        public bool autoDismiss = true;
        public float autoDismissTime = 3f;
    }

    Queue<DialogueRequest> dialogueQueue;
    Queue<Tuple<ulong, NotificationRequest>> notificationQueue;

    Tuple<DialogueRequest, GameObject> currentDialogue;
    List<ValueTuple<ulong, NotificationRequest, GameObject, bool>> currentNotifications;

    ulong notificationCounter = 0;

    //I don't need to store the queue myself, it will auto manage?
    // public List<ToastMessageEvent> 
    // Start is called before the first frame update
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            if (onRequest == null) {
                onRequest = new UnityEvent();
            }
            dialogueQueue = new Queue<DialogueRequest>();
            notificationQueue = new Queue<Tuple<ulong, NotificationRequest>>();
            currentDialogue = null;
            currentNotifications = new List<ValueTuple<ulong, NotificationRequest, GameObject, bool>>();
        }
    }

    private void Update() {
        if (dialogueQueue.Count > 0 && currentDialogue == null) {
            StartCoroutine(DisplayDialogue(dialogueQueue.Dequeue()));
        }
        while (notificationQueue.Count > 0) {
            StartCoroutine(DisplayNotification(notificationQueue.Dequeue()));
        }
    }

    public void SendDialogue(string message, bool autoDismiss = true, float autoDismissTime = 3f) {
        dialogueQueue.Enqueue(new DialogueRequest{
            message = message,
            autoDismiss = autoDismiss,
            autoDismissTime = autoDismissTime,
        });
    }

    bool dialogueAdvanced = false;
    public void AdvanceDialogue() {
        dialogueAdvanced = true;
    }

    private IEnumerator DisplayDialogue(DialogueRequest dialogue) {
        var dialogueObject = Instantiate(dialoguePrefab, transform);
        var text = dialogueObject.GetComponentInChildren<TMP_Text>();
        text.SetText(dialogue.message);
        currentDialogue = Tuple.Create(dialogue, dialogueObject);
        dialogueAdvanced = false;
        if (dialogue.autoDismiss) {
            var elapsedTime = 0f;
            while (!dialogueAdvanced && elapsedTime < dialogue.autoDismissTime) {
                yield return null;
                elapsedTime += Time.unscaledDeltaTime;
            }
        } else {
            while (!dialogueAdvanced) {
                yield return null;
            }
        }
        currentDialogue = null;
        Destroy(dialogueObject);
    }

    private void ReflowNotifications() {
        currentNotifications.Sort((tuple1, tuple2) => tuple1.Item2.priority.CompareTo(tuple2.Item2.priority));
        var counter = 0;
        foreach (var (_, _, obj, _) in currentNotifications) {
            obj.TryGetComponent(out RectTransform rectTransform);
            rectTransform.anchoredPosition = new Vector2(0, -175 * counter);
            counter++;
        }
    }

    public ulong SendNotification(string message, NotificationPriority priority = NotificationPriority.Low, bool autoDismiss = true, float autoDismissTime = 3f) {
        var id = notificationCounter++;
        notificationQueue.Enqueue(Tuple.Create(id, new NotificationRequest {
            message = message,
            priority = priority,
            autoDismiss = autoDismiss,
            autoDismissTime = autoDismissTime,
        }));
        return id;
    }

    public void DismissNotification(ulong id) {
        var idx = currentNotifications.FindIndex(tuple => tuple.Item1 == id);
        var tuple = currentNotifications[idx];
        tuple.Item4 = true;
        currentNotifications[idx] = tuple;
    }

    private IEnumerator DisplayNotification(Tuple<ulong, NotificationRequest> request) {
        var (id, notification) = request;
        var notificationObject = Instantiate(notificationPrefab, transform);
        var text = notificationObject.GetComponentInChildren<TMP_Text>();
        text.SetText(notification.message);
        var background = notificationObject.GetComponentInChildren<Image>();
        switch (notification.priority) {
            case NotificationPriority.Low:
                background.color = Color.green;
                text.color = Color.black;
                break;
            case NotificationPriority.Medium:
                background.color = Color.yellow;
                text.color = Color.black;
                break;
            case NotificationPriority.High:
                background.color = Color.red;
                text.color = Color.white;
                break;
        }
        currentNotifications.Add((id, notification, notificationObject, false));
        ReflowNotifications();
        if (notification.autoDismiss) {
            var elapsedTime = 0f;
            while (!currentNotifications.Find(tuple => tuple.Item1 == id).Item4 && elapsedTime < notification.autoDismissTime) {
                yield return null;
                elapsedTime += Time.unscaledDeltaTime;
            }
        } else {
            while (!currentNotifications.Find(tuple => tuple.Item1 == id).Item4) {
                yield return null;
            }
        }
        Destroy(notificationObject);
        currentNotifications.RemoveAll(tuple => tuple.Item1 == id);
        ReflowNotifications();
    }
}
