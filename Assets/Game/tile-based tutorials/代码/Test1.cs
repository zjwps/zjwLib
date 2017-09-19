using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

namespace TileBaseGame
{
    /// <summary>
    /// http://www.gotoandplay.it/_articles/2004/02/tonypa_p05.php
    /// 用unity来实现下经典的tile-based
    /// </summary>
    public class Test1 : MonoBehaviour
    {
        public UnityEvent unity事件;
        public SpriteAtlas atlas;
        private Game game;
        private void Start()
        {
            game = new Game();
        }
    }
}
