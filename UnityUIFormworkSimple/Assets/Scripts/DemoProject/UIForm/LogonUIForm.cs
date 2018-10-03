/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: 登录窗体
 *		Description:
 *				功能:
 *
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */

using System;
using SimpleUIFramework;
using UnityEngine.UI;


namespace DemoProject
{
  
    public class LogonUIForm : UIFormBase
	{
        public Dropdown LanguageDropdown;
	    public Text label;
	    public Text btnlabel;
	    public void Awake()
	    {
	        // 定义本窗体的性质(默认值可以不写)
	        CurrentUiType.UiFormsShowMode = UIFormShowMode.Normal;
	        CurrentUiType.UiFormsType = UIFormsType.Normal;
	        CurrentUiType.UiFormsLucencyType = UIFormLucenyType.Lucency;
            CurrentUiType.IsClearStack = false;
            // 给按钮注册事件：登入游戏
            RigisterButtonObjectEvent("Btn_OK", LogonSys);
            // 给下拉框注册事件 
	        RigisterDropDownObjectEvent("DropLanguage", LanguageIndex);
	    }

	    private void LogonSys()
	    {
	        // 前台或后台检测用户名与密码 
	        // 如果成功，则登录
	        ShowUiForm(ProjectConfig.SELECT_HERO_FORM);
	    }


	    private void LanguageIndex(int type)
	    {
            SetLanguageType((LanguageType)type);
        }

        private void SetText() {
            label.text = ShowText("LogonSystem");
            btnlabel.text = ShowText("Logon");
        }

        private void Start()
	    {
            // 初始化默认为英文
	        LanguageDropdown.value = ProjectConfig.SYS_LANGUAGE;
            SetLanguageType((LanguageType)LanguageDropdown.value);
        }

        private void Update()
        {
            SetText();
        }




    }
}