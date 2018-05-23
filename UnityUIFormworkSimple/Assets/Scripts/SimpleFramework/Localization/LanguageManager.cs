/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题:  语言国际化
 *		Description:
 *				功能: 是的我们的语言根据不同国家，选择语言
 *              使用方法：在继承UIFormBase的类中，直接使用 SetLanguageType 和 ShowText 方法即可
 *              使用前提：制作对应的Json语言文件
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
    public enum LanguageType
    {
        CN, // 中文
        EN  // 英文
    }
    public class LanguageManager
    {
        private static LanguageManager _Instance;
        private static LanguageType currentType = LanguageType.CN;
        private static Dictionary<string, string> _DicLanguageCache;
        private LanguageManager()
        {
            _DicLanguageCache =new Dictionary<string, string>();
        }

        /// <summary>
        /// 得到本类的实例
        /// </summary>
        /// <returns></returns>
        public static LanguageManager GetInstance() 
        {
            if (_Instance == null)
            {
                _Instance = new LanguageManager();
                _Instance.SetLanguageType(currentType);
            }
            return _Instance;
        }

        /// <summary>
        /// 设置语言类型
        /// </summary>
        /// <param name="type"></param>
        public void SetLanguageType(LanguageType type) {
            if (currentType != type)
            {
                InitLanguageCache(type);
                currentType = type;
            }
        }

        /// <summary>
        /// 初始化语言缓存集合
        /// </summary>
        /// <param name="LanguagePath">语言json文件路径</param>
        private  void InitLanguageCache(LanguageType type)
        {
            string LanguagePath = null;
            switch (type)
            {
                case LanguageType.CN:
                    LanguagePath = UIFrameSysDefine.PATH_LANGUAGE_CN;
                    break;
                case LanguageType.EN:
                    LanguagePath = UIFrameSysDefine.PATH_LANGUAGE_EN;
                    break;
            }
            IConfigManager config = new ConfigManagerByJson(LanguagePath);
            if (config != null)
            {
                _DicLanguageCache.Clear();
                _DicLanguageCache = config.AppSetting;
            }
        }


        /// <summary>
        /// 根据ID显示文本信息
        /// </summary>
        /// <param name="languageID"></param>
        /// <returns></returns>
        public string ShowCurrentText(string languageID)
        {
            if (string.IsNullOrEmpty(languageID)) return null;
            if (_DicLanguageCache!= null && _DicLanguageCache.Count >= 1)
            {
                string strQueryResult = String.Empty;
                _DicLanguageCache.TryGetValue(languageID, out strQueryResult);
                if (!string.IsNullOrEmpty(strQueryResult))
                    return strQueryResult;
            }
            return null;
        }

    }
}