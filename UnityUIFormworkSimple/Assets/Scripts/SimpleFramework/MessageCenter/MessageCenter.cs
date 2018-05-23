/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: 消息（传递）中心
 *		Description:
 *				功能: 负责 UI 框架中，所有UI窗体之间的数据传递
 *              注意：添加了PureMVC之后就采用MVC框架中的消息机制
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */

using System.Collections.Generic;

namespace SimpleUIFramework
{
    public class MessageCenter
    {
        public delegate void DelMessageDelivery(KeyValueUpdate kv);

        /// <summary>
        ///  消息中心缓存集合
        /// string:数据大的分类 DelMessageDelivery:数据执行委托
        /// </summary>
        public static Dictionary<string, DelMessageDelivery> _dicMessages =new Dictionary<string, DelMessageDelivery>();

        /// <summary>
        /// 添加消息的监听 
        /// 功能：在具体的UI窗体脚本中，当需要监听某一类（messageType）信息时，添加绑定委托方法
        /// </summary>
        /// <param name="messageType">消息分类</param>
        /// <param name="handler">消息委托</param>
        public static void AddMsgListeener(string messageType, DelMessageDelivery handler)
        {
            if (!_dicMessages.ContainsKey(messageType))
            {
                _dicMessages.Add(messageType,null);
            }
            _dicMessages[messageType] += handler;
        }

        /// <summary>
        /// 取消消息的监听
        /// </summary>
        /// <param name="messageType">消息的分类</param>
        /// <param name="handler">消息委托</param>
        public static void RemoveMsgListeener(string messageType, DelMessageDelivery handler)
        {
            if (_dicMessages.ContainsKey(messageType))
            {
                _dicMessages[messageType] -= handler;
            }
        }

        /// <summary>
        /// 清除所有消息的监听
        /// </summary>
        public static void ClearAllMsgListener()
        {
            if (_dicMessages!=null)
            {
                _dicMessages.Clear();
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageType">消息的分类</param>
        /// <param name="kv">键值对（对象）</param>
        public static void SendMessage(string messageType, KeyValueUpdate kv)
        {
            DelMessageDelivery del = null;
            if (!_dicMessages.TryGetValue(messageType,out del))return;
            if (del != null) del(kv); // 调用委托
        }
    }

    /// <summary>
    /// 键值更新类
    /// 功能：配合委托，实现委托数据传递
    /// </summary>
    public class KeyValueUpdate
    {
        // 键
        private string _Key;
        // 值
        private object _Value;
        /* 只读属性 */
        public string Key
        {
            get { return _Key; }
        }

        public object Value
        {
            get { return _Value; }
        }

        public KeyValueUpdate(string key, object value)
        {
            _Key = key;
            _Value = value;
        }
    }
}