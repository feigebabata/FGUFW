using UnityEngine;

namespace FGUFW
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class DynamicCube : MonoBehaviour
    {
        public Vector3 Size = new Vector3(1,1,1);
        public Vector3 Pivot = new Vector3(0.5f,0.5f,0.5f);
        
        
        private Mesh mesh;
        private MeshFilter meshFilter;
        
        void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
        }
        
        void OnValidate()
        {
            UpdateMeshData();
        }

        public void UpdateMeshData()
        {
            if(mesh==default)
            {
                mesh = new Mesh();
                mesh.name = "DynamicCube";

                meshFilter.mesh = mesh;
            }

            var centerOffset = new Vector3(-Size.x*Pivot.x , -Size.y*Pivot.y , -Size.z*Pivot.z);

            // 定义8个顶点的本地位置（相对于中心点）
            Vector3 halfSize = Size * 0.5f;
            
            // 8个角点相对于中心点的位置
            // Vector3[] corners = new Vector3[8]
            // {
            //     new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), // 0: 左下后
            //     new Vector3( halfSize.x, -halfSize.y, -halfSize.z), // 1: 右下后
            //     new Vector3( halfSize.x,  halfSize.y, -halfSize.z), // 2: 右上后
            //     new Vector3(-halfSize.x,  halfSize.y, -halfSize.z), // 3: 左上后
            //     new Vector3(-halfSize.x, -halfSize.y,  halfSize.z), // 4: 左下前
            //     new Vector3( halfSize.x, -halfSize.y,  halfSize.z), // 5: 右下前
            //     new Vector3( halfSize.x,  halfSize.y,  halfSize.z), // 6: 右上前
            //     new Vector3(-halfSize.x,  halfSize.y,  halfSize.z)  // 7: 左上前
            // };

            Vector3[] corners = new Vector3[8]
            {
                new Vector3(0, 0, 0), // 0: 左下后
                new Vector3( Size.x, 0, 0), // 1: 右下后
                new Vector3( Size.x,  Size.y, 0), // 2: 右上后
                new Vector3(0,  Size.y, 0), // 3: 左上后
                new Vector3(0, 0,  Size.z), // 4: 左下前
                new Vector3( Size.x, 0,  Size.z), // 5: 右下前
                new Vector3( Size.x,  Size.y,  Size.z), // 6: 右上前
                new Vector3(0,  Size.y,  Size.z)  // 7: 左上前
            };
            
            // 定义6个面的顶点索引（每个面4个顶点）
            int[][] faceIndices = new int[6][]
            {
                new int[] { 3, 2, 6, 7 }, // 前面 (+Z)
                new int[] { 1, 0, 4, 5 }, // 后面 (-Z)
                new int[] { 0, 3, 7, 4 }, // 左面 (-X)
                new int[] { 2, 1, 5, 6 }, // 右面 (+X)
                new int[] { 3, 0, 1, 2 }, // 上面 (+Y)
                new int[] { 4, 7, 6, 5 }  // 下面 (-Y)
            };
            
            // 定义每个面的法线方向
            Vector3[] faceNormals = new Vector3[6]
            {
                Vector3.forward,   // 前
                Vector3.back,      // 后
                Vector3.left,      // 左
                Vector3.right,     // 右
                Vector3.up,        // 上
                Vector3.down       // 下
            };
            
            // 构建顶点数组
            Vector3[] vertices = new Vector3[24];
            Vector3[] normals = new Vector3[24];
            Vector2[] uv = new Vector2[24];
            
            for (int face = 0; face < 6; face++)
            {
                int startIndex = face * 4;
                
                // 获取该面的4个角点索引
                int v0 = faceIndices[face][0];
                int v1 = faceIndices[face][1];
                int v2 = faceIndices[face][2];
                int v3 = faceIndices[face][3];
                
                // 设置顶点位置（加上中心点偏移）
                vertices[startIndex]     = corners[v0] + centerOffset;
                vertices[startIndex + 1] = corners[v1] + centerOffset;
                vertices[startIndex + 2] = corners[v2] + centerOffset;
                vertices[startIndex + 3] = corners[v3] + centerOffset;
                
                // 设置法线
                normals[startIndex]     = faceNormals[face];
                normals[startIndex + 1] = faceNormals[face];
                normals[startIndex + 2] = faceNormals[face];
                normals[startIndex + 3] = faceNormals[face];
                
                // 设置UV坐标
                uv[startIndex]     = new Vector2(0, 0);
                uv[startIndex + 1] = new Vector2(1, 0);
                uv[startIndex + 2] = new Vector2(1, 1);
                uv[startIndex + 3] = new Vector2(0, 1);
            }
            
            // 构建三角形索引
            int[] triangles = new int[36];
            for (int face = 0; face < 6; face++)
            {
                int startIndex = face * 6;
                int vertexStart = face * 4;
                
                // 第一个三角形
                triangles[startIndex]     = vertexStart;
                triangles[startIndex + 1] = vertexStart + 1;
                triangles[startIndex + 2] = vertexStart + 2;
                
                // 第二个三角形
                triangles[startIndex + 3] = vertexStart;
                triangles[startIndex + 4] = vertexStart + 2;
                triangles[startIndex + 5] = vertexStart + 3;
            }
            
            // 应用数据到网格
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.triangles = triangles;
            
            
            // 重新计算边界
            mesh.RecalculateBounds();
        }

    }
}