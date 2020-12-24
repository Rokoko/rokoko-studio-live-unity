using Rokoko.Helper;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rokoko
{
    [ExecuteInEditMode]
    public class TPoseGuideGameComponent : MonoBehaviour
    {
        public GameObject followTarget;
        public Vector3 followOffset = Vector3.zero;

        private void Awake()
        {
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

#if !UNITY_EDITOR
            RokokoHelper.Destroy(this.gameObject);
#endif
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
#if UNITY_EDITOR
            RokokoHelper.Destroy(this.gameObject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
#endif
        }

        // Update is called once per frame
        void Update()
        {
            this.transform.rotation = Quaternion.LookRotation(Vector3.up * -1);

            if(followTarget != null)
            {
                this.transform.position = followTarget.transform.position + followOffset;
            }
        }
    }
}