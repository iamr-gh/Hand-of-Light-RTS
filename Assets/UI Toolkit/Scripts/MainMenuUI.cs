using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour {
    public static MainMenuUI instance;

    public GameObject mainMenu;
    public GameObject levelSelect;
    public GameObject credits;
    public GameObject loadingOverlay;

    public AudioClip menuMusic;
    public AudioClip backgroundMusic;

    private UIDocument mainMenuUiDocument;
    private UIDocument levelSelectUiDocument;
    private UIDocument creditsUiDocument;
    private UIDocument loadingOverlayUiDocument;

    private Button startButton;
    private Button levelSelectButton;
    private Button creditsButton;
    private Button quitButton;
    private Button creditsReturnButton;
    private Button levelSelectReturnButton;
    private VisualElement loadingOverlayElement;
    private ProgressBar progressBar;

    // Start is called before the first frame update
    private void OnEnable() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        mainMenuUiDocument = mainMenu.GetComponent<UIDocument>();
        startButton = mainMenuUiDocument.rootVisualElement.Q("StartButton") as Button;
        levelSelectButton = mainMenuUiDocument.rootVisualElement.Q("LevelSelectButton") as Button;
        creditsButton = mainMenuUiDocument.rootVisualElement.Q("CreditsButton") as Button;
        quitButton = mainMenuUiDocument.rootVisualElement.Q("QuitButton") as Button;
        startButton.RegisterCallback<ClickEvent>(StartGame);
        levelSelectButton.RegisterCallback<ClickEvent>(SwitchToLevelSelect);
        creditsButton.RegisterCallback<ClickEvent>(SwitchToCredits);
        quitButton.RegisterCallback<ClickEvent>(QuitGame);
        levelSelectUiDocument = levelSelect.GetComponent<UIDocument>();
        levelSelectReturnButton = levelSelectUiDocument.rootVisualElement.Q("ReturnToMainMenuButton") as Button;
        levelSelectReturnButton.RegisterCallback<ClickEvent>(SwitchToMainMenu);
        creditsUiDocument = credits.GetComponent<UIDocument>();
        creditsReturnButton = creditsUiDocument.rootVisualElement.Q("ReturnToMainMenuButton") as Button;
        creditsReturnButton.RegisterCallback<ClickEvent>(SwitchToMainMenu);
        loadingOverlayUiDocument = loadingOverlay.GetComponent<UIDocument>();
        loadingOverlayElement = loadingOverlayUiDocument.rootVisualElement.Q("LoadingOverlayElement");
        progressBar = loadingOverlayUiDocument.rootVisualElement.Q("LoadingBar") as ProgressBar;
        mainMenuUiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        levelSelectUiDocument.rootVisualElement.style.display = DisplayStyle.None;
        creditsUiDocument.rootVisualElement.style.display = DisplayStyle.None;
        loadingOverlayUiDocument.rootVisualElement.style.display = DisplayStyle.None;
        AudioManager.instance.StopBGM();
        if (menuMusic != null) {
            AudioManager.instance.ChangeBGMClip(menuMusic);
            AudioManager.instance.PlayBGM();
        }
    }

    private void OnDisable() {
        startButton.UnregisterCallback<ClickEvent>(StartGame);
        levelSelectButton.UnregisterCallback<ClickEvent>(SwitchToLevelSelect);
        creditsButton.UnregisterCallback<ClickEvent>(SwitchToCredits);
        quitButton.UnregisterCallback<ClickEvent>(QuitGame);
        levelSelectReturnButton.UnregisterCallback<ClickEvent>(SwitchToMainMenu);
        creditsReturnButton.UnregisterCallback<ClickEvent>(SwitchToMainMenu);
    }

    private void StartGame(ClickEvent evt) {
        LoadScene(1);
    }

    private void SwitchToLevelSelect(ClickEvent evt) {
        levelSelectUiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        mainMenuUiDocument.rootVisualElement.style.display = DisplayStyle.None;
        creditsUiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void SwitchToCredits(ClickEvent evt) {
        creditsUiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        mainMenuUiDocument.rootVisualElement.style.display = DisplayStyle.None;
        levelSelectUiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void SwitchToMainMenu(ClickEvent evt) {
        mainMenuUiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        levelSelectUiDocument.rootVisualElement.style.display = DisplayStyle.None;
        creditsUiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void QuitGame(ClickEvent evt) {
        Application.Quit();
    }

    public void LoadScene(int id) {
        StartCoroutine(LoadSceneCoroutine(id));
    }

    private IEnumerator LoadSceneCoroutine(int id) {
        progressBar.value = 0;
        loadingOverlayUiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        yield return null;
        loadingOverlayElement.style.opacity = 100;
        if (backgroundMusic != null) {
            AudioManager.instance.StopBGM();
            AudioManager.instance.ChangeBGMClip(backgroundMusic);
            AudioManager.instance.PlayBGM();
        }
        var asyncLoad = SceneManager.LoadSceneAsync(id);
        while (!asyncLoad.isDone) {
            if (progressBar != null) {
                progressBar.value = asyncLoad.progress;
            }
            yield return null;
        }
    }
}
