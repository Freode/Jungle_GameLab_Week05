using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TechViewer : MonoBehaviour
{
    public static TechViewer instance;

    [SerializeField] private GameObject uiPrefab;
    [SerializeField] private List<TechData> techDatas;      // 데이터 원본

    private Dictionary<TechData, TechEachUI> techEachUIs;   // 테크 UI

    void Awake()
    {
        instance = this;
        techEachUIs = new Dictionary<TechData, TechEachUI>();
    }

    void Start()
    {
        InitUI();
    }

    // 특정 테크의 사전 테크가 모두 해제되었는지 확인
    public void CheckUnlockPreTech(TechData techData)
    {
        foreach(var preTech in techData.preTeches)
        {
            if (techEachUIs[preTech].GetTechUnlock() != LockState.Complete)
                return;
        }

        // 해금 가능하다고 업데이트
        techEachUIs[techData].SetTechUnlock();
        techEachUIs[techData].OnCurrentGoldAmountChanged();
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
        if (techEachUI is null) return;

        techEachUIs.Add(techData, techEachUI);
        techEachUI.RegisterData(techData);
    }
}
