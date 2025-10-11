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
        OnCheckTechActive();
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

        // 업데이트
        PrintCost();
        PrintLevelOrCapacity();

        // 효과 적용
        foreach (var effect in techState.techData.effects)
        {
            effect.ApplyTechEffect();
        }

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
        if(techState.lockState == LockState.Block)
        {
            techName = "????";
            techDescription = "아직 확인할 수 없습니다.";
            techIcon = unlockIcon;
        }
        else
        {
            techName = techState.techData.techName;
            techDescription = techState.techData.techDescription;
            techIcon = techState.techData.techIcon;
        }

        OnActiveInfo?.Invoke(techName, techDescription, techIcon, loc);

        //// 구조물 정보 출력
        //if (techState.techData.techKind == TechKind.Structure)
        //    TechViewer.instance.ActiveStructureInfo(techState);
    }

    // 마우스가 빠져 나감
    public void OnPointerExit(PointerEventData eventData)
    {
        OnInactiveInfo?.Invoke();

        if (techState.techData.techKind == TechKind.Structure)
            TechViewer.instance.InactiveStructureInfo();
    }

    // 설명 문서 작성
    private string SetDescriptionContent()
    {
        return string.Empty;
    }
}
