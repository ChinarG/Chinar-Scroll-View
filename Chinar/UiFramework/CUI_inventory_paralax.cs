using UnityEngine;



namespace Chinar.UiFramework
{
    public class CUI_inventory_paralax : MonoBehaviour
    {
        [SerializeField] private Transform closeViewCanvas;   //近相机视角画布
        [SerializeField] private Transform farViewCanvas;     //远相机视角画布
        private                  Vector3   initCvcV3;         //初始近画布位置
        private                  Vector3   initFvcV3;         //初始远画布位置
        public                   float     Displacement = 50; //位移


        void Start()
        {
            initCvcV3 = closeViewCanvas.position;
            initFvcV3 = farViewCanvas.position;
        }


        void Update()
        {
            closeViewCanvas.position = closeViewCanvas.position.ModifyX(initCvcV3.x + Input.mousePosition.x.Remap(0, Screen.width, -Displacement, Displacement));
            farViewCanvas.position   = farViewCanvas.position.ModifyX(initFvcV3.x   - Input.mousePosition.x.Remap(0, Screen.width, -Displacement, Displacement));
            closeViewCanvas.position = closeViewCanvas.position.ModifyY(initCvcV3.y + Input.mousePosition.y.Remap(0, Screen.height, -Displacement, Displacement) /** (Screen.height / Screen.width)*/);
            farViewCanvas.position   = farViewCanvas.position.ModifyY(initFvcV3.y   - Input.mousePosition.y.Remap(0, Screen.height, -Displacement, Displacement) /** (Screen.height / Screen.width)*/);
        }
    }

    /// <summary>
    /// 静态类扩展
    /// </summary>
    public static  class ChinarAPIExtension
    {
        public static Vector3 ModifyX(this Vector3 trans, float newVal)
        {
            trans = new Vector3(newVal, trans.y, trans.z);
            return trans;
        }


        public static Vector3 ModifyY(this Vector3 trans, float newVal)
        {
            trans = new Vector3(trans.x, newVal, trans.z);
            return trans;
        }


        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}