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
    [SerializeField] private Color highlightColor = Color.green; 
    [SerializeField] private Color defaultColor = Color.black; 

    [Header("Player Settings")]
    [SyncVar(hook = nameof(OnScoreChanged))] public string playerName; 
    [SyncVar(hook = nameof(OnScoreChanged))] public int playerScore;

    //[SerializeField] private PlayerInputTemp playerData;
    [SerializeField] private CannonHUD hud;

    private static List<ScoreboardManager> allPlayers = new List<ScoreboardManager>();

    //[SerializeField]private LBController controller;


/*    [Server]
    public void GetController(ScoreboardManager entry)
    {
        //controller = 
        LBController.Instance.playerScores.Add(entry);
    }
    
    [ClientRpc]
    public void RcpGetController(ScoreboardManager entry)
    {
        LBController.Instance.playerScores.Add(entry);
    }
*/


    public override void OnStartServer()
    {
        base.OnStartServer();
        allPlayers.Add(this);

       
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        allPlayers.Remove(this);
    }

    [Command]
    public void AddToServerList(ScoreboardManager player)
    {
        //bool check = false;
        /*    foreach (ScoreboardManager entry in allPlayers)
            {
                if(entry != player)
                {

                     check = false
                    //LBController.Instance.
                }
            }*/

        allPlayers.Add(player);
        //controller.playerScores.Add(player);
        //GetController(player);

    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        


        //playerName = $"Player {netId}";
        playerName = hud.GetCustomName();
        AddToServerList(this);
        //AddToServerList(this);
        //ScoreNameChange();
        
        //playerName = playerData.CaptainName;

        //PlayerInputTemp playerDat = NetworkClient.localPlayer.gameObject.GetComponent<PlayerInputTemp>();
        //playerName = playerDat.CaptainName;

     
        // = this.GetComponentInParent<PlayerInputTemp>();

        /*  if (playerData != null)
          {
              playerName = playerData.CaptainName;
              RefreshScoreboard();

          }*/
        //add but set name and scores first
        
        RefreshScoreboard();

    }


    [Command]
    public void ScoreNameChange()
    {

        //playerName = hud.GetCustomName();
        //Debug.Log("Back and forth");
        clientScoreNameChange();
    }

   [Server]
   public void clientScoreNameChange()
    {

        playerName = hud.GetCustomName();
        Debug.Log("Back and forth");
        RefreshScoreboard();
    }






    [Command]
    public void CmdIncreaseScore(int amount)
    {
        playerScore += amount;
        Debug.Log($"[Server] {playerName} score increased to {playerScore}");
    }

    private void OnScoreChanged(string oldValue, string newValue)
    {
        Debug.Log($"[Client] Player name changed from {oldValue} to {newValue}");
        RefreshScoreboard();
    }

    private void OnScoreChanged(int oldValue, int newValue)
    {
        Debug.Log($"[Client] Player score changed from {oldValue} to {newValue}");
        RefreshScoreboard();
    }

    private void RefreshScoreboard()
    {
        if (isServer)
        {
            UpdateScoreboard();
        }
        else
        {
            CmdRequestScoreboardUpdate();
        }
    }

    [Server]
    private void UpdateScoreboard()
    {

        var sortedPlayers = allPlayers.OrderByDescending(p => p.playerScore).ToList();


        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (i < sortedPlayers.Count)
            {
                var player = sortedPlayers[i];


                playerNameTexts[i].text = player.playerName;
                playerScoreTexts[i].text = player.playerScore.ToString();


                if (i == 0)
                {
                    playerNameTexts[i].color = highlightColor;
                    playerScoreTexts[i].color = highlightColor;
                }
                else
                {
                    playerNameTexts[i].color = defaultColor;
                    playerScoreTexts[i].color = defaultColor;
                }
            }
            else
            {

                playerNameTexts[i].text = "Waiting...";
                playerScoreTexts[i].text = "0";
                playerNameTexts[i].color = defaultColor;
                playerScoreTexts[i].color = defaultColor;
            }
        }

        //for (int i = 0; i < LBController.Instance.playerScores.Count; i++)
        //{
        //    playerNameTexts[i].text = LBController.Instance.playerScores[i].name;
        //    playerScoreTexts[i].text = LBController.Instance.playerScores[i].playerScore.ToString();

        //    if (playerName == LBController.Instance.playerScores[i].playerName)
        //    {
        //        playerNameTexts[i].color = highlightColor;
        //        playerScoreTexts[i].color = highlightColor;
        //    }
        //    else
        //    {
        //        playerNameTexts[i].color = defaultColor;
        //        playerScoreTexts[i].color = defaultColor;
        //    }

        //}



        RpcUpdateScoreboardUI();
    }


  








    [Command]
    private void CmdRequestScoreboardUpdate()
    {
        UpdateScoreboard();
    }

    [ClientRpc]
    private void RpcUpdateScoreboardUI()
    {

        var sortedPlayers = allPlayers.OrderByDescending(p => p.playerScore).ToList();

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (i < sortedPlayers.Count)
            {
                var player = sortedPlayers[i];


                playerNameTexts[i].text = player.playerName;
                playerScoreTexts[i].text = player.playerScore.ToString();


                if (i == 0)
                {
                    playerNameTexts[i].color = highlightColor;
                    playerScoreTexts[i].color = highlightColor;
                }
                else
                {
                    playerNameTexts[i].color = defaultColor;
                    playerScoreTexts[i].color = defaultColor;
                }
            }
            else
            {

                playerNameTexts[i].text = "Waiting...";
                playerScoreTexts[i].text = "0";
                playerNameTexts[i].color = defaultColor;
                playerScoreTexts[i].color = defaultColor;
            }
        }

        //for (int i = 0; i < LBController.Instance.playerScores.Count; i++)
        //{
        //    playerNameTexts[i].text = LBController.Instance.playerScores[i].name;
        //    playerScoreTexts[i].text = LBController.Instance.playerScores[i].ToString();

        //    if (playerName == LBController.Instance.playerScores[i].playerName)
        //    {
        //        playerNameTexts[i].color = highlightColor;
        //        playerScoreTexts[i].color = highlightColor;
        //    }
        //    else
        //    {
        //        playerNameTexts[i].color = defaultColor;
        //        playerScoreTexts[i].color = defaultColor;
        //    }

        //}
    }

    
    [ContextMenu("Manual Refresh Scoreboard")]
    public void ManualRefreshScoreboard()
    {
        Debug.Log("[Inspector] Manual refresh triggered.");
        RefreshScoreboard();
    }
}