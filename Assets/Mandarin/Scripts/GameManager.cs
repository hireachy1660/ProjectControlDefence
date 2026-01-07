using UnityEngine;

public class GameManager :  MonoBehaviour
{
    public void StartSummonProcess(CardRawData data)
    {
        // 리소스 매니저를 통해 실제 프리팹 파일을 가져온다
        GameObject unitPrefab = ResourceManager.Instance.GetPrefab(data.prefabPath);

        if (unitPrefab != null)
        {
            // 실제 월드에 소환
            Instantiate(unitPrefab, Vector3.zero, Quaternion.identity);
            // 실제로 소환되는 대신 로그를 찍어 확인한다.
            Debug.Log($"<color=green>[GameManager]</color> {data.cardName} 소환 프로세스 시작");
        }
        else
        {
            Debug.LogError($"{data.prefabPath} 경로에 프리팹이 없습니다.");
        }

        // 타입 용도
        if (data.cardType == ECardType.Unit)
        {
            Debug.Log($"{data.cardName} 유닛은 앞으로 걸어가게 설정합니다.");
            // 유닛 AI 활성화 로직...
        }
        else if (data.cardType == ECardType.Tower)
        {
            Debug.Log($"{data.cardName} 타워는 제자리에 고정합니다.");
            // 설치 위치 지정 로직...
        }

    }
}
