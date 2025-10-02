using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PeopleDropGold : MonoBehaviour
{
    [SerializeField] float inactiveTime = 1f;

    // 골드 주머니 드랍 시작
    public void StartGoldDrop()
    {
        GameObject obj = ObjectPooler.Instance.SpawnObject(ObjectType.AcquireInfoUI);
        obj.transform.SetParent(GameManager.instance.canvasObject.transform);

        obj.TryGetComponent(out AcquireGoldAmountUI acquireComp);
        if (acquireComp == null)
            return;

        int baseAmount = GameManager.instance.GetClickIncreaseTotalAmount();
        int amount = Random.Range(baseAmount * 3, baseAmount * 15);

        GameManager.instance.AddCurrentGoldAmount(amount);

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        acquireComp.AcquireGold(FuncSystem.Format(amount), screenPosition + new Vector3(0f, 30f, 0f), screenPosition + new Vector3(0f, 60f, 0f), Color.black);

        StartCoroutine(DestroyTimer());
    }

    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(inactiveTime);
        gameObject.SetActive(false);
    }
}
