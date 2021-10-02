using UnityEngine;

namespace DefaultNamespace.LevelGenerator
{
    [CreateAssetMenu(fileName = "New Modular Terrain", menuName = "Terrain/Modular Terrain", order = 0)]
    public class ModularTerrain : ScriptableObject
    {
        public GameObject terrain;
        public Vector2 offset;
        public float length;
    }
}
