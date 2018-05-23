/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: 通用配置管理器接口
 *		Description:
 *				功能: 基于“键值对”配置文件的通用解析
 *
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */

using System;
using System.Collections.Generic;
namespace SimpleUIFramework
{
	public interface IConfigManager 
	{
        /// <summary>
        /// 应用设置
        /// 功能：得到键值对集合数据
        /// </summary>
	    Dictionary<string, string> AppSetting { get; }
        /// <summary>
        /// 得到配置文件（AppSetting）最大的数量
        /// </summary>
        /// <returns></returns>
	    int GetAppSettingMaxNumber();
	}

    [Serializable]
    internal class KeyValueInfo
    {
        public List<KeyValueNode> ConfigInfo = null;
    }
    [Serializable]
    internal class KeyValueNode
    {
        // 键
        public string Key = null;
        // 值
        public string Value = null;
    }

}