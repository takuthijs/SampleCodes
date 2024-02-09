using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private GameObject _loadingUI;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _sliderText;
    public static LoadingManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        SceneManager.activeSceneChanged += ActiveSceneChanged;
    }

    void ActiveSceneChanged(Scene thisScene, Scene nextScene)
    {
        instance._loadingUI.SetActive(false);
    }

    public void LoadNextScene(string nextScene)
    {
        Debug.Log("LoadNextScene");
        _loadingUI.SetActive(true);
        StartCoroutine(LoadScene(nextScene));
    }

    IEnumerator LoadScene(string SceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(SceneName);
        while (!async.isDone)
        {
            Debug.Log(async.progress);
            _slider.value = Mathf.Round(async.progress * 1.1f * 0.9f);
            _sliderText.text = Mathf.Round(_slider.value * 100).ToString("F0") + "%";
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
