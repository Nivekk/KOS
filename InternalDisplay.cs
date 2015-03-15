using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace kOS
{
    public class kOSInternalDisplay : InternalModule
    {
        public RenderTexture InternalDisplayTexture = new RenderTexture(512, 512, 32);

        public void Start()
        {
            var prop = internalProp;

            var size = 0.1f;
            var m = new Mesh();
            m.name = "Scripted_Plane_New_Mesh";
            m.vertices = new Vector3[] { new Vector3(size, -size, 0.01f), new Vector3(size, size, 0.01f), new Vector3(-size, size, 0.01f), new Vector3(-size, -size, 0.01f) };
            m.uv = new Vector2[] { new Vector2(0, 0.4f), new Vector2(0, 1), new Vector2(0.79f, 1), new Vector2(0.79f, 0.4f) };
            m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            m.RecalculateNormals();

            MeshFilter mf = prop.FindModelComponents<MeshFilter>()[0];
            prop.FindModelComponents<MeshFilter>()[1].mesh = null;
            mf.mesh = m;

            foreach (MeshRenderer renderer in prop.FindModelComponents<MeshRenderer>())
            {
                Material mat = new Material(Shader.Find(" Diffuse"));
                mat.SetTexture("_MainTex", InternalDisplayTexture);
                mat.SetTextureScale("_MainTex", new Vector2(1, 1));

                renderer.material = mat;
            }

            foreach (PartModule module in part.Modules)
            {
                if (module is kOSProcessor)
                {
                    var k = (kOSProcessor)module;

                    k.InternalDisplayTexture = InternalDisplayTexture;
                }
            }
        }

        public void OnMouseDown()
        {
            Debug.Log("OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
        }
    }
}
