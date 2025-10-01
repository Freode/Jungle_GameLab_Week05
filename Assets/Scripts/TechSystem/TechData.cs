using UnityEngine;
using System.Collections.Generic;

public enum LockState
{
    Block = 0,
    CanUnlock,
    Complete,
};

// 데이터를 저장하는 클래스
[CreateAssetMenu(fileName = "TechData", menuName = "Scriptable Objects/TechData")]
public class TechData : ScriptableObject
{
    public AreaType areaType;               // 존재할 구역 타입(나중에 수정)
    public string techName;                 // 기술 이름
    public string techNamePrint;            // 출력용 기술 이름
    public string techDescription;          // 기술 설명
    public Sprite techIcon;                 // 기술 아이콘
    public int baseRequiredGold;            // 기본 요구 바이트 (레벨 1)
    public int increaseGoldValue;           // 레벨 당 증가하는 골드 양
    public int maxLevel;                    // 최대 레벨 - 변경 불가능
    public int baseCapacity;                // 기본 수용량 (레벨 1)
    public bool isUsingLevel;               // 레벨을 사용하는지 여부
    public Vector3 localPos;                // 연구 위치
    public List<TechData> preTeches;        // 선행 기술 목록
    public List<TechData> postTeches;       // 다음 기술 목록
    public List<BaseTechEffect> effects;    // 해금 시, 적용할 효과 목록
}

// 상태를 저장하는 클래스
[System.Serializable] 
public class TechState
{
    public TechData techData;               // 어떤 기술의 상태인지 원본 참조
    public int currentLevel;                // 현재 레벨
    public int curCapacity;                 // 현재 수용량
    public int maxCapacity;                 // 최대 수용량
    public LockState lockState;             // 연구 가능 상태
    public int requaireAmount = 0;          // 요구하는 양

    // 생성자
    public TechState(TechData data)
    {
        techData = data;
        currentLevel = 0;
        curCapacity = 0;
        maxCapacity = data.baseCapacity;
        lockState = LockState.Block;
        requaireAmount = data.baseRequiredGold;
    }

    // 레벨업 적용
    public void LevelUp()
    {
        lockState = LockState.Complete;
        // ++curCapacity; 수정 필요
        ++currentLevel;
        requaireAmount += techData.increaseGoldValue;
    }

    // 현재 수용량이 최대 수용량보다 적은지 확인
    public bool CheckCapacity()
    {
        // 바로 활성화 가능으로 판단
        if (techData.isUsingLevel)
            return true;

        // 잉여 인원이 있는지 확인 ==== 수정 필요 ====
        if (PeopleManager.Instance.Count(AreaType.Normal) == 0)
            return false;

        return curCapacity < maxCapacity;
    }
}
