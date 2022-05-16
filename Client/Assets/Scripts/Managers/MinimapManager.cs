using Models;
using UnityEngine;

namespace Managers
{   
    /// <summary>
    /// 小地图管理器
    /// </summary>
    class MinimapManager : Singleton<MinimapManager>
    {
        /// <summary>
        /// 小地图
        /// </summary>
        public UIMinimap minimap;

        public Collider minimapBoundingBox;

        /// <summary>
        /// 加载小地图
        /// </summary>
        public Sprite LoadCurrentMinimap()
        {
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.MiniMap);
        }

        /// <summary>
        /// 更新小地图
        /// </summary>
        /// <param name="minimapBoundingBox">边界盒</param>
        public void UpdateMinimap(Collider minimapBoundingBox)
        {
            this.minimapBoundingBox = minimapBoundingBox;
            if (this.minimap != null)
            {
                this.minimap.UpdateMap();
            }
        }
    }
}
