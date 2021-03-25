using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool gamePaused = false;
    public GameObject pauseMenuUI;
    public Texture2D hover_cursor, click_cursor;
    public UnityEngine.UI.Button[] buttons;

    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        //GetComponent<Canvas>().worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (gamePaused)
            {
                Resume();
            }
            else {
                Pause();
            }
        }
    }
    public void Resume() {
        Cursor.SetCursor(hover_cursor, Vector2.zero, CursorMode.ForceSoftware);
        EventManager.Instance.Game_Paused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }
    void Pause() {
        Cursor.SetCursor(click_cursor, Vector2.zero, CursorMode.ForceSoftware);
        EventManager.Instance.Game_Paused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;

        buttons = GetComponentsInChildren<UnityEngine.UI.Button>();
        foreach (UnityEngine.UI.Button b in buttons)
        {
            if (b.gameObject.name.Equals("MuteButton"))
            {
                b.gameObject.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text
                    = GameSave.Muted ? "Unmute" : "Mute";
            }
        }
    }
    public void muteGame() {
        Debug.Log("Muting game....");
        CameraController.Instance.Mute(!GameSave.ToggleMute());

        foreach (UnityEngine.UI.Button b in buttons)
        {
            if (b.gameObject.name.Equals("MuteButton"))
            {
                //Debug.Log("mute button changing -> " + (GameSave.Muted ? "Unmute" : "Mute"));
                b.gameObject.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text
                    = GameSave.Muted ? "Unmute" : "Mute";
            }
        }
    }

    public void quitGame()
    {
        if (ScriptableObjectManager.Instance.ScriptManager.CurrentScript.name_.Equals(ScriptDispenser.MAINMENU))
        {
            Debug.Log("Currently in main menu, quit directly!");
            Application.Quit();
        } else if (ScriptableObjectManager.Instance.ScriptManager.CurrentScript.name_.Equals(ScriptDispenser.TUTORIAL))
        {
            Debug.Log("Currently in tutorial, not saving");
            EventManager.Instance.TransitionTo(ScriptDispenser.MAINMENU, false);
            Time.timeScale = 1.0f;
        }
        else
        {
            Debug.Log("Currently in plot, saving and then quitting!");
            GameSave.SaveProgress();
            EventManager.Instance.TransitionTo(ScriptDispenser.MAINMENU, false);
            Time.timeScale = 1.0f;
        }
    }
}
