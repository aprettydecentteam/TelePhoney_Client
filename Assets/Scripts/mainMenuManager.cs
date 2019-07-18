using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuManager : MonoBehaviour
{
    public void enterLobby() => SceneManager.LoadScene("Lobby", LoadSceneMode.Additive);

    public void quitGame() => Application.Quit();
}
