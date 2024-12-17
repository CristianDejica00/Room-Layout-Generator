using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomGenerator : MonoBehaviour {
    [SerializeField] ObjectLibrary library;

    public int mapSize;
    public int roomSize;
    public GameObject mapPreview;
    public int[,] dungeonMap;
    public int[,] objectMap;
    public int[,] roomMap;
    
    public void DecorateRooms() {
        mapSize = GetComponent<DungeonGenerator>().mapSize * GetComponent<DungeonGenerator>().roomSize;
        roomSize = GetComponent<DungeonGenerator>().roomSize;
        dungeonMap = GetComponent<DungeonGenerator>().dungeonMap;
        objectMap = new int[mapSize, mapSize];
        roomMap = new int[mapSize, mapSize];

        GenerateObjectPlacement();

        DisplayDungeon();
    }

    void GenerateObjectPlacement() {
        for(int x=0;x<mapSize;x++) {
            for(int y=0;y<mapSize;y++) {
                if(!(x>0 && y>0 && x<mapSize-1 && y<mapSize-1) || ((x>0 && y>0 && x<mapSize-1 && y<mapSize-1) &&
                (dungeonMap[x, y] != dungeonMap[x-1, y] ||
                dungeonMap[x, y] != dungeonMap[x, y-1] ||
                dungeonMap[x, y] != dungeonMap[x, y+1] ||
                dungeonMap[x, y] != dungeonMap[x+1, y]))) {
                    float xCoord = (float)(x+GetComponent<DungeonGenerator>().levelSeed) / (float)mapSize * 30f;
                    float yCoord = (float)(y+GetComponent<DungeonGenerator>().levelSeed) / (float)mapSize * 30f;
                    float sample = Mathf.PerlinNoise(xCoord, yCoord);

                    if(sample >= 0.5f) {
                        objectMap[x, y] = 1;
                    }
                }
            }
        }

        for(int x=0;x<mapSize;x++) {
            for(int y=0;y<mapSize;y++) {
                if(objectMap[x, y] == 0) {
                    if(x>0 && y>0 && x<mapSize-1 && y<mapSize-1) {
                        if((dungeonMap[x, y] != dungeonMap[x-1, y] && dungeonMap[x, y] != dungeonMap[x, y-1] ||
                        dungeonMap[x, y] != dungeonMap[x-1, y] && dungeonMap[x, y] != dungeonMap[x, y+1] ||
                        dungeonMap[x, y] != dungeonMap[x+1, y] && dungeonMap[x, y] != dungeonMap[x, y-1] ||
                        dungeonMap[x, y] != dungeonMap[x+1, y] && dungeonMap[x, y] != dungeonMap[x, y+1])) {
                            if(dungeonMap[x, y] != dungeonMap[x-1, y] && dungeonMap[x, y] != dungeonMap[x, y-1]) {
                                if(objectMap[x+1, y] == 1 && objectMap[x, y+1] == 1) {
                                    objectMap[x, y] = 1;
                                }
                            } else if(dungeonMap[x, y] != dungeonMap[x-1, y] && dungeonMap[x, y] != dungeonMap[x, y+1]) {
                                if(objectMap[x+1, y] == 1 && objectMap[x, y-1] == 1) {
                                    objectMap[x, y] = 1;
                                }
                            } else if(dungeonMap[x, y] != dungeonMap[x+1, y] && dungeonMap[x, y] != dungeonMap[x, y-1]) {
                                if(objectMap[x-1, y] == 1 && objectMap[x, y+1] == 1) {
                                    objectMap[x, y] = 1;
                                }
                            } else if(dungeonMap[x, y] != dungeonMap[x+1, y] && dungeonMap[x, y] != dungeonMap[x, y+1]) {
                                if(objectMap[x-1, y] == 1 && objectMap[x, y-1] == 1) {
                                    objectMap[x, y] = 1;
                                }
                            }
                        }
                    } else {
                        if(x==0) {
                            if(y==0 && objectMap[x+1, y] == 1 && objectMap[x, y+1] == 1) objectMap[x, y] = 1;
                            else if(y==mapSize-1 && objectMap[x+1, y] == 1 && objectMap[x, y-1] == 1) objectMap[x, y] = 1;
                            else if(y>0 && y<mapSize-1 && dungeonMap[x, y] != dungeonMap[x, y-1] && objectMap[x, y+1] == 1 && objectMap[x+1, y] == 1) objectMap[x, y] = 1;
                            else if(y>0 && y<mapSize-1 && dungeonMap[x, y] != dungeonMap[x, y+1] && objectMap[x, y-1] == 1 && objectMap[x+1, y] == 1) objectMap[x, y] = 1;
                        } else if(x==mapSize-1) {
                            if(y==0 && objectMap[x-1, y] == 1 && objectMap[x, y+1] == 1) objectMap[x, y] = 1;
                            else if(y==mapSize-1 && objectMap[x-1, y] == 1 && objectMap[x, y-1] == 1) objectMap[x, y] = 1;
                            else if(y>0 && y<mapSize-1 && dungeonMap[x, y] != dungeonMap[x, y-1] && objectMap[x, y+1] == 1 && objectMap[x-1, y] == 1) objectMap[x, y] = 1;
                            else if(y>0 && y<mapSize-1 && dungeonMap[x, y] != dungeonMap[x, y+1] && objectMap[x, y-1] == 1 && objectMap[x-1, y] == 1) objectMap[x, y] = 1;
                        } else if(y==0) {
                            if(x>0 && x<mapSize-1 && dungeonMap[x, y] != dungeonMap[x-1, y] && objectMap[x+1, y] == 1 && objectMap[x, y+1] == 1) objectMap[x, y] = 1;
                            else if(x>0 && x<mapSize-1 && dungeonMap[x, y] != dungeonMap[x+1, y] && objectMap[x-1, y] == 1 && objectMap[x, y+1] == 1) objectMap[x, y] = 1;
                        } else if(y==mapSize-1) {
                            if(x>0 && x<mapSize-1 && dungeonMap[x, y] != dungeonMap[x-1, y] && objectMap[x+1, y] == 1 && objectMap[x, y-1] == 1) objectMap[x, y] = 1;
                            else if(x>0 && x<mapSize-1 && dungeonMap[x, y] != dungeonMap[x+1, y] && objectMap[x-1, y] == 1 && objectMap[x, y-1] == 1) objectMap[x, y] = 1;
                        }
                    }
                }
            }
        }

        foreach(Room r in GetComponent<DungeonGenerator>().roomList) {
            foreach(Cell c in r.cells) {
                for(int x=c.position.x*roomSize;x<c.position.x*roomSize + roomSize;x++) {
                    for(int y=c.position.y*roomSize;y<c.position.y*roomSize + roomSize;y++) {
                        if(c.doors.Contains(0) && y==c.position.y*roomSize + roomSize-1 && x==c.position.x*roomSize+roomSize/2) objectMap[x, y] = 0;
                        if(c.doors.Contains(6) && y==c.position.y*roomSize && x==c.position.x*roomSize+roomSize/2) objectMap[x, y] = 0;
                        if(c.doors.Contains(9) && y==c.position.y*roomSize+roomSize/2 && x==c.position.x*roomSize) objectMap[x, y] = 0;
                        if(c.doors.Contains(3) && y==c.position.y*roomSize+roomSize/2 && x==c.position.x*roomSize + roomSize-1) objectMap[x, y] = 0;
                    }
                }
            }
        }

        foreach(Room r in GetComponent<DungeonGenerator>().roomList) {
            foreach(Cell c in r.cells) {
                for(int x=c.position.x*roomSize;x<c.position.x*roomSize + roomSize;x++) {
                    for(int y=c.position.y*roomSize;y<c.position.y*roomSize + roomSize;y++) {
                        if(objectMap[x, y] == 1) {
                            if(x>0 && dungeonMap[x, y] != dungeonMap[x-1, y] || x==0) {
                                if(y>0 && dungeonMap[x, y] != dungeonMap[x, y-1] || y==0) {
                                    if(objectMap[x+1, y] == 1 && objectMap[x, y+1] == 1) {
                                        InstantiateObject("c", new Vector2(x, y), 180);
                                        objectMap[x, y] = 3;
                                        objectMap[x+1, y] = 3;
                                        objectMap[x, y+1] = 3;
                                    } else if(objectMap[x+1, y] == 1) {
                                        InstantiateObject("l_2", new Vector2(x, y), 90, new Vector2(1, -1));
                                        objectMap[x, y] = 3;
                                        objectMap[x+1, y] = 3;
                                    } else if(objectMap[x, y+1] == 1) {
                                        InstantiateObject("l_2", new Vector2(x, y), 180);
                                        objectMap[x, y] = 3;
                                        objectMap[x, y+1] = 3;
                                    }
                                } else if(y<mapSize-1 && dungeonMap[x, y] != dungeonMap[x, y+1] || y==mapSize-1) {
                                    if(objectMap[x+1, y] == 1 && objectMap[x, y-1] == 1) {
                                        InstantiateObject("c", new Vector2(x, y), -90);
                                        objectMap[x, y] = 3;
                                        objectMap[x+1, y] = 3;
                                        objectMap[x, y-1] = 3;
                                    } else if(objectMap[x+1, y] == 1) {
                                        InstantiateObject("l_2", new Vector2(x, y), -90);
                                        objectMap[x, y] = 3;
                                        objectMap[x+1, y] = 3;
                                    } else if(objectMap[x, y-1] == 1) {
                                        InstantiateObject("l_2", new Vector2(x, y), 0, new Vector2(-1, 1));
                                        objectMap[x, y] = 3;
                                        objectMap[x, y-1] = 3;
                                    }
                                }
                            } else if(x<mapSize-1 && dungeonMap[x, y] != dungeonMap[x+1, y] || x==mapSize-1) {
                                if(y>0 && dungeonMap[x, y] != dungeonMap[x, y-1] || y==0) {
                                    if(objectMap[x-1, y] == 1 && objectMap[x, y+1] == 1) {
                                        InstantiateObject("c", new Vector2(x, y), 90);
                                        objectMap[x, y] = 3;
                                        objectMap[x-1, y] = 3;
                                        objectMap[x, y+1] = 3;
                                    } else if(objectMap[x-1, y] == 1) {
                                        InstantiateObject("l_2", new Vector2(x, y), 90);
                                        objectMap[x, y] = 3;
                                        objectMap[x-1, y] = 3;
                                    } else if(objectMap[x, y+1] == 1) {
                                        InstantiateObject("l_2", new Vector2(x, y), 0, new Vector2(1, -1));
                                        objectMap[x, y] = 3;
                                        objectMap[x, y+1] = 3;
                                    }
                                } else if(y<mapSize-1 && dungeonMap[x, y] != dungeonMap[x, y+1] || y==mapSize-1) {
                                    if(objectMap[x-1, y] == 1 && objectMap[x, y-1] == 1) {
                                        InstantiateObject("c", new Vector2(x, y), 0);
                                        objectMap[x, y] = 3;
                                        objectMap[x-1, y] = 3;
                                        objectMap[x, y-1] = 3;
                                    } else if(objectMap[x-1, y] == 1) {
                                        InstantiateObject("l_2", new Vector2(x, y), 90, new Vector2(-1, 1));
                                        objectMap[x, y] = 3;
                                        objectMap[x-1, y] = 3;
                                    } else if(objectMap[x, y-1] == 1) {
                                        InstantiateObject("l_2", new Vector2(x, y), 0);
                                        objectMap[x, y] = 3;
                                        objectMap[x, y-1] = 3;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for(int x=0;x<mapSize;x++) {
            for(int y=0;y<mapSize;y++) {
                if(objectMap[x, y]==1) {
                    var randLength = Random.Range(1, 4);
                    if(randLength == 2 && y<mapSize-1 && dungeonMap[x, y] == dungeonMap[x, y+1] &&
                    objectMap[x, y+1] == 1) {
                        if(x==0 || x>0 && dungeonMap[x, y] != dungeonMap[x-1, y]) InstantiateObject("l_2", new Vector2(x, y), 180, new Vector2(1, 1));
                        else InstantiateObject("l_2", new Vector2(x, y), 0, new Vector2(1, -1));
                        objectMap[x, y] = 3;
                        objectMap[x, y+1] = 3;
                    } else if(randLength == 3 && y<mapSize-2 && dungeonMap[x, y] == dungeonMap[x, y+1] && dungeonMap[x, y] == dungeonMap[x, y+2] &&
                    objectMap[x, y+1] == 1 && objectMap[x, y+2] == 1) {
                        if(x==0 || x>0 && dungeonMap[x, y] != dungeonMap[x-1, y]) InstantiateObject("l_3", new Vector2(x, y), 180, new Vector2(1, 1));
                        else InstantiateObject("l_3", new Vector2(x, y), 0, new Vector2(1, -1));
                        objectMap[x, y] = 3;
                        objectMap[x, y+1] = 3;
                        objectMap[x, y+2] = 3;
                    }
                }
            }
        }

        for(int y=0;y<mapSize;y++) {
            for(int x=0;x<mapSize;x++) {
                if(objectMap[x, y]==1) {
                    var randLength = Random.Range(1, 4);
                    if(randLength == 2 && x<mapSize-1 && dungeonMap[x, y] == dungeonMap[x+1, y] &&
                    objectMap[x+1, y] == 1) {
                        if(y==0 || y>0 && dungeonMap[x, y] != dungeonMap[x, y-1]) InstantiateObject("l_2", new Vector2(x, y), 90, new Vector2(1, -1));
                        else InstantiateObject("l_2", new Vector2(x, y), -90, new Vector2(1, 1));
                        objectMap[x, y] = 3;
                        objectMap[x+1, y] = 3;
                    } else if(randLength == 3 && x<mapSize-2 && dungeonMap[x, y] == dungeonMap[x+1, y] && dungeonMap[x, y] == dungeonMap[x+2, y] &&
                    objectMap[x+1, y] == 1 && objectMap[x+2, y] == 1) {
                        if(y==0 || y>0 && dungeonMap[x, y] != dungeonMap[x, y-1]) InstantiateObject("l_3", new Vector2(x, y), 90, new Vector2(1, -1));
                        else InstantiateObject("l_3", new Vector2(x, y), -90, new Vector2(1, 1));
                        objectMap[x, y] = 3;
                        objectMap[x+1, y] = 3;
                        objectMap[x+2, y] = 3;
                    }
                }
            }
        }


        foreach(Room r in GetComponent<DungeonGenerator>().roomList) {
            foreach(Cell c in r.cells) {
                for(int x=c.position.x*roomSize;x<c.position.x*roomSize + roomSize;x++) {
                    for(int y=c.position.y*roomSize;y<c.position.y*roomSize + roomSize;y++) {
                        if(objectMap[x, y] == 1) {
                            if(x>0 && dungeonMap[x, y] != dungeonMap[x-1, y] || x==0) {
                                InstantiateObject("sq_1", new Vector2(x, y), 180);
                                objectMap[x, y] = 3;
                            } else if(x<mapSize-1 && dungeonMap[x, y] != dungeonMap[x+1, y] || x==mapSize-1) {
                                InstantiateObject("sq_1", new Vector2(x, y), 0);
                                objectMap[x, y] = 3;
                            } else if(y>0 && dungeonMap[x, y] != dungeonMap[x, y-1] || y==0) {
                                InstantiateObject("sq_1", new Vector2(x, y), 90);
                                objectMap[x, y] = 3;
                            } else if(y<mapSize-1 && dungeonMap[x, y] != dungeonMap[x, y+1] || y==mapSize-1) {
                                InstantiateObject("sq_1", new Vector2(x, y), -90);
                                objectMap[x, y] = 3;
                            }
                        }
                    }
                }
            }
        }


        foreach(Room r in GetComponent<DungeonGenerator>().roomList) {
            foreach(Cell c in r.cells) {
                //if(r.cells.Count >= 4) {
                    if(Random.Range(0, 10) > 3) {
                        if(Random.Range(0, 10) > 3) {
                            InstantiateObject("sq_3", new Vector2(c.position.x*roomSize+roomSize/2-1, c.position.y*roomSize+roomSize/2-1), 180);
                            for(int x=c.position.x*roomSize+roomSize/2-1;x<c.position.x*roomSize+roomSize/2-1+3;x++) {
                                for(int y=c.position.y*roomSize+roomSize/2-1;y<c.position.y*roomSize+roomSize/2-1+3;y++) {
                                    objectMap[x, y] = 3;
                                }
                            }
                        } else {
                            var offset1 = Random.Range(0, 2);
                            var offset2 = Random.Range(0, 2);
                            InstantiateObject("sq_2", new Vector2(c.position.x*roomSize+roomSize/2-1+offset1, c.position.y*roomSize+roomSize/2-1+offset2), 180);
                            for(int x=c.position.x*roomSize+roomSize/2-1+offset1;x<c.position.x*roomSize+roomSize/2-1+2+offset1;x++) {
                                for(int y=c.position.y*roomSize+roomSize/2-1+offset2;y<c.position.y*roomSize+roomSize/2-1+2+offset2;y++) {
                                    objectMap[x, y] = 3;
                                }
                            }
                        }
                    }
                //}
            }
        }

    }

    void InstantiateObject(string type, Vector2 pos, int angle, Vector2 scale = default(Vector2)/*, Color color = default(Color)*/) {
        if(scale == new Vector2(0, 0)) scale = new Vector2(1, 1);

        List<ObjectLibrary.RoomObject> listOfType = new List<ObjectLibrary.RoomObject>();

        foreach(ObjectLibrary.RoomObject g in library.objectList) {
            if(type == g.type) {
                listOfType.Add(g);
            }
        }

        var newObject = Instantiate(listOfType[Random.Range(0, listOfType.Count)].prefab);
        newObject.transform.parent = GetComponent<DungeonGenerator>().layoutParent.transform;
        newObject.transform.localPosition = new Vector3(pos.x, 0, pos.y);
        newObject.transform.localScale = new Vector3(scale.x, 1, scale.y);
        newObject.transform.localEulerAngles = new Vector3(0, angle, 0);
        //newObject.GetComponent<MeshRenderer>().material.color = color;
    }

    void DisplayDungeon() {
        Texture2D texture = new Texture2D(mapSize, mapSize);
        mapPreview.GetComponent<RawImage>().texture = texture;
        texture.filterMode = FilterMode.Point;

        for(int x=0;x<mapSize;x++) {
            for(int y=0;y<mapSize;y++) {
                Color color = GetColorFromInt(dungeonMap[x, y]);
                if(objectMap[x, y] == 1) color = Color.black;
                if(objectMap[x, y] == 2) color = Color.gray;
                if(objectMap[x, y] == 3) color = Color.red;
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
    }

    public static Color GetColorFromInt(int value)
    {
        System.Random random = new System.Random(value.GetHashCode());

        float r = (float)random.NextDouble();
        float g = (float)random.NextDouble();
        float b = (float)random.NextDouble();

        return new Color(r, g, b);
    }
}