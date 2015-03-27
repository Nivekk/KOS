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
        public GameObject screenGo;
        public CPU Cpu;

        private bool lockState = false;
        private int unlockWait = 0;

        public void Start()
        {
            var prop = internalProp;

            Cpu = part.GetComponent<kOSProcessor>().cpu;

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

            screenGo = mf.gameObject;

            BoxCollider b = screenGo.GetComponent<BoxCollider>() ?? screenGo.AddComponent<BoxCollider>();
            ClickHandler handler = screenGo.GetComponent<ClickHandler>() ?? screenGo.AddComponent<ClickHandler>();
            handler.OnClick = OnMouseDown;
            
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

        public void LockCamera()
        {
            InputManager.Lock(Cpu);
            lockState = true;

            unlockWait = 3;
        }

        public void UnlockCamera()
        {
            InputManager.Unlock();
            lockState = false;
        }

        public void Update()
        {
            foreach (MeshRenderer renderer in internalProp.FindModelComponents<MeshRenderer>())
            {
                renderer.enabled = Cpu.InternalDisplayEnabled;
            }

            if (lockState == true)
            {
                if (unlockWait > 0) unlockWait --;

                if (unlockWait == 0 && Event.current.type == EventType.MouseDown)
                {
                    UnlockCamera();
                }
                else
                { 
                    InternalCamera.Instance.camera.transform.LookAt(screenGo.transform.position, screenGo.transform.up);
                    InternalCamera.Instance.camera.fieldOfView = 18;
                    InternalCamera.Instance.UnlockMouse();
                    FlightCamera.fetch.transform.localRotation = InternalCamera.Instance.camera.transform.localRotation;

                    InputManager.ProcessKeyStrokes();
                }
            }
        }

        public void OnMouseDown()
        {
            if (!lockState) LockCamera(); else UnlockCamera();
        }
    }

    public class ClickHandler : MonoBehaviour
    {
        public Action OnClick;

        public void OnMouseDown()
        {
            OnClick();
        }
    }
}
