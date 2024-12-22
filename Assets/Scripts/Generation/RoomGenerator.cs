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
    [Range(1, 100)]
    public int furniturePercentage;

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
        List<Vector2Int> availableSpots = new List<Vector2Int>();
        List<Vector2Int> occupiedSpots = new List<Vector2Int>();

        for(int x=0;x<mapSize;x++) {
            for(int y=0;y<mapSize;y++) {
                if(x<=1 || x>=mapSize-2 || y<=1 || y>=mapSize-2) {
                    if(dungeonMap[x, y] != -1 && dungeonMap[x, y] != -2) {
                        objectMap[x, y] = 1;
                        if(!availableSpots.Contains(new Vector2Int(x, y))) availableSpots.Add(new Vector2Int(x, y));
                    }
                } else {
                    for(int m=-2;m<=2;m++) {
                        for(int n=-2;n<=2;n++) {
                            if(dungeonMap[x, y] != -1 && dungeonMap[x, y] != -2 && dungeonMap[x+m, y+n] == -1) {
                                objectMap[x, y] = 1;
                                if(!availableSpots.Contains(new Vector2Int(x, y))) availableSpots.Add(new Vector2Int(x, y));
                            }
                        }
                    }
                }
            }
        }

        
        while((occupiedSpots.Count*1f/availableSpots.Count*1f) <= 0.4f) {
            int randSpot = Random.Range(0, availableSpots.Count);
            Vector2Int randPos = new Vector2Int(availableSpots[randSpot].x, availableSpots[randSpot].y);
            int shapeWidth = WeightedRandom(2);
            int shapeHeight = WeightedRandom(2);
            shapeWidth = shapeWidth == 0 ? 1 : shapeWidth;
            shapeHeight = shapeHeight == 0 ? 1 : shapeHeight;

            if(randPos.x+shapeWidth < mapSize && randPos.y+shapeHeight < mapSize) {
                var isSameRoom = true;
                var isAvailable = true;
                for(int x=randPos.x;x<=randPos.x+shapeWidth;x++) {
                    for(int y=randPos.y;y<=randPos.y+shapeHeight;y++) {
                        if(dungeonMap[randPos.x, randPos.y] != dungeonMap[x, y]) isSameRoom = false;
                        if(objectMap[x, y] != 1) isAvailable = false;
                    }
                }
                if(isSameRoom && isAvailable) {
                    for(int x=randPos.x;x<=randPos.x+shapeWidth;x++) {
                        for(int y=randPos.y;y<=randPos.y+shapeHeight;y++) {
                            objectMap[x, y] = 2;
                            if(!occupiedSpots.Contains(new Vector2Int(x, y))) occupiedSpots.Add(new Vector2Int(x, y));
                        }
                    }
                }
            }
        }

        for(int x=0;x<mapSize;x++) {
            for(int y=0;y<mapSize;y++) {
                if((x>0 && dungeonMap[x, y] != dungeonMap[x-1, y] || x==0) && x<mapSize-1 && objectMap[x, y] == 1 && objectMap[x+1, y] == 2) {
                    objectMap[x, y] = 3;
                }
                if((x<mapSize-1 && dungeonMap[x, y] != dungeonMap[x+1, y] || x==mapSize-1) && x>0 && objectMap[x, y] == 1 && objectMap[x-1, y] == 2) {
                    objectMap[x, y] = 3;
                }
                if((y>0 && dungeonMap[x, y] != dungeonMap[x, y-1] || y==0) && y<mapSize-1 && objectMap[x, y] == 1 && objectMap[x, y+1] == 2) {
                    objectMap[x, y] = 3;
                }
                if((y<mapSize-1 && dungeonMap[x, y] != dungeonMap[x, y+1] || y==mapSize-1) && y>0 && objectMap[x, y] == 1 && objectMap[x, y-1] == 2) {
                    objectMap[x, y] = 3;
                }
            }
        }

        for(int x=0;x<mapSize;x++) {
            for(int y=0;y<mapSize;y++) {
                if(objectMap[x, y] == 1) objectMap[x, y] = 0;
                if(objectMap[x, y] == 2) objectMap[x, y] = 3;
            }
        }

        foreach(Room r in GetComponent<DungeonGenerator>().roomList) {
            foreach(Cell c in r.cells) {
                for(int x=c.position.x*roomSize;x<c.position.x*roomSize + roomSize;x++) {
                    for(int y=c.position.y*roomSize;y<c.position.y*roomSize + roomSize;y++) {
                        if(c.doors.Contains(0) && y==c.position.y*roomSize + roomSize-1 && x==c.position.x*roomSize+roomSize/2) {
                            objectMap[x, y] = 0;
                            objectMap[x, y-1] = 0;
                            objectMap[x, y-2] = 0;
                        }
                        if(c.doors.Contains(6) && y==c.position.y*roomSize && x==c.position.x*roomSize+roomSize/2) {
                            objectMap[x, y] = 0;
                            objectMap[x, y+1] = 0;
                            objectMap[x, y+2] = 0;
                        }
                        if(c.doors.Contains(9) && y==c.position.y*roomSize+roomSize/2 && x==c.position.x*roomSize) {
                            objectMap[x, y] = 0;
                            objectMap[x+1, y] = 0;
                            objectMap[x+2, y] = 0;
                        }
                        if(c.doors.Contains(3) && y==c.position.y*roomSize+roomSize/2 && x==c.position.x*roomSize + roomSize-1) {
                            objectMap[x, y] = 0;
                            objectMap[x-1, y] = 0;
                            objectMap[x-2, y] = 0;
                        }
                    }
                }
            }
        }


        foreach(Room r in GetComponent<DungeonGenerator>().roomList) {
            foreach(Cell c in r.cells) {
                objectMap[c.position.x*roomSize + roomSize/2, c.position.y*roomSize + roomSize/2] = 2;
            }
        }

        var filled = false;
        while(!filled) {
            filled = true;
            for(int x=1;x<mapSize-1;x++) {
                for(int y=1;y<mapSize-1;y++) {
                    if(objectMap[x, y] == 2) {
                        if(dungeonMap[x, y] == dungeonMap[x-1, y] && objectMap[x-1, y] == 0) {
                            objectMap[x-1, y] = 2;
                            filled = false;
                        }
                        if(dungeonMap[x, y] == dungeonMap[x+1, y] && objectMap[x+1, y] == 0) {
                            objectMap[x+1, y] = 2;
                            filled = false;
                        }
                        if(dungeonMap[x, y] == dungeonMap[x, y-1] && objectMap[x, y-1] == 0) {
                            objectMap[x, y-1] = 2;
                            filled = false;
                        }
                        if(dungeonMap[x, y] == dungeonMap[x, y+1] && objectMap[x, y+1] == 0) {
                            objectMap[x, y+1] = 2;
                            filled = false;
                        }
                    }
                }
            }
        }

        for(int x=0;x<mapSize;x++) {
            for(int y=0;y<mapSize;y++) {
                if(dungeonMap[x, y] != -1 && dungeonMap[x, y] != -2) {
                    if(objectMap[x, y] == 0) objectMap[x, y] = 3;
                    else if(objectMap[x, y] == 1) objectMap[x, y] = 3;
                    else if(objectMap[x, y] == 2) objectMap[x, y] = 0;
                }
            }
        }

        for(int x=1;x<mapSize-1;x++) {
            for(int y=1;y<mapSize-1;y++) {
                if(objectMap[x, y] == 3) {
                    if(objectMap[x-1, y] != 3 && objectMap[x+1, y] != 3 || objectMap[x, y-1] != 3 && objectMap[x, y+1] != 3) {
                        objectMap[x, y] = 0;
                    }
                }
            }
        }

        List<Node> nodeList = new List<Node>();

        foreach(Room r in GetComponent<DungeonGenerator>().roomList) {
            foreach(Cell c in r.cells) {
                Node newNode = new Node(c);
                foreach(Cell g in r.cells) {
                    if(c != g) {
                        if((c.position.x == g.position.x || c.position.y == g.position.y) &&
                        Mathf.Abs(c.position.x - g.position.x) < 1.5f && Mathf.Abs(c.position.y - g.position.y) < 1.5f) {
                            newNode.neighbors.Add(g);
                        }
                    }
                }
                //if(newNode.neighbors.Count > 0) nodeList.Add(newNode);
                nodeList.Add(newNode);
            }
        }

        foreach(Node n in nodeList) {
            if(Random.Range(0, 10) >= 5 && n.neighbors.Count>0) {
                Cell currentCell = n.parentCell;
                Cell selectedCell = n.neighbors[Random.Range(0, n.neighbors.Count)];
                
                if(selectedCell.position.x == currentCell.position.x) {
                    if(selectedCell.position.y < currentCell.position.y) {
                        for(int y=selectedCell.position.y*roomSize;y<=currentCell.position.y*roomSize;y++) {
                            if(y!=(selectedCell.position.y*roomSize+currentCell.position.y*roomSize)/2 && y!=1+(selectedCell.position.y*roomSize+currentCell.position.y*roomSize)/2) {
                                objectMap[selectedCell.position.x*roomSize + roomSize/2, y + roomSize/2] = 3;
                            }
                        }
                    } else {
                        for(int y=currentCell.position.y*roomSize;y<=selectedCell.position.y*roomSize;y++) {
                            if(y!=(currentCell.position.y*roomSize+selectedCell.position.y*roomSize)/2 && y!=1+(currentCell.position.y*roomSize+selectedCell.position.y*roomSize)/2) {
                                objectMap[selectedCell.position.x*roomSize + roomSize/2, y + roomSize/2] = 3;
                            }
                        }
                    }
                } else if(selectedCell.position.y == currentCell.position.y) {
                    if(selectedCell.position.x < currentCell.position.x) {
                        for(int x=selectedCell.position.x*roomSize;x<=currentCell.position.x*roomSize;x++) {
                            if(x!=(selectedCell.position.x*roomSize+currentCell.position.x*roomSize)/2 && x!=1+(selectedCell.position.x*roomSize+currentCell.position.x*roomSize)/2) {
                                objectMap[x + roomSize/2, selectedCell.position.y*roomSize + roomSize/2] = 3;
                            }
                        }
                    } else {
                        for(int x=currentCell.position.x*roomSize;x<=selectedCell.position.x*roomSize;x++) {
                            if(x!=(currentCell.position.x*roomSize+selectedCell.position.x*roomSize)/2 && x!=1+(currentCell.position.x*roomSize+selectedCell.position.x*roomSize)/2) {
                                objectMap[x + roomSize/2, selectedCell.position.y*roomSize + roomSize/2] = 3;
                            }
                        }
                    }
                }

                n.occupied = true;
                foreach(Node m in nodeList) {
                    if(m.parentCell == selectedCell) {
                        foreach(Cell c in m.neighbors) {
                            if(c == n.parentCell) m.occupied = true;
                        }
                    }
                }
            }
        }

        foreach(Node n in nodeList) {
            if(!n.occupied && Random.Range(0, 10) >= 5) {
                Cell currentCell = n.parentCell;
                objectMap[currentCell.position.x*roomSize + roomSize/2-1, currentCell.position.y*roomSize + roomSize/2-1] = 3;
                objectMap[currentCell.position.x*roomSize + roomSize/2-1, currentCell.position.y*roomSize + roomSize/2] = 3;
                objectMap[currentCell.position.x*roomSize + roomSize/2-1, currentCell.position.y*roomSize + roomSize/2+1] = 3;
                objectMap[currentCell.position.x*roomSize + roomSize/2, currentCell.position.y*roomSize + roomSize/2-1] = 3;
                objectMap[currentCell.position.x*roomSize + roomSize/2, currentCell.position.y*roomSize + roomSize/2] = 3;
                objectMap[currentCell.position.x*roomSize + roomSize/2, currentCell.position.y*roomSize + roomSize/2+1] = 3;
                objectMap[currentCell.position.x*roomSize + roomSize/2+1, currentCell.position.y*roomSize + roomSize/2-1] = 3;
                objectMap[currentCell.position.x*roomSize + roomSize/2+1, currentCell.position.y*roomSize + roomSize/2] = 3;
                objectMap[currentCell.position.x*roomSize + roomSize/2+1, currentCell.position.y*roomSize + roomSize/2+1] = 3;
            }
        }

        for(int x=0;x<mapSize-10;x++) {
            for(int y=0;y<mapSize-10;y++) {
                var foundSpace = true;
                for(int m=0;m<10;m++) {
                    for(int n=0;n<10;n++) {
                        if(objectMap[x+m, y+n] != 0 || dungeonMap[x+m, y+n] < 0) foundSpace = false;
                    }
                }
                if(foundSpace) {
                    for(int m=3;m<7;m++) {
                        for(int n=3;n<7;n++) {
                            objectMap[x+m, y+n] = 3;
                        }
                    }
                }
            }
        }

    }

    int WeightedRandom(int n)
    {
        float r = Mathf.Pow(Random.value, 2f);
        return Mathf.Clamp(Mathf.FloorToInt(r * (n + 1)), 0, n);
    }

    void InstantiateObject(string type, Vector2 pos, int angle, Vector2 scale = default(Vector2)) {
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
    }

    void DisplayDungeon() {
        Texture2D texture = new Texture2D(mapSize, mapSize);
        mapPreview.GetComponent<RawImage>().texture = texture;
        texture.filterMode = FilterMode.Point;

        for(int x=0;x<mapSize;x++) {
            for(int y=0;y<mapSize;y++) {
                Color color = GetColorFromInt(dungeonMap[x, y]);
                color = new Color(color.r,color.r,color.r);
                if(objectMap[x, y] == 1) color = Color.red;
                if(objectMap[x, y] == 2) color = Color.red;
                if(objectMap[x, y] == 3) color = Color.blue;
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

public class Node {
    public Cell parentCell;
    public List<Cell> neighbors;
    public bool occupied;

    public Node(Cell pos) {
        parentCell = pos;
        neighbors = new List<Cell>();
        occupied = false;
    }
}