using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Character;
namespace GridSystem
{
    public class Node : MonoBehaviour
    {
        private static Texture movableTex;
        private static Texture dangerousTex;
        private static Texture normalTex;

        [HideInInspector]
        public int x, y;

        [HideInInspector]
        public int gCost, hCost;//gCost离起点的距离，hCost离终点的距离

        [HideInInspector]
        public Node parent;

        [HideInInspector]
        public Unit unit;

        private Renderer renderer1;
        private NodeStatus status;
        public NodeStatus Status
        {
            get { return status; }
        }

        public int fCost
        {
            get { return gCost + hCost; }
        }

        void Awake()
        {
            if(movableTex==null)
                movableTex = Resources.Load("Textures/nodeMovable") as Texture;
            if (dangerousTex == null)
                dangerousTex = Resources.Load("Textures/nodeDangerous") as Texture;
            if (normalTex == null)
                normalTex = Resources.Load("Textures/nodeNormal") as Texture;


            renderer1 = GetComponent<Renderer>();
            status = NodeStatus.Normal;
        }
        public void changeStatus(NodeStatus nodeStatus)
        {
            switch (nodeStatus)
            {
                case NodeStatus.Movable:
                    renderer1.material.mainTexture = movableTex;
                    status = NodeStatus.Movable;
                    break;
                case NodeStatus.Normal:
                    renderer1.material.mainTexture = normalTex;
                    status = NodeStatus.Normal;
                    break;
                case NodeStatus.Obstacle:
                    renderer1.material.mainTexture = normalTex;
                    status = NodeStatus.Obstacle;
                    break;
                case NodeStatus.Occupied:
                    renderer1.material.mainTexture = normalTex;
                    status = NodeStatus.Occupied;
                    break;
                case NodeStatus.Attackable:
                    renderer1.material.mainTexture = dangerousTex;
                    status = NodeStatus.Attackable;
                    break;
            }
        }

        public void applyToSurface()
        {
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                RaycastHit hit;
                Vector3 verticeWorld = transform.TransformPoint(vertices[i]);

                verticeWorld -= new Vector3(0, 0.001f, 0);

                if (Physics.Raycast(verticeWorld, Vector3.down, out hit, 100))
                {
                    verticeWorld.y = hit.point.y + 0.0001f;
                }
                vertices[i] = transform.InverseTransformPoint(verticeWorld);
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();

            //重置碰撞体
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }
    }

    public enum NodeStatus
    {
        Movable,
        Occupied,
        Normal,
        Obstacle,
        Attackable
    }
}

