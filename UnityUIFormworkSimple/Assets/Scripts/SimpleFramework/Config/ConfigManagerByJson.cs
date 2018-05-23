/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: 基于Json文件的 配置管理器
 *		Description:
 *				功能:
 *
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */

using System.Collections.Generic;
using UnityEngine;
namespace SimpleUIFramework
{
	public class ConfigManagerByJson : IConfigManager
	{
        // 保存（键值对）应用设置集合
	    private static Dictionary<string, string> _AppSetting;
        /// <summary>
        /// 只读属性：得到应用设置
        /// </summary>
        public Dictionary<string, string> AppSetting
        {
            get { return _AppSetting; }
        }

	    public ConfigManagerByJson(string jsonPath)
	    {
	        _AppSetting =new Dictionary<string, string>();
            // 初始化解析 Json 数据，加载到集合
	        InitAndAnalysisJson(jsonPath);
	    }
        /// <summary>
        /// 得到AppSetting的最大数值
        /// </summary>
        /// <returns></returns>
        public int GetAppSettingMaxNumber()
	    {
	        if (_AppSetting != null && _AppSetting.Count > 0)
	        {
	            return _AppSetting.Count;
	        }
	        return 0;
	    }

        /// <summary>
        /// 初始化解析 Json 数据，加载到 _AppSetting 字典
        /// </summary>
        /// <param name="jsonPath"></param>
	    private void InitAndAnalysisJson(string jsonPath)
	    {
	        TextAsset configInfo = null;
	        KeyValueInfo keyValueInfo = null;
	        // 参数检查
            if (string.IsNullOrEmpty(jsonPath))
                return;
            try
	        {
	            configInfo = Resources.Load<TextAsset>(jsonPath); // config文件在 Resources中
	            keyValueInfo = JsonUtility.FromJson<KeyValueInfo>(configInfo.text); 
	        }
	        catch 
	        {
	           // 自定义异常
               throw new JsonAnalysisException(GetType()+ "/InitAndAnalysisJson()/ Exception! jsonpath ="+ jsonPath);
	        }
            // 数据加载到 _AppSetting 字典中
            foreach (KeyValueNode nodeInfo in keyValueInfo.ConfigInfo)
	        {
	            _AppSetting.Add(nodeInfo.Key, nodeInfo.Value);
            }
	    }

	}
}