using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class Map : MonoBehaviour
{
    int height;
    int width;
    private static float mapOffset = 0.75f;
    GameObject HealthBox;
    GameObject AmmoBox;
    GameObject Wall;
    GameObject Background;
    public MapNode[,] MapGrid;

    public Map(int w,int h,GameObject Back,GameObject WallBackGround, GameObject Health,GameObject AmmoBox)
{
        MapGrid = new MapNode[w,h];
        Generate(MapGrid,w,h,Back,WallBackGround, Health, AmmoBox); 
}
    public enum status { untouched, searched, onOpenList };

    public class MapNode
    {
        public int id;
        public bool passable;
        public int type;
        public status state;
        public float fcost; // f = g + h;
        public float gcost;
        public float hcost;
        public bool isDiag;
        public GameObject grid;
        public List<MapNode> neighbours;
        public int posX;
        public int posY;
        public MapNode[] parent;
        public MapNode[] cameFrom;
    }
    public MapNode[,] Generate(MapNode[,] Node, int w, int h, GameObject Back, GameObject WallBackGround, GameObject Health, GameObject Ammo)
    {
        HealthBox = Health;
        AmmoBox = Ammo;
        Wall = WallBackGround;
        Background = Back;
        width = w;
        height = h;
        int id = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Node[i, j] = new MapNode();
                Node[i, j].id = id;
                Node[i, j].passable = false;
                Node[i, j].type = 0;
                Node[i, j].state = status.untouched;
                Node[i, j].fcost = 1;
                Node[i, j].gcost = 0;
                Node[i, j].hcost = 0;
                Node[i, j].isDiag = false;
                Node[i, j].grid = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Node[i, j].grid.transform.position = new Vector3(1.0f * (j) + mapOffset, -1.0f * (i) -mapOffset, 0f);
                Node[i, j].grid.name = "Cube" + id;
                Node[i, j].neighbours = new List<MapNode>();
                Node[i, j].parent = new MapNode[1];
                Node[i, j].cameFrom = new MapNode[1];
                Node[i, j].posX = i;
                Node[i, j].posY = j;
                ++id;
            }
        }


       // WriteToFile(Grid);
      ReadFromFile(Node);
      calcNeighbours(Node);

        return Node;
    }

    public void WriteToFile(MapNode[,] a) //only used if we need to make a fresh clear map
    {

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Map.txt");
        StreamWriter writer = new StreamWriter(path, true);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                writer.Write(a[i, j].type);
                writer.Write(" ");
            }
            writer.Write("\n");
        }
        writer.Close();
    }
    public void ReadFromFile(MapNode[,] a) /* Read in the map and converts it back into a array */
    {

        int id = 0;
        string filepath = System.IO.Path.Combine(Application.streamingAssetsPath, "Map.txt");
        StreamReader reader = new StreamReader(filepath);
        char[] c = null;
        List<int> ids;
        ids = new List<int>();


        while (reader.Peek() >= 0)
        {
            c = new char[width];
            reader.Read(c, 0, width);
            string s = new string(c);

            foreach (char character in s)
            {
                Console.WriteLine(character);
                if (character != '\n')
                {
                    if (character != ' ')
                    {
                        { id = (int)Char.GetNumericValue(character); }
                        ids.Add(id);

                    }
                }
            }
        }

        int index = 0;
        for (int i = 0; i < width; i++) // loop through the entire map populate each map square
        {
            for (int j = 0; j < height; j++)
            {

                a[i, j].type = ids[index];
                ++index;

                if (a[i, j].type == 3)
                {
                    GameObject health = Instantiate(HealthBox,
                                                     new Vector3(1.0f * (j)+mapOffset, -1.0f * (i) - mapOffset, -1.0f),
                                                     Quaternion.identity)
                                                     as GameObject;
                    health.name = "HealthBox";


                    a[i, j].passable = true;
                    Renderer ren = a[i, j].grid.GetComponent<Renderer>();
                    ren.material.color = new Color(57, 55, 255);
                    a[i, j].grid.layer = 11;
                }

                if (a[i, j].type == 2)
                {
                    GameObject ammoBox = Instantiate(AmmoBox,
                                                     new Vector3(1.0f * (j)+ mapOffset, -1.0f * (i)- mapOffset, -1f),
                                                     Quaternion.identity)
                                                     as GameObject;
                    ammoBox.name = "AmmoBox";


                    a[i, j].passable = true;
                    Renderer ren = a[i, j].grid.GetComponent<Renderer>();
                    ren.material.color = new Color(57, 55, 255);
                    a[i, j].grid.layer = 11;
                }
                    if (a[i, j].type == 1)
                {
                    a[i, j].passable = false;
                    Renderer ren = a[i, j].grid.GetComponent<Renderer>();
                    ren.material.color = new Color(10, 10, 0);
                    Destroy(a[i, j].grid.gameObject);
                    a[i, j].grid = Instantiate(Wall,
                                                  new Vector3(1.0f * (j) + mapOffset, -1.0f * (i) - mapOffset, -1f),
                                                  Quaternion.identity)
                                                  as GameObject;

                    a[i, j].grid.AddComponent<CollisionsOnWalls>();
                    a[i, j].grid.AddComponent<Rigidbody>();
                    a[i, j].grid.name = "Wall";
                    a[i, j].grid.layer = 8;

                    Rigidbody bo = a[i, j].grid.GetComponent<Rigidbody>();
                    bo.useGravity = false;
                    bo.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    bo.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

                }

                if (a[i, j].type == 0)
                {
                    a[i, j].passable = true;
                    Renderer ren = a[i, j].grid.GetComponent<Renderer>();
                    Destroy(a[i, j].grid.gameObject);
                    a[i, j].grid = Instantiate(Background,
                                                  new Vector3(1.0f * (j) + mapOffset, -1.0f * (i) - mapOffset, 0.0f),
                                                  Quaternion.identity)
                                                  as GameObject;



                    a[i, j].grid.layer = 11;
                }
            }

        }
        reader.Close();
    }

    public void calcNeighbours(MapNode[,] a)

        //=================//
        //|| 7 || 0 || 1 ||//
        //|| 6 || X || 2 ||//
        //|| 5 || 4 || 3 ||//
        //=================//
    {
        for (int i = 1; i < width - 1; i++) 
        {
            if (a[i, height - 1].passable == true) //5 neighbours sides
            {
                if(a[i, height - 1 - 1].passable == true)
                a[i, height - 1].neighbours.Add(a[i, height - 1 - 1]);
                if(a[i - 1, height - 1 - 1].passable == true)
                a[i, height - 1].neighbours.Add(a[i - 1, height - 1 - 1]);
            }
            if (a[i, 0].passable == true) 
            {
                if (a[i + 1, 0 + 1].passable == true)
                  a[i, 0].neighbours.Add(a[i + 1, 0 + 1]);
                if(a[i, 0 + 1].passable == true)
                  a[i, 0].neighbours.Add(a[i, 0 + 1]);
                if(a[i - 1, 0 + 1].passable == true)
                a[i, 0].neighbours.Add(a[i - 1, 0 + 1]);
            }

         ///------------------------------------------------------------------------//
        
            for (int j = 1; j < height - 1; j++) //8 neighbours
            {
                if (a[i, j].passable == true)
                {
                    if (a[i, j - 1].passable == true) // 0
                        a[i, j].neighbours.Add(a[i, j - 1]);


                    //if (a[i + 1, j - 1].passable == true)
                    //{
                    //    MapNode node = a[i + 1, j - 1];
                    //    node.isDiag = true;
                    //    a[i, j].neighbours.Add(node); //1
                    //}

                    if (a[i + 1, j].passable == true)
                        a[i, j].neighbours.Add(a[i + 1, j]); //2

                    //if (a[i + 1, j + 1].passable == true)
                    //{
                    //    MapNode node = a[i + 1, j + 1]; //3
                    //    node.isDiag = true;
                    //    a[i, j].neighbours.Add(node);
                    //}

                    if (a[i, j + 1].passable == true)
                        a[i, j].neighbours.Add(a[i, j + 1]); //4
                    
                    //if (a[i - 1, j + 1].passable == true)
                    //{
                    //    MapNode node = a[i - 1, j + 1]; //5
                    //    node.isDiag = true;
                    //    a[i, j].neighbours.Add(node);
                    //}

                    if (a[i - 1, j].passable == true)
                        a[i, j].neighbours.Add(a[i - 1, j]); //6

                    //if (a[i - 1, j - 1].passable == true)
                    //{
                    //    MapNode node = a[i - 1, j - 1]; //7
                    //    node.isDiag = true;
                    //    a[i, j].neighbours.Add(node);
                    //}

                }
            }
        }
        for (int j = 1; j < height - 1; j++)
        {
            if (a[0, j].passable == true)
            {
                if (a[0 + 1, j - 1].passable == true)
                    a[0, j].neighbours.Add(a[0 + 1, j - 1]);
                if (a[0 + 1, j].passable == true)
                    a[0, j].neighbours.Add(a[0 + 1, j]);
                if (a[0 + 1, j + 1].passable == true)
                    a[0, j].neighbours.Add(a[0 + 1, j + 1]);
                if (a[width - 1 - 1, j + 1].passable == true)
                    a[width - 1, j].neighbours.Add(a[width - 1 - 1, j + 1]);
                if (a[width - 1 - 1, j].passable == true)
                    a[width - 1, j].neighbours.Add(a[width - 1 - 1, j]);
                if (a[width - 1 - 1, j - 1].passable == true)
                    a[width - 1, j].neighbours.Add(a[width - 1 - 1, j - 1]);

            }  
        }

        if (a[0, 0].passable == true)
        {
            if (a[0 + 1, 0].passable == true)
                a[0, 0].neighbours.Add(a[0 + 1, 0]);
            if (a[0 + 1, 0 + 1].passable == true)
                a[0, 0].neighbours.Add(a[0 + 1, 0 + 1]);
            if (a[0, 0 + 1].passable == true)
                a[0, 0].neighbours.Add(a[0, 0 + 1]);
        }

        if (a[0, height - 1].passable == true)
        {
            if (a[0, height - 1 - 1].passable == true)
                a[0, height - 1].neighbours.Add(a[0, height - 1 - 1]);
            if (a[0 + 1, height - 1 - 1].passable == true)
                a[0, height - 1].neighbours.Add(a[0 + 1, height - 1 - 1]);
            if (a[0 + 1, height - 1].passable == true)
                a[0, height - 1].neighbours.Add(a[0 + 1, height - 1]);
        }

        if (a[width - 1, 0].type != 1)
        {
            if (a[width - 1, 0 + 1].passable == true)
                a[width - 1, 0].neighbours.Add(a[width - 1, 0 + 1]);
            if (a[width - 1 - 1, 0 + 1].passable == true)
                a[width - 1, 0].neighbours.Add(a[width - 1 - 1, 0 + 1]);
            if (a[width - 1 - 1, 0].passable == true)
                a[width - 1, 0].neighbours.Add(a[width - 1 - 1, 0]);
        }

        if (a[width - 1, height - 1].passable == true)
        {
            if (a[width - 1 - 1, height - 1].passable == true)
                a[width - 1, height - 1].neighbours.Add(a[width - 1 - 1, height - 1]);
            if (a[width - 1 - 1, height - 1 - 1].passable == true)
                a[width - 1, height - 1].neighbours.Add(a[width - 1 - 1, height - 1 - 1]);
            if (a[width - 1, height - 1 - 1].passable == true)
                a[width - 1, height - 1].neighbours.Add(a[width - 1, height - 1 - 1]);

        }
    }
    public static Vector3 ConvertGridNodeToVector(MapNode a)
    {
        
        float x = -a.posX;
        float y = a.posY;

        int xPos = (int)x;
        int yPos = (int)y;
        Vector3 pos = new Vector3(yPos + mapOffset, xPos - mapOffset, -1.0f);
        return pos;
    }

    public static MapNode ConvertPositionToGrid(MapNode[,] a, Vector3 b)
    {
        Vector3 pos = b;
        float x = pos.x;
        float y = -pos.y;
        

        int xval = (int)x;
        int yval = (int)y;


        return a[yval, xval];
    }

    public static float HCost(MapNode Dest, MapNode Current) // Des, Current;
    {
       
        int x2 = Dest.posX;
        int x1 = Current.posX;
        int y2 = Dest.posY;
        int y1 = Current.posY;

        int dX = Math.Abs(x2 - x1);
        int dY = Math.Abs(y2 - y1);


        //float hCost = dX + dY; //Manhattan Distance

        float hCost = (float)Math.Sqrt((dX*dX) + (dY*dY)); // Euclidean Distance
        return hCost;

    }

    public static float GCost(MapNode Current, MapNode Neighbour) //Current and Start Cost so far
    {
        int x2 = Neighbour.posX;
        int x1 = Current.posX;
        int y2 = Neighbour.posY;
        int y1 = Current.posY;

        int dX = Math.Abs(x2 - x1);
        int dY = Math.Abs(y2 - y1);
        float DistBetween = (float)Math.Sqrt((dX * dX) + (dY * dY));
        float gcost = Current.gcost + DistBetween;

       
        return gcost;

    }
}

    


