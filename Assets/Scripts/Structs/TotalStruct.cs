using System.Collections.Generic;
using System;
using UnityEngine;

// 증가량 구조체
public class IncreaseInfo
{
    public long clickLinear = 0;
    public long clickRate = 0;
    public long periodLinear = 0;
    public long periodRate = 0;
};

// 구조체 레벨에 따른 변화
[System.Serializable]
public class LevelAppearance
{
    public int level;
    public Sprite sprite;
    public Vector3 scale = Vector3.one;
}

[Serializable]
public class StructureData
{
    public TechData techData;
    public GameObject areaStructure;
}


// 테크 유형에 따른 배열
[System.Serializable]
public struct TechKindInfo
{
    public TechKind techKind;
    public List<TechData> techDatas;
}

// 테크 업그레이드 유형 출력
[System.Serializable]
public struct TechPrintUpgradeKind
{
    public bool isAcquireClickGold;
    public bool isAcquirePeriodGold;
    public bool isReducePeoplePeriod;
}

// 테크 효과 값이 담긴 구조체
[System.Serializable]
public struct TechTotalUpgradeAmount
{
    public long clickLinearAmount;
    public long clickRateAmount;
    public long periodLinearAmount;
    public long periodRateAmount;
    public float respawnTime;
}
