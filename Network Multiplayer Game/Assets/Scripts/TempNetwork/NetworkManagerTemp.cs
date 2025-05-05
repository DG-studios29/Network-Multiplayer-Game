using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class NetworkManagerTemp : NetworkManager
{
    public string player_Name;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        //safety checks
        if (playerPrefab == null)
        {
            Debug.LogError("No players assigned!");
            return;
        }

        base.OnServerAddPlayer(conn); //base method will spawn the player prefab in


        GameObject player = conn.identity.gameObject; //get the player game object
       // from its ID
        PlayerInputTemp playerCTRL = player.GetComponent<PlayerInputTemp>(); //get the exmaple script
        playerCTRL.SetPlayerName(conn,player_Name,numPlayers);

        //on the player prefab
        if (numPlayers == 1) //the very first player to join the game, numPlayers
                           
        {
            // First player (usually the host)
             playerCTRL.TargetShowWelcomeMessage(conn, "You are the host! Lets hope you dont have hacks to find that treasure.");
         }
        else
        {
            // Any player after that
             playerCTRL.TargetShowWelcomeMessage(conn, "You are a client. Get ready to find yourself some treasure!");
         }


        /* 

        GameObject player = Instantiate(playerPrefab);

        NetworkServer.AddPlayerForConnection(conn, player);*/
    }
}

   