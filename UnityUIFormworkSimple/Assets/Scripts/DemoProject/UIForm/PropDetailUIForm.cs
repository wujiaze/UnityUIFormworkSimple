﻿/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: 道具详细信息窗体
 *		Description:
 *				功能: 显示各种道具详细信息
 *
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */

using SimpleUIFramework;
using UnityEngine.UI;

namespace DemoProject
{
	public class PropDetailUIForm : UIFormBase
	{
	    public Text TxtName; // 窗体显示名称
	    void Awake()
	    {
	        CurrentUiType.UIForms_LucencyType = UIFormLucenyType.Translucence;
	        CurrentUiType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
	        CurrentUiType.UIForms_Type = UIFormsType.PopUp;
            CurrentUiType.IsClearStack = false;
            /* 按钮注册 */
            RigisterButtonObjectEvent("Btn_Close", () => CloseUIForm());
            RigisterButtonObjectEvent("Btn_Buy", () => { OpenUIForm("BuyUIForm"); });
            /* 接收信息：注册成监听者 */
            ReceiveMessage("Props", (kv) =>
	        {
	            if (TxtName != null)
	            {
	                TxtName.text = kv.Value.ToString();
	            }
	        });
                
	    }
	}
}