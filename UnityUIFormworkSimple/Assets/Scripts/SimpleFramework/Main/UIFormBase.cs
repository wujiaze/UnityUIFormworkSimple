/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: UI窗体的父类
 *		Description:
 *				功能: 定义四个生命周期
 *              1、Display   显示状态
 *              2、Hiding    隐藏状态
 *              3、ReDisplay 再显示状态
 *              4、Freeze    冻结状态
 *              
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SimpleUIFramework
{
    // 基础UI窗体
	public class UIFormBase : MonoBehaviour 
	{
        /*字段*/
        private UIType _uiType =new UIType();
        /*属性*/
        // 当前UI窗体类型
        public UIType CurrentUiType
	    {
	        get { return _uiType; }
	    }

        #region 窗体生命周期（状态）

	    /// <summary>
	    /// 显示状态
	    /// </summary>
	    public virtual void Display()
	    {
	        gameObject.SetActive(true);
            // 设置模态窗体调用（必须是弹出窗体）
	        if (_uiType.UiFormsType == UIFormsType.PopUp)
	        {
	            UiMaskManager.Instance.SetMaskForm(this.gameObject, _uiType.UiFormsLucencyType);
	        }
	    }

	    /// <summary>
	    /// 隐藏状态
	    /// </summary>
	    public virtual void Hiding()
	    {
	        gameObject.SetActive(false);
	        // 设置模态窗体调用（必须是弹出窗体）
	        if (_uiType.UiFormsType == UIFormsType.PopUp)
	        {
	            UiMaskManager.Instance.CancelMaskForm(this.gameObject);
	        }
        }

	    /// <summary>
	    /// 再显示状态
	    /// </summary>
	    public virtual void ReDisplay()
	    {
	        gameObject.SetActive(true);
	        // 设置模态窗体调用（必须是弹出窗体）
	        if (_uiType.UiFormsType == UIFormsType.PopUp)
	        {
	            UiMaskManager.Instance.SetMaskForm(this.gameObject, _uiType.UiFormsLucencyType);
	        }
        }
	    /// <summary>
	    /// 冻结状态
	    /// </summary>
	    public virtual void Freeze()
	    {
	        gameObject.SetActive(true);
	    }
        #endregion



        #region 封装子类常用的方法

       
        /// <summary>
        /// 打开UI窗体
        /// </summary>
        /// <param name="uiFormName">窗体名称</param>
	    protected void ShowUiForm(string uiFormName)
	    {
	        UIManager.Instance.ShowUiForm(uiFormName);
	    }

        /// <summary>
        /// 关闭当前UI窗体
        /// 因为关闭是关闭自己，所以可以用反射来获取自己的类名，而类名又和窗体名称一致，所以可以这样取巧
        /// </summary>
	    protected void CloseUiForm()
        {
            UIManager.Instance.CloseUiForm(GetType().Name);
        }

        /* 有需啊哟还可以自行添加 */
       
        /// <summary>
        /// 注册按钮事件,简化了寻找路径
        /// </summary>
        /// <param name="buttonName">按钮名称</param>
        /// <param name="delHandle">委托事件</param>
	    protected void RigisterButtonObjectEvent(string buttonName, UnityAction delHandle)
        {
            Button btn = UnityHelper.FindTheChildNode(this.gameObject, buttonName).GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(delHandle);
            }
        }

        /// <summary>
        /// 注册下拉框事件
        /// </summary>
        /// <param name="dropName">按钮名称</param>
        /// <param name="delHandle">委托事件</param>
        protected void RigisterDropDownObjectEvent(string dropName, UnityAction<int> delHandle)
        {
            Dropdown drop = UnityHelper.FindTheChildNode(this.gameObject, dropName).GetComponent<Dropdown>();
            if (drop != null)
            {
                drop.onValueChanged.AddListener(delHandle);
            }
        }



        /* 语言国际化 */
        protected LanguageManager mgr = LanguageManager.Instance;
        /// <summary>
        /// 显示语言 
        /// </summary>
        /// <param name="languageId">语言的ID</param>
        /// <returns></returns>
	    protected string ShowText(string languageId)
        {
            string strResult;
	        strResult = mgr.ShowCurrentText(languageId);
	        return strResult;
	    }

        /// <summary>
        /// 设置语言类型
        /// </summary>
        /// <param name="type"></param>
        protected void SetLanguageType(LanguageType type)
	    {
            mgr.SetLanguageType(type);
	    }


        /* 不使用MVC框架时，用来传递UI之间的消息 */
        /// <summary>
        /// 发送消息方法 
        /// </summary>
        /// <param name="msgType">消息的类型</param>
        /// <param name="msgName">消息的名字</param>
        /// <param name="msgContent">消息的内容</param>
        protected void MySendMessage(string msgType, string msgName, object msgContent)
        {
            KeyValueUpdate kv = new KeyValueUpdate(msgName, msgContent);
            MessageCenter.SendMessage(msgType, kv);
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="msgType">消息的类型</param>
        /// <param name="handler">具体的委托方法</param>
	    protected void ReceiveMessage(string msgType, MessageCenter.DelMessageDelivery handler)
        {
            MessageCenter.AddMsgListeener(msgType, handler);
        }
        #endregion

    }
}