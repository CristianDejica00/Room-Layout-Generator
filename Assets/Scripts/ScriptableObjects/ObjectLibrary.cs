using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ObjectLibrary")]
public class ObjectLibrary : ScriptableObject
{
    [SerializeField]
    public RoomObject[] objectList;

    private List<string> environmentList = new List<string>() {"dungeon", "mansion", "castle", "housing"};
    private List<string> objectFunctionList = new List<string>() {"sleeping", "lightning", "storage", "decoration", "sitting"};
    private List<string> roomFunctionList = new List<string>() {"sleep", "cooking", "dining", "entertainment", "storage", "pantry"};

    [System.Serializable]
    public class RoomObject {
        [Header("General Attributes")]
        public string name;
        public string type;
        public GameObject prefab;
        public bool interactable;

        [Header("Surroundings")]
        [Dropdown("environmentList")]
        public string envioronment;
        public bool abandoned;
        [Dropdown("objectFunctionList")]
        public string objectFunction;
        [Dropdown("roomFunctionList")]
        public string roomFunction;

        [Header("Position")]
        public bool wall;
        public bool floor;
        public bool corner;
    }
}

