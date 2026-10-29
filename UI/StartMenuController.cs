using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("Unity_Platformer");
    }

    public void OnExitClick()
    {
#if Unity_Editor
        UnityEditor.EditorApplication.isPlaying = False; 
#endif 
        Application.Quit(); 
    }
}