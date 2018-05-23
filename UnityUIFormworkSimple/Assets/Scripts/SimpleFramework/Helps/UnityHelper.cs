/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: Unity 帮助脚本
 *		Description:
 *				功能: 提供程序用户一些常用的功能方法实现，方便开发
 *
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */
using UnityEngine;
namespace SimpleUIFramework
{
	public class UnityHelper : MonoBehaviour 
	{
        /// <summary>
        /// 层级视图查找子对象的方法
        /// 要求：每一个对象都要有唯一的名称
        /// </summary>
        /// <param name="goParent">父对象</param>
        /// <param name="childName">查找的子对象</param>
        /// <returns></returns>
        public static Transform FindTheChildNode(GameObject goParent,string childName)
	    {
	        Transform searchTrans = null;
            searchTrans = goParent.transform.Find(childName);
	        if (searchTrans == null)
	        {
	            foreach (Transform trans in goParent.transform)
	            {
	                searchTrans = FindTheChildNode(trans.gameObject, childName);
	                if (searchTrans!=null)
	                {
	                    return searchTrans;
	                }
	            }
	        }
	        return searchTrans;
	    }

        /// <summary>
        /// 获取子节点(对象)脚本
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="goParent">父对象</param>
        /// <param name="childName">子对象</param>
        /// <returns></returns>
        public static T GetTheChildNodeComopnetScripts<T>(GameObject goParent, string childName) where T:class 
        {
            Transform searchTransformNode = null;
            searchTransformNode = FindTheChildNode(goParent, childName);
            if (searchTransformNode != null)
            {
                return searchTransformNode.gameObject.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 给子节点添加脚本(继承MonoBehaviour 的脚本)
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="goParent">父对象</param>
        /// <param name="childName">子对象</param>
        /// <returns></returns>
	    public static T AddTheChildNodeComopnetScripts<T>(GameObject goParent, string childName) where T : Component 
	    {
	        Transform searchTransformNode = null;
	        searchTransformNode = FindTheChildNode(goParent, childName);
	        if (searchTransformNode != null)
	        {
                // 如果已有相同的脚本，则先删除
	            T[] CompnentScriptsArray = searchTransformNode.GetComponents<T>();
	            for (int i = 0; i < CompnentScriptsArray.Length; i++)
	            {
	                Destroy(CompnentScriptsArray[i]); 
                }
                return searchTransformNode.gameObject.AddComponent<T>();
	        }
	        return null;
	    }

        /// <summary>
        /// 给子节点添加父对象
        /// </summary>
        /// <param name="parent">父对象</param>
        /// <param name="child">子节点</param>
	    public static void AddChildNodeToParentNode(Transform parent,Transform child)
	    {
	        child.SetParent(parent,false);
	        child.localPosition = Vector3.zero;
            child.localScale = Vector3.zero;
            child.localEulerAngles =Vector3.zero;
	    }

	}
}