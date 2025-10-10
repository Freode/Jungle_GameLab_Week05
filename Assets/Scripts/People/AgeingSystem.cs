// 파일 이름: AgeingSystem.cs
using UnityEngine;

[RequireComponent(typeof(PeopleActor))]
public class AgeingSystem : MonoBehaviour
{
    public VoidEventChannelSO OnYearPassedChannel;
    private PeopleActor owner;

    void Awake()
    {
        owner = GetComponent<PeopleActor>();
    }

    private void OnEnable()
    {
        OnYearPassedChannel.OnEventRaised += GetOlder;
    }

    private void OnDisable()
    {
        OnYearPassedChannel.OnEventRaised -= GetOlder;
    }

    private void GetOlder()
    {
        owner.AddAge(1);
    }
}