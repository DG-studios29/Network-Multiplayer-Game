using Mirror;
using UnityEngine;
using TMPro;
using System.Collections;

public class GameTimer : NetworkBehaviour
{
    [Header("Match Settings")]
    public float matchDuration = 300f;

    [SyncVar(hook = nameof(OnTimerChanged))]
    private float remainingTime;

   
    public TextMeshProUGUI timerText;

    EndGameManager endGameManager;

    private bool gameOver = false;

    #region Server Logic
    public override void OnStartServer()
    {
        remainingTime = matchDuration;
    
        InvokeRepeating(nameof(ServerUpdateTimer), 1f, 1f);
    }

    [Server]
    void ServerUpdateTimer()
    {
        if (gameOver) return;

        remainingTime = Mathf.Max(0f, remainingTime - 1f);
        if (remainingTime <= 0f)
        {
            RpcOnGameOver("Time's Up! Game Over.");
            gameOver = true;
            CancelInvoke(nameof(ServerUpdateTimer));
        }
    }

    [Server]
    public void EndGameEarly(string reason)
    {
        if (gameOver) return;
        RpcOnGameOver(reason);
        gameOver = true;
        CancelInvoke(nameof(ServerUpdateTimer));
    }
    #endregion

    #region Client Logic
    public override void OnStartClient()
    {
        base.OnStartClient();

        // if someone forgot to hook it up in the Inspector, try to find it for them
        if (timerText == null)
            FindTimerText();

        // initialize the UI to the current time
        OnTimerChanged(0, remainingTime);
    }

 
    IEnumerator RetryFindTimerText()
    {
        yield return null;
        FindTimerText();
        OnTimerChanged(0, remainingTime);
    }

    void FindTimerText()
    {
      
        var go = GameObject.Find("TimerText");
        if (go != null)
        {
            timerText = go.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("[GameTimer] Couldn't find GameObject named 'TimerText'; retrying next frame.");
      
            StartCoroutine(RetryFindTimerText());
        }
    }

    // SyncVar hook will update every client automatically
    void OnTimerChanged(float oldTime, float newTime)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(newTime / 60f);
        int seconds = Mathf.FloorToInt(newTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    [ClientRpc]
    void RpcOnGameOver(string reason)
    {
        Debug.Log(reason);
       endGameManager.RpcShowEndGameScreen(reason); 
    }
    #endregion
}
