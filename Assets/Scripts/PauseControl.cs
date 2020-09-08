using Assets.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseControl : MonoBehaviour
{
    private float last_select;
    public float moveInterval;  // non-target movement
    public int selected = 0;
    public List<GameObject> selections;

    private GameObject pauseScreen;
    // Start is called before the first frame update
    void Start()
    {
        pauseScreen = GameObject.Find("UI").transform.Find("PauseScreen").gameObject.transform.Find("screen").gameObject;
        selections = new List<GameObject>();
        selections.Add(pauseScreen.transform.Find("startscreen").gameObject);
        selections.Add(pauseScreen.transform.Find("exit").gameObject);

        for(int i = 0; i < selections.Count;i++)
        {
            selections[i].transform.Find("selected").gameObject.SetActive(false);
        }
        selections[0].transform.Find("selected").gameObject.SetActive(false);

        pauseScreen.SetActive(false);
        last_select = Time.time;
    }

    void FixedUpdate()
    {
        if (GameData.paused)
        {
            if (!pauseScreen.activeSelf)
            {
                pauseScreen.SetActive(true);
                SetSelection(selected);
            }
            if (Time.time - last_select > moveInterval)
            {
                if (Cin.CheckKey("XBoxBack", GameData.pausedPlayer, Cin.greaterThan))
                {
                    GameData.paused = false;
                    pauseScreen.SetActive(false);
                    last_select = Time.time;
                }
                else if (Cin.CheckKey("XBoxVert", GameData.pausedPlayer, Cin.greaterThan))
                {
                    SetSelection(selected - 1);
                }
                else if (Cin.CheckKey("XBoxVert", GameData.pausedPlayer, Cin.lessThan))
                {
                    SetSelection(selected + 1);
                }
                else if (Cin.CheckKey("XBoxAction", GameData.pausedPlayer, Cin.greaterThan))
                {
                    HandleSelected();
                }
            }
        }
    }

    void SetSelection(int newSelected)
    {
        
        if (newSelected >= 0 && newSelected < selections.Count)
        {
            selections[selected].transform.Find("selected").gameObject.SetActive(false);
            selections[newSelected].transform.Find("selected").gameObject.SetActive(false);

            selections[newSelected].transform.Find("selected").gameObject.SetActive(true);
            selected = newSelected;
            last_select = Time.time;
        }
    }

    void HandleSelected()
    {
        if (selected == 1)  // exit game
        {
            Debug.Log("exiting game");
            Application.Quit();
        }
        if (selected == 0)
        {
            GameData.paused = false;
            SceneManager.LoadScene(0); // load char select
        }
    }
}
