using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PathInfo;

public class AStarSearch
{
    public static PathRoute Search(Map.MapNode[,] MapNode, Vector3 Position, Vector3 objPosition, PathRoute Path, List<Map.MapNode> openList, List<Map.MapNode> closedList)
    {
        Map.MapNode start = Map.ConvertPositionToGrid(MapNode, Position);

        Map.MapNode obj = Map.ConvertPositionToGrid(MapNode, objPosition);

        return aStarSearch(start, obj, openList, closedList, MapNode, Path);
    }
    private static PathRoute aStarSearch(Map.MapNode start, Map.MapNode obj, List<Map.MapNode> openList, List<Map.MapNode> closedList, Map.MapNode[,] grid, PathRoute path)
    {
        Reset(grid, openList, closedList);
        Map.MapNode currentNode = start; // will be changed to whichever has the lowerest cost

        int x = currentNode.posX;
        int y = currentNode.posY;

        currentNode.fcost = Map.HCost(obj, currentNode); // calc f cost
        currentNode.cameFrom[0] = currentNode;

        openList.Add(currentNode);

        while (openList.Count != 0)
        {
            if (currentNode.id == obj.id)
            {
                closedList.Add(currentNode);
                path = generatePath(closedList, grid, start, path);
                return path;

            }
            else
            {


                for (int i = 0; i < currentNode.neighbours.Count; i++) //loop all neighbours
                {
                    x = currentNode.neighbours[i].posX;
                    y = currentNode.neighbours[i].posY;

                    var temp = currentNode.neighbours[i];

                    temp.gcost = Map.GCost(currentNode, currentNode.neighbours[i]);
                    temp.hcost = Map.HCost(obj, currentNode.neighbours[i]);
                    temp.cameFrom[0] = currentNode;

                    temp.fcost = temp.gcost + temp.hcost;

                    currentNode.neighbours[i] = temp;

                    for (int j = 0; j < openList.Count; j++)
                    {
                        if (openList[j].id == currentNode.neighbours[i].id) //if it's already on the open list then
                        {

                            grid[x, y].state = Map.status.onOpenList;

                            if (temp.fcost >= openList[j].fcost)           //we already have a cheaper node
                            {
                                break;
                            }

                            if (temp.fcost < openList[j].fcost)             //if this happens our heuristic is broken
                            {
                                openList[j].parent[0] = currentNode;
                            }

                        }
                    }
                    if (grid[x, y].state != Map.status.onOpenList)            //if not on the openList we check if it's already on the closed list if it isn't add it to the openlist
                    {
                        for (int k = 0; k < closedList.Count; k++)
                        {
                            if (closedList[k].id == currentNode.neighbours[i].id) //if already on the closed list, check if neighbour is cheaper if it is set parent
                            {
                                grid[x, y].state = Map.status.searched;
                                if (temp.fcost >= closedList[k].fcost)
                                {
                                    break;
                                }
                                if (temp.fcost < closedList[k].fcost)
                                {
                                    closedList[k].parent[0] = currentNode;
                                }
                            }

                        }
                        if (grid[x, y].state != Map.status.searched)
                        {
                            currentNode.neighbours[i].parent[0] = currentNode;
                            grid[x, y].state = Map.status.onOpenList;

                            openList.Add(currentNode.neighbours[i]);
                        }
                    }
                }
                x = currentNode.posX;
                y = currentNode.posY;
                grid[x, y].state = Map.status.searched;

                openList.Remove(currentNode);
                closedList.Add(currentNode);
                openList.Sort(delegate (Map.MapNode a, Map.MapNode b) { return a.fcost.CompareTo(b.fcost); }); //sorting function so first one on openList is cheapest

                currentNode = openList[0];
            }
        }
        return path;
    }


    public static PathRoute generatePath(List<Map.MapNode> closedList, Map.MapNode[,] grid, Map.MapNode Start, PathRoute Path)
    {
        int index = closedList.Count - 1; //need to work backwards to try and get to the start node
        int PathIndex = 0;
        Map.MapNode a = closedList[index];
        Map.MapNode temp = a;

        Path.PathList[0] = Map.ConvertGridNodeToVector(a);
        ++PathIndex;

        while (temp.id != Start.id)
        {
            Path.PathList[PathIndex] = (Map.ConvertGridNodeToVector(a.parent[0]));
            temp = a.parent[0];
            a = temp;
            ++PathIndex;
        }

        Path.PathSize = PathIndex;
        Path.Index = 0;

        Array.Reverse(Path.PathList, 0, PathIndex);

        return Path;
    }


    public static void Reset(Map.MapNode[,] grid, List<Map.MapNode> openList, List<Map.MapNode> closedList) //reset all nodes so we can do another fresh search
    {
        closedList.Clear();
        openList.Clear();
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                grid[i, j].fcost = 0;
                grid[i, j].gcost = 0;
                grid[i, j].hcost = 0;
                grid[i, j].cameFrom[0] = grid[i, j].cameFrom[0];
                grid[i, j].state = Map.status.untouched;
            }
        }
    }
}

