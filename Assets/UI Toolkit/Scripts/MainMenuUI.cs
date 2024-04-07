using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour {
    private Button startButton;
    private VisualElement loadingOverlay;
    private ProgressBar progressBar;

    // Start is called before the first frame update
    private void OnEnable() {
        var uiDocument = GetComponent<UIDocument>();
        startButton = uiDocument.rootVisualElement.Q("StartButton") as Button;
        loadingOverlay = uiDocument.rootVisualElement.Q("LoadingOverlay");
        progressBar = uiDocument.rootVisualElement.Q("LoadingBar") as ProgressBar;
        startButton.RegisterCallback<ClickEvent>(StartGame);
    }

    private void OnDisable() {
        startButton.UnregisterCallback<ClickEvent>(StartGame);
    }

    private void StartGame(ClickEvent evt) {
        StartCoroutine(LoadInitialScene());
    }

    private IEnumerator LoadInitialScene() {
        var asyncLoad = SceneManager.LoadSceneAsync(1);
        loadingOverlay.style.visibility = Visibility.Visible;
        loadingOverlay.style.opacity = 100;
        while (!asyncLoad.isDone) {
            progressBar.value = asyncLoad.progress;
            yield return null;
        }
    }
}
