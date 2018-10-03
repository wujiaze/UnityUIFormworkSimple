/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: 英雄信息窗体
 *		Description:
 *				功能:
 *
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */

using SimpleUIFramework;
namespace DemoProject
{
	public class HeroInfoUIForm : UIFormBase 
	{

        private void Awake()
        {
            // 窗体性质
            CurrentUiType.UiFormsShowMode = UIFormShowMode.Normal;
            CurrentUiType.UiFormsLucencyType = UIFormLucenyType.Lucency;
            CurrentUiType.UiFormsType = UIFormsType.Fixed;
            // 注册事件
        }

    }
}