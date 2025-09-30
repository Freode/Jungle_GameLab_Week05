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
    public string techName;                 // 기술 이름
    public string techNamePrint;            // 출력용 기술 이름
    public string techDescription;          // 기술 설명
    public Sprite techIcon;                 // 기술 아이콘
    public int baseRequiredByte;            // 기본 요구 바이트 (레벨 1)
    public float byteIncreaseRate;          // 레벨 당 요구 바이트 증가율
    public int increaseByteValue;           // 해금을 위한 필요 바이트 수
    public int maxLevel;                    // 최대 레벨
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
    public LockState lockState;             // 연구 가능 상태
    public int requaireAmount = 0;          // 요구하는 양

    // 생성자
    public TechState(TechData data)
    {
        techData = data;
        currentLevel = 0;
        lockState = LockState.Block;
        requaireAmount = data.baseRequiredByte;
    }

    // 현재 레벨에 따른 요구 바이트 계산
    public int GetCurrentRequiredByte()
    {
        if (currentLevel >= techData.maxLevel) 
            return int.MaxValue; 

        // 복리 공식 예시
        return (int)(techData.baseRequiredByte * Mathf.Pow(techData.byteIncreaseRate, currentLevel));
    }

    // 레벨업 적용
    public void LevelUp()
    {
        lockState = LockState.Complete;
        ++currentLevel;
        requaireAmount += techData.increaseByteValue;
    }
}
