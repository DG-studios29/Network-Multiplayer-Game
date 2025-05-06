using UnityEngine;
using Mirror;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ScoreboardManager : NetworkBehaviour
{

    [Header("Scoreboard UI")]
    [SerializeField] private TMP_Text[] playerNameTexts;
    [SerializeField] private TMP_Text[] playerScoreTexts;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.black;

    [Header("Player Settings")]
    [SyncVar(hook = nameof(OnNameChanged))] public string playerName;
    [SyncVar(hook = nameof(OnScoreChanged))] public int playerScore;

    //[SerializeField] private PlayerInputTemp playerData;
    [SerializeField] private CannonHUD hud;

    private static readonly List<ScoreboardManager> allPlayers = new List<ScoreboardManager>();

    public override void OnStartServer()
    {
        allPlayers.Add(this);
    }

    public override void OnStopServer()
    {
        allPlayers.Remove(this);
    }

    public override void OnStartLocalPlayer()
    {
        CmdSetPlayerName($"Player {netId}");
    }

    [Command]
    private void CmdSetPlayerName(string name)
    {
        playerName = name;
        UpdateScoreboardForAllClients();
    }

    [Command]
    public void CmdIncreaseScore(int amount)
    {
        playerScore += amount;
        UpdateScoreboardForAllClients();
    }

    private void OnNameChanged(string oldName, string newName)
    {
        UpdateScoreboardForAllClients();
    }

    private void OnScoreChanged(int oldScore, int newScore)
    {
        UpdateScoreboardForAllClients();
    }

    [Server]
    private void UpdateScoreboardForAllClients()
    {
        // Prepare data to send to all clients
        List<string> names = new List<string>();
        List<int> scores = new List<int>();

        var sorted = allPlayers.OrderByDescending(p => p.playerScore).ToList();

        foreach (var player in sorted)
        {
            names.Add(player.playerName);
            scores.Add(player.playerScore);
        }

        RpcUpdateScoreboard(names.ToArray(), scores.ToArray());
    }

    [ClientRpc]
    private void RpcUpdateScoreboard(string[] names, int[] scores)
    {
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (i < names.Length)
            {
                playerNameTexts[i].text = names[i];
                playerScoreTexts[i].text = scores[i].ToString();

                bool isTop = i == 0;
                playerNameTexts[i].color = isTop ? highlightColor : defaultColor;
                playerScoreTexts[i].color = isTop ? highlightColor : defaultColor;
            }
            else
            {
                playerNameTexts[i].text = "Waiting...";
                playerScoreTexts[i].text = "0";
                playerNameTexts[i].color = defaultColor;
                playerScoreTexts[i].color = defaultColor;
            }
        }
    }

    [ContextMenu("Manual Refresh Scoreboard")]
    public void ManualRefreshScoreboard()
    {
        if (isServer)
        {
            UpdateScoreboardForAllClients();
        }
    }

    private void OnDestroy()
    {
        if (isServer)
        {
            allPlayers.Remove(this);
        }
    }
}
