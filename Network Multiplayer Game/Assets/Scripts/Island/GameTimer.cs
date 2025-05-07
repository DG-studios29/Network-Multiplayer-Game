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

    private bool gameOver = false;

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

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (timerText == null)
            FindTimerText();

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
            Debug.LogWarning("[GameTimer] Couldn't find TimerText; retrying.");
            StartCoroutine(RetryFindTimerText());
        }
    }

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
        // TODO: show end-game UI
    }
}