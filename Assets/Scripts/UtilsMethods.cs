using UnityEngine;

namespace DefaultNamespace
{
    public class UtilsMethods
    {
        public static Vector2 GetMousePosition()
        {
            var mouseRawPos = Input.mousePosition;
            return Camera.main.ScreenToWorldPoint(mouseRawPos);
        }
        
        public static void LookAtMouse(Transform objTransform)
        {
            var angleDeg = GetAngleToMouse(objTransform);
            objTransform.rotation = Quaternion.Euler(0,0,angleDeg);
        }

        public static float GetAngleToMouse(Transform objTransform, bool isRad = false)
        {
            var mousePos = GetMousePosition();
            var playerPos = objTransform.position;
            var x = mousePos.x - playerPos.x;
            var y = mousePos.y - playerPos.y;
            var angleRad = Mathf.Atan2(y, x);
            return isRad ? angleRad - Mathf.Deg2Rad * 90 : (180 / Mathf.PI) * angleRad - 90;
        }
    }
}