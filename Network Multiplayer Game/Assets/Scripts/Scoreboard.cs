// This script is for a scoreboard in a multiplayer game. The scoreboard will display the scores of all players in the game, by dynamically fetching the score values from a variable. It should have full mulitplayer functionality.
// Simulates players and score values for testing purposes.

// Note: Currently, placeholder values are used for player names and scores.
// Will add multiplayer functionality later.


using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class Scoreboard : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnPlayer1ScoreChanged))]
    public int player1Score = 0;

    [SyncVar(hook = nameof(OnPlayer2ScoreChanged))]
    public int player2Score = 0;

    [SyncVar(hook = nameof(OnPlayer3ScoreChanged))]
    public int player3Score = 0;

    [SyncVar(hook = nameof(OnPlayer4ScoreChanged))]
    public int player4Score = 0;

    public GameObject scoreboard;

    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI player3ScoreText;
    public TextMeshProUGUI player4ScoreText;

    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player3NameText;
    public TextMeshProUGUI player4NameText;

    public Button incrementPlayer1Button;
    public Button incrementPlayer2Button;
    public Button incrementPlayer3Button;
    public Button incrementPlayer4Button;

    public override void OnStartClient()
    {
        base.OnStartClient();
      
        NetworkServer.Spawn(scoreboard);
        
        UpdateScoreUI();

        
        if (isLocalPlayer)
        {
            incrementPlayer1Button.onClick.AddListener(() => CmdIncrementScore(1));
            incrementPlayer2Button.onClick.AddListener(() => CmdIncrementScore(2));
            incrementPlayer3Button.onClick.AddListener(() => CmdIncrementScore(3));
            incrementPlayer4Button.onClick.AddListener(() => CmdIncrementScore(4));
        }
    }

    private void UpdateScoreUI()
    {
        player1ScoreText.text = player1Score.ToString();
        player2ScoreText.text = player2Score.ToString();
        player3ScoreText.text = player3Score.ToString();
        player4ScoreText.text = player4Score.ToString();
    }

    
    private void OnPlayer1ScoreChanged(int oldScore, int newScore)
    {
        player1ScoreText.text = newScore.ToString();
    }

    private void OnPlayer2ScoreChanged(int oldScore, int newScore)
    {
        player2ScoreText.text = newScore.ToString();
    }

    private void OnPlayer3ScoreChanged(int oldScore, int newScore)
    {
        player3ScoreText.text = newScore.ToString();
    }

    private void OnPlayer4ScoreChanged(int oldScore, int newScore)
    {
        player4ScoreText.text = newScore.ToString();
    }

    [Command]
    public void CmdIncrementScore(int playerIndex)
    {
       
        switch (playerIndex)
        {
            case 1:
                player1Score += 10;
                break;
            case 2:
                player2Score += 10;
                break;
            case 3:
                player3Score += 10;
                break;
            case 4:
                player4Score += 10;
                break;
        }
    }

   
    [ContextMenu("Refresh Score UI")]
    public void RefreshScoreUI()
    {
        OnPlayer1ScoreChanged(player1Score, player1Score);
        OnPlayer2ScoreChanged(player2Score, player2Score);
        OnPlayer3ScoreChanged(player3Score, player3Score);
        OnPlayer4ScoreChanged(player4Score, player4Score);
    }
}
