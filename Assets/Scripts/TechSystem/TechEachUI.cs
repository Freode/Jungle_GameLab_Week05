using Mono.Cecil;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TechEachUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button buttonBG;
    public Sprite unlockIcon;
    public Image imageIcon;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textCost;
    public TextMeshProUGUI textLevel;
    public RectTransform totalRectTransform;
    public Color baseColor;             // 기본 색상

    public event System.Action<string, string, Sprite, Vector3> OnActiveInfo;
    public event System.Action OnInactiveInfo;

    private TechState techState;        // 데이터 원본과 상태 저장
    private float upperY = 5000f;
    private float leftX = 5000f;
    private bool isInteractTechInfoUI = false;  // 테크 정보 UI와 상호작용 여부

    private void Start()
    {
        leftX = gameObject.transform.position.x - totalRectTransform.rect.width / 2f - 5f;
        buttonBG.onClick.AddListener(CheckTechLevelUp);
    }

    private void OnDestroy()
    {
        //OnActiveInfo = null;
        //OnInactiveInfo = null;
        //GameManager.instance.OnCurrentGoldAmountChanged -= OnCheckTechActive;

        //// === 수정 필요 ===
        //if (techState.techData.areaType != AreaType.Normal)
        //    PeopleManager.Instance.OnAreaPeopleCountChanged -= CurrentCapacityChange;
    }

    // 데이터 등록
    public void RegisterState(TechState techState)
    {
        this.techState = techState;

        // 버튼 활성화 여부 설정
        GameManager.instance.OnCurrentGoldAmountChanged += OnCheckTechActive;

        // === 수정 필요 ===
        if (techState.techData.techKind == TechKind.Job)
            PeopleManager.Instance.OnAreaPeopleCountChanged += CurrentCapacityChange;

        // 아직 잠겨 있는 상태
        if (techState.lockState == LockState.Block)
            TechViewer.instance.CheckUnlockPreTech(techState.techData);
        // 이미 열 수 있는 상태면, 바로 해제
        else 
            OnCheckTechActive();
    }

    // 상태 제거하는 함수
    public void RemoveState()
    {
        UnlockUI();
        if (techState == null)
            return;

        // 모두 초기화
        buttonBG.interactable = false;
        textCost.color = baseColor;

        // 모두 제거
        GameManager.instance.OnCurrentGoldAmountChanged -= OnCheckTechActive;

        // === 수정 필요 ===
        if (techState.techData.techKind == TechKind.Job)
            PeopleManager.Instance.OnAreaPeopleCountChanged -= CurrentCapacityChange;

        techState = null;
    }

    // 비용이 충분하지 않다면, 비활성화
    // 현재 수용량이 최대 수용량보다 적어서 개발 가능한지 확인
    public void OnCheckTechActive()
    {
        long amount = GameManager.instance.GetCurrentGoldAmount();
        // 선행 조건이 다 해결되지 않았다면, 물음표 상태로 표시
        if (techState.lockState == LockState.Block)
            return;

        imageIcon.sprite = techState.techData.techIcon;
        textName.text = techState.techData.techName;

        PrintCost();
        PrintLevelOrCapacity();

        // 비활성화 또는 수용량 여유가 있는 경우
        if (amount < techState.requaireAmount || techState.CheckCapacity() == false)
        {
            buttonBG.interactable = false;
            textCost.color = Color.red;
        }
        // 활성화
        else
        {
            buttonBG.interactable = true;
            textCost.color = Color.green;
        }
    }

    // 잠겨 있을 때, UI 상태를 물음표로 변경
    public void UnlockUI()
    {
        imageIcon.sprite = unlockIcon;
        textName.text = "????";
        textCost.text = "????";
        textLevel.text = "??";
    }

    // 현재 노드의 해금 여부를 반환
    public LockState GetTechUnlock()
    {
        return techState.lockState;
    }

    // 현재 노드의 해금 가능하다고 설정
    public void SetTechUnlock()
    {
        techState.lockState = LockState.CanUnlock;
    }

    // 최대 수용량(유사 최대 레벨) 증가
    public void IncreaseMaxCapacity(int amount)
    {
        techState.maxCapacity += amount;
        OnCheckTechActive();
    }

    [ContextMenu("Do Something")]
    public void ModifyTest()
    {
        ModifyCurrentCapacity(-1);
    }

    // 현재 수용량 변경
    public void ModifyCurrentCapacity(int amount)
    {
        // techState.curCapacity += amount;
        PrintLevelOrCapacity();
        OnCheckTechActive();
    }

    // 현재 수용량(인원수) 변경
    private void CurrentCapacityChange()
    {
        techState.curCapacity = PeopleManager.Instance.Count(techState.techData.areaType);
        OnCheckTechActive(); // 잉여 인력이 변경되면서 추가 === 수정 필요 ===
        PrintLevelOrCapacity();

        // 인원 수 변경된 것에 따라 다시 테크 UI 적용
        if (isInteractTechInfoUI)
            PrintTechInfo();
    }

    // 레벨업 확인
    private void CheckTechLevelUp()
    {
        // 비용 충분한지 확인
        long goldAmount = GameManager.instance.GetCurrentGoldAmount();
        if (goldAmount < techState.requaireAmount)
            return;

        OperateTechLevelUp();
    }

    // 레벨업 
    private void OperateTechLevelUp()
    {
        // 게임 클리어 버튼 누르면, 더 이상 작동 x
        if (techState.techData.isClearTech)
            buttonBG.interactable = false;

        int minusAmount = -1 * techState.requaireAmount;
        techState.LevelUp();

        // 외형 변경
        GameManager.instance.ModifyStructureLevel(techState.techData, techState.currentLevel); ;

        // 현재 금액 감소
        GameManager.instance.AddCurrentGoldAmount(minusAmount);

        // 효과 적용
        foreach (var effect in techState.techData.effects)
        {
            effect.ApplyTechEffect();
        }

        // 업데이트
        PrintCost();
        PrintLevelOrCapacity();
        PrintTechInfo();

        // 다음 기술의 선행 기술들 확인
        foreach (var nextTech in techState.techData.postTeches)
        {
            TechViewer.instance.CheckUnlockPreTech(nextTech);
        }

        // 구조물 정보 업데이트
        if (techState.techData.techKind == TechKind.Structure)
            TechViewer.instance.ActiveStructureInfo(techState);
    }

    // 업그레이드 비용 출력
    private void PrintCost()
    {
        textCost.text = FuncSystem.Format(techState.requaireAmount);
    }

    // 레벨 또는 현재 수용량 출력
    private void PrintLevelOrCapacity()
    {
        if (techState.lockState == LockState.Block)
            return;

        string value = techState.techData.isUsingLevel ? techState.currentLevel.ToString() : techState.curCapacity.ToString();
        textLevel.text = value;
    }

    // 마우스 올려 놓기
    public void OnPointerEnter(PointerEventData eventData)
    {
        isInteractTechInfoUI = true;
        PrintTechInfo();
    }

    // 마우스가 빠져 나감
    public void OnPointerExit(PointerEventData eventData)
    {
        isInteractTechInfoUI = false;
        OnInactiveInfo?.Invoke();

        if (techState.techData.techKind == TechKind.Structure)
            TechViewer.instance.InactiveStructureInfo();
    }

    // 설명 문서 출력
    private void PrintTechInfo()
    {
        // 위치 초기화가 되지 않았을 때만 진행
        if (upperY == 5000f)
        {
            Vector3[] corners = new Vector3[4];
            totalRectTransform.GetWorldCorners(corners);
            upperY = corners[1].y;
            leftX = corners[1].x;
        }

        // 테크 정보 위치 설정
        Vector3 loc = new Vector3(leftX, upperY, 0);

        // 테크 정보 데이터 설정
        string techName;
        string techDescription;
        Sprite techIcon;
        if (techState.lockState == LockState.Block)
        {
            techName = "????";
            techDescription = "아직 확인할 수 없습니다.";
            techIcon = unlockIcon;
        }
        else
        {
            techName = techState.techData.techName;
            techDescription = SetDescriptionContent();
            techIcon = techState.techData.techIcon;
        }

        OnActiveInfo?.Invoke(techName, techDescription, techIcon, loc);

        //// 구조물 정보 출력
        //if (techState.techData.techKind == TechKind.Structure)
        //    TechViewer.instance.ActiveStructureInfo(techState);
    }


    // 설명 문서 작성
    private string SetDescriptionContent()
    {
        // 기존 설명
        string description = techState.techData.techDescription + "\n";

        // 현재 단계 효과 계산
        IncreaseInfo increaseInfo = GameManager.instance.GetIncreaseGoldInfo(techState.techData.areaType);

        long curLinearAmount = increaseInfo.clickLinear * (100 + increaseInfo.clickRate) / 100;
        long curPeriodAmount = increaseInfo.periodLinear * (100 + increaseInfo.periodRate) / 100;

        // 다음 단계 효과 계산
        TechTotalUpgradeAmount next = techState.CalculateNextEffectAmount(increaseInfo);

        if (techState.techData.printTech.isAcquireClickGold || techState.techData.printTech.isAcquirePeriodGold)
        {
            // === 클릭 시 금 얻는 효과 출력 ===
            long nextClickLinear = increaseInfo.clickLinear + next.clickLinearAmount;
            long nextClickRate = increaseInfo.clickRate + next.clickRateAmount;

            long resultClickAmount = nextClickLinear * (100 + nextClickRate) / 100;
            // 클릭 세금 변경점
            if (techState.techData.printTech.isAcquireClickGold)
            {
                string clickLine;
                if (curLinearAmount != resultClickAmount)
                    clickLine = $"클릭 세금:<color=#00FF00>{FuncSystem.Format(curLinearAmount)}</color>▶<color=#00FF00>{FuncSystem.Format(resultClickAmount)}</color>\n";
                else
                    clickLine = $"클릭 세금:{FuncSystem.Format(curLinearAmount)}▶{FuncSystem.Format(resultClickAmount)}\n";

                description += clickLine;
            }

            // === 주기적으로 세금 얻는 효과 출력 ===
            long nextPeriodLinear = increaseInfo.periodLinear + next.periodLinearAmount;
            long nextPeriodRate = increaseInfo.periodRate + next.periodRateAmount;

            long resultPeriodAmount = (nextPeriodLinear) * (100 + nextPeriodRate) / 100;

            if (techState.techData.printTech.isAcquirePeriodGold)
            {
                // 주기 세금 변경점
                string periodLine;
                if (curPeriodAmount != resultPeriodAmount)
                    periodLine = $"주기 세금:<color=#00FF00>{FuncSystem.Format(curPeriodAmount)}</color>▶<color=#00FF00>{FuncSystem.Format(resultPeriodAmount)}</color>\n";
                else
                    periodLine = $"주기 세금:{FuncSystem.Format(curPeriodAmount)}▶{FuncSystem.Format(resultPeriodAmount)}\n";

                description += periodLine;

                long curTotalPeriodAmount = GameManager.instance.GetPeriodIncreaseTotalAmount();
                
                decimal curTotalPeriodPercent = 0;
                decimal nextTotalPeriodPercent = 0;

                if (curTotalPeriodAmount != 0)
                    curTotalPeriodPercent = (decimal)curPeriodAmount / (decimal)curTotalPeriodAmount * 100;
                else
                    curTotalPeriodPercent = 0;

                long nextTotalPeriodAmount = resultPeriodAmount - curPeriodAmount + curTotalPeriodAmount;
                if (nextTotalPeriodAmount != 0)
                    nextTotalPeriodPercent = (decimal)resultPeriodAmount / (decimal)(nextTotalPeriodAmount) * 100;
                else
                    nextTotalPeriodPercent = 0;

                // 주기 세금 총 지분
                string periodTechPercentLine;
                if (curPeriodAmount != resultPeriodAmount)
                    periodTechPercentLine = $"세금 지분:<color=#00FF00>{curTotalPeriodPercent.ToString("F2")}</color>%▶<color=#00FF00>{nextTotalPeriodPercent.ToString("F2")}</color>%\n";
                else
                    periodTechPercentLine = $"세금 지분:{curTotalPeriodPercent.ToString("F2")}%▶{nextTotalPeriodPercent.ToString("F2")}%\n";

                description += periodTechPercentLine;
            }

            //// === 인원 수 출력 ===
            //int people = PeopleManager.Instance.Count(techState.techData.areaType);
            //string peopleLine;
            //if(techState.techData.techKind == TechKind.Job)
            //    peopleLine = $"배정 인원:<color=#00FF00>{people}</color>명▶<color=#00FF00>{people + 1}</color>명\n";
            //else
            //    peopleLine = $"배정 인원:{people}명▶{people}명\n";

            //description += peopleLine;

            // === 1명당 효율 ===
            string onePersonLine;
            // 클릭으로 인한 효율
            //if (techState.techData.printTech.isAcquireClickGold)
            //{
            //    if (increaseInfo.clickLinear != nextClickLinear)
            //        onePersonLine = $"증가 양:<color=#00FF00>{FuncSystem.Format(increaseInfo.clickLinear)}</color>▶<color=#00FF00>{FuncSystem.Format(nextClickLinear)}</color>\n";
            //    else
            //        onePersonLine = $"증가 양:{FuncSystem.Format(increaseInfo.clickLinear)}▶{FuncSystem.Format(nextClickLinear)}\n";
            //}
            //// 주기로 인한 효율
            //else
            //{
            //    if (increaseInfo.periodLinear != nextPeriodLinear)
            //        onePersonLine = $"증가 양:<color=#00FF00>{FuncSystem.Format(increaseInfo.periodLinear)}</color>▶<color=#00FF00>{FuncSystem.Format(nextPeriodLinear)}</color>\n";
            //    else
            //        onePersonLine = $"증가 양:{FuncSystem.Format(increaseInfo.periodLinear)}▶{FuncSystem.Format(nextPeriodLinear)}\n";
            //}

            //description += onePersonLine;

            //// === 증가 비율 ===
            //string rateLine;
            //// 클릭으로 인한 효율
            //if (techState.techData.printTech.isAcquireClickGold)
            //{
            //    if (increaseInfo.clickRate != nextClickRate)
            //        rateLine = $"증가 비율:<color=#00FF00>{FuncSystem.Format(increaseInfo.clickRate)}</color>%▶<color=#00FF00>{FuncSystem.Format(nextClickRate)}</color>%\n";
            //    else
            //        rateLine = $"증가 비율:{FuncSystem.Format(increaseInfo.clickRate)}%▶{FuncSystem.Format(nextClickRate)}%\n";
            //}
            //// 주기로 인한 효율
            //else
            //{
            //    if (increaseInfo.periodRate != nextPeriodRate)
            //        rateLine = $"증가 비율:<color=#00FF00>{FuncSystem.Format(increaseInfo.periodRate)}</color>%▶<color=#00FF00>{FuncSystem.Format(nextPeriodRate)}</color>%\n";
            //    else
            //        rateLine = $"증가 비율:{FuncSystem.Format(increaseInfo.periodRate)}%▶{FuncSystem.Format(nextPeriodRate)}%\n";
            //}
            //description += rateLine;
        }

        // 잉여 인력 생성 주기 효과 출력
        if (techState.techData.printTech.isReducePeoplePeriod)
        {
            float curRespawnPeriod = GameManager.instance.GetRespawnTime();

            string respawnLine;
            if (curRespawnPeriod != next.respawnTime)
                respawnLine = $"생성 주기:<color=#00FF00>{curRespawnPeriod.ToString("F3")}</color>s▶<color=#00FF00))>{next.respawnTime.ToString("F3")}</color>s\n";
            else
                respawnLine = $"생성 주기:{curRespawnPeriod.ToString("F3")}s▶{next.respawnTime.ToString("F3")}s\n";

            description += respawnLine;
        }

        return description;
    }
}
