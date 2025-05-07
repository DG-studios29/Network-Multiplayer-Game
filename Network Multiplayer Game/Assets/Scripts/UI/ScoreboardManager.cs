using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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

    private static readonly List<ScoreboardManager> allPlayers = new();

    [System.Serializable]
    public struct PlayerInfo
    {
        public string name;
        public int score;

        public PlayerInfo(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }

    #region Unity + Mirror Hooks

    public override void OnStartServer()
    {
        base.OnStartServer();
        allPlayers.Add(this);
        StartCoroutine(DelayedRefresh());
    }

    public override void OnStopServer()
    {
        allPlayers.Remove(this);
        base.OnStopServer();
        RefreshScoreboard();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isLocalPlayer)
        {
            CmdSetPlayerName($"Player {netId}");
            CmdRequestScoreboardUpdate();
        }

       
        StartCoroutine(ForceLocalUIRefresh());
    }

    #endregion

    #region SyncVar Hooks

    private void OnNameChanged(string _, string __)
    {
        RefreshScoreboard();
    }

    private void OnScoreChanged(int _, int __)
    {
        RefreshScoreboard();
    }

    #endregion

    #region Commands

    [Command]
    public void CmdIncreaseScore(int amount)
    {
        Debug.Log($"[Server] Increasing score for {playerName} by {amount}");
        playerScore += amount;
        RefreshScoreboard();
    }

    [Command]
    public void CmdSetPlayerName(string name)
    {
        playerName = name;
    }

    [Command]
    public void CmdRequestScoreboardUpdate()
    {
        RefreshScoreboard();
    }

    #endregion

    #region Scoreboard Logic

    private IEnumerator DelayedRefresh()
    {
        yield return new WaitForSeconds(0.2f);
        RefreshScoreboard();
    }

    private void RefreshScoreboard()
    {
        if (!isServer) return;

        var sortedList = allPlayers
            .Select(p => new PlayerInfo(p.playerName, p.playerScore))
            .OrderByDescending(p => p.score)
            .ToArray();

        RpcUpdateScoreboardUI(sortedList);

       
        if (isClient)
        {
            Debug.Log("[Host] Forcing scoreboard UI update locally on host.");
            UpdateScoreboardUI(sortedList);
        }
    }

    [ClientRpc]
    private void RpcUpdateScoreboardUI(PlayerInfo[] playerInfos)
    {
        Debug.Log("[Client] Updating scoreboard UI via RPC");
        UpdateScoreboardUI(playerInfos);
    }

    private void UpdateScoreboardUI(PlayerInfo[] playerInfos)
    {
        if (playerNameTexts == null || playerScoreTexts == null || playerNameTexts.Length == 0)
        {
            Debug.LogWarning("[UI] Text arrays not assigned!");
            return;
        }

        Debug.Log($"[UI] Updating scoreboard locally. Player count: {playerInfos.Length}");

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (i < playerInfos.Length)
            {
                var p = playerInfos[i];
                playerNameTexts[i].text = p.name;
                playerScoreTexts[i].text = p.score.ToString();

                bool isLeader = (i == 0);
                playerNameTexts[i].color = isLeader ? highlightColor : defaultColor;
                playerScoreTexts[i].color = isLeader ? highlightColor : defaultColor;
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

    private IEnumerator ForceLocalUIRefresh()
    {
       
        yield return new WaitForSeconds(1f);
        CmdRequestScoreboardUpdate();
    }

    #endregion
}
