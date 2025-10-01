using System;
using UnityEngine;

// 증가량 구조체
class IncreaseInfo
{
    public int clickLinear = 0;
    public int clickRate = 0;
    public int periodLinear = 0;
    public int periodRate = 0;
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