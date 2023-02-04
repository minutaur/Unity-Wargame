using UnityEngine;
using UnityEngine.Serialization;

namespace WeaponSystem
{
    [CreateAssetMenu(fileName = "Trail Config", menuName = "FPS/Trail Configuration", order = 4)]
    public class TrailConfigScriptable : ScriptableObject
    {
        public Material material;
        public AnimationCurve widthCurve;
        public float duration = .5f;
        public float minVertexDistance = .1f;
        public Gradient color;

        public float missDistance = 100f;
        public float simulationSpeed = 100f;
    }
}