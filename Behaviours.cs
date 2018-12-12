using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateInfo;
public class behaviours
{
    static public PathInfo.PathRoute Roaming(Rigidbody rb, Map.MapNode[,] Map, PathInfo.PathRoute path, List<Vector3> validpath, List<Map.MapNode> openList, List<Map.MapNode> closedList, bool seenEnemy, GameObject Enemy)
    {
        //this behaviour scouts around the areas until they find an enemy. Once they have they will continue to attack them until the enemy is dead or themselves are dead

        if (!seenEnemy)
        {
            if (path.Index == path.PathSize)
            {
                var seed = (int)System.DateTime.Now.Ticks;
                System.Random rnd = new System.Random(seed);

                int ran = rnd.Next(1, validpath.Count - 1);


                path = AStarSearch.Search(Map, rb.transform.position, validpath[ran], path, openList, closedList);
            }
        }

        else if(seenEnemy)
        {
            if (path.Index == path.PathSize)
            {
                if (Enemy == null)
                {
                    seenEnemy = false;
                }
                else
                path = AStarSearch.Search(Map, rb.transform.position, Enemy.transform.position, path, openList, closedList);
            }     
        }
        return path;
    }

    static public Vector2 Seek(Rigidbody rb, Vector2 targetPos)
    {
        Vector2 position = rb.transform.position;
        Vector2 velocity = rb.velocity;

        velocity.Normalize();

        Vector2 desiredVelocity = (targetPos - position);
        desiredVelocity.Normalize();
        Vector2 steering = desiredVelocity - velocity;


        float angle = Vector2.Angle(rb.velocity.normalized, rb.transform.forward);
        if (rb.velocity.normalized.x < rb.transform.forward.x)
        {
            angle *= 1;
        }
        angle = (angle + 180f) % 360.0f;

        rb.transform.eulerAngles = new Vector3(0, 0, angle);

        float dist = Vector2.Distance(position, targetPos);

        return steering;
    }

    static public PathInfo.PathRoute CaptureFlag(Map.MapNode[,] grid, Vector3 position, Vector3 objPos, Vector3 ourFlag, PathInfo.PathRoute path, bool hasFlag, List<Map.MapNode> openList, List<Map.MapNode> closedList, string Team, Rigidbody rb, bool goingToSafeSpot, GameObject player)
    {

        if (hasFlag == false)
        {
            if (path.Index == path.PathSize)
            {
                float dist = Vector3.Distance(position, objPos); //has the flag already been taken, if so go back to base
                if (dist > 1.0f)
                {
                    path = AStarSearch.Search(grid, position, objPos, path, openList, closedList);
                }
                else
                {
                    path = returnToFlag(grid, position, ourFlag, path, openList, closedList);
                }
                
            }
        }

        else if (hasFlag == true) //if we have the flag we need to check if we are back at the flag and if we are then we need to find a nice position near where our flag and wait for it to be returned
        {
            if (goingToSafeSpot == false)
            {
                if (path.Index == path.PathSize)
                {
                    float distance = Vector2.Distance(position, ourFlag);
                    if (distance > 1.0f)
                    {
                        path = returnToFlag(grid, position, ourFlag, path, openList, closedList);
                    }

                    else // we are near our flag, is it there?
                    {
                        distance = Vector2.Distance(ourFlag, GameObject.Find(Team + "Flag").transform.position);
                        if (distance > 1.0f) //someone has our flag, so find a nice spot
                        {
                            if (goingToSafeSpot == false)
                                findSafePosition(path, grid, position, ourFlag, openList, closedList, rb, goingToSafeSpot, player, Team);
                        }
                    }
                }
            }
            else if (goingToSafeSpot)
            {
                float DistAgentAndSafePos;
                if (path.PathSize > 0)
                {
                    DistAgentAndSafePos = Vector3.Distance(position, path.PathList[path.PathSize - 1]); //dist agent and safe spot
                }

                else { DistAgentAndSafePos = 0.001f; } //temp fix so we dont get an index error
                if (DistAgentAndSafePos < 0.1f)
                {
                    DistAgentAndSafePos = Vector3.Distance(ourFlag, GameObject.Find(Team + "Flag").transform.position); //if we are in a safe location is our flag back
                    if (DistAgentAndSafePos < 0.1f)
                    {
                        player.GetComponent<AI>().AtSafeSpot = false;
                        var temp = player.GetComponent<AI>().Board.getBoard();
                        temp.seek = true;
                        player.GetComponent<AI>().Board.setBoard(temp);
                        player.GetComponent<AI>().resetPath();
                    }
                    else
                    {
                            Stop(rb);
                            var temp = player.GetComponent<AI>().Board.getBoard();
                            temp.seek = false;
                            player.GetComponent<AI>().Board.setBoard(temp);              
                    }
                }
                else if(path.Index == path.PathSize)
                {
                    player.GetComponent<AI>().AtSafeSpot = false;
                    var temp = player.GetComponent<AI>().Board.getBoard();
                    temp.seek = true;
                    player.GetComponent<AI>().Board.setBoard(temp);
                    path = returnToFlag(grid, position, ourFlag, path, openList, closedList);
                }
            }
        } 
        return path;
    }

    static public PathInfo.PathRoute findSafePosition(PathInfo.PathRoute path, Map.MapNode[,] grid, Vector3 position, Vector3 ourflag, List<Map.MapNode> openList, List<Map.MapNode> closedList, Rigidbody rb, bool goingToSafeSpot,GameObject Player, string TeamColour)
    {
        //for this behaviour we need to find a good position to hold

        //we need to do a raycast to each direction and see what is the furthest

        
        //float rayDistance = 0.0f;
        if (goingToSafeSpot == false)
        {
            if (path.Index == path.PathSize)
            {
                if (TeamColour == "Red")
                {
                    Vector3 safe = new Vector3(4.75f, -17.75f, -1.0f);
                    path = AStarSearch.Search(grid, Player.transform.position, safe, path, openList, closedList);
                }
                else if (TeamColour == "Blue")
                {
                    Vector3 safe = new Vector3(25.75f, -12.75f, -1.0f);
                    path = AStarSearch.Search(grid, Player.transform.position, safe, path, openList, closedList);
                }
                Player.GetComponent<AI>().AtSafeSpot = true;


               // Was going to try and raycast to the next safe location, but for a easy cheap fix just hardcoded the locations for now


                //Ray[] rays = new Ray[4];
                //rays[0] = new Ray(ourflag, Vector3.up * 30);
                //rays[1] = new Ray(ourflag, Vector3.left * 30);
                //rays[2] = new Ray(ourflag, Vector3.right * 30);
                //rays[3] = new Ray(ourflag, Vector3.down * 30);

                //Vector3[] direction = new Vector3[4];
                //direction[0] = (Vector3.up);
                //direction[1] = (Vector3.left);
                //direction[2] = (Vector3.right);
                //direction[3] = (Vector3.down);

                //RaycastHit hit;
                //Vector3 bestReyPos = new Vector3(0, 0, 0);
                //Vector3 bestDirection = new Vector3(0,0,0);
                //for (int i = 0; i < 4; i++)
                //{
                //    if (Physics.Raycast(rays[i], out hit))
                //    {
                //        if (hit.distance > rayDistance)
                //        {
                //            rayDistance = hit.distance;
                //            bestReyPos = hit.point;
                //            bestDirection = direction[i];
                //        }
                //    }
                //}

                ////once we have found the furthest away. Then we will check for the closest wall. This should be a nice corner to sit until the flag is back.

                

                //Debug.DrawLine(ourflag, bestReyPos - bestDirection, Color.red, 10.0f);

                //Vector3 flagToBestRay = bestReyPos;
                //float shortestDist = 1000.0f;

                //rays[0] = new Ray(bestReyPos, Vector3.up * 30);
                //rays[1] = new Ray(bestReyPos, Vector3.left * 30);
                //rays[2] = new Ray(bestReyPos, Vector3.right * 30);
                //rays[3] = new Ray(bestReyPos, Vector3.down * 30);
                //for (int i = 0; i < 4; i++)
                //{
                //    if (Physics.Raycast(rays[i], out hit))
                //    {
                //        if (hit.distance > 0.50f)
                //        {
                //            if (hit.distance < shortestDist)
                //            {
                //                shortestDist = hit.distance;
                //                bestReyPos = hit.point;
                //                bestDirection = direction[i];
                //               // Debug.DrawLine(flagToBestRay, bestReyPos, Color.black, 10.0f);
                //            }
                //        }
                //    }
                //}
                //Debug.DrawLine(flagToBestRay, bestReyPos, Color.red, 10.0f);

                //Vector3 dest = bestReyPos - bestDirection;
                //float dist = Vector3.Distance(position, bestReyPos);
                //if (dist >= 3.0f)
                //{
                //    path = AStarSearch.search(grid, position, dest, path, openList, closedList);
                //    Player.GetComponent<AI>().goingToSafeSpot = true;
                //}       
            }

           
        }
        return path;
    }
    static public PathInfo.PathRoute returnToFlag(Map.MapNode[,] grid, Vector3 playerPos, Vector3 ourFlag, PathInfo.PathRoute path, List<Map.MapNode> openList, List<Map.MapNode> closedList)
    {

        if (path.Index == path.PathSize)
            {
            float distToOurFlag = Vector2.Distance(playerPos, ourFlag);
            if (distToOurFlag > 1.0f)
            {
                path = AStarSearch.Search(grid, playerPos, ourFlag,path,openList,closedList);
            }
        }
       
        return path;
    }
    static public PathInfo.PathRoute defending(Vector3 pos, PathInfo.PathRoute path, StateMachine<AI> owner, string TeamColour, Map.MapNode[,] grid, Vector3 obj, Vector3 ourFlag, List<Map.MapNode> openList, List<Map.MapNode> closedList,Rigidbody rb,bool inSafeSpot, GameObject player)
    {
        //this behaviour is for the tank, it will go around the flag to protect it. If it's taken will change state and go and try and get it back.
       
        if (path.Index == path.PathSize)
            {
            float distToOurFlag = Vector2.Distance(pos, ourFlag);

            if (distToOurFlag > 3.0f)
            {
                path = returnToFlag(grid, pos, ourFlag, path,openList,closedList);
            }
            else if (distToOurFlag <= 3.0f)
            {
                float dist = Vector2.Distance(GameObject.Find(TeamColour + "Flag").transform.position, ourFlag);
                if (dist > 3.0f)
                {
                    path = CaptureFlag(grid, pos, obj, ourFlag, path,false,openList,closedList,TeamColour,rb, inSafeSpot, player);
                }

                if (dist <= 3.0f) //we still have our flag so defend it
                {
                    path.PathList[0] = (new Vector3(ourFlag.x, ourFlag.y + 1));
                    path.PathList[1] = (new Vector3(ourFlag.x + 1, ourFlag.y + 1));
                    path.PathList[2] = (new Vector3(ourFlag.x + 1, ourFlag.y));
                    path.PathList[3] = (new Vector3(ourFlag.x + 1, ourFlag.y - 1));
                    path.PathList[4] = (new Vector3(ourFlag.x, ourFlag.y - 1));
                    path.PathList[5] = (new Vector3(ourFlag.x - 1, ourFlag.y - 1));
                    path.PathList[6] = (new Vector3(ourFlag.x - 1, ourFlag.y));
                    path.PathList[7] = (new Vector3(ourFlag.x - 1, ourFlag.y + 1));
                    path.PathList[8] = (new Vector3(ourFlag.x, ourFlag.y + 1));
                    path.Index = 0;
                    path.PathSize = 8;
                }
            }
       
        }       
        return path;
    }

    static public PathInfo.PathRoute shooting(PathInfo.PathRoute path)
    {
        return path;
        
    }

    static public PathInfo.PathRoute Escort(Map.MapNode[,] grid,PathInfo.PathRoute path, Vector3 playerPos, GameObject personToEscort, Blackboard board,List<Map.MapNode>openList, List<Map.MapNode> closedList, Rigidbody rb)
    {
        //we will need to get the agents path and calculate where they might be by the time we can get to their position.

       
        if (path.Index == path.PathSize)
        {
            var temp = personToEscort.GetComponent<AI>().Path;
            Vector3[] EscortsPath = temp.PathList;
            int pathSize = temp.PathSize;

            Vector3 playerEscortPos = personToEscort.transform.position;
            float dist = Vector2.Distance(playerPos, playerEscortPos);
            float halfDist = dist / 2;

            int bestIndex = 0;
            float tempDif = Mathf.Abs(Vector2.Distance(playerPos, EscortsPath[0]) - halfDist);

            for (int i = 1; i < pathSize; i++)
            {
                float pathDist = Vector2.Distance(playerPos, EscortsPath[i]);
                float dif = Mathf.Abs(pathDist - halfDist);
                if (dif < tempDif)
                {
                    bestIndex = i;
                    tempDif = dif;
                }
            }

            if (dist < 3.0f)
            {
                var b = board.getBoard();
                b.seek = false;
                b.stop = true;
                board.setBoard(b);
            }
            else
            {
                var b = board.getBoard();
                b.seek = true;
                b.stop = false;
                board.setBoard(b);
                path = AStarSearch.Search(grid, playerPos, EscortsPath[bestIndex], path,openList,closedList);
            }
        }
        return path;
    }

    static public PathInfo.PathRoute needAmmo(Map.MapNode[,] grid, PathInfo.PathRoute path, Vector3 playerPos, Vector3 ourFlag, List<Map.MapNode> openList, List<Map.MapNode> closedList)
    {
        if (path.Index == path.PathSize) //we need to calculate which ammo box is the 'safest'
        {
            List<Vector3> ammoBox = new List<Vector3>();
            var detectedObjects = GameObject.FindGameObjectsWithTag("AmmoBox");

            foreach(var detectedObject in detectedObjects)
            {
                if(detectedObject.GetComponent<Renderer>().enabled)
                ammoBox.Add(detectedObject.transform.position);
            }

            int bestAmmoBox = 0;
            float DistFromFlag = 300.0f;
            float DistToNearestBox = 300.0f;
            float tempDistFromFlag = 0.0f;
            float tempDistToNearestBox = 0.0f;
            for (int i = 0; i < ammoBox.Count; i++)
            {
                tempDistFromFlag = Vector3.Distance(playerPos, ourFlag);
                tempDistToNearestBox = Vector3.Distance(playerPos, ammoBox[i]);
                if ((tempDistFromFlag + tempDistToNearestBox) < (DistFromFlag + DistToNearestBox))
                {
                    DistFromFlag = tempDistFromFlag;
                    DistToNearestBox = tempDistToNearestBox;
                    bestAmmoBox = i;
                }
            }
            path = AStarSearch.Search(grid, playerPos, ammoBox[bestAmmoBox],path,openList,closedList);
        }
        return path;
    }

    static public PathInfo.PathRoute NeedHealth(Map.MapNode[,] grid, PathInfo.PathRoute path, Vector3 playerPos, Vector3 ourFlag, List<Map.MapNode> openList, List<Map.MapNode> closedList)
    {
        if (path.Index == path.PathSize) //we need to calculate which healthpack 'safest'
        {
            List<Vector3> healthPack = new List<Vector3>();
            var detectedObjects = GameObject.FindGameObjectsWithTag("HealthPack");

            foreach (var detectedObject in detectedObjects)
            {
                if (detectedObject.GetComponent<Renderer>().enabled == true) // need to make sure it's active
                {
                    healthPack.Add(detectedObject.transform.position);
                }      
            }

            if (healthPack.Count == 0) //if one isnt avaiable. Go to one anyway once we have got there we'll recalculate to actually collect one
            {
                return path = AStarSearch.Search(grid,playerPos,GameObject.Find("HealthPack").transform.position,path,openList,closedList);
            }

            int bestHealthPack = 0;
            float DistFromFlag = 300.0f;
            float DistToNearestBox = 300.0f;
            float tempDistFromFlag = 0.0f;
            float tempDistToNearestBox = 0.0f;
            for (int i = 0; i < healthPack.Count; i++)
            {
                tempDistFromFlag = Vector2.Distance(playerPos, ourFlag);
                tempDistToNearestBox = Vector2.Distance(playerPos, healthPack[i]);
                if ((tempDistFromFlag + tempDistToNearestBox) < (DistFromFlag + DistToNearestBox))
                {
                    DistFromFlag = tempDistFromFlag;
                    DistToNearestBox = tempDistToNearestBox;
                    bestHealthPack = i;
                }
            }
            path = AStarSearch.Search(grid, playerPos, healthPack[bestHealthPack],path,openList,closedList);
        }
        return path;
    }

    static public void Stop(Rigidbody a)
    {
        a.velocity = Vector3.zero;
      a.transform.rotation = Quaternion.Euler(Vector3.zero);
      a.angularVelocity = Vector3.zero;
    }

    static public void DoWeHaveFlag(List<GameObject> Team, StateMachine<AI> stateMachine, GameObject player, Messager mess)
    {
        for (int i = 0; i < Team.Count; i++)
        {
            if (player.name != Team[i].name)
            {
                mess.SendMessage(player, 3); //check if we have the flag
            }
        }
    }

}

    

