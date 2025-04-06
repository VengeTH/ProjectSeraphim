using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour
{
    public AudioSource buttonSound;
    private bool canProceed = false;

    void Start()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("EventSystem not found!");
            return;
        }
        if (buttonSound == null)
        {
            Debug.LogError("Button Sound not assigned!");
            return;
        }
        Invoke(nameof(EnableInput), 0.5f);
    }

    void EnableInput()
    {
        canProceed = true;
    }

    void Update()
    {
        if (!canProceed) return;

        // Handle keyboard input (ignore UI checks)
        if (Input.anyKeyDown)
        {
            Debug.Log("Keyboard input detected. Proceeding...");
            buttonSound.PlayOneShot(buttonSound.clip);
            SceneManager.LoadScene(0);
            return;
        }

        // Handle mouse/touch input (check UI)
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUIElement())
            {
                Debug.Log("Mouse/Touch over UI. Ignoring.");
                return;
            }
            Debug.Log("Mouse/Touch input detected. Proceeding...");
            buttonSound.PlayOneShot(buttonSound.clip);
            SceneManager.LoadScene(0);
        }
    }

    private bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}