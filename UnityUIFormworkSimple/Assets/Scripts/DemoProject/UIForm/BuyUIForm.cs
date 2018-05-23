/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: 
 *		Description:
 *				功能:
 *
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */
namespace SimpleUIFramework
{
    public class BuyUIForm : UIFormBase
    {

        private void Awake()
        {
            CurrentUiType.UIForms_LucencyType = UIFormLucenyType.ImPenetrable;
            CurrentUiType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
            CurrentUiType.UIForms_Type = UIFormsType.PopUp;
            CurrentUiType.IsClearStack = false;
            RigisterButtonObjectEvent("Btn_Close",() => CloseUIForm());
        }
            
    }

}
