using UnityEngine;

namespace ScriptableObjects.Obstacles {
    [CreateAssetMenu(menuName = "ScriptableObjects/Obstacles/ObstaclesParams")]
    public class ObstaclesParamsSO : ScriptableObject {
        [SerializeField] private int jumpOverScore;
        [SerializeField] private int destroyScore;
        [SerializeField] private Sprite obstacleSprite;

        public int JumpOverScore => jumpOverScore;
        public int DestroyScore => destroyScore;
        public Sprite ObstacleSprite => obstacleSprite;
    }
}
