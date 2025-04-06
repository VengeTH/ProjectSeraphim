using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public Button myButton;
    public Button Quit;
    public AudioSource buttonSound;

    void Start()
    {
        // Only handle Start button and Quit
        myButton.onClick.AddListener(ButtonPressed);
        Quit.onClick.AddListener(() => Application.Quit());
    }

    public void ButtonPressed()
    {
        buttonSound.Play();
        SceneManager.LoadScene(1); // Load game scene
    }

    void OnApplicationQuit()
    {
        buttonSound.Play();
        Application.Quit();
    }
}