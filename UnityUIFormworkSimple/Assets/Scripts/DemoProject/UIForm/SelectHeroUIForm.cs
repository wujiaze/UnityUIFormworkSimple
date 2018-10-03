/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: RPG 游戏 “选择角色” 窗体 
 *			    
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
    public class SelectHeroUIForm : UIFormBase
    {
        public void Awake()
        {
            // 定义本窗体的性质(默认值可以不写)
            CurrentUiType.UiFormsType = UIFormsType.Normal;
            CurrentUiType.UiFormsShowMode = UIFormShowMode.HideOther;
            CurrentUiType.UiFormsLucencyType = UIFormLucenyType.Lucency;
            CurrentUiType.IsClearStack = false;
            // 给按钮注册事件:进入主程
            RigisterButtonObjectEvent("BtnConfirm", () =>
            {
                // 顺序不能换
                ShowUiForm(ProjectConfig.MAIN_CITY_FORM); // 因为这个是Hideother属性
                ShowUiForm(ProjectConfig.HERO_INFO_FORM);
            });
            // 给按钮注册事件：返回登录
            RigisterButtonObjectEvent("BtnClose", () => CloseUiForm());
        }
    }
}