using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public static class UtilsMethods
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

        /// <summary>
        /// Simple function to get conditional signed value.
        /// </summary>
        /// <param name="val">Value to be returned</param>
        /// <param name="condition">Condition to be checked</param>
        /// <returns>Given value if condition is met or reversed value if condition is not met.</returns>
        public static float ConditionalSignedValue(this float val, bool condition)
        {
            return condition ? val : -val;
        }
        
        /*public static void SetCatMaterialColors(SpriteRenderer spriteRenderer)
        {
            // red -> nos
            // blue -> fur
            // green -> eyes
            spriteRenderer.sprite = catSprite;
            var material = spriteRenderer.material;
            material.SetColor("_Red", catNoseColor);
            material.SetColor("_Blue", catColor);
            material.SetColor("_Green", catEyeColor);
        }
        public static void SetCatMaterialColors(Image image)
        {
            // red -> nos
            // blue -> fur
            // green -> eyes
            image.sprite = catSprite;
            var material = Instantiate(image.material);
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

            material.SetColor("_Red", catNoseColor);
            material.SetColor("_Blue", catColor);
            material.SetColor("_Green", catEyeColor);
            image.material = material;
        }*/
    }
}