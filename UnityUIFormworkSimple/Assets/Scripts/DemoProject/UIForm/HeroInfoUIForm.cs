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
            CurrentUiType.UIForms_ShowMode = UIFormShowMode.Normal;
            CurrentUiType.UIForms_LucencyType = UIFormLucenyType.Lucency;
            CurrentUiType.UIForms_Type = UIFormsType.Fixed;
            // 注册事件
        }

    }
}