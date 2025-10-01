using System.Collections;
using TMPro;
using UnityEngine;

public class GoldClickAreaUI : MonoBehaviour
{
    public TextMeshProUGUI textCurrentGoldAmount;       // 현재 골드 소지량
    public TextMeshProUGUI textClickAmount;             // 현재 골드 클릭 시, 획득량
    public TextMeshProUGUI textPeriodAmount;            // 주기적으로 얻는 골드 양 출력

    private int _localCurrentGold = 0;

    private float _interval = 0.05f;
    private float _curTime = 0f;
    private float _endTime = 0f;
    private Coroutine _animCoroutine;

    private void Start()
    {
        GameManager.instance.OnCurrentGoldAmountChanged += StartModifyCurrentGoldAmount;
        GameManager.instance.OnClickIncreaseTotalAmountChanged += PrintIncreaseGoldAmount;
        GameManager.instance.OnPeriodIncreaseAmountChanged += PrintPeriodGoldAmount;

        _localCurrentGold = GameManager.instance.GetCurrentGoldAmount();
        PrintCurrentGoldAmount(_localCurrentGold);
    }

    private void OnDestroy()
    {
        GameManager.instance.OnCurrentGoldAmountChanged -= StartModifyCurrentGoldAmount;
        GameManager.instance.OnClickIncreaseTotalAmountChanged -= PrintIncreaseGoldAmount;
        GameManager.instance.OnPeriodIncreaseAmountChanged -= PrintPeriodGoldAmount;
    }

    private void Update()
    {
        _curTime += Time.deltaTime;
    }

    // 골드 양 업데이트
    IEnumerator UpdateLocalGoldAmount()
    {
        float startTime = _curTime;

        while(_curTime <= _endTime)
        {
            if (GameManager.instance.GetIsGameOver())      
                break;

            float dt = (_curTime - startTime) / (_endTime - startTime);

            int finalAmount = GameManager.instance.GetCurrentGoldAmount();

            float nextAmountF = Mathf.Lerp((float)_localCurrentGold, (float)finalAmount, dt);
            int nextAmount = (int)nextAmountF;
            PrintCurrentGoldAmount(nextAmount);


            // 골드 양이 선형적으로 증가하는 애니메이션
            yield return new WaitForSeconds(_interval);


        }
        // 최종 양 재지정
        PrintCurrentGoldAmount(GameManager.instance.GetCurrentGoldAmount());

        // 완료
        EndModifyCurrentGoldAmount();
    }

    // 현재 골드 양 업데이트 되었다고 호출하는 함수
    private void StartModifyCurrentGoldAmount()
    {
        // 값이 실제로도 변경되었으면 호출
        int amount = GameManager.instance.GetCurrentGoldAmount();
        if (amount == _localCurrentGold)
            return;

        _endTime = _curTime + 0.3f;

        if (_animCoroutine != null)
            return;

        _animCoroutine = StartCoroutine(UpdateLocalGoldAmount());
    }

    // 현재 골드 양을 모두 업데이트 했을 때의 함수
    private void EndModifyCurrentGoldAmount()
    {
        _animCoroutine = null;
    }

    // 현재 골드 양 출력
    private void PrintCurrentGoldAmount(int amount)
    {
        textCurrentGoldAmount.text = "Current Gold\n" + amount;
        _localCurrentGold = amount;
    }

    // 한 번 클릭 시, 얻는 골드 양 출력
    private void PrintIncreaseGoldAmount()
    {
        int amount = GameManager.instance.GetClickIncreaseTotalAmount();
        textClickAmount.text = "Click Gold\n" + amount;
    }

    // 주기적으로 얻는 골드 양 출력
    private void PrintPeriodGoldAmount()
    {
        int amount = GameManager.instance.GetPeriodIncreaseGoldAmount();
        textPeriodAmount.text = "Period Gold\n" + amount;
    }
}
