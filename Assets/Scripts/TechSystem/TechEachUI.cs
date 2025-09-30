using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Mesh;

public class TechEachUI : MonoBehaviour
{
    public Button buttonBG;
    public Image imageIcon;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textCost;
    public TextMeshProUGUI textLevel;

    private TechState techState;        // 데이터 원본과 상태 저장

    private void Start()
    {
        GameManager.instance.OnCurrentGoldAmountChanged += OnCurrentGoldAmountChanged;
        buttonBG.onClick.AddListener(CheckTechLevelUp);
        StartCoroutine(CheckUnlock());
    }

    // 0.15초 후에 재검사
    IEnumerator CheckUnlock()
    {
        yield return new WaitForSeconds(0.15f);
        TechViewer.instance.CheckUnlockPreTech(techState.techData);
    }

    private void OnDestroy()
    {
        GameManager.instance.OnCurrentGoldAmountChanged -= OnCurrentGoldAmountChanged;
    }

    // 데이터 등록
    public void RegisterData(TechData techData)
    {
        techState = new TechState(techData);

        imageIcon.sprite = techData.techIcon;
        textName.text = techData.techName;
        PrintTextValue();
    }

    // 비용이 충분하지 않다면, 비활성화
    public void OnCurrentGoldAmountChanged()
    {
        int amount = GameManager.instance.GetCurrentGoldAmount();
        // 선행 조건이 다 해결되지 않았다면, 무시
        if (techState.lockState == LockState.Block)
            return;

        // 비활성화
        if (amount < techState.requaireAmount)
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
        // 현재 금액 감소
        GameManager.instance.AddCurrentGoldAmount(-1 * techState.requaireAmount);

        // 레벨업 및 업데이트
        techState.LevelUp();
        PrintTextValue();

        // 효과 적용
        foreach(var effect in techState.techData.effects)
        {
            effect.ApplyTechEffect();
        }

        // 다음 기술의 선행 기술들 확인
        foreach (var nextTech in techState.techData.postTeches)
        {
            TechViewer.instance.CheckUnlockPreTech(nextTech);
        }
    }

    // 레벨과 수치 출력
    private void PrintTextValue()
    {
        textCost.text = techState.requaireAmount.ToString();
        textLevel.text = techState.currentLevel.ToString();
    }

}
