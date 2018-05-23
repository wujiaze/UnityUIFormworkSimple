/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: 窗体类型
 *		Description:
 *				功能: 
 *
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */
using UnityEngine;
namespace SimpleUIFramework
{
    public class UIType
    {
        // UI窗体位置类型
        public UIFormsType UIForms_Type = UIFormsType.Normal;
        // UI窗体显示类型
        public UIFormShowMode UIForms_ShowMode = UIFormShowMode.Normal;
        // UI窗体透明度类型
        public UIFormLucenyType UIForms_LucencyType = UIFormLucenyType.Lucency;
        // 是否清空 “栈集合”，直接关闭所有的【反向切换】窗体
        public bool IsClearStack = false;
    }
}