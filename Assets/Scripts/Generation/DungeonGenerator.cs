using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
    [Range(1, 10)]
    public int mapSize;
    private int publicMapSize;
    public int roomSize;
    public int[,] dungeonMap;
    public List<Room> roomList;
    public List<Cell> cellList;
    Vector2Int startingRoom;
    public GameObject[] buildingBlocks;
    public GameObject layoutParent;
    public int levelSeed;
    public Color[] areaTypeColors;

    public RoomType[] roomTypeList;

    void Start()
    {
        Random.InitState(levelSeed);
        InitializeDungeon();
        publicMapSize = mapSize;
    }

    void Update() {
        if(mapSize != publicMapSize && mapSize > 0) {
            publicMapSize = mapSize;
            InitializeDungeon();
        }
    }

    void InitializeDungeon() {
        EmptyParent();
        InitializeRooms();
        SetStartingRoom();
        ConnectRooms();
        UpscaleDungeon();
        AssignRoomRole();
        BuildRooms();
        layoutParent.transform.position = new Vector3(-(mapSize*roomSize)/2f, 0, -(mapSize*roomSize)/2f);
        GetComponent<RoomGenerator>().DecorateRooms();
        
    }

    void EmptyParent() {
        foreach(Transform child in layoutParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void UpscaleDungeon() {
        dungeonMap = new int[mapSize*roomSize, mapSize*roomSize];
        foreach(Room r in roomList) {
            foreach(Cell c in r.cells) {
                for(int x=0;x<roomSize;x++) {
                    for(int y=0;y<roomSize;y++) {
                        dungeonMap[c.position.x*roomSize+x, c.position.y*roomSize+y] = r.id;
                    }
                }
            }
        }
    }


    void BuildRooms() {
        for(int x=1;x<roomSize*mapSize-1;x++) {
            for(int y=1;y<roomSize*mapSize-1;y++) {
                if(dungeonMap[x, y] == dungeonMap[x-1, y-1] &&
                dungeonMap[x, y] == dungeonMap[x-1, y] &&
                dungeonMap[x, y] == dungeonMap[x-1, y+1] &&
                dungeonMap[x, y] == dungeonMap[x, y-1] &&
                dungeonMap[x, y] == dungeonMap[x, y+1] &&
                dungeonMap[x, y] == dungeonMap[x+1, y-1] &&
                dungeonMap[x, y] == dungeonMap[x+1, y] &&
                dungeonMap[x, y] == dungeonMap[x+1, y+1]) {
                    var newBlock = Instantiate(buildingBlocks[0]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(x, 0, y);
                } else if(dungeonMap[x, y] != dungeonMap[x-1, y] && dungeonMap[x, y] != dungeonMap[x, y-1] ||
                dungeonMap[x, y] != dungeonMap[x-1, y] && dungeonMap[x, y] != dungeonMap[x, y+1] ||
                dungeonMap[x, y] != dungeonMap[x+1, y] && dungeonMap[x, y] != dungeonMap[x, y-1] ||
                dungeonMap[x, y] != dungeonMap[x+1, y] && dungeonMap[x, y] != dungeonMap[x, y+1]) {
                    var newBlock = Instantiate(buildingBlocks[2]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(x, 0, y);
                    if(dungeonMap[x, y] != dungeonMap[x-1, y] && dungeonMap[x, y] != dungeonMap[x, y-1]) {
                        newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);
                    } else if(dungeonMap[x, y] != dungeonMap[x-1, y] && dungeonMap[x, y] != dungeonMap[x, y+1]) {
                        newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
                    } else if(dungeonMap[x, y] != dungeonMap[x+1, y] && dungeonMap[x, y] != dungeonMap[x, y-1]) {
                        newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);
                    } else if(dungeonMap[x, y] != dungeonMap[x+1, y] && dungeonMap[x, y] != dungeonMap[x, y+1]) {
                        newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);
                    }
                } else if(dungeonMap[x, y] == dungeonMap[x-1, y-1] && dungeonMap[x, y] == dungeonMap[x-1, y+1] && dungeonMap[x, y] == dungeonMap[x+1, y-1] && dungeonMap[x, y] != dungeonMap[x+1, y+1] && dungeonMap[x, y] == dungeonMap[x+1, y] && dungeonMap[x, y] == dungeonMap[x, y+1] ||
                dungeonMap[x, y] == dungeonMap[x-1, y-1] && dungeonMap[x, y] == dungeonMap[x-1, y+1] && dungeonMap[x, y] != dungeonMap[x+1, y-1] && dungeonMap[x, y] == dungeonMap[x+1, y+1] && dungeonMap[x, y] == dungeonMap[x+1, y] && dungeonMap[x, y] == dungeonMap[x, y-1] ||
                dungeonMap[x, y] == dungeonMap[x-1, y-1] && dungeonMap[x, y] != dungeonMap[x-1, y+1] && dungeonMap[x, y] == dungeonMap[x+1, y-1] && dungeonMap[x, y] == dungeonMap[x+1, y+1] && dungeonMap[x, y] == dungeonMap[x-1, y] && dungeonMap[x, y] == dungeonMap[x, y+1] ||
                dungeonMap[x, y] != dungeonMap[x-1, y-1] && dungeonMap[x, y] == dungeonMap[x-1, y+1] && dungeonMap[x, y] == dungeonMap[x+1, y-1] && dungeonMap[x, y] == dungeonMap[x+1, y+1] && dungeonMap[x, y] == dungeonMap[x-1, y] && dungeonMap[x, y] == dungeonMap[x, y-1]) {
                    var newBlock = Instantiate(buildingBlocks[3]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(x, 0, y);
                    if(dungeonMap[x, y] == dungeonMap[x-1, y-1] && dungeonMap[x, y] == dungeonMap[x-1, y+1] && dungeonMap[x, y] == dungeonMap[x+1, y-1] && dungeonMap[x, y] != dungeonMap[x+1, y+1] && dungeonMap[x, y] == dungeonMap[x+1, y] && dungeonMap[x, y] == dungeonMap[x, y+1]) {
                        newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);
                    } else if(dungeonMap[x, y] == dungeonMap[x-1, y-1] && dungeonMap[x, y] == dungeonMap[x-1, y+1] && dungeonMap[x, y] != dungeonMap[x+1, y-1] && dungeonMap[x, y] == dungeonMap[x+1, y+1] && dungeonMap[x, y] == dungeonMap[x+1, y] && dungeonMap[x, y] == dungeonMap[x, y-1]) {
                        newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);
                    } else if(dungeonMap[x, y] == dungeonMap[x-1, y-1] && dungeonMap[x, y] != dungeonMap[x-1, y+1] && dungeonMap[x, y] == dungeonMap[x+1, y-1] && dungeonMap[x, y] == dungeonMap[x+1, y+1] && dungeonMap[x, y] == dungeonMap[x-1, y] && dungeonMap[x, y] == dungeonMap[x, y+1]) {
                        newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
                    } else if(dungeonMap[x, y] != dungeonMap[x-1, y-1] && dungeonMap[x, y] == dungeonMap[x-1, y+1] && dungeonMap[x, y] == dungeonMap[x+1, y-1] && dungeonMap[x, y] == dungeonMap[x+1, y+1] && dungeonMap[x, y] == dungeonMap[x-1, y] && dungeonMap[x, y] == dungeonMap[x, y-1]) {
                        newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);
                    }
                } else if(dungeonMap[x, y] != dungeonMap[x-1, y] ||
                dungeonMap[x, y] != dungeonMap[x+1, y] ||
                dungeonMap[x, y] != dungeonMap[x, y-1] ||
                dungeonMap[x, y] != dungeonMap[x, y+1]) {
                    if(dungeonMap[x, y] != dungeonMap[x-1, y]) {
                        if(GetCellByPosition(new Vector2Int(x/roomSize, y/roomSize)) != null && GetCellByPosition(new Vector2Int(x/roomSize, y/roomSize)).doors.Contains(9)) {
                            if(y == GetCellByPosition(new Vector2Int(x/roomSize, y/roomSize)).position.y*roomSize+roomSize/2) {
                                var newBlock = Instantiate(buildingBlocks[4]);
                                newBlock.transform.parent = layoutParent.transform;
                                newBlock.transform.localPosition = new Vector3(x, 0, y);
                                newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);

                                var newDoor = Instantiate(buildingBlocks[5]);
                                newDoor.transform.parent = layoutParent.transform;
                                newDoor.transform.localPosition = new Vector3(x-0.5f, 0, y);
                            } else {
                                var newBlock = Instantiate(buildingBlocks[1]);
                                newBlock.transform.parent = layoutParent.transform;
                                newBlock.transform.localPosition = new Vector3(x, 0, y);
                                newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);
                            }
                        } else {
                            var newBlock = Instantiate(buildingBlocks[1]);
                            newBlock.transform.parent = layoutParent.transform;
                            newBlock.transform.localPosition = new Vector3(x, 0, y);
                            newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);
                        }
                    } else if(dungeonMap[x, y] != dungeonMap[x+1, y]) {
                        if(GetCellByPosition(new Vector2Int(x/roomSize, y/roomSize)) != null && GetCellByPosition(new Vector2Int(x/roomSize, y/roomSize)).doors.Contains(3)) {
                            if(y == GetCellByPosition(new Vector2Int(x/roomSize, y/roomSize)).position.y*roomSize+roomSize/2) {
                                var newBlock = Instantiate(buildingBlocks[4]);
                                newBlock.transform.parent = layoutParent.transform;
                                newBlock.transform.localPosition = new Vector3(x, 0, y);
                                newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);
                            } else {
                                var newBlock = Instantiate(buildingBlocks[1]);
                                newBlock.transform.parent = layoutParent.transform;
                                newBlock.transform.localPosition = new Vector3(x, 0, y);
                                newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);
                            }
                        } else {
                            var newBlock = Instantiate(buildingBlocks[1]);
                            newBlock.transform.parent = layoutParent.transform;
                            newBlock.transform.localPosition = new Vector3(x, 0, y);
                            newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);
                        }
                    } else if(dungeonMap[x, y] != dungeonMap[x, y-1]) {
                        if(GetCellByPosition(new Vector2Int(x/roomSize, y/roomSize)) != null && GetCellByPosition(new Vector2Int(x/roomSize, y/roomSize)).doors.Contains(6)) {
                            if(x == GetCellByPosition(new Vector2Int(x/roomSize, y/roomSize)).position.x*roomSize+roomSize/2) {
                                var newBlock = Instantiate(buildingBlocks[4]);
                                newBlock.transform.parent = layoutParent.transform;
                                newBlock.transform.localPosition = new Vector3(x, 0, y);
                                newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);

                                var newDoor = Instantiate(buildingBlocks[5]);
                                newDoor.transform.parent = layoutParent.transform;
                                newDoor.transform.localPosition = new Vector3(x, 0, y-0.5f);
                                newDoor.transform.localEulerAngles = new Vector3(0, 90, 0);
                            } else {
                                var newBlock = Instantiate(buildingBlocks[1]);
                                newBlock.transform.parent = layoutParent.transform;
                                newBlock.transform.localPosition = new Vector3(x, 0, y);
                                newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);
                            }
                        } else {
                            var newBlock = Instantiate(buildingBlocks[1]);
                            newBlock.transform.parent = layoutParent.transform;
                            newBlock.transform.localPosition = new Vector3(x, 0, y);
                            newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);
                        }
                    } else if(dungeonMap[x, y] != dungeonMap[x, y+1]) {
                        if(GetCellByPosition(new Vector2Int(x/roomSize, y/roomSize)) != null && GetCellByPosition(new Vector2Int(x/roomSize, y/roomSize)).doors.Contains(0)) {
                            if(x == GetCellByPosition(new Vector2Int(x/roomSize, y/roomSize)).position.x*roomSize+roomSize/2) {
                                var newBlock = Instantiate(buildingBlocks[4]);
                                newBlock.transform.parent = layoutParent.transform;
                                newBlock.transform.localPosition = new Vector3(x, 0, y);
                                newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
                            } else {
                                var newBlock = Instantiate(buildingBlocks[1]);
                                newBlock.transform.parent = layoutParent.transform;
                                newBlock.transform.localPosition = new Vector3(x, 0, y);
                                newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
                            }
                        } else {
                            var newBlock = Instantiate(buildingBlocks[1]);
                            newBlock.transform.parent = layoutParent.transform;
                            newBlock.transform.localPosition = new Vector3(x, 0, y);
                            newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
                        }
                    }
                }
            }
        }

        for(int x=0;x<mapSize*roomSize;x++) {
            if(x==0) {
                var newBlock = Instantiate(buildingBlocks[2]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(x, 0, 0);
                newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);

                newBlock = Instantiate(buildingBlocks[3]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(x-1, 0, -1);
                newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);

                newBlock = Instantiate(buildingBlocks[2]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(x, 0, mapSize*roomSize-1);
                newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);

                newBlock = Instantiate(buildingBlocks[3]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(x-1, 0, mapSize*roomSize);
                newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);

                newBlock = Instantiate(buildingBlocks[1]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(x, 0, mapSize*roomSize);
                newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);

                newBlock = Instantiate(buildingBlocks[1]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(x, 0, -1);
                newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);

                newBlock = Instantiate(buildingBlocks[1]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(mapSize*roomSize-1, 0, mapSize*roomSize);
                newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);

                newBlock = Instantiate(buildingBlocks[1]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(mapSize*roomSize-1, 0, -1);
                newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
            } else if(x==mapSize*roomSize-1) {
                var newBlock = Instantiate(buildingBlocks[2]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(x, 0, 0);
                newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);

                newBlock = Instantiate(buildingBlocks[3]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(x+1, 0, -1);
                newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);

                newBlock = Instantiate(buildingBlocks[2]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(x, 0, mapSize*roomSize-1);
                newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);

                newBlock = Instantiate(buildingBlocks[3]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(x+1, 0, mapSize*roomSize);
                newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);

                newBlock = Instantiate(buildingBlocks[1]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(x+1, 0, mapSize*roomSize-1);
                newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);

                newBlock = Instantiate(buildingBlocks[1]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(x+1, 0, 0);
                newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);

                newBlock = Instantiate(buildingBlocks[1]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(-1, 0, mapSize*roomSize-1);
                newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);

                newBlock = Instantiate(buildingBlocks[1]);
                newBlock.transform.parent = layoutParent.transform;
                newBlock.transform.localPosition = new Vector3(-1, 0, 0);
                newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);
            } else {
                if(x>0 && x<mapSize*roomSize-1 && dungeonMap[x, 0] == dungeonMap[x-1, 0] && dungeonMap[x, 0] == dungeonMap[x+1, 0]) {
                    if(GetCellByPosition(new Vector2Int(x/roomSize, 0)) != null && GetCellByPosition(new Vector2Int(x/roomSize, 0)).doors.Contains(6)) {
                        if(x == GetCellByPosition(new Vector2Int(startingRoom.x, 0)).position.x*roomSize+roomSize/2) {
                            var newBlock = Instantiate(buildingBlocks[4]);
                            newBlock.transform.parent = layoutParent.transform;
                            newBlock.transform.localPosition = new Vector3(x, 0, 0);
                            newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);

                            var newDoor = Instantiate(buildingBlocks[5]);
                            newDoor.transform.parent = layoutParent.transform;
                            newDoor.transform.localPosition = new Vector3(x, 0, -0.5f);
                            newDoor.transform.localEulerAngles = new Vector3(0, 90, 0);
                            
                            newBlock = Instantiate(buildingBlocks[4]);
                            newBlock.transform.parent = layoutParent.transform;
                            newBlock.transform.localPosition = new Vector3(x, 0, -1);
                            newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
                        } else {
                            var newBlock = Instantiate(buildingBlocks[1]);
                            newBlock.transform.parent = layoutParent.transform;
                            newBlock.transform.localPosition = new Vector3(x, 0, 0);
                            newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);
                            
                            newBlock = Instantiate(buildingBlocks[1]);
                            newBlock.transform.parent = layoutParent.transform;
                            newBlock.transform.localPosition = new Vector3(x, 0, -1);
                            newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
                        }
                    } else {
                        var newBlock = Instantiate(buildingBlocks[1]);
                        newBlock.transform.parent = layoutParent.transform;
                        newBlock.transform.localPosition = new Vector3(x, 0, 0);
                        newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);
                        
                        newBlock = Instantiate(buildingBlocks[1]);
                        newBlock.transform.parent = layoutParent.transform;
                        newBlock.transform.localPosition = new Vector3(x, 0, -1);
                        newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
                    }
                } else if(x>0 && dungeonMap[x, 0] == dungeonMap[x-1, 0]) {
                    var newBlock = Instantiate(buildingBlocks[2]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(x, 0, 0);
                    newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);
                        
                    newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(x, 0, -1);
                    newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
                } else if(x<mapSize*roomSize-1 && dungeonMap[x, 0] == dungeonMap[x+1, 0]) {
                    var newBlock = Instantiate(buildingBlocks[2]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(x, 0, 0);
                    newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);
                    
                    newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(x, 0, -1);
                    newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
                }

                if(x>0 && x<mapSize*roomSize-1 && dungeonMap[0, x] == dungeonMap[0, x-1] && dungeonMap[0, x] == dungeonMap[0, x+1]) {
                    var newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(0, 0, x);
                    newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);
                        
                    newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(-1, 0, x);
                    newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);
                } else if(x>0 && dungeonMap[0, x] == dungeonMap[0, x-1]) {
                    var newBlock = Instantiate(buildingBlocks[2]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(0, 0, x);
                    newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
                        
                    newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(-1, 0, x);
                    newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);
                } else if(x<mapSize*roomSize-1 && dungeonMap[0, x] == dungeonMap[0, x+1]) {
                    var newBlock = Instantiate(buildingBlocks[2]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(0, 0, x);
                    newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);
                        
                    newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(-1, 0, x);
                    newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);
                }

                if(x>0 && x<mapSize*roomSize-1 && dungeonMap[x, mapSize*roomSize-1] == dungeonMap[x-1, mapSize*roomSize-1] && dungeonMap[x, mapSize*roomSize-1] == dungeonMap[x+1, mapSize*roomSize-1]) {
                    var newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(x, 0, mapSize*roomSize-1);
                    newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
                        
                    newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(x, 0, mapSize*roomSize);
                    newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);
                } else if(x>0 && dungeonMap[x, mapSize*roomSize-1] == dungeonMap[x-1, mapSize*roomSize-1]) {
                    var newBlock = Instantiate(buildingBlocks[2]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(x, 0, mapSize*roomSize-1);
                    newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);
                        
                    newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(x, 0, mapSize*roomSize);
                    newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);
                } else if(x<mapSize*roomSize-1 && dungeonMap[x, mapSize*roomSize-1] == dungeonMap[x+1, mapSize*roomSize-1]) {
                    var newBlock = Instantiate(buildingBlocks[2]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(x, 0, mapSize*roomSize-1);
                    newBlock.transform.localEulerAngles = new Vector3(0, -90, 0);
                        
                    newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(x, 0, mapSize*roomSize);
                    newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);
                }

                if(x>0 && x<mapSize*roomSize-1 && dungeonMap[mapSize*roomSize-1, x] == dungeonMap[mapSize*roomSize-1, x-1] && dungeonMap[mapSize*roomSize-1, x] == dungeonMap[mapSize*roomSize-1, x+1]) {
                    var newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(mapSize*roomSize-1, 0, x);
                    newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);
                        
                    newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(mapSize*roomSize, 0, x);
                    newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);
                } else if(x>0 && dungeonMap[mapSize*roomSize-1, x] == dungeonMap[mapSize*roomSize-1, x-1]) {
                    var newBlock = Instantiate(buildingBlocks[2]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(mapSize*roomSize-1, 0, x);
                    newBlock.transform.localEulerAngles = new Vector3(0, 0, 0);
                        
                    newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(mapSize*roomSize, 0, x);
                    newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);
                } else if(x<mapSize*roomSize-1 && dungeonMap[mapSize*roomSize-1, x] == dungeonMap[mapSize*roomSize-1, x+1]) {
                    var newBlock = Instantiate(buildingBlocks[2]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(mapSize*roomSize-1, 0, x);
                    newBlock.transform.localEulerAngles = new Vector3(0, 90, 0);
                        
                    newBlock = Instantiate(buildingBlocks[1]);
                    newBlock.transform.parent = layoutParent.transform;
                    newBlock.transform.localPosition = new Vector3(mapSize*roomSize, 0, x);
                    newBlock.transform.localEulerAngles = new Vector3(0, 180, 0);
                }
            }
        }
    }

    Cell GetCellByPosition(Vector2Int pos) {
        foreach(Cell c in cellList) {
            if(c.position == pos) return c;
        }
        return null;
    }

    void SetStartingRoom() {
        startingRoom = new Vector2Int(Random.Range(0, mapSize), 0);
        foreach(Cell c in GetRoomByPosition(startingRoom).cells) {
            if(c.position == startingRoom) { 
                c.doors.Add(6);
            }
        }
        GetRoomByPosition(startingRoom).connected = true;
        GetRoomByPosition(startingRoom).type = GetRoomTypeById("hallway");
    }

    Room GetRoomByPosition(Vector2Int cellPos) {
        foreach(Room r in roomList) {
            foreach(Cell c in r.cells) {
                if(cellPos == c.position) return r;
            }
        }
        return null;
    }

    void ConnectRooms() {
        var isConnected = false;
        while(!isConnected) {
            isConnected = true;
            foreach(Room r in roomList) {
                if(!r.connected) isConnected = false;
            }
            if(!isConnected) {
                var randomPos = new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize));
                while(GetRoomByPosition(randomPos).connected) randomPos = new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize));
                if(randomPos.x > 0 && GetRoomByPosition(randomPos + new Vector2Int(-1, 0)).connected) {
                    foreach(Cell c in GetRoomByPosition(randomPos).cells) {
                        if(c.position == randomPos) {
                            c.doors.Add(9);
                        }
                    }
                    foreach(Cell c in GetRoomByPosition(randomPos + new Vector2Int(-1, 0)).cells) {
                        if(c.position == randomPos + new Vector2Int(-1, 0)) {
                            c.doors.Add(3);
                        }
                    }
                    GetRoomByPosition(randomPos).connected = true;
                    GetRoomByPosition(randomPos).neighbors.Add(GetRoomByPosition(randomPos + new Vector2Int(-1, 0)));
                    GetRoomByPosition(randomPos + new Vector2Int(-1, 0)).neighbors.Add(GetRoomByPosition(randomPos));
                } else if(randomPos.x < mapSize-1 && GetRoomByPosition(randomPos + new Vector2Int(1, 0)).connected) {
                    foreach(Cell c in GetRoomByPosition(randomPos).cells) {
                        if(c.position == randomPos) {
                            c.doors.Add(3);
                        }
                    }
                    foreach(Cell c in GetRoomByPosition(randomPos + new Vector2Int(1, 0)).cells) {
                        if(c.position == randomPos + new Vector2Int(1, 0)) {
                            c.doors.Add(9);
                        }
                    }
                    GetRoomByPosition(randomPos).connected = true;
                    GetRoomByPosition(randomPos).neighbors.Add(GetRoomByPosition(randomPos + new Vector2Int(1, 0)));
                    GetRoomByPosition(randomPos + new Vector2Int(1, 0)).neighbors.Add(GetRoomByPosition(randomPos));
                } else if(randomPos.y > 0 && GetRoomByPosition(randomPos + new Vector2Int(0, -1)).connected) {
                    foreach(Cell c in GetRoomByPosition(randomPos).cells) {
                        if(c.position == randomPos) {
                            c.doors.Add(6);
                        }
                    }
                    foreach(Cell c in GetRoomByPosition(randomPos + new Vector2Int(0, -1)).cells) {
                        if(c.position == randomPos + new Vector2Int(0, -1)) {
                            c.doors.Add(0);
                        }
                    }
                    GetRoomByPosition(randomPos).connected = true;
                    GetRoomByPosition(randomPos).neighbors.Add(GetRoomByPosition(randomPos + new Vector2Int(0, -1)));
                    GetRoomByPosition(randomPos + new Vector2Int(0, -1)).neighbors.Add(GetRoomByPosition(randomPos));
                } else if(randomPos.y < mapSize-1 && GetRoomByPosition(randomPos + new Vector2Int(0, 1)).connected) {
                    foreach(Cell c in GetRoomByPosition(randomPos).cells) {
                        if(c.position == randomPos) {
                            c.doors.Add(0);
                        }
                    }
                    foreach(Cell c in GetRoomByPosition(randomPos + new Vector2Int(0, 1)).cells) {
                        if(c.position == randomPos + new Vector2Int(0, 1)) {
                            c.doors.Add(6);
                        }
                    }
                    GetRoomByPosition(randomPos).connected = true;
                    GetRoomByPosition(randomPos).neighbors.Add(GetRoomByPosition(randomPos + new Vector2Int(0, 1)));
                    GetRoomByPosition(randomPos + new Vector2Int(0, 1)).neighbors.Add(GetRoomByPosition(randomPos));
                }
            }
        }
    }

    void InitializeRooms() {
        dungeonMap = new int[mapSize, mapSize];
        roomList = new List<Room>();
        cellList = new List<Cell>();
        var completed = false;
        while(!completed) {
            completed = true;
            var occupiedRooms = 0;
            for(int x=0;x<mapSize;x++) {
                for(int y=0;y<mapSize;y++) {
                    if(dungeonMap[x, y] == 1) occupiedRooms += 1;
                }
            }
            if(occupiedRooms != mapSize*mapSize) {
                completed = false;
                Vector2Int randomPos = new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize));
                while(dungeonMap[randomPos.x, randomPos.y] == 1) randomPos = new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize));
                TrySetRoom(randomPos);
            }
        }
    }

    void TrySetRoom(Vector2Int randomPos) {
        var randLayout = GetRandomRoomLayout();
        var x = randomPos.x;
        var y = randomPos.y;
        if(randLayout == 7) {
            if(x>0 && y>0 && x<mapSize-1 && y<mapSize-1 && dungeonMap[x-1, y] == 0 && dungeonMap[x+1, y] == 0 && dungeonMap[x, y-1] == 0 && dungeonMap[x, y+1] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x-1, y, newRoom);
                CreateCell(x+1, y, newRoom);
                CreateCell(x, y-1, newRoom);
                CreateCell(x, y+1, newRoom);
                roomList.Add(newRoom);
            }
        } else if(randLayout == 6) {
            if(x>0 && y>0 && dungeonMap[x-1, y] == 0 && dungeonMap[x, y-1] == 0 && dungeonMap[x-1, y-1] == 0) {
                if(x<mapSize-1 && dungeonMap[x+1, y] == 0) {
                    var newRoom = new Room(roomList.Count);
                    CreateCell(x, y, newRoom);
                    CreateCell(x-1, y, newRoom);
                    CreateCell(x, y-1, newRoom);
                    CreateCell(x-1, y-1, newRoom);
                    CreateCell(x+1, y, newRoom);
                    roomList.Add(newRoom);
                } else if(y<mapSize-1 && dungeonMap[x, y+1] == 0) {
                    var newRoom = new Room(roomList.Count);
                    CreateCell(x, y, newRoom);
                    CreateCell(x-1, y, newRoom);
                    CreateCell(x, y-1, newRoom);
                    CreateCell(x-1, y-1, newRoom);
                    CreateCell(x, y+1, newRoom);
                    roomList.Add(newRoom);
                }
            } else if(x>0 && y<mapSize-1 && dungeonMap[x-1, y] == 0 && dungeonMap[x, y+1] == 0 && dungeonMap[x-1, y+1] == 0) {
                if(x<mapSize-1 && dungeonMap[x+1, y] == 0) {
                    var newRoom = new Room(roomList.Count);
                    CreateCell(x, y, newRoom);
                    CreateCell(x-1, y, newRoom);
                    CreateCell(x, y+1, newRoom);
                    CreateCell(x-1, y+1, newRoom);
                    CreateCell(x+1, y, newRoom);
                    roomList.Add(newRoom);
                } else if(y>0 && dungeonMap[x, y-1] == 0) {
                    var newRoom = new Room(roomList.Count);
                    CreateCell(x, y, newRoom);
                    CreateCell(x-1, y, newRoom);
                    CreateCell(x, y+1, newRoom);
                    CreateCell(x-1, y+1, newRoom);
                    CreateCell(x, y-1, newRoom);
                    roomList.Add(newRoom);
                }
            } else if(x<mapSize-1 && y>0 && dungeonMap[x+1, y] == 0 && dungeonMap[x, y-1] == 0 && dungeonMap[x+1, y-1] == 0) {
                if(x>0 && dungeonMap[x-1, y] == 0) {
                    var newRoom = new Room(roomList.Count);
                    CreateCell(x, y, newRoom);
                    CreateCell(x+1, y, newRoom);
                    CreateCell(x, y-1, newRoom);
                    CreateCell(x+1, y-1, newRoom);
                    CreateCell(x-1, y, newRoom);
                    roomList.Add(newRoom);
                } else if(y<mapSize-1 && dungeonMap[x, y+1] == 0) {
                    var newRoom = new Room(roomList.Count);
                    CreateCell(x, y, newRoom);
                    CreateCell(x+1, y, newRoom);
                    CreateCell(x, y-1, newRoom);
                    CreateCell(x+1, y-1, newRoom);
                    CreateCell(x, y+1, newRoom);
                    roomList.Add(newRoom);
                }
            } else if(x<mapSize-1 && y<mapSize-1 && dungeonMap[x+1, y] == 0 && dungeonMap[x, y+1] == 0 && dungeonMap[x+1, y+1] == 0) {
                if(x>0 && dungeonMap[x-1, y] == 0) {
                    var newRoom = new Room(roomList.Count);
                    CreateCell(x, y, newRoom);
                    CreateCell(x+1, y, newRoom);
                    CreateCell(x, y+1, newRoom);
                    CreateCell(x+1, y+1, newRoom);
                    CreateCell(x-1, y, newRoom);
                    roomList.Add(newRoom);
                } else if(y>0 && dungeonMap[x, y-1] == 0) {
                    var newRoom = new Room(roomList.Count);
                    CreateCell(x, y, newRoom);
                    CreateCell(x+1, y, newRoom);
                    CreateCell(x, y+1, newRoom);
                    CreateCell(x+1, y+1, newRoom);
                    CreateCell(x, y-1, newRoom);
                    roomList.Add(newRoom);
                }
            }
        } else if(randLayout == 5) {
            if(x>0 && y>0 && dungeonMap[x-1, y] == 0 && dungeonMap[x, y-1] == 0 && dungeonMap[x-1, y-1] == 0) {
                    var newRoom = new Room(roomList.Count);
                    CreateCell(x, y, newRoom);
                    CreateCell(x-1, y, newRoom);
                    CreateCell(x, y-1, newRoom);
                    CreateCell(x-1, y-1, newRoom);
                    roomList.Add(newRoom);
            } else if(x>0 && y<mapSize-1 && dungeonMap[x-1, y] == 0 && dungeonMap[x, y+1] == 0 && dungeonMap[x-1, y+1] == 0) {
                    var newRoom = new Room(roomList.Count);
                    CreateCell(x, y, newRoom);
                    CreateCell(x-1, y, newRoom);
                    CreateCell(x, y+1, newRoom);
                    CreateCell(x-1, y+1, newRoom);
                    roomList.Add(newRoom);
            } else if(x<mapSize-1 && y>0 && dungeonMap[x+1, y] == 0 && dungeonMap[x, y-1] == 0 && dungeonMap[x+1, y-1] == 0) {
                    var newRoom = new Room(roomList.Count);
                    CreateCell(x, y, newRoom);
                    CreateCell(x+1, y, newRoom);
                    CreateCell(x, y-1, newRoom);
                    CreateCell(x+1, y-1, newRoom);
                    roomList.Add(newRoom);
            } else if(x<mapSize-1 && y<mapSize-1 && dungeonMap[x+1, y] == 0 && dungeonMap[x, y+1] == 0 && dungeonMap[x+1, y+1] == 0) {
                    var newRoom = new Room(roomList.Count);
                    CreateCell(x, y, newRoom);
                    CreateCell(x+1, y, newRoom);
                    CreateCell(x, y+1, newRoom);
                    CreateCell(x+1, y+1, newRoom);
                    roomList.Add(newRoom);
            }
        } else if(randLayout == 4) {
            if(x>0 && y>0 && y<mapSize-1 && dungeonMap[x-1, y] == 0 && dungeonMap[x, y-1] == 0 && dungeonMap[x, y+1] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x-1, y, newRoom);
                CreateCell(x, y-1, newRoom);
                CreateCell(x, y+1, newRoom);
                roomList.Add(newRoom);
            } else if(x<mapSize-1 && y>0 && y<mapSize-1 && dungeonMap[x+1, y] == 0 && dungeonMap[x, y-1] == 0 && dungeonMap[x, y+1] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x+1, y, newRoom);
                CreateCell(x, y-1, newRoom);
                CreateCell(x, y+1, newRoom);
                roomList.Add(newRoom);
            } else if(y>0 && x>0 && x<mapSize-1 && dungeonMap[x, y-1] == 0 && dungeonMap[x-1, y] == 0 && dungeonMap[x+1, y] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x, y-1, newRoom);
                CreateCell(x-1, y, newRoom);
                CreateCell(x+1, y, newRoom);
                roomList.Add(newRoom);
            } else if(y<mapSize-1 && x>0 && x<mapSize-1 && dungeonMap[x, y+1] == 0 && dungeonMap[x-1, y] == 0 && dungeonMap[x+1, y] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x, y+1, newRoom);
                CreateCell(x-1, y, newRoom);
                CreateCell(x+1, y, newRoom);
                roomList.Add(newRoom);
            }
        } else if(randLayout == 3) {
            if(x>0 && y>0 && dungeonMap[x-1, y] == 0 && dungeonMap[x, y-1] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x-1, y, newRoom);
                CreateCell(x, y-1, newRoom);
                roomList.Add(newRoom);
            } else if(x>0 && y<mapSize-1 && dungeonMap[x-1, y] == 0 && dungeonMap[x, y+1] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x-1, y, newRoom);
                CreateCell(x, y+1, newRoom);
                roomList.Add(newRoom);
            } else if(x<mapSize-1 && y>0 && dungeonMap[x+1, y] == 0 && dungeonMap[x, y-1] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x+1, y, newRoom);
                CreateCell(x, y-1, newRoom);
                roomList.Add(newRoom);
            } else if(x<mapSize-1 && y<mapSize-1 && dungeonMap[x+1, y] == 0 && dungeonMap[x, y+1] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x+1, y, newRoom);
                CreateCell(x, y+1, newRoom);
                roomList.Add(newRoom);
            }
        } else if(randLayout == 2) {
            if(x>0 && x<mapSize-1 && dungeonMap[x-1, y] == 0 && dungeonMap[x+1, y] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x-1, y, newRoom);
                CreateCell(x+1, y, newRoom);
                roomList.Add(newRoom);
            } else if(y>0 && y<mapSize-1 && dungeonMap[x, y-1] == 0 && dungeonMap[x, y+1] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x, y-1, newRoom);
                CreateCell(x, y+1, newRoom);
                roomList.Add(newRoom);
            }
        } else if(randLayout == 1) {
            if(x>0 && dungeonMap[x-1, y] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x-1, y, newRoom);
                roomList.Add(newRoom);
            } else if(x<mapSize-1 && dungeonMap[x+1, y] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x+1, y, newRoom);
                roomList.Add(newRoom);
            } else if(y>0 && dungeonMap[x, y-1] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x, y-1, newRoom);
                roomList.Add(newRoom);
            } else if(y<mapSize-1 && dungeonMap[x, y+1] == 0) {
                var newRoom = new Room(roomList.Count);
                CreateCell(x, y, newRoom);
                CreateCell(x, y+1, newRoom);
                roomList.Add(newRoom);
            }
        } else if(randLayout == 0) {
            var newRoom = new Room(roomList.Count);
            CreateCell(x, y, newRoom);
            roomList.Add(newRoom);
        }
    }

    int GetRandomRoomLayout() {
        List<int> percentageList = new List<int>();
        int randomValue = Random.Range(0, 100);
        percentageList.Add(6);
        percentageList.Add(8);
        percentageList.Add(10);
        percentageList.Add(12);
        percentageList.Add(14);
        percentageList.Add(16);
        percentageList.Add(18);
        percentageList.Add(20);

        int cumulativeProbability = 0;

        for(int x=0;x<percentageList.Count;x++) {
            cumulativeProbability += percentageList[x];
            if(randomValue <= cumulativeProbability) {
                return x;
            }
        }
        return 0;
    }

    void CreateCell(int x, int y, Room r) {
        dungeonMap[x, y] = 1;
        var newCell = new Cell(new Vector2Int(x, y), r);
        r.cells.Add(newCell);
        cellList.Add(newCell);
    }

    void AssignRoomRole() {

        List<string> cornerTypes = new List<string>{"food", "tech", "medical", "general"};

        GetRoomByPosition(new Vector2Int(1, 1)).areaType = cornerTypes[Random.Range(0, cornerTypes.Count)];
        cornerTypes.Remove(GetRoomByPosition(new Vector2Int(1, 1)).areaType);

        GetRoomByPosition(new Vector2Int(1, (mapSize-2))).areaType = cornerTypes[Random.Range(0, cornerTypes.Count)];
        cornerTypes.Remove(GetRoomByPosition(new Vector2Int(1, (mapSize-2))).areaType);

        GetRoomByPosition(new Vector2Int((mapSize-2), 1)).areaType = cornerTypes[Random.Range(0, cornerTypes.Count)];
        cornerTypes.Remove(GetRoomByPosition(new Vector2Int((mapSize-2), 1)).areaType);

        GetRoomByPosition(new Vector2Int((mapSize-2), (mapSize-2))).areaType = cornerTypes[Random.Range(0, cornerTypes.Count)];


        var areaTypeFound = false;
        while(!areaTypeFound) {
            areaTypeFound = true;
            foreach(Room r in roomList) {
                if(r.areaType == "") { 
                    areaTypeFound = false;
                    foreach(Room n in r.neighbors) {
                        if(n.areaType != "") {
                            r.areaType = n.areaType;
                        }
                    }
                }
            }
        }
    }

    RoomType GetRoomTypeById(string id) {
        foreach(RoomType r in roomTypeList) {
            if(r.id == id) return r;
        }
        return null;
    }

    List<RoomType> GetDeadEndRoomTypeList() {
        List<RoomType> rList = new List<RoomType>();
        foreach(RoomType r in roomTypeList) {
            if(r.branches.Length == 0) rList.Add(r);
        }
        return rList;
    }

    List<RoomType> GetRoomTypeBranchList(RoomType rm) {
        List<RoomType> rList = new List<RoomType>();
        foreach(RoomType r in roomTypeList) {
            if(r == rm) {
                foreach(RoomType b in r.branches) {
                    rList.Add(b);
                }
            }
        }
        return rList;
    }
}

public class Cell {
    public Vector2Int position;
    public List<int> doors;
    public Room parentRoom;

    public Cell(Vector2Int pos, Room r) {
        doors = new List<int>();
        parentRoom = r;
        position = pos;
    }
}

public class Room {
    public int id;
    public List<Cell> cells;
    public int doorCount;
    public bool connected;
    public RoomType type;
    public List<Room> neighbors;
    public string areaType;

    public Room(int i) {
        id = i;
        cells = new List<Cell>();
        neighbors = new List<Room>();
        areaType = "";
    }
}