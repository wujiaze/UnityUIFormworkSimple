/***
 * 
 *    Title: "SUIFW" UI框架项目
 *           主题： 资源加载管理器      
 *    Description: 
 *           功能： 本功能是在Unity的Resources类的基础之上，增加了“缓存”的处理。
 *                  
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 *   
 */
using UnityEngine;
using System.Collections;

namespace SimpleUIFramework
{
    public class ResourcesMgr : MonoBehaviour
    {
        /* 字段 */
        private static ResourcesMgr _instance;              //本脚本私有单例实例
        private Hashtable _ht ;                        //容器键值对集合

        /// <summary>
        /// 得到单例
        /// </summary>
        /// <returns></returns>
        public static ResourcesMgr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("_ResourceMgr").AddComponent<ResourcesMgr>();
                }
                return _instance;
            }
        }

        void Awake()
        {
            _ht = new Hashtable();
            DontDestroyOnLoad(this);
        }
        /// <summary>
        /// 加载资源（带对象缓冲技术），并且实例化
        /// </summary>
        /// <param name="path">相对Resources文件夹的路径</param>
        /// <param name="isCatch">是否需要进入缓存</param>
        /// <returns></returns>
        public GameObject LoadAsset(string path, bool isCatch)
        {
            GameObject goObj = LoadResource<GameObject>(path, isCatch);
            GameObject goObjClone = Instantiate(goObj);
            if (goObjClone == null)
            {
                Debug.LogError(GetType() + "/LoadAsset()/克隆资源不成功，请检查。 path=" + path);
            }
            return goObjClone;
        }

        /// <summary>
        /// 加载资源（带对象缓冲技术）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="isCatch"></param>
        /// <returns></returns>
        private T LoadResource<T>(string path, bool isCatch) where T : UnityEngine.Object
        {
            if (_ht.Contains(path))
            {
                return _ht[path] as T;
            }

            T resource = Resources.Load<T>(path);
            if (resource == null)
            {
                Debug.LogError(GetType() + "/Instance()/TResource 提取的资源找不到，请检查。 path=" + path);
            }
            else if (isCatch)
            {
                _ht.Add(path, resource);
            }
            return resource;
        }


    }
}