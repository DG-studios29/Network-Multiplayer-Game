using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class NetworkManagerTemp : NetworkManager
{
    //public List<string> player_Names = new List<string>();
    //public string player_Name;
    //private GameObject lbInstance;
    //[SerializeField]public GameObject LBPrefab;


    public override void OnStartServer()
    {
        base.OnStartServer();
        //lbInstance = GameObject.Instantiate(LBPrefab);

    }


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        //safety checks
        if (playerPrefab == null)
        {
            Debug.LogError("No players assigned!");
            return;
        }

        base.OnServerAddPlayer(conn); //base method will spawn the player prefab in

        ///player_Names.Add(player_Name);


        GameObject player = conn.identity.gameObject; //get the player game object
       // from its ID
        PlayerInputTemp playerCTRL = player.GetComponent<PlayerInputTemp>(); //get the exmaple script
       
        
        //on the player prefab
        if (numPlayers == 1) //the very first player to join the game, numPlayers
                           
        {
            // First player (usually the host)
            playerCTRL.SetPlayerName(conn, numPlayers);
            playerCTRL.TargetShowWelcomeMessage(conn, "You are the host! Lets hope you dont have hacks to find that treasure.");
            
        }
        else
        {
            // Any player after that
            playerCTRL.SetPlayerName(conn, numPlayers);
            playerCTRL.TargetShowWelcomeMessage(conn, "You are a client. Get ready to find yourself some treasure!");
           
        }


        /* 

        GameObject player = Instantiate(playerPrefab);

        NetworkServer.AddPlayerForConnection(conn, player);*/

    }


  /*  public void PlayerNewName(string name)
    {
        //player_Name = null;
        //player_Names.Add(name);
        player_Name = name;
    }*/
}

   