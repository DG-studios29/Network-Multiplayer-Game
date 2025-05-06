using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;

public class EndGameManager : NetworkBehaviour
{
    [Header("UI Elements")]
    public GameObject endGameCanvas;
    public TextMeshProUGUI winnerText;

    void Start()
    {
        
        if (endGameCanvas != null)
        {
            endGameCanvas.SetActive(false);
        }
    }

    [Server]
    public void ShowWinner(string winnerName)
    {
        Debug.Log("[EndGameManager] Showing winner: " + winnerName);
        RpcShowEndGameScreen(winnerName + " Wins!");
    }

    [ClientRpc]
    public void RpcShowEndGameScreen(string winnerDisplayText)
    {
        Debug.Log("[EndGameManager] Displaying winner text: " + winnerDisplayText);

        if (endGameCanvas != null)
            endGameCanvas.SetActive(true);

        if (winnerText != null)
            winnerText.text = winnerDisplayText;
    }
}
