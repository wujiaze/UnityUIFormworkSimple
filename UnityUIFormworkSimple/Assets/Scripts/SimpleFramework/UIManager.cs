/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: UI管理器
 *		Description:
 *				功能:是整个UI框架的核心，用户通过本脚本，来实现框架绝大多数的功能实现
 *
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */

using System.Collections.Generic;
using UnityEngine;
namespace SimpleUIFramework
{
	public class UIManager : MonoBehaviour 
	{	
        /*字段*/
	    private static UIManager _instance = null;
        private static readonly object _lockobj =new object();
        // UI窗体预设路径（参数1：窗体预设名称，参数2：表示窗体预设路径）
	    private Dictionary<string, string> _dicFormsPaths;
        // 缓存所有UI窗体
	    private Dictionary<string, UIFormBase> _dicAllUiForms;
        // 当前显示的UI窗体(除了反向切换)
	    private Dictionary<string, UIFormBase> _dicCurrentShowUiForms;
        // 定义“栈”集合，存储显示当前所有【反向切换】的窗体类型
	    private Stack<UIFormBase> _stackCurrentUiForms;
        // UI根节点
	    private Transform _traCanvasTransform = null;
        // 全屏幕显示的节点
	    private Transform _traNormal = null;
        // 固定显示的节点
	    private Transform _traFixed = null;
        // 弹出节点
	    private Transform _traPopUp = null;
        // UI管理脚本的节点
	    private Transform _traUiScripts = null;

        /// <summary>
        /// 得到实例
        /// </summary>
        /// <returns></returns>
        public static UIManager GetInstance()
	    {
	        if (_instance==null)
	        {
                lock (_lockobj)
                {
                    if (_instance == null) _instance = new GameObject("_UIManager").AddComponent<UIManager>();
                }
	        }
	        return _instance;
	    }

        // 初始化核心数据，加载"UI窗体路径"到集合中
        public void Awake()
        {
            // 字段初始化
            _dicAllUiForms =new Dictionary<string, UIFormBase>();
            _dicCurrentShowUiForms =new Dictionary<string, UIFormBase>();
            _dicFormsPaths = new Dictionary<string, string>();
            _stackCurrentUiForms =new Stack<UIFormBase>();
            // 初始化加载（根UI窗体）Canvas预设
            GameObject _Canvas = InitRootCanvasLoading();
            // 获取UI根节点  全屏节点 固定节点 弹出节点 
            _traCanvasTransform = _Canvas.transform;
            _traNormal = UnityHelper.FindTheChildNode(_Canvas, UIFrameSysDefine.SYS_NORMAL_NODE);
            _traFixed = UnityHelper.FindTheChildNode(_Canvas, UIFrameSysDefine.SYS_FIXED_NODE);
            _traPopUp = UnityHelper.FindTheChildNode(_Canvas, UIFrameSysDefine.SYS_POPUP_NODE);
            _traUiScripts = UnityHelper.FindTheChildNode(_Canvas, UIFrameSysDefine.SYS_SCRIPTMANAGER_NODE);
            // 将本脚本对象作为“根UI窗体”的子节点
            gameObject.transform.SetParent(_traUiScripts, false);
            // “根UI窗体”在场景转换的时候，不允许销毁
            DontDestroyOnLoad(_traCanvasTransform);
            // 初始化“UI窗体预设”路径数据
            InitUIFormPathData(UIFrameSysDefine.SYS_PATH_UIFORMS_CONFIG_INFO);
        }

        /// <summary>
        /// 初始化 UI窗体预设 路径数据
        /// </summary>
        /// <param name="pathdata"></param>
	    private void InitUIFormPathData(string pathdata)
	    {
	        IConfigManager config =new ConfigManagerByJson(pathdata);
	        if (_dicFormsPaths != null)
	            _dicFormsPaths = config.AppSetting;
	    }

        private void Update()
        {
            //print("所有窗体集合中的数量" + UIManager.GetInstance().ShowALLUIFormCount());
            //print("当前窗体集合中的数量" + UIManager.GetInstance().ShowCurrentUIFormCount());
            //print("栈窗体集合中的数量" + UIManager.GetInstance().ShowCurrentStackUIFormCount());
        }

        /// <summary>
        /// 显示（打开）UI窗体
        ///  功能：
        /// 1、根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
        /// 2、根据不同的UI窗体的显示模式，分别做不同的加载处理
        /// </summary>
        /// <param name="uiFormName">UI窗体预设的名称</param>
        public void ShowUIForms(string uiFormName)
        {
            UIFormBase baseUIForm = null;
            //参数的检查
            if (string.IsNullOrEmpty(uiFormName)) return;  // 参数错误返回
            //根据UI窗体的名称，加载到“所有UI窗体”缓存集合中，并且从 “所有UI窗体”缓存集合中 获取本窗体
            baseUIForm = LoadFormsToAllFormsCache(uiFormName);
            if (baseUIForm==null)return;

            if (_dicCurrentShowUiForms.ContainsKey(uiFormName)) return; //当前显示的窗体中 已有需要显示的窗体，则返回（普通显示和隐藏其他）
            if (_stackCurrentUiForms.Contains(baseUIForm)) return; //栈中已有需要显示的窗体 则返回（反向切换）

            // 清空“栈集合”中的数据
            if (baseUIForm.CurrentUiType.IsClearStack)
            {
                bool ClearResult = ClearStackArray();
                if (!ClearResult)
                {
                    Debug.Log("栈中的数据没有清空，检查 uiFormName " + uiFormName);
                }
            }

            //根据不同的UI窗体的显示模式，分别做不同的加载处理
            switch (baseUIForm.CurrentUiType.UIForms_ShowMode)
            {
                // 将 窗体 加载到 _dicCurrentShowUiForms 中
                case UIFormShowMode.Normal:         // 普通显示
                    LoadUIToCurrentCache(uiFormName);
                    break;
                case UIFormShowMode.ReverseChange:  // 反向切换
                    PushUIFormToStack(baseUIForm);
                    break;
                case UIFormShowMode.HideOther:      //隐藏其他
                    EnterUIFormsAndHideOther(uiFormName);
                    break;
            }
            Debug.Log("_dicCurrentShowUiForms" + _dicCurrentShowUiForms.Count);
            Debug.Log("_stackCurrentUiForms" + _stackCurrentUiForms.Count);
        }

        /// <summary>
        /// 关闭指定窗体
        /// </summary>
        /// <param name="uiFormNAME">窗体名称</param>
	    public void CloseUIForms(string uiFormName)
        {
            UIFormBase baseUiForm=null;
            //参数的检查
	        if (string.IsNullOrEmpty(uiFormName)) return;  // 参数错误返回
            // _dicAllUiForms 中，如果没有记录，则直接返回(即关闭一个不存在的窗体)
            _dicAllUiForms.TryGetValue(uiFormName, out baseUiForm);
            if (baseUiForm==null)return;

            // 根据窗体不同的显示类型，分别做不同的关闭处理
            switch (baseUiForm.CurrentUiType.UIForms_ShowMode)
            {
                case UIFormShowMode.Normal:
                    // 普通窗体的关闭
                    ExitUIForms(uiFormName);
                    break;
                case UIFormShowMode.ReverseChange:
                    // 反向切换的关闭
                    PopUIForm(uiFormName);
                    break;
                case UIFormShowMode.HideOther:
                    // 隐藏其他关闭
                    ExitUIFormsAndHideOther(uiFormName);
                    break;
            }
            Debug.Log("_dicCurrentShowUiForms" + _dicCurrentShowUiForms.Count);
            Debug.Log("_stackCurrentUiForms" + _stackCurrentUiForms.Count);
        }



	   


        #region 私有内部方法

        /// <summary>
        /// 初始化加载Canvas
        /// </summary>
        /// <returns></returns>
        private GameObject InitRootCanvasLoading()
        {
            GameObject canvas = ResourcesMgr.GetInstance().LoadAsset(UIFrameSysDefine.SYS_PATH_CANVAS, false);
            canvas.name = "Canvas";
            return canvas;
        }

        /// <summary>
        /// 清空栈集合中的数据 
        /// </summary>
        /// <returns></returns>
        private bool ClearStackArray()
        {
            if (_stackCurrentUiForms != null && _stackCurrentUiForms.Count >= 1)
            {
                // 全部隐藏
                foreach (UIFormBase item in _stackCurrentUiForms)
                {
                    item.Hiding();
                }
                // 清空栈集合
                _stackCurrentUiForms.Clear();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
        /// </summary>
        /// <param name="uiFormName">UI窗体（预设）的名称</param>
        /// <returns></returns>
        private UIFormBase LoadFormsToAllFormsCache(string uiFormName)
        {
            UIFormBase baseUIResult = null;
            if (!_dicAllUiForms.TryGetValue(uiFormName, out baseUIResult))
                baseUIResult = LoadUIForm(uiFormName);// 加载指定名称的“UI窗体”
            return baseUIResult;
        }

        /// <summary>
        /// 加载指定名称的“UI窗体”
        /// 功能：
        /// 1、根据“UI窗体名“加载预设克隆体
        /// 2、根据克隆体中不同位置信息，设置不同父对象
        /// 3、隐藏刚创建的UI克隆体
        /// 4、把克隆体加入到 _dicAllUiForms 中
        /// </summary>
        /// <param name="uiFormName">UI窗体的名称</param>
	    private UIFormBase LoadUIForm(string uiFormName)
        {
            string strUIFormsPaths = null;
            GameObject goCloneUIPrefabs = null;
            UIFormBase baseUiForm = null;
            // 根据UI窗体名称，得到对应的加载路径
            _dicFormsPaths.TryGetValue(uiFormName, out strUIFormsPaths);
            //1、根据“UI窗体名“加载预设克隆体
            if (!string.IsNullOrEmpty(strUIFormsPaths))
            {
                goCloneUIPrefabs = ResourcesMgr.GetInstance().LoadAsset(strUIFormsPaths, false);
                goCloneUIPrefabs.name = uiFormName;
            }
            //2、根据克隆体中不同位置信息，设置不同父对象
            if (_traCanvasTransform != null && goCloneUIPrefabs != null)
            {
                baseUiForm = goCloneUIPrefabs.GetComponent<UIFormBase>();
                if (baseUiForm == null)
                {
                    Debug.Log(strUIFormsPaths + "  克隆的窗体上没有BaseUIForm");
                    return null;
                }
                // 设置位置
                switch (baseUiForm.CurrentUiType.UIForms_Type)
                {
                    case UIFormsType.Normal: // 普通窗体
                        goCloneUIPrefabs.transform.SetParent(_traNormal,false);
                        break;
                    case UIFormsType.Fixed: // 固定窗体
                        goCloneUIPrefabs.transform.SetParent(_traFixed, false);
                        break;
                    case UIFormsType.PopUp: //弹出窗体
                        goCloneUIPrefabs.transform.SetParent(_traPopUp, false);
                        break;
                    default:
                        break;
                }
                //3、显示刚创建的UI克隆体
                goCloneUIPrefabs.SetActive(true); // 这个地方坑你需要改成false，碰到问题了再说 ？？
                //4、把克隆体加入到 _dicAllUiForms 中
                _dicAllUiForms.Add(uiFormName, baseUiForm);
                return baseUiForm;
            }
            else
            {
                Debug.Log("_traCanvasTransform = null Or  goCloneUIPrefabs = null" + uiFormName);
                return null;
            }

        }

        /// <summary>
        /// 把当前窗体加载到 _dicCurrentShowUiForms 中
        /// </summary>
        /// <param name="uiFormName"></param>
	    private void LoadUIToCurrentCache(string uiFormName)
        {
            UIFormBase baseUIFormFromAllCache;  // 从_DicAllUIForms 中得到窗体基类
            // 把当前窗体，加载到   _dicCurrentShowUiForms    集合中
            _dicAllUiForms.TryGetValue(uiFormName, out baseUIFormFromAllCache);
            if (baseUIFormFromAllCache != null)
            {
                _dicCurrentShowUiForms.Add(uiFormName, baseUIFormFromAllCache);

                baseUIFormFromAllCache.Display();  //显示当前窗体
            }
            else
            {
                Debug.Log(GetType() + "/baseUIFormFromAllCache  =NULL , uiFormName = " + uiFormName);
            }
        }

        /// <summary>
        /// UI窗体入栈
        /// </summary>
        /// <param name="uiFormName">窗体的名称</param>
	    private void PushUIFormToStack(UIFormBase baseUiForm)
        {
            if (baseUiForm == null)
            {
                Debug.Log(GetType() + "/PushUIFormToStack  UIFormBase =NULL , uiFormName = " + baseUiForm.name);
            }
            else
            {
                // 判断栈集合中，是否已有窗体，有则“冻结”处理
                if (_stackCurrentUiForms.Count > 0)
                {
                    UIFormBase topUIForm = _stackCurrentUiForms.Peek();
                    topUIForm.Freeze();
                }
                // 把指定的UI窗体，入栈操作
                _stackCurrentUiForms.Push(baseUiForm);
                // 显示窗体
                baseUiForm.Display();
            }
        }

        /// <summary>
        /// 打来"隐藏其他属性"的窗体，且隐藏其他所有窗体
        /// </summary>
        /// <param name="uiFormName">需要打开的窗体名称</param>
	    private void EnterUIFormsAndHideOther(string uiFormName)
        {
            UIFormBase baeUiFormFromALL = null;
            _dicAllUiForms.TryGetValue(uiFormName, out baeUiFormFromALL);
            if (baeUiFormFromALL == null) return;
            // 把  _dicCurrentShowUiForms  和  _stackCurrentUiForms 中所有的窗体都隐藏
            foreach (UIFormBase baseuiForm in _dicCurrentShowUiForms.Values)
            {
                baseuiForm.Hiding();
            }
            foreach (UIFormBase uiForm in _stackCurrentUiForms)
            {
                uiForm.Hiding();
            }
            // 加入到  _dicCurrentShowUiForms
            _dicCurrentShowUiForms.Add(uiFormName, baeUiFormFromALL);
            //显示当前窗体
            baeUiFormFromALL.Display();
        }




        /// <summary>
        /// 退出指定UI窗体
        /// </summary>
        /// <param name="uiFormName">指定的窗体名称</param>
        private void ExitUIForms(string uiFormName)
        {
            if (string.IsNullOrEmpty(uiFormName)) return;
            UIFormBase baseUiForm = null;
            // _dicCurrentShowUiForms 中不存在 uiFormName，直接返回
            _dicCurrentShowUiForms.TryGetValue(uiFormName, out baseUiForm);
            if (baseUiForm == null) return;
            // 指定窗体，标记为“隐藏窗体”，并从 _dicCurrentShowUiForms 移除
            baseUiForm.Hiding();
            _dicCurrentShowUiForms.Remove(uiFormName);
        }


        /// <summary>
        ///  “反向切换”属性窗体的出栈
        /// </summary>
	    private void PopUIForm(string uiFormName)
        {
            // 参数检查
            if (_stackCurrentUiForms.Count > 0)
            {
                UIFormBase topUiForm = _stackCurrentUiForms.Peek();
                if (topUiForm.name != uiFormName) return;
            }
            else
            {
                return;
            }

            if (_stackCurrentUiForms.Count >= 2)
            {
                // 出栈处理
                UIFormBase topUiForm = _stackCurrentUiForms.Pop();
                // 隐藏处理
                topUiForm.Hiding();
                // 新的栈顶重新显示
                UIFormBase nextUiForm = _stackCurrentUiForms.Peek();
                nextUiForm.ReDisplay();
            }
            else if (_stackCurrentUiForms.Count == 1)
            {
                // 出栈处理
                UIFormBase topUiForm = _stackCurrentUiForms.Pop();
                // 隐藏处理
                topUiForm.Hiding();
            }
           
        }

        /// <summary>
        /// 关闭"隐藏其他属性"的窗体，且显示其他所有窗体 // todo zhexuyao改变一下
        /// </summary>
        /// <param name="uiFormName">需要关闭的窗体名称</param>
        private void ExitUIFormsAndHideOther(string uiFormName)
        {
            UIFormBase baseUiForm = null;
            // 参数检查
            if (string.IsNullOrEmpty(uiFormName)) return;
            _dicCurrentShowUiForms.TryGetValue(uiFormName, out baseUiForm);
            if (baseUiForm == null) return;
            // 隐藏当前窗体，并且在 _dicCurrentShowUiForms 中移除
            baseUiForm.Hiding();
            _dicCurrentShowUiForms.Remove(uiFormName);

            // todo zhexuyao改变一下
            // 把  _dicCurrentShowUiForms 中所有的窗体都显示
            foreach (UIFormBase uiForm in _dicCurrentShowUiForms.Values)
            {
                Debug.Log(uiForm.name);
                switch (uiForm.CurrentUiType.UIForms_ShowMode)
                {
                    // 将 窗体 加载到 _dicCurrentShowUiForms 中
                    case UIFormShowMode.Normal:         // 普通显示
                        uiForm.ReDisplay();  //显示当前窗体
                        break;
                    case UIFormShowMode.HideOther:      //隐藏其他
                        foreach (UIFormBase baseuiForm in _dicCurrentShowUiForms.Values)
                        {
                            baseuiForm.Hiding();
                        }
                        foreach (UIFormBase baseuiForm in _stackCurrentUiForms)
                        {
                            baseuiForm.Hiding();
                        }
                        uiForm.ReDisplay();
                        break;
                }
            }

            //  _stackCurrentUiForms 中所有的窗体都按顺序显示
            // 注意：根据Canvas中的顺序，栈中的窗体也要后重新显示
            Stack<UIFormBase> tempStack = new Stack<UIFormBase>();
            foreach (UIFormBase uiForm in _stackCurrentUiForms)
            {
                tempStack.Push(uiForm);
            }
            // 这就、样就仿佛是重新打开了一遍
            foreach (UIFormBase uiForm in tempStack)
            {
                uiForm.ReDisplay();
            }

        }



        #endregion

        #region 显示“UI管理器”内部核心数据，供测试使用

        /// <summary>
        /// 显示所有UI窗体的数量  
        /// </summary>
        /// <returns></returns>
        public int ShowALLUIFormCount()
        {
            if (_dicAllUiForms != null)
            {
                return _dicAllUiForms.Count;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 显示 _dicCurrentShowUiForms 当前窗体的数量
        /// </summary>
        /// <returns></returns>
	    public int ShowCurrentUIFormCount()
        {
            if (_dicCurrentShowUiForms != null)
            {
                return _dicCurrentShowUiForms.Count;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 显示 当前栈的窗体数量
        /// </summary>
        /// <returns></returns>
	    public int ShowCurrentStackUIFormCount()
        {
            if (_stackCurrentUiForms != null)
            {
                return _stackCurrentUiForms.Count;
            }
            else
            {
                return 0;
            }
        }

        #endregion

    }
}