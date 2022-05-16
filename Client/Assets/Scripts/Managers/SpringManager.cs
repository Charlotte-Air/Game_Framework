using UnityEngine;
using System.Collections.Generic;

namespace UnityChan
{
	public class SpringManager : MonoBehaviour
	{
		public float dynamicRatio = 1.0f;
		public float			stiffnessForce;
		public float			dragForce;
		private List<SpringBone> springBones;
        public float HorizonForce = 1;
        public float VerticalForce = 1;
        public int UpdatePerSec = 100;
        public Vector3 springForce = new Vector3(0, -0.0001f, 0);
        [System.NonSerialized]
        public Vector3 HorizonDeltaMove;
        private Vector3 LastPos;
        private float UpdateSecAcc = 0;
        private Transform MyTrans;
        // 用来稳定帧率，避免两帧之间的更新速度变化太快
        private float lastUpdateCount = 0;
        private bool isHero = false;
        private float fps = 100;

#if UNITY_EDITOR
        void Awake() 
        {
            ResetSpringBone(false);
        }
#endif

        public void ResetSpringBone(bool isHero) {
            springBones = new List<SpringBone>();

            this.isHero = isHero;

            // 非主角降低更新频率
            UpdatePerSec = isHero ? 100 : 30;

            if (transform != null) {
                transform.GetComponentsInChildren<SpringBone>(false, springBones);
                UpdateParameters();

                MyTrans = transform;
                LastPos = MyTrans.position;
            }
        }

		void Start ()
		{
            UpdatePerSec = isHero ? 100: 30;
            UpdateParameters ();
            MyTrans = transform;
            LastPos = MyTrans.position;
        }
#if UNITY_EDITOR
        void Update ()
		{

		//Kobayashi
		if(dynamicRatio >= 1.0f)
			dynamicRatio = 1.0f;
		else if(dynamicRatio <= 0.0f)
			dynamicRatio = 0.0f;
        }
#endif

        private void LateUpdate ()
		{
            fps = (fps * 9 + 1 / Time.deltaTime) / 10;
            //拟合出来的曲线
            springForce.y = -10 * Mathf.Pow(fps, -2.5f);

            HorizonDeltaMove = MyTrans.position - LastPos;
            LastPos = MyTrans.position;

            //Kobayashi
            if (dynamicRatio != 0.0f && springBones != null) {
                for (int i = 0; i < springBones.Count; i++) {
                    if (dynamicRatio > springBones[i].threshold) {
                springBones[i].ApplyRootMotion();
                springBones[i].UpdateSpring();
                    }
                }
            }
		}

		private void UpdateParameters ()
		{
            if (springBones != null && springBones.Count > 0) {
                for (int i = 0; i < springBones.Count; i++) {
                    //Kobayashi
                    if (!springBones[i].isUseEachBoneForceSettings) {
                        springBones[i].stiffnessForce = stiffnessForce;
                        springBones[i].dragForce = dragForce;
                    }
                }
            }
        }
	}
}