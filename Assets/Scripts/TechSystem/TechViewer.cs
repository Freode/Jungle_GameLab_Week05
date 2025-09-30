using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TechViewer : MonoBehaviour
{
    public static TechViewer instance;

    [SerializeField] private GameObject uiPrefab;
    [SerializeField] private List<TechData> techDatas;      // 데이터 원본

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitUI();
    }

    void InitUI()
    {
        for (int i = 0; i < techDatas.Count; i++)
        {
            Vector3 pos = new Vector3(0, 300 - 90 * i, 0);
            CreateTechEachUI(techDatas[i], pos);
        }
    }

    // 각 테크 UI 만들기
    void CreateTechEachUI(TechData techData, Vector3 pos)
    {
        GameObject eachUI = Instantiate(uiPrefab, transform);
        eachUI.transform.localPosition = pos;

        eachUI.TryGetComponent(out TechEachUI techEachUI);
        if (eachUI is null) return;

        techEachUI.RegisterData(techData);
    }
}
