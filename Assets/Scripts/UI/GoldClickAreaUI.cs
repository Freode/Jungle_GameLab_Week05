using System.Collections;
using TMPro;
using UnityEngine;

public class GoldClickAreaUI : MonoBehaviour
{
    public TextMeshProUGUI textCurrentGoldAmount;       // 현재 골드 소지량
    public TextMeshProUGUI textClickAmount;             // 현재 골드 클릭 시, 획득량
    public TextMeshProUGUI textPeriodAmount;            // 주기적으로 얻는 골드 양 출력
    public Transform startPosAcquireGold;               // 금 얻었을 때, 출력 창 시작 위치
    public Transform endPosAcquireGold;                 // 금 얻었을 때, 출력 창 종료 위치
    public GameObject acquireGoldAmountPrefab;          // 클릭으로 금 획득 시, 출력할 UI 프리팹

    private long _localCurrentGold = 0;

    private float _interval = 0.05f;
    private float _curTime = 0f;
    private float _endTime = 0f;
    private Coroutine _animCoroutine;

    private void Start()
    {
        GameManager.instance.OnCurrentGoldAmountChanged += StartModifyCurrentGoldAmount;
        GameManager.instance.OnClickIncreaseTotalAmountChanged += PrintClickGoldAmount;
        GameManager.instance.OnPeriodIncreaseAmountChanged += PrintPeriodGoldAmount;
        GameManager.instance.OnClickIncreaseGoldAmount += PrintIncreaseGoldAmountWhenClicked;

        _localCurrentGold = GameManager.instance.GetCurrentGoldAmount();
        PrintCurrentGoldAmount(_localCurrentGold);
    }

    private void OnDestroy()
    {
        GameManager.instance.OnCurrentGoldAmountChanged -= StartModifyCurrentGoldAmount;
        GameManager.instance.OnClickIncreaseTotalAmountChanged -= PrintClickGoldAmount;
        GameManager.instance.OnPeriodIncreaseAmountChanged -= PrintPeriodGoldAmount;
        GameManager.instance.OnClickIncreaseGoldAmount -= PrintIncreaseGoldAmountWhenClicked;
    }

    private void Update()
    {
        _curTime += Time.deltaTime;
    }

    // 골드 양 업데이트
    IEnumerator UpdateLocalGoldAmount()
    {
        float startTime = _curTime;

        while (_curTime <= _endTime)
        {
            if (GameManager.instance.GetIsGameOver())
                break;

            decimal dt = (decimal)(_curTime - startTime) / (decimal)(_endTime - startTime);

            long finalAmount = GameManager.instance.GetCurrentGoldAmount();

            decimal nextAmountF = _localCurrentGold + (finalAmount - _localCurrentGold) * dt;
            long nextAmount = (long)nextAmountF;
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
        long amount = GameManager.instance.GetCurrentGoldAmount();
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

    // 한 번 클릭했을 때, 얻는 양의 금을 출력
    private void PrintIncreaseGoldAmountWhenClicked(long amount, Color color)
    {
        GameObject obj = ObjectPooler.Instance.SpawnObject(ObjectType.AcquireInfoUI);
        obj.transform.SetParent(transform, false);

        obj.TryGetComponent(out AcquireGoldAmountUI acquireComp);
        if (acquireComp == null)
            return;

        acquireComp.AcquireGold(FuncSystem.Format(amount), startPosAcquireGold.transform.position, endPosAcquireGold.transform.position, color);
    }

    // 현재 골드 양 출력
    private void PrintCurrentGoldAmount(long amount)
    {
        textCurrentGoldAmount.text = "Current Gold\n" + FuncSystem.Format(amount);
        _localCurrentGold = amount;
    }

    // 한 번 클릭 시, 얻는 골드 양 출력
    private void PrintClickGoldAmount()
    {
        long amount = GameManager.instance.GetBaseClickIncreaseTotalAmount();
        textClickAmount.text = "Click Gold\n" + FuncSystem.Format(amount);
    }

    // 주기적으로 얻는 골드 양 출력
    private void PrintPeriodGoldAmount()
    {
        long amount = GameManager.instance.GetPeriodIncreaseTotalAmount();
        textPeriodAmount.text = "Period Gold\n" + FuncSystem.Format(amount);
    }
}
