using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using sept.gameFramework.debug;

namespace sept.gameFramework.mesh
{
    public class Mesh
    {
        private int vertexBufferIndex;
        private int elementBufferIndex;

        private List<float> vertexData = new List<float>();
        private List<uint> triangleData = new List<uint>();

        public uint numVertices { get { return (uint)(vertexData.Count / 3); } }
        public uint numTriangles { get { return (uint)(triangleData.Count / 3); } }

        public uint addVertex(Vector3 position)
        {
            vertexData.Add(position.X);
            vertexData.Add(position.Y);
            vertexData.Add(position.Z);
            return numVertices-1;
        }

        public uint addVertex(float x, float y, float z)
        {
            vertexData.Add(x);
            vertexData.Add(y);
            vertexData.Add(z);
            return numVertices - 1;
        }        

        public uint addTriangle(uint v0, uint v1, uint v2)
        {
            triangleData.Add(v0);
            triangleData.Add(v1);
            triangleData.Add(v2);
            return numTriangles - 1;
        }

        /// <summary> 
        /// Uploads the geometry data to the graphics card’s memory
        /// </summary>         
        public void upload()
        {
            vertexBufferIndex = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferIndex);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexData.Count * sizeof(float)), vertexData.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            OpenGLHelper.checkError();

            elementBufferIndex = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferIndex);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(triangleData.Count * sizeof(uint)), triangleData.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void prepareDraw()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferIndex);
        }

        public void draw()
        {                        
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferIndex);
            GL.DrawElements(BeginMode.Triangles, triangleData.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

    }
}
