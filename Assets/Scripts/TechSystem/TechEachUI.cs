using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        
    }

    // 데이터 등록
    public void RegisterData(TechData techData)
    {
        techState = new TechState(techData);

        imageIcon.sprite = techData.techIcon;
        textName.text = techData.techName;
        textCost.text = techData.baseRequiredByte.ToString();
        textLevel.text = "0";
    }

    // 레벨업 확인
    private void CheckTechLevelUp()
    {
        // TODO : 비용 충분한지 확인
    }

    // 레벨업 
    private void OperateTechLevelUp()
    {
        
    }

    //public void LevelUpTech(string techName)
    //{
    //    if (techStates.ContainsKey(techName))
    //    {
    //        TechState state = techStates[techName];
    //        // 돈이 충분한지 확인...
    //        state.currentLevel++;
    //        // 레벨업 효과 적용...
    //        Debug.Log($"{state.techData.techNamePrint} 레벨 업! 현재 레벨: {state.currentLevel}");
    //    }
    //}
}
