using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuUI : MonoBehaviour {
    private Button restartLevelButton;
    private Button quitToMainMenuButton;
    private VisualElement loadingOverlay;
    private VisualElement loadingOverlayElement;
    private ProgressBar progressBar;

    // Start is called before the first frame update
    private void OnEnable() {
        var uiDocument = GetComponent<UIDocument>();
        restartLevelButton = uiDocument.rootVisualElement.Q("RestartLevelButton") as Button;
        quitToMainMenuButton = uiDocument.rootVisualElement.Q("QuitToMainMenuButton") as Button;
        loadingOverlay = uiDocument.rootVisualElement.Q("LoadingOverlay");
        loadingOverlayElement = uiDocument.rootVisualElement.Q("LoadingOverlayElement");
        progressBar = uiDocument.rootVisualElement.Q("LoadingBar") as ProgressBar;
        restartLevelButton.RegisterCallback<ClickEvent>(RestartLevel);
        quitToMainMenuButton.RegisterCallback<ClickEvent>(QuitToMainMenu);
        loadingOverlay.style.display = DisplayStyle.None;
    }

    private void OnDisable() {
        restartLevelButton.UnregisterCallback<ClickEvent>(RestartLevel);
        quitToMainMenuButton.UnregisterCallback<ClickEvent>(QuitToMainMenu);
    }

    private void RestartLevel(ClickEvent evt) {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    private void QuitToMainMenu(ClickEvent evt) {
        StartCoroutine(LoadScene(0));
    }

    private IEnumerator LoadScene(int sceneIndex) {
        progressBar.value = 0;
        loadingOverlay.style.display = DisplayStyle.Flex;
        yield return null;
        loadingOverlayElement.style.opacity = 100;
        var asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad.isDone) {
            if (progressBar != null) {
                progressBar.value = asyncLoad.progress;
            }
            yield return null;
        }
    }
}
