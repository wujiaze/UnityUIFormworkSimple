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
            CurrentUiType.UiFormsShowMode = UIFormShowMode.HideOther;
            CurrentUiType.UiFormsLucencyType = UIFormLucenyType.Lucency;
            CurrentUiType.UiFormsType = UIFormsType.Normal;
            // 事件注册
            RigisterButtonObjectEvent("BtnMarket", () =>
            {
                ShowUiForm(ProjectConfig.MARKET_FORM);
            });
        }


    }
}