using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class GameStartSequenceManager : MonoBehaviour
{
    [Header("Countdown")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float countdownSeconds = 3f;

    [Header("Spawn Delay After Start")]
    [SerializeField] private float spawnDelayAfterStart = 2f;

    private CancellationToken destroyToken;

    private void Awake()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();
    }

    private void Start()
    {
        RunStartSequence(destroyToken).Forget();
    }

    private async UniTaskVoid RunStartSequence(CancellationToken token)
    {
        if (SpawnGate.Instance != null)
        {
            SpawnGate.Instance.SetGameplayStarted(false);
            SpawnGate.Instance.SetSpawnAllowed(false);
        }

        // 3, 2, 1 카운트다운
        for (int i = Mathf.CeilToInt(countdownSeconds); i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(true);
                countdownText.text = i.ToString();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
        }

        // START 표시
        if (countdownText != null)
        {
            countdownText.text = "START!";
        }

        if (SpawnGate.Instance != null)
        {
            SpawnGate.Instance.SetGameplayStarted(true);
        }

        // START 잠깐 보여주기
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        // 게임 시작 후 2초 뒤 스폰 허용
        await UniTask.Delay(TimeSpan.FromSeconds(spawnDelayAfterStart), cancellationToken: token);

        if (SpawnGate.Instance != null)
        {
            SpawnGate.Instance.SetSpawnAllowed(true);
        }
    }
}