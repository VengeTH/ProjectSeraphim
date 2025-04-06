using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class gameOverScript : MonoBehaviour
{
    public AudioSource buttonSound;
    public Button restart;
    public Button quit;
    private bool canProceed = false;

    void Start()
    {
        // Safety checks
        if (buttonSound == null) Debug.LogError("Button Sound not assigned!");
        if (restart == null) Debug.LogError("Restart Button not assigned!");
        if (quit == null) Debug.LogError("Quit Button not assigned!");

        // Add listeners once
        restart.onClick.AddListener(RestartGame);
        quit.onClick.AddListener(QuitGame);

        // Enable input after delay
        Invoke("EnableInput", 0.5f);
    }

    void EnableInput()
    {
        canProceed = true;
        Debug.Log("Input enabled!"); // Check if this appears
    }

    public void RestartGame()
    {
        if (!canProceed) return;
        Debug.Log("Restart button pressed.");
        buttonSound.Play();
        if (CharacterScript.instance != null)
        {
            CharacterScript.instance.isGameOver = false; // Reset game over state
            CharacterScript.instance.isWin = false; // Reset win state
            Destroy(CharacterScript.instance.gameObject); // Destroy the player object
        }
        SceneManager.LoadScene(1); // Load game scene (Scene 1)
    }

    public void QuitGame()
    {
        if (!canProceed) return;
        Debug.Log("Quit button pressed.");
        buttonSound.Play();
        Application.Quit(); // Quit the application
    }
}