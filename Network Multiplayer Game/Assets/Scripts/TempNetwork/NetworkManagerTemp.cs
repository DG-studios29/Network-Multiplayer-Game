using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class NetworkManagerTemp : NetworkManager
{
    public string player_Name;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
       
        base.OnServerAddPlayer(conn); //base method will spawn the player prefab in


        GameObject player = conn.identity.gameObject; //get the player game object
       // from its ID
        CannonHUD playerHUD = player.GetComponent<CannonHUD>(); //get the exmaple script
        //on the player prefab
        if (numPlayers == 1) //the very first player to join the game, numPlayers
                             //is a property from the Network Manager script from Mirror
        {
            // First player (usually the host)
            playerHUD.TargetShowWelcomeMessage(conn, "You are the host! Lets hope you dont have treasure finding cheats.");
         }
        else
        {
            // Any player after that
            playerHUD.TargetShowWelcomeMessage(conn, "You are a client. Get ready to find yourself some treasure!");
         }


        /* if (playerPrefab == null)
        {
            Debug.LogError("No players assigned!");
            return;
        }

        GameObject player = Instantiate(playerPrefab);

        NetworkServer.AddPlayerForConnection(conn, player);*/
    }
}

   