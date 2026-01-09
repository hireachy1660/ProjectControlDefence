using System.Xml;
using UnityEngine;

public enum ECardType
{
    Unit,   // 유닛
    Tower   // 타워
}

[System.Serializable]
public class CardRawData   // 태스트용 데이터 클래스
{
    public string cardName;
    public int cost;
    public Sprite cardIcon;
    public ResourceManager.DicType type;

    public ECardType cardType;
}