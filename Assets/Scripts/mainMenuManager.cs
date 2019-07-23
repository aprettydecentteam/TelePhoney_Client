using UnityEngine;


public class mainMenuManager : MonoBehaviour
{
    public void enterLobby() => sceneManager.changeScene("Lobby");

    public void quitGame() => sceneManager.quitGame();
}
