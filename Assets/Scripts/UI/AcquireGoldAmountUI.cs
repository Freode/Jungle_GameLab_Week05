using System.Collections;
using TMPro;
using UnityEngine;

public class AcquireGoldAmountUI : MonoBehaviour
{
    public TextMeshProUGUI textAmount;

    [SerializeField] float maxTime = 0.5f;

    // 금 획득량 표기 시작
    public void AcquireGold(string amount, Vector3 startPos, Vector3 endPos)
    {
        textAmount.text = "+" + amount;
        StartCoroutine(AnimGold(startPos, endPos));
    }

    // 금 획득량 애님 업데이트
    IEnumerator AnimGold(Vector3 startPos, Vector3 endPos)
    {
        Vector3 curPos = startPos;
        float curTime = 0f;

        // 위치 변경
        while (Vector3.Distance(curPos, endPos) > 0.05f)
        {
            curPos = startPos + (endPos - startPos) * (1f - (maxTime - curTime) / maxTime);
            gameObject.transform.position = curPos;
            curTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        CompleteAnimGold();
    }

    private void CompleteAnimGold()
    {
        Destroy(gameObject);
    }
}
