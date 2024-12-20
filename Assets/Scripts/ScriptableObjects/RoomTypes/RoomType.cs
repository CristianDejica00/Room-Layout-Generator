using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RoomType")]
public class RoomType : ScriptableObject {
    public string id;
    public RoomType[] branches;
    public int minSize;
    public int maxSize;
    public Color lightColor;
}