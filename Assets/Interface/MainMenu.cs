using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject camera;
    public Button[] buttons;
    void Start()
    {
        buttons[0].onClick.AddListener(StartGame);
        buttons[3].onClick.AddListener(Exit);
    }
    
    void Update()
    {
        camera.transform.position += new Vector3(0,1,0) * Time.deltaTime;
    }
    
    private void StartGame()
    {
        SceneManager.LoadScene("NewGame");
    }
    
    private void Exit()
    {
        Application.Quit();
    }
}
