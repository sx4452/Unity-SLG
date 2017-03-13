using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GridSystem
{
    public class Node : MonoBehaviour
    {
        private static Texture movableTex = Resources.Load("Textures/nodeMovable") as Texture;
        private static Texture dangerousTex = Resources.Load("Textures/nodeDangerous") as Texture;
        private static Texture normalTex = Resources.Load("Textures/nodeNormal") as Texture;

        public int x, y;
        private Renderer renderer;
        private NodeStatus status;
        public NodeStatus Status
        {
            get { return status; }
        }

        void Awake()
        {
            renderer = GetComponent<Renderer>();
            status = NodeStatus.Normal;
        }
        public void changeStatus(NodeStatus nodeStatus)
        {
            switch (nodeStatus)
            {
                case NodeStatus.Movable:
                    renderer.material.mainTexture = movableTex;
                    status = NodeStatus.Movable;
                    break;
                case NodeStatus.Normal:
                    renderer.material.mainTexture = normalTex;
                    status = NodeStatus.Normal;
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
        Obstacle
    }
}

