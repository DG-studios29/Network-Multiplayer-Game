using UnityEngine;
using System.Collections.Generic;
using kcp2k;
using Mirror;

public class LBController : NetworkBehaviour
{
    public static LBController Instance { get; private set; }

    public List<ScoreboardManager> playerScores = new List<ScoreboardManager>();


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
