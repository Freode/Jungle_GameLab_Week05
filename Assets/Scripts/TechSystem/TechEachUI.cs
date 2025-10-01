using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TechEachUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button buttonBG;
    public Image imageIcon;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textCost;
    public TextMeshProUGUI textLevel;
    public RectTransform totalRectTransform;

    public event System.Action<string, string, Vector3> OnActiveInfo;
    public event System.Action OnInactiveInfo;

    private TechState techState;        // 데이터 원본과 상태 저장
    private float upperY;
    private float leftX;

    private void Start()
    {
        GameManager.instance.OnCurrentGoldAmountChanged += OnCheckTechActive;
        buttonBG.onClick.AddListener(CheckTechLevelUp);
        StartCoroutine(CheckUnlock());
        upperY = gameObject.transform.position.y - totalRectTransform.rect.height / 2f;
        leftX = gameObject.transform.position.x - totalRectTransform.rect.width / 2f - 5f;

        // === 수정 필요 ===
        if (techState.techData.areaType != AreaType.Normal)
            PeopleManager.Instance.OnAreaPeopleCountChanged += CurrentCapacityChange;
    }

    // 0.15초 후에 재검사
    IEnumerator CheckUnlock()
    {
        yield return new WaitForSeconds(0.15f);
        TechViewer.instance.CheckUnlockPreTech(techState.techData);
    }

    private void OnDestroy()
    {
        OnActiveInfo = null;
        OnInactiveInfo = null;
        GameManager.instance.OnCurrentGoldAmountChanged -= OnCheckTechActive;

        // === 수정 필요 ===
        if (techState.techData.areaType != AreaType.Normal)
            PeopleManager.Instance.OnAreaPeopleCountChanged -= CurrentCapacityChange;
    }

    // 데이터 등록
    public void RegisterData(TechData techData)
    {
        techState = new TechState(techData);

        imageIcon.sprite = techData.techIcon;
        textName.text = techData.techName;
        PrintCost();
        PrintLevelOrCapacity();
    }

    // 비용이 충분하지 않다면, 비활성화
    // 현재 수용량이 최대 수용량보다 적어서 개발 가능한지 확인
    public void OnCheckTechActive()
    {
        int amount = GameManager.instance.GetCurrentGoldAmount();
        // 선행 조건이 다 해결되지 않았다면, 무시
        if (techState.lockState == LockState.Block)
            return;

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
        int goldAmount = GameManager.instance.GetCurrentGoldAmount();
        if (goldAmount < techState.requaireAmount)
            return;

        OperateTechLevelUp();
    }

    // 레벨업 
    private void OperateTechLevelUp()
    {
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
    }

    // 업그레이드 비용 출력
    private void PrintCost()
    {
        textCost.text = FuncSystem.Format(techState.requaireAmount);
    }

    // 레벨 또는 현재 수용량 출력
    private void PrintLevelOrCapacity()
    {
        string value = techState.techData.isUsingLevel ? techState.currentLevel.ToString() : techState.curCapacity.ToString();
        textLevel.text = value;
    }

    // 마우스 올려 놓기
    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 loc = new Vector3(leftX, upperY, 0);
        OnActiveInfo?.Invoke(techState.techData.techName, techState.techData.techDescription, loc);
    }

    // 마우스가 빠져 나감
    public void OnPointerExit(PointerEventData eventData)
    {
        OnInactiveInfo?.Invoke();
    }
}
