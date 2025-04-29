#region Using statements

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Bitgem.VFX.StylisedWater
{
    public class WateverVolumeFloater : MonoBehaviour
    {
        #region Public fields

        public WaterVolumeHelper WaterVolumeHelper = null;
        public float splashStrength = 0.5f;
        private Vector3 lastPos;

        #endregion

        #region MonoBehaviour events

        void Update()
        {
            var instance = WaterVolumeHelper ? WaterVolumeHelper : WaterVolumeHelper.Instance;
            if (!instance) return;

            // Apply floating
            float? height = instance.GetHeight(transform.position);
            if (height.HasValue)
            {
                transform.position = new Vector3(transform.position.x, height.Value, transform.position.z);
            }

            // Basic water disturbance prototype
            if ((transform.position - lastPos).magnitude > 0.1f)
            {
                CreateRipple(transform.position, splashStrength);
            }

            lastPos = transform.position;
        }

        void CreateRipple(Vector3 pos, float strength)
        {
            // Custom function to inject ripple/wave - could modify shader param, buffer, etc.
            Shader.SetGlobalVector("_RipplePosition", pos);
            Shader.SetGlobalFloat("_RippleStrength", strength);
        }

        #endregion
    }
}