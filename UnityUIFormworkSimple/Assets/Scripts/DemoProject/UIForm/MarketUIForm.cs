/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: 商城窗体
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
	public class MarketUIForm : UIFormBase 
	{

        private void Awake()
        {
            // 窗体性质
            CurrentUiType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
            CurrentUiType.UIForms_LucencyType = UIFormLucenyType.ImPenetrable;
            CurrentUiType.UIForms_Type = UIFormsType.PopUp;
            // 注册事件
            // 关闭按钮事件
            RigisterButtonObjectEvent("Btn_Close", () =>
            {
                CloseUIForm();
            });
            // 注册道具事件：衣服按钮
            RigisterButtonObjectEvent("Btn_Cloth", () =>
            {
                OpenUIForm(ProjectConfig.PROP_DETAIL_FORM);
                // 传递数据
                MySendMessage("Props", "cloth", "这是一件衣服");
            });
            // 注册道具事件：裤子按钮
            RigisterButtonObjectEvent("Btn_Path", () =>
            {
                OpenUIForm(ProjectConfig.PROP_DETAIL_FORM);
                // 传递数据
                MySendMessage("Props", "path", "这是一条裤子");
            });
            // 注册道具事件：鞋子按钮
            RigisterButtonObjectEvent("Btn_Shoes", () =>
            {
                OpenUIForm(ProjectConfig.PROP_DETAIL_FORM);
                // 传递数据
                MySendMessage("Props", "shoes", "这是一双鞋子");
            });
            // 注册道具事件：神杖按钮
            RigisterButtonObjectEvent("Btn_Ticket", () =>
            {
                OpenUIForm(ProjectConfig.PROP_DETAIL_FORM);
                // 传递数据
                MySendMessage("Props", "ticket", "这是一根神杖");
            });
        }


    }
}