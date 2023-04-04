using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("BoneCracker Games/Highway Racer/UI/HR UI Gameover Panel")]
public class HR_UIGameOverPanel : MonoBehaviour
{
    public GameObject content;

    [Header("UI Texts On Scoreboard")]
    public Text totalScore;
    public Text subTotalMoney;
    public Text totalMoney;

    public Text totalDistance;
    public Text totalNearMiss;
    public Text totalOverspeed;
    public Text totalOppositeDirection;

    public Text totalDistanceMoney;
    public Text totalNearMissMoney;
    public Text totalOverspeedMoney;
    public Text totalOppositeDirectionMoney;

    [Space] [Header("Buttons")]
    [SerializeField] private GameObject nextButton;

    void OnEnable()
    {
        HR_PlayerHandler.OnGameOver += HR_PlayerHandler_OnGameOver;
    }

    void HR_PlayerHandler_OnGameOver(HR_PlayerHandler player, int[] scores, bool didWin)
    {
        StartCoroutine(DisplayResults(player, scores, didWin));
    }

    public IEnumerator DisplayResults(HR_PlayerHandler player, int[] scores, bool didWin)
    {
        yield return new WaitForSecondsRealtime(1f);

        content.SetActive(true);

        if (GameState.SelectedGameMode == GameState.GameMode.Challenges)
        {
            GetComponentInChildren<Text>().gameObject.SetActive(true);
            GetComponentInChildren<Text>().text = didWin ? "Level Finished" : "Level Failed";
            nextButton.SetActive(didWin);
        }

        totalScore.text = Mathf.Floor(player.Score).ToString("F0");
        totalDistance.text = (player.Distance).ToString("F2");
        totalNearMiss.text = (player.nearMisses).ToString("F0");
        totalOverspeed.text = (player.highSpeedTotal).ToString("F1");
        totalOppositeDirection.text = (player.oppositeDirectionTotal).ToString("F1");

        totalDistanceMoney.text = scores[0].ToString("F0");
        totalNearMissMoney.text = scores[1].ToString("F0");
        totalOverspeedMoney.text = scores[2].ToString("F0");
        totalOppositeDirectionMoney.text = scores[3].ToString("F0");

        totalMoney.text = (scores[0] + scores[1] + scores[2] + scores[3]).ToString();

        gameObject.BroadcastMessage("Animate");
        gameObject.BroadcastMessage("GetNumber");
    }

    void OnDisable()
    {
        HR_PlayerHandler.OnGameOver -= HR_PlayerHandler_OnGameOver;
    }

}