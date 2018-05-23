/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: 主城窗体
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
	public class MainCityUIForm : UIFormBase
    {

        private void Awake()
        {
            // 窗体性质
            CurrentUiType.UIForms_ShowMode = UIFormShowMode.HideOther;
            CurrentUiType.UIForms_LucencyType = UIFormLucenyType.Lucency;
            CurrentUiType.UIForms_Type = UIFormsType.Normal;
            // 事件注册
            RigisterButtonObjectEvent("BtnMarket", () =>
            {
                OpenUIForm(ProjectConfig.MARKET_FORM);
            });
        }


    }
}