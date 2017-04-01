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
        List<GameObject> curAttackableNodeObjs;
        int horiCount, vertCount;

        private int NodeLayer = 8;
        private int UnitLayer = 9;

        public LayerMask ObstacleLayerMask;
        public LayerMask unitLayerMask;

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

            GameInput.OnClick += OnClick;

        }

        void Destory()
        {
            GameInput.OnClick -= OnClick;
        }

        public void hightLightUnitMovable()
        {
            clear();
            //GameObject nodeObj = getNodeObjFromPosition(unitPos);
            List<GameObject> movableNodeObjs = getMovableNodeObjs(GameManager.selectedUnitNodeObj, GameManager.selectedUnit.speed);
            if (!movableNodeObjs.Equals(curHighLightedNodeObjs))
            {
                curHighLightedNodeObjs = movableNodeObjs;
                foreach (GameObject node in curHighLightedNodeObjs)
                    node.GetComponent<Node>().changeStatus(NodeStatus.Movable);
            }
        }

        private void OnClick(Vector2 mousePos)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePos), out hit, 1000))
            {
                if(hit.collider.gameObject.layer == NodeLayer)
                {
                    GameObject hitNodeObj = getNodeObjFromPosition(hit.point);

                    if (GameManager.selectedUnitNodeObj != null && hitNodeObj != GameManager.selectedUnitNodeObj)
                    {
                        Node hitNode = hitNodeObj.GetComponent<Node>();
                        if (hitNode.Status == NodeStatus.Movable)
                        {
                            List<GameObject> path = getShortestPath(GameManager.selectedUnitNodeObj, hitNodeObj);
                            StartCoroutine(GameManager.selectedUnit.move(path));
                        }
                        clear();
                    }
                }
            }
        }

        private List<GameObject> getShortestPath(GameObject fromNodeObj, GameObject toNodeObj)
        {
            Node startNode = fromNodeObj.GetComponent<Node>();
            Node targetNode = toNodeObj.GetComponent<Node>();

            HashSet<Node> closedSet = new HashSet<Node>();
            List<Node> openSet = new List<Node>();
            openSet.Add(startNode);
            while (openSet.Count != 0)
            {
                Node currentNode = openSet[0];
                for(int i = 1; i < openSet.Count; i++)
                {
                    if(openSet[i].fCost < currentNode.fCost
                        || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);


                //结束
                if(currentNode.Equals(targetNode))
                {
                    List<GameObject> path = new List<GameObject>();
                    Node currNode = targetNode;
                    while(!currNode.Equals(startNode))
                    {
                        path.Add(currNode.gameObject);
                        currNode = currNode.parent;
                    }
                    path.Add(startNode.gameObject);
                    path.Reverse();
                    return optimizePath(path);
                }

                foreach (GameObject neighbourObj in getAdjacentMovableNodeObjs(currentNode.gameObject))
                {
                    Node neighbour = neighbourObj.GetComponent<Node>();
                    if (neighbour.Status != NodeStatus.Movable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    int movementCostToNeighbour = currentNode.gCost + 1;
                    if(movementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = movementCostToNeighbour;
                        neighbour.hCost = getNodeDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if(!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
            return null;

        }

        private int getNodeDistance(Node fromNode, Node toNode)
        {
            return Math.Abs(toNode.x - fromNode.x) + Math.Abs(toNode.y - fromNode.y);
        }

        public void clear()
        {
            if(curHighLightedNodeObjs != null)
            {
                foreach (GameObject node in curHighLightedNodeObjs)
                    node.GetComponent<Node>().changeStatus(NodeStatus.Normal);
                curHighLightedNodeObjs = null;
            }
            if(curAttackableNodeObjs != null)
            {
                foreach (GameObject node in curAttackableNodeObjs)
                    node.GetComponent<Node>().changeStatus(NodeStatus.Occupied);
                curAttackableNodeObjs = null;
            }

            
            //selectedUnitNodeObj = null;
            //selectedUnit = null;
        }

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

                    GameObject nodeObj = createNodeObj("Node" + i + j, nodeCenter, i, j);
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
            nodeObj.transform.parent = transform;

            //检查node位置是否有障碍物
            if (Physics.CheckSphere(pos, 0.5f, ObstacleLayerMask))
            {
                node.changeStatus(NodeStatus.Obstacle);
            }
            return nodeObj;
        }

        public GameObject getNodeObjFromPosition(Vector3 position)
        {
            int x = (int)((position.x + sizeX / 2 - transform.position.x) / nodeSize);
            int y = (int)((position.z + sizeY / 2 - transform.position.z) / nodeSize);
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
            startNode.gCost = 0;
            s.Push(startNodeObj);
            while(s.Count!=0)
            {
                GameObject curNodeObj = s.Pop();
                Node curNode = curNodeObj.GetComponent<Node>();
                foreach (GameObject nodeObj in getAdjacentMovableNodeObjs(curNodeObj))
                {
                    Node node = nodeObj.GetComponent<Node>();
                    if(!discoverdNodeObjs.Contains(nodeObj))
                    {
                        int moveCostToNodeObj = curNode.gCost + 1;
                        //int distance = Math.Abs(startNode.x - node.x) + Math.Abs(startNode.y - node.y);
                        if (moveCostToNodeObj <= speed)
                        {
                            node.gCost = moveCostToNodeObj;
                            discoverdNodeObjs.Add(nodeObj);
                            s.Push(nodeObj);
                        }
                    }
                }
            }
            return discoverdNodeObjs;
        }

        /// <summary>
        /// 用于查找移动范围
        /// </summary>
        /// <param name="nodeObj"></param>
        /// <returns></returns>
        private List<GameObject> getAdjacentMovableNodeObjs(GameObject nodeObj)
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
        
        /// <summary>
        /// 排除在范围外和有障碍物的node
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool isValidNodePos(int x,  int y)
        {

            if (x >= horiCount || x < 0 || y >= vertCount || y < 0)
                return false;

            Node node = nodeObjs[x, y].GetComponent<Node>();
            if (node.Status == NodeStatus.Obstacle || node.Status == NodeStatus.Occupied)
                return false;
            else
                return true;
        }
        
        private List<GameObject> optimizePath(List<GameObject> path)
        {
            List<GameObject> optimizedPath = new List<GameObject>();
            optimizedPath.Add(path[0]);
            Vector2 curDir = getDirectionBetweenNodeObjs(path[0], path[1]);
            for (int i = 1; i < path.Count - 1; i++)
            {
                Vector2 nextDir = getDirectionBetweenNodeObjs(path[i], path[i+1]);
                if(!nextDir.Equals(curDir))
                {
                    optimizedPath.Add(path[i]);
                    curDir = nextDir;
                }
            }
            optimizedPath.Add(path[path.Count - 1]);
            return optimizedPath;
        }

        private Vector2 getDirectionBetweenNodeObjs(GameObject nodeObj1, GameObject nodeObj2)
        {
            Node node1 = nodeObj1.GetComponent<Node>();
            Node node2 = nodeObj2.GetComponent<Node>();
            return new Vector2(node2.x - node1.x, node2.y - node1.y).normalized;
        }
        
        public void setNodeStatus(GameObject nodeObj,NodeStatus status)
        {
            Node node = nodeObj.GetComponent<Node>();
            node.changeStatus(status);
        }

        public List<GameObject> getAttackableNodeObjs()
        {
            curAttackableNodeObjs = new List<GameObject>();
            GameObject currentNodeObj = getNodeObjFromPosition(GameManager.selectedUnit.transform.position);
            Node startNode = currentNodeObj.GetComponent<Node>();
            List<GameObject> closedSet = new List<GameObject>();
            Stack<GameObject> s = new Stack<GameObject>();
            startNode.gCost = 0;

            s.Push(currentNodeObj);
            while (s.Count != 0)
            {
                GameObject curNodeObj = s.Pop();
                Node curNode = curNodeObj.GetComponent<Node>();
                foreach (GameObject nodeObj in getAdjacentNodeObjs(curNodeObj))
                {
                    Node node = nodeObj.GetComponent<Node>();
                    if (!closedSet.Contains(nodeObj))
                    {
                        int CostToNodeObj = curNode.gCost + 1;
                        //int distance = Math.Abs(startNode.x - node.x) + Math.Abs(startNode.y - node.y);
                        if (CostToNodeObj <= GameManager.selectedUnit.attackRange)
                        {
                            node.gCost = CostToNodeObj;
                            closedSet.Add(nodeObj);
                            if (isAttackableNode(node.x, node.y))
                            {
                                setNodeStatus(nodeObj, NodeStatus.Attackable);
                                curAttackableNodeObjs.Add(nodeObj);
                            }
                            s.Push(nodeObj);
                        }
                    }
                }
            }


            

            //if (isAttackableNode(node.x, node.y - 1))
            //{
            //    curAttackableNodeObjs.Add(nodeObjs[node.x, node.y - 1]);
            //    setNodeStatus(nodeObjs[node.x, node.y - 1], NodeStatus.Attackable);
            //}

            //if (isAttackableNode(node.x, node.y + 1))
            //{
            //    curAttackableNodeObjs.Add(nodeObjs[node.x, node.y + 1]);
            //    setNodeStatus(nodeObjs[node.x, node.y + 1], NodeStatus.Attackable);
            //}

            //if (isAttackableNode(node.x + 1, node.y))
            //{
            //    curAttackableNodeObjs.Add(nodeObjs[node.x + 1, node.y]);
            //    setNodeStatus(nodeObjs[node.x + 1, node.y], NodeStatus.Attackable);
            //}

            //if (isAttackableNode(node.x - 1, node.y))
            //{
            //    curAttackableNodeObjs.Add(nodeObjs[node.x - 1, node.y]);
            //    setNodeStatus(nodeObjs[node.x - 1, node.y], NodeStatus.Attackable);
            //}

            return curAttackableNodeObjs;
        }

        private List<GameObject> getAdjacentNodeObjs(GameObject nodeObj)
        {
            List<GameObject> adjacentNodes = new List<GameObject>();
            Node node = nodeObj.GetComponent<Node>();

            if (isNodePosInRange(node.x, node.y - 1))
                adjacentNodes.Add(nodeObjs[node.x, node.y - 1]);
            if (isNodePosInRange(node.x, node.y + 1))
                adjacentNodes.Add(nodeObjs[node.x, node.y + 1]);
            if (isNodePosInRange(node.x + 1, node.y))
                adjacentNodes.Add(nodeObjs[node.x + 1, node.y]);
            if (isNodePosInRange(node.x - 1, node.y))
                adjacentNodes.Add(nodeObjs[node.x - 1, node.y]);

            return adjacentNodes;

        }

        private bool isNodePosInRange(int x, int y)
        {
            return x < horiCount && x >= 0 && y < vertCount && y >= 0;
        }

        private bool isAttackableNode(int x, int y)
        {
            if (x >= horiCount || x < 0 || y >= vertCount || y < 0)
                return false;

            Node node = nodeObjs[x, y].GetComponent<Node>();
            if(node.Status == NodeStatus.Occupied)
            {
                RaycastHit hit;
                Vector3 raycastPosition = node.transform.position;
                raycastPosition.y -= 2;//raycast起点放低点，不然在unit中间，hit不到

                if (Physics.Raycast(raycastPosition,Vector3.up, out hit,1000, unitLayerMask))
                {
                    Unit hitUnit = hit.collider.gameObject.GetComponent<Unit>();
                    if (hitUnit.Team != GameManager.selectedUnit.Team)
                        return true;
                }
            }
            return false;
        }

    }
}
