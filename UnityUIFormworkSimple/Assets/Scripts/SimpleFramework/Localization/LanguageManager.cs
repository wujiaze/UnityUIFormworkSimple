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
        Cn, // 中文
        En  // 英文
    }
    public class LanguageManager
    {
        private static LanguageManager _instance;
        private static LanguageType _currentType = LanguageType.Cn;
        private static Dictionary<string, string> _dicLanguageCache;
        private LanguageManager()
        {
            _dicLanguageCache = new Dictionary<string, string>();
        }

        /// <summary>
        /// 得到本类的实例
        /// </summary>
        /// <returns></returns>
        public static LanguageManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LanguageManager();
                    _instance.SetLanguageType(_currentType);
                }
                return _instance;

            }
        }

        /// <summary>
        /// 设置语言类型
        /// </summary>
        /// <param name="type"></param>
        public void SetLanguageType(LanguageType type)
        {
            if (_currentType != type)
            {
                InitLanguageCache(type);
                _currentType = type;
            }
        }

        /// <summary>
        /// 初始化语言缓存集合
        /// </summary>
        private void InitLanguageCache(LanguageType type)
        {
            string LanguagePath = null;
            switch (type)
            {
                case LanguageType.Cn:
                    LanguagePath = UIFrameSysDefine.PATH_LANGUAGE_CN;
                    break;
                case LanguageType.En:
                    LanguagePath = UIFrameSysDefine.PATH_LANGUAGE_EN;
                    break;
            }
            IConfigManager config = new ConfigManagerByJson(LanguagePath);
            if (config != null)
            {
                _dicLanguageCache.Clear();
                _dicLanguageCache = config.AppSetting;
            }
        }


        /// <summary>
        /// 根据ID显示文本信息
        /// </summary>
        /// <param name="languageId"></param>
        /// <returns></returns>
        public string ShowCurrentText(string languageId)
        {
            if (string.IsNullOrEmpty(languageId))
                return null;
            if (_dicLanguageCache != null && _dicLanguageCache.Count >= 1)
            {
                string strQueryResult;
                _dicLanguageCache.TryGetValue(languageId, out strQueryResult);
                if (!string.IsNullOrEmpty(strQueryResult))
                    return strQueryResult;
            }
            return null;
        }

    }
}