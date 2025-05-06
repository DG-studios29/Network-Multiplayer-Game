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

    // if you *do* want to assign it manually in the Inspector, you still can—
    // otherwise we'll try to find it at runtime
    public TextMeshProUGUI timerText;

    private bool gameOver = false;

    #region Server Logic
    public override void OnStartServer()
    {
        remainingTime = matchDuration;
        // tick every second
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

    // sometimes your UI prefab takes one frame to spawn in; if you 
    // still don’t find it, try again after a tiny delay:
    IEnumerator RetryFindTimerText()
    {
        yield return null;
        FindTimerText();
        OnTimerChanged(0, remainingTime);
    }

    void FindTimerText()
    {
        // look for an object called "TimerText" (you can also use tags)
        var go = GameObject.Find("TimerText");
        if (go != null)
        {
            timerText = go.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("[GameTimer] Couldn't find GameObject named 'TimerText'; retrying next frame.");
            // if you want, kick off the retry coroutine
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
        // TODO: trigger whatever end‑game UI / freeze logic you need
    }
    #endregion
}
