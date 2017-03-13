using UnityEngine;
using System;
using System.Collections.Generic;

namespace GridSystem
{
    public class Grid : MonoBehaviour
    {
        public static Grid instance;
        private static GameObject nodePrefab = Resources.Load("Prefabs/node") as GameObject;

        public int sizeX = 50;
        public int sizeY = 50;
        public float nodeSize = 1;
        public float maxHeight = 1;//地形上最大高度

        private GameObject[,] nodeObjs;
        List<GameObject> curHighLightedNodeObjs;
        int horiCount, vertCount;

        public List<GameObject> path;
        void Start()
        {
            createNodes();
            curHighLightedNodeObjs = null;

            // if the singleton hasn't been initialized yet
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            path.Add(nodeObjs[1, 5]);
            path.Add(nodeObjs[2, 5]);
            path.Add(nodeObjs[2, 4]);
            path.Add(nodeObjs[2, 3]);

        }
        public void hightLightUnitMovable(Vector3 unitPos, int speed)
        {
            GameObject nodeObj = getNodeObjFromPosition(unitPos);
            List<GameObject> movableNodeObjs = getMovableNodeObjs(nodeObj,speed);
            if (!movableNodeObjs.Equals(curHighLightedNodeObjs))
            {
                clear();
                curHighLightedNodeObjs = movableNodeObjs;
                foreach (GameObject node in curHighLightedNodeObjs)
                    node.GetComponent<Node>().changeStatus(NodeStatus.Movable);
            }
        }

        public void clear()
        {
            if(curHighLightedNodeObjs != null)
            {
                foreach (GameObject node in curHighLightedNodeObjs)
                    node.GetComponent<Node>().changeStatus(NodeStatus.Normal);
                curHighLightedNodeObjs = null;
            }
        }
        //void Update()
        //{
        //    RaycastHit hit;
        //    if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 1000, 1<<8))
        //    {
        //        GameObject hitNodeObj = getNodeObjFromPosition(hit.point);

        //        List<GameObject> movableNodeObjs = getMovableNodeObjs(hitNodeObj);

        //        if (hitNodeObj != null && !movableNodeObjs.Equals(curMovableNodeObjs))
        //        {
        //            if (curMovableNodeObjs != null)
        //            {
        //                foreach (GameObject node in curMovableNodeObjs)
        //                {
        //                    node.GetComponent<Node>().changeStatus(NodeStatus.Normal);
        //                }
        //            }
        //            curMovableNodeObjs = movableNodeObjs;

        //            foreach (GameObject node in curMovableNodeObjs)
        //            {
        //                node.GetComponent<Node>().changeStatus(NodeStatus.Movable);
        //            }

        //        }
                 
        //    }
        //}

        private void createNodes()
        {
            horiCount = (int)Math.Floor(sizeX / nodeSize);
            vertCount = (int)Math.Floor(sizeY / nodeSize);
            nodeObjs = new GameObject[horiCount, vertCount];

            float originX = transform.position.x - sizeX / 2 + nodeSize / 2;
            float originZ = transform.position.z - sizeY / 2 + nodeSize / 2; //左下角的起始点
            for (int i = 0; i < horiCount; i++)
            {
                float x = originX + i * nodeSize;
                for (int j = 0; j < vertCount; j++)
                {
                    float z = originZ + j * nodeSize;
                    Vector3 nodeCenter = new Vector3(x, transform.position.y + maxHeight, z);
                    nodeCenter.y += 0.01f;
                    GameObject nodeObj = createNodeObj("Node" + i + j, nodeCenter,i,j);
                    
                    nodeObjs[i, j] = nodeObj;
                }
            }
        }

        private GameObject createNodeObj(string name, Vector3 pos, int x, int y)
        {
            GameObject nodeObj = Instantiate(nodePrefab, pos, Quaternion.Euler(90, 0, 0)) as GameObject;
            nodeObj.transform.localScale = Vector3.one * nodeSize;
            nodeObj.name = name;
            Node node = nodeObj.AddComponent<Node>();
            node.applyToSurface();
            node.x = x;
            node.y = y;
            return nodeObj;
        }

        private GameObject getNodeObjFromPosition(Vector3 position)
        {
            int x = (int)((position.x + sizeX / 2) / nodeSize);
            int y = (int)((position.z + sizeY / 2) / nodeSize);
            if (x < horiCount && y < vertCount)
            {
                return nodeObjs[x, y];
            }
            else
                return null;
        }

        private List<GameObject> getMovableNodeObjs(GameObject startNodeObj, int speed)
        {
            Node startNode = startNodeObj.GetComponent<Node>();
            List<GameObject> discoverdNodeObjs = new List<GameObject>();
            Stack<GameObject> s = new Stack<GameObject>();
            s.Push(startNodeObj);
            while(s.Count!=0)
            {
                GameObject curNodeObj = s.Pop();
                Node curNode = curNodeObj.GetComponent<Node>();
                foreach(GameObject nodeObj in getAdjacentNodeObjs(curNodeObj))
                {
                    Node node = nodeObj.GetComponent<Node>();
                    if(!discoverdNodeObjs.Contains(nodeObj))
                    {
                        int distance = Math.Abs(startNode.x - node.x) + Math.Abs(startNode.y - node.y);
                        if (distance <= speed)
                        {
                            discoverdNodeObjs.Add(nodeObj);
                            s.Push(nodeObj);
                        }
                    }
                }
            }
            return discoverdNodeObjs;
        }

        private List<GameObject> getAdjacentNodeObjs(GameObject nodeObj)
        {
            List<GameObject> adjacentNodes = new List<GameObject>();
            Node node = nodeObj.GetComponent<Node>();

            if (isValidNodePos(node.x, node.y - 1))
                adjacentNodes.Add(nodeObjs[node.x, node.y - 1]);

            if (isValidNodePos(node.x, node.y + 1))
                adjacentNodes.Add(nodeObjs[node.x , node.y + 1]);

            if (isValidNodePos(node.x + 1, node.y))
                adjacentNodes.Add(nodeObjs[node.x + 1, node.y]);

            if (isValidNodePos(node.x - 1, node.y))
                adjacentNodes.Add(nodeObjs[node.x - 1, node.y]);

            return adjacentNodes;

        }
        
        private bool isValidNodePos(int x,  int y)
        {
            return x < horiCount && x >= 0 && y < vertCount && y >= 0;
        }

        
    }
}
