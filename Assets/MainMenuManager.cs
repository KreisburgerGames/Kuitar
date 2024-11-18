using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadSelections()
    {
        SceneManager.LoadScene("Select");
    }

    public void LoadEditorSelections()
    {
        SceneManager.LoadScene("Editor Select");
    }

    public void LoadSettings()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
