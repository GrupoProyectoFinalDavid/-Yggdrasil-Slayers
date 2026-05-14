using UnityEngine;

public class RoomPrefabInfo : MonoBehaviour
{
    [Header("Tipo de sala")]
    public RoomType roomType = RoomType.Normal;
    public RowType rowType;
    [Header("Puertas disponibles")]
    public bool hasUp;
    public bool hasDown;
    public bool hasLeft;
    public bool hasRight;

    public bool Matches(bool needUp, bool needDown, bool needLeft, bool needRight)
    {
        return hasUp == needUp &&
               hasDown == needDown &&
               hasLeft == needLeft &&
               hasRight == needRight;
    }
}