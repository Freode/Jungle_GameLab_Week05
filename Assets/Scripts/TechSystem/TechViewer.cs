using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TechViewer : MonoBehaviour
{
    public static TechViewer instance;
    public GameObject techInfo;

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
        techEachUIs[techData].OnCheckTechActive();
    }

    void InitUI()
    {
        for (int i = 0; i < techDatas.Count; i++)
        {
            CreateTechEachUI(techDatas[i]);
        }

        GameManager.instance.OnMaxCapacityUpgrade += MaxCapacityUpgrade;
        GameManager.instance.OnCurrentCapacityChanged += ModifyCurrentCapacity;
    }

    private void OnDestroy()
    {
        GameManager.instance.OnMaxCapacityUpgrade -= MaxCapacityUpgrade;
        GameManager.instance.OnCurrentCapacityChanged -= ModifyCurrentCapacity;
    }

    // 각 테크 UI 만들기
    void CreateTechEachUI(TechData techData)
    {
        GameObject eachUI = Instantiate(uiPrefab, transform);
        eachUI.transform.localPosition = techData.localPos;

        eachUI.TryGetComponent(out TechEachUI techEachUI);
        if (techEachUI is null) return;

        techInfo.TryGetComponent(out TechInfo info);

        techEachUI.OnActiveInfo += info.OnActiveInfo;
        techEachUI.OnInactiveInfo += info.OnInactiveInfo;

        techEachUIs.Add(techData, techEachUI);
        techEachUI.RegisterData(techData);
    }

    // 특정 테크의 수용량(유사 최대 레벨) 업그레이드
    void MaxCapacityUpgrade(TechData techType, int amount)
    {
        techEachUIs[techType].IncreaseMaxCapacity(amount);
    }

    // 특정 테크의 현재 수용량 변경
    void ModifyCurrentCapacity(TechData techType, int amount)
    {
        techEachUIs[techType].ModifyCurrentCapacity(amount);
    }
}
