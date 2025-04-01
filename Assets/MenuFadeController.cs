using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFadeController : MonoBehaviour
{
    private FadeUI fadeUI;
    [SerializeField] private float fadeTime;
    [SerializeField] private string transitionTo;

    // Start is called before the first frame update
    void Start()
    {
        fadeUI = GetComponent<FadeUI>();
        fadeUI.FadeUIOut(fadeTime);
    }

    public void CallFadeAndStartGame(string sceneToLoad)
    {
        StartCoroutine(FadeAndStartGame(sceneToLoad));
    }

    public void ChangeScene(string sceneToLoad)
    {
        SceneManager.LoadScene(transitionTo);
    }

    IEnumerator FadeAndStartGame(string sceneToLoad)
    {
        fadeUI.FadeUIOut(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneToLoad);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
