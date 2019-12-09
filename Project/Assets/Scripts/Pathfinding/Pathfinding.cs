﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private Grid grid;
    private List<Node> finalPath;
    public static Pathfinding instance;

    void Awake()
    {
        grid = GetComponent<Grid>();
        if (instance != null)
            Debug.LogError("More than one Pathfinding in scene");
        else
            instance = this;
    }

    public void FindPath(Vector3 a_StartPosition, Vector3 a_TargetPosition)
    {
        Debug.Log("Start position: " + a_StartPosition + " / " + a_TargetPosition);
        //a_StartPosition = new Vector3(0, 0, 0);
        //a_TargetPosition = new Vector3(-15, 0, -15);
        Node StartNode = grid.NodeFromWorldPostion(a_StartPosition);
        Node TargetNode = grid.NodeFromWorldPostion(a_TargetPosition);

        Debug.Log("Startnode: " + StartNode.GridX + " / " + StartNode.GridY +  " Startnode Position: " + TargetNode.Position + " /  TargetNode: " + TargetNode.GridX + " / " + TargetNode.GridY +  " Target position: " +  TargetNode.Position);

        List<Node> OpenList = new List<Node>();
        HashSet<Node> ClosedList = new HashSet<Node>();
        OpenList.Add(StartNode);


        while (OpenList.Count > 0)
        {
            Node CurrentNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)//Loop through the open list starting from the second object
            {
                if (OpenList[i].FConst < CurrentNode.FConst || OpenList[i].FConst == CurrentNode.FConst && OpenList[i].HCost < CurrentNode.HCost)//If the f cost of that object is less than or equal to the f cost of the current node
                {
                    CurrentNode = OpenList[i];//Set the current node to that object
                }
            }
            OpenList.Remove(CurrentNode);
            ClosedList.Add(CurrentNode);

            if(CurrentNode == TargetNode)
            {
                GetFinalPath(StartNode, TargetNode);
            }

            foreach (Node NeighborNode in grid.GetNeighboringNodes(CurrentNode))
            {
                if(!NeighborNode.IsWalkable || ClosedList.Contains(NeighborNode))
                {
                    continue;
                }
                int MoveCost = CurrentNode.GCost + GetManhattenDistance(CurrentNode, NeighborNode);

                if (!OpenList.Contains(NeighborNode) || MoveCost < NeighborNode.FConst)
                {
                    NeighborNode.GCost = MoveCost;
                    NeighborNode.HCost = GetManhattenDistance(NeighborNode, TargetNode);
                    NeighborNode.Parent = CurrentNode;

                    if(!OpenList.Contains(NeighborNode))
                    {
                        OpenList.Add(NeighborNode);
                    }
                }
            }
        }
    }

    private void GetFinalPath(Node a_StartingNode, Node a_EndNode)
    {
        List<Node> FinalPath = new List<Node>();
        Node CurrentNode = a_EndNode;

        while(CurrentNode != a_StartingNode)
        {
            FinalPath.Add(CurrentNode);
            CurrentNode = CurrentNode.Parent;
        }

        FinalPath.Reverse();
        finalPath = new List<Node>();
        finalPath = FinalPath;
        grid.FinalPath = FinalPath;
    }


    public List<Node> PublicPath
    {
        get
        {
            return finalPath;
        }
    }

    int GetManhattenDistance(Node a_NodeA, Node a_NodeB)
    {
        int ix = Mathf.Abs(a_NodeA.GridX - a_NodeB.GridX);
        int iy = Mathf.Abs(a_NodeA.GridY - a_NodeB.GridY);

        return ix + iy;
    }
}
