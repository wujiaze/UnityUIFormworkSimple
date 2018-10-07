/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: UI管理器
 *		Description:
 *				功能:是整个UI框架的核心，用户通过本脚本，来实现框架绝大多数的功能实现
 *				
 *				
 *              本框架用法提示：
 *                          1、CloseUIForm 用于关闭自身，ShowUiForm 可以打开任意UI窗体（也可以代替CloseUIForm）
 *                          2、Json文件中的Key值、预设体名字 以及 脚本名字 以及 预设体自身的名字 最好一样
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleUIFramework
{
    public class UIManager : MonoBehaviour
    {
        /*字段*/
        // 单例
        private static UIManager _instance = null;
        private static readonly object _lockobj = new object();
        // UI窗体预设体路径（参数1：窗体预设名称，参数2：表示窗体预设路径）
        private Dictionary<string, string> _dicFormsPaths;
        // 缓存所有UI窗体，空间换时间
        private Dictionary<string, UIFormBase> _dicAllUiForms;
        // 保存内存中窗体（包括：显示 和 隐藏 ），用于“隐藏其他”窗体的回退操作
        private List<UIFormBase> _allUiFormList;
        // 正在显示的UI窗体(除了反向切换)
        private Dictionary<string, UIFormBase> _dicCurrentShowUiForms;
        // 正在显示的UI窗体(反向切换)
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
        /// 得到单例
        /// </summary>
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockobj)
                    {
                        if (_instance == null)
                            _instance = new GameObject("_UIManager").AddComponent<UIManager>();
                    }
                }
                return _instance;
            }
        }


        // 初始化核心数据，加载"UI窗体路径"到集合中
        public void Awake()
        {
            // 字段初始化
            _dicAllUiForms = new Dictionary<string, UIFormBase>();
            _allUiFormList = new List<UIFormBase>();
            _dicCurrentShowUiForms = new Dictionary<string, UIFormBase>();
            _dicFormsPaths = new Dictionary<string, string>();
            _stackCurrentUiForms = new Stack<UIFormBase>();
            // 初始化加载（根UI窗体）Canvas预设
            GameObject canvas = InitRootCanvasLoading();
            _traCanvasTransform = canvas.transform;
            // 获取UI根节点  全屏节点 固定节点 弹出节点 
            _traNormal = UnityHelper.FindTheChildNode(canvas, UIFrameSysDefine.SYS_NORMAL_NODE);
            _traFixed = UnityHelper.FindTheChildNode(canvas, UIFrameSysDefine.SYS_FIXED_NODE);
            _traPopUp = UnityHelper.FindTheChildNode(canvas, UIFrameSysDefine.SYS_POPUP_NODE);
            _traUiScripts = UnityHelper.FindTheChildNode(canvas, UIFrameSysDefine.SYS_SCRIPTMANAGER_NODE);
            // 将本脚本作为_ScripMgr 的子对象
            transform.SetParent(_traUiScripts, false);
            // “根UI窗体”在场景转换的时候，不允许销毁
            DontDestroyOnLoad(canvas);
            // 初始化“UI窗体预设”路径数据
            InitUiFormPathData(UIFrameSysDefine.SYS_PATH_UIFORMS_CONFIG_INFO);
        }

        /// <summary>
        /// 初始化 UI窗体预设 路径数据
        /// </summary>
        /// <param name="pathdata"></param>
	    private void InitUiFormPathData(string pathdata)
        {
            IConfigManager config = new ConfigManagerByJson(pathdata);
            if (_dicFormsPaths != null)
                _dicFormsPaths = config.AppSetting;
        }




        /// <summary>
        /// 显示（打开）UI窗体
        ///  功能：
        /// 1、根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
        /// 2、根据不同的UI窗体的显示模式，分别做不同的加载处理
        /// </summary>
        /// <param name="uiFormName">UI窗体预设的名称</param>
        public bool ShowUiForm(string uiFormName)
        {
            UIFormBase baseUiForm = null;
            //参数的检查
            if (string.IsNullOrEmpty(uiFormName))
                return false;
            //根据UI窗体的名称，加载到“所有UI窗体”缓存集合中，并且从 “所有UI窗体”缓存集合中 获取本窗体
            baseUiForm = LoadFormsToAllFormsCache(uiFormName);
            if (baseUiForm == null)
                return false;
            if (!_allUiFormList.Contains(baseUiForm))
            {
                // 最新的UI窗体(内存中不存在的窗体)
                SimpleShow(uiFormName, baseUiForm);
            }
            else
            {
                if (baseUiForm.isActiveAndEnabled)
                {
                    return true;            // 误操作不做处理
                }
                // 已经显示过的UI（内存中还存在，但是隐藏了，需要重新显示）
                HandleReShowUi(uiFormName, baseUiForm);
            }
            return true;
        }


        /// <summary>
        /// 关闭指定(单个)窗体，重新显示之前的窗体
        /// 关闭永远是按顺序从后往前关
        /// </summary>
        /// <param name="uiFormName">窗体名称</param>
	    public bool CloseUiForm(string uiFormName)
        {
            UIFormBase baseUiForm = null;
            //参数的检查
            if (string.IsNullOrEmpty(uiFormName))
                return false;  // 参数错误返回
            // _dicAllUiForms 中，如果没有记录，则直接返回(即关闭一个不存在的窗体)
            _dicAllUiForms.TryGetValue(uiFormName, out baseUiForm);
            if (baseUiForm == null)
                return false;
            // 从所有的 内存 窗体中，删除需要关闭的
            _allUiFormList.Remove(baseUiForm);
            // 根据窗体不同的显示类型，分别做不同的关闭处理
            switch (baseUiForm.CurrentUiType.UiFormsShowMode)
            {
                // 将 窗体 从 _dicCurrentShowUiForms 中移除
                case UIFormShowMode.Normal:
                    // 普通窗体的关闭
                    ExitUiForms(uiFormName);
                    break;
                case UIFormShowMode.ReverseChange:
                    // 反向切换的关闭
                    PopUiForm(uiFormName);
                    break;
                case UIFormShowMode.HideOther:
                    // 隐藏其他的关闭
                    ExitUiFormsAndHideOther(uiFormName);
                    break;
            }
            return true;
        }

        /// <summary>
        /// 切换场景时，移除当前场景的所有UI
        /// </summary>
        public void CloseAllWhenChangeScene()
        {
            foreach (UIFormBase uiFormBase in _allUiFormList)
            {
                Destroy(uiFormBase.gameObject);
            }
            _allUiFormList.Clear();
            _dicCurrentShowUiForms.Clear();
            _stackCurrentUiForms.Clear();
        }

        #region 私有内部方法

        /// <summary>
        /// 初始化加载Canvas
        /// 如果已经拖入，则不需要加载
        /// </summary>
        /// <returns></returns>
        private GameObject InitRootCanvasLoading()
        {
            GameObject canvas = GameObject.Find(UIFrameSysDefine.CANVAS);
            if (canvas == null)
                canvas = ResourcesMgr.Instance.LoadAsset(UIFrameSysDefine.SYS_PATH_CANVAS, false);
            canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.width, Screen.height);
            canvas.name = UIFrameSysDefine.CANVAS;
            return canvas;
        }




        /// <summary>
        /// 根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
        /// </summary>
        /// <param name="uiFormName">UI窗体（预设）的名称</param>
        /// <returns></returns>
        private UIFormBase LoadFormsToAllFormsCache(string uiFormName)
        {
            UIFormBase baseUiResult = null;
            if (!_dicAllUiForms.TryGetValue(uiFormName, out baseUiResult))
                baseUiResult = LoadUiForm(uiFormName);// 加载指定名称的“UI窗体”
            return baseUiResult;
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
	    private UIFormBase LoadUiForm(string uiFormName)
        {
            string strUiFormsPaths = null;
            GameObject goCloneUiPrefabs = null;
            // 根据UI窗体名称，得到对应的加载路径
            _dicFormsPaths.TryGetValue(uiFormName, out strUiFormsPaths);
            //1、根据“UI窗体名“加载预设克隆体
            if (!string.IsNullOrEmpty(strUiFormsPaths))
            {
                goCloneUiPrefabs = ResourcesMgr.Instance.LoadAsset(strUiFormsPaths, false);
                goCloneUiPrefabs.name = uiFormName;
            }
            //2、根据克隆体中不同位置信息，设置不同父对象
            if (_traCanvasTransform != null && goCloneUiPrefabs != null)
            {
                UIFormBase baseUiForm = goCloneUiPrefabs.GetComponent<UIFormBase>();
                if (baseUiForm == null)
                {
                    Debug.Log(strUiFormsPaths + "  克隆的窗体上没有BaseUIForm");
                    return null;
                }
                // 设置位置
                switch (baseUiForm.CurrentUiType.UiFormsType)
                {
                    case UIFormsType.Normal: // 普通窗体
                        goCloneUiPrefabs.transform.SetParent(_traNormal, false);
                        break;
                    case UIFormsType.Fixed: // 固定窗体
                        goCloneUiPrefabs.transform.SetParent(_traFixed, false);
                        break;
                    case UIFormsType.PopUp: //弹出窗体
                        goCloneUiPrefabs.transform.SetParent(_traPopUp, false);
                        break;
                    default:
                        break;
                }
                //3、显示刚创建的UI克隆体
                goCloneUiPrefabs.SetActive(true);
                //4、把克隆体加入到 _dicAllUiForms 中
                _dicAllUiForms.Add(uiFormName, baseUiForm);
                return baseUiForm;
            }
            else
            {
                Debug.Log("_traCanvasTransform = null Or  goCloneUIPrefabs = null " + uiFormName);
                return null;
            }

        }


        private void SimpleShow(string uiFormName, UIFormBase baseUiForm)
        {
            _allUiFormList.Add(baseUiForm);
            // 如果这个UI需要清空所有的[反向切换]窗口， 清空“栈集合”中的数据
            if (baseUiForm.CurrentUiType.IsClearStack)
            {
                bool clearResult = ClearStackArray();
                if (!clearResult)
                {
                    Debug.Log("栈中的数据没有清空，检查 uiFormName " + uiFormName);
                }
            }
            //根据不同的UI窗体的显示模式，分别做不同的加载处理
            switch (baseUiForm.CurrentUiType.UiFormsShowMode)
            {
                // 将 窗体 加载到 _dicCurrentShowUiForms 中
                case UIFormShowMode.Normal:         // 普通显示
                    LoadUiToCurrentCache(uiFormName);
                    break;
                case UIFormShowMode.HideOther:      //隐藏其他
                    EnterUiFormsAndHideOther(uiFormName);
                    break;
                case UIFormShowMode.ReverseChange:  // 反向切换
                    PushUiFormToStack(baseUiForm);
                    break;
            }
        }

        /// <summary>
        /// 清空栈集合中的数据 
        /// </summary>
        /// <returns></returns>
        private bool ClearStackArray()
        {
            if (_stackCurrentUiForms != null && _stackCurrentUiForms.Count > 0)
            {
                // 全部隐藏
                foreach (UIFormBase item in _stackCurrentUiForms)
                {
                    item.Hiding();             
                    _allUiFormList.Remove(item);    // 因为反向切换的UI，不需要再次显示，所以从 _allUiFormList 中删除，防止重新显示时，显示出来了
                }
                // 清空栈集合
                _stackCurrentUiForms.Clear();
                return true;
            }
            return false;
        }


        /// <summary>
        /// 把当前窗体加载到 _dicCurrentShowUiForms 中
        /// </summary>
        /// <param name="uiFormName"></param>
	    private void LoadUiToCurrentCache(string uiFormName)
        {
            UIFormBase baseUiFormFromAllCache = null;  // 从_DicAllUIForms 中得到窗体基类
            // 把当前窗体，加载到   _dicCurrentShowUiForms    集合中
            _dicAllUiForms.TryGetValue(uiFormName, out baseUiFormFromAllCache);
            if (baseUiFormFromAllCache != null)
            {
                _dicCurrentShowUiForms.Add(uiFormName, baseUiFormFromAllCache);
                baseUiFormFromAllCache.Display();  //显示当前窗体
            }
            else
            {
                Debug.Log(GetType() + "/baseUIFormFromAllCache  =NULL , uiFormName = " + uiFormName);
            }
        }


        /// <summary>
        /// 打来"隐藏其他属性"的窗体，且隐藏其他所有窗体
        /// </summary>
        /// <param name="uiFormName">需要打开的窗体名称</param>
        private void EnterUiFormsAndHideOther(string uiFormName)
        {
            UIFormBase baeUiFormFromAll = null;
            _dicAllUiForms.TryGetValue(uiFormName, out baeUiFormFromAll);
            if (baeUiFormFromAll == null)
                return;
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
            _dicCurrentShowUiForms.Add(uiFormName, baeUiFormFromAll);
            //显示当前窗体
            baeUiFormFromAll.Display();
        }



        /// <summary>
        /// UI窗体入栈
        /// </summary>
        /// <param name="baseUiForm">窗体的名称</param>
	    private void PushUiFormToStack(UIFormBase baseUiForm)
        {
            if (baseUiForm == null)
            {
                Debug.Log(GetType() + "/PushUIFormToStack  UIFormBase =NULL");
            }
            else
            {
                // 判断栈集合中，是否已有窗体，有则“冻结”处理
                if (_stackCurrentUiForms.Count > 0)
                {
                    UIFormBase topUiForm = _stackCurrentUiForms.Peek();
                    topUiForm.Freeze();
                }
                // 把指定的UI窗体，入栈操作
                _stackCurrentUiForms.Push(baseUiForm);
                // 显示窗体
                baseUiForm.Display();
            }
        }



        /// <summary>
        /// 处理重新显示（隐藏的并且内存中存在的)UI窗体
        /// </summary>
        private void HandleReShowUi(string uiFormName, UIFormBase baseUiForm)
        {
            // 当前显示的窗体中 已有需要显示的窗体,类似于重新打开，那就回到最开始，重新按顺序显示
            if (_dicCurrentShowUiForms.ContainsKey(uiFormName) || _stackCurrentUiForms.Contains(baseUiForm) && _allUiFormList.Count > 0)
            {
                while (_allUiFormList[_allUiFormList.Count - 1] != baseUiForm)
                {
                    UIFormBase tempUi = _allUiFormList[_allUiFormList.Count - 1];   // 由于是按顺序存入  _allUiFormList ，所以显示某一个UI时，需要把后面的UI全部关闭（我是这么处理的，具体需求还有其他的处理办法）
                    // 根据这个UI类型关闭自身
                    CloseUiForm(tempUi.name);    // 会从 _allUiFormList 中 删除 tempUi
                }
                // 循环结束，说明已经显示了需要的UI窗体
            }
        }


        /// <summary>
        /// 退出指定UI窗体
        /// </summary>
        /// <param name="uiFormName">指定的窗体名称</param>
        private void ExitUiForms(string uiFormName)
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
	    private void PopUiForm(string uiFormName)
        {
            UIFormBase topUiForm = null;
            // 参数检查
            if (_stackCurrentUiForms.Count > 0)
            {
                topUiForm = _stackCurrentUiForms.Peek();
                if (topUiForm.name == uiFormName)
                {
                    topUiForm = _stackCurrentUiForms.Pop();
                    // 隐藏处理
                    topUiForm.Hiding();
                    if (_stackCurrentUiForms.Count > 0)
                    {
                        // 新的栈顶重新显示
                        UIFormBase nextUiForm = _stackCurrentUiForms.Peek();
                        nextUiForm.ReDisplay();
                    }
                } 
            }
        }
        #endregion
        /// <summary>
        /// 关闭"隐藏其他属性"的窗体，且显示其他所有窗体 
        /// </summary>
        /// <param name="uiFormName">需要关闭的窗体名称</param>
        private void ExitUiFormsAndHideOther(string uiFormName)
        {
            UIFormBase baseUiForm = null;
            // 参数检查
            if (string.IsNullOrEmpty(uiFormName)) return;
            _dicCurrentShowUiForms.TryGetValue(uiFormName, out baseUiForm);
            if (baseUiForm == null) return;
            // 隐藏当前窗体，并且在 _dicCurrentShowUiForms 中移除
            baseUiForm.Hiding();
            _dicCurrentShowUiForms.Remove(uiFormName);
            // 重新按顺序显示隐藏其他之前的UI窗体
            ReShowUi();
        }

        /// <summary>
        /// 显示存在于_dicCurrentShowUiForms和 _stackCurrentUiForms 中的窗体，按照原来的顺序
        /// </summary>
        private void ReShowUi()
        {
            if (_allUiFormList.Count == 0) return;
            List<UIFormBase> tmplist = new List<UIFormBase>();
            // 从头开始显示
            foreach (UIFormBase uiForm in _allUiFormList)
            {
                //根据不同的UI窗体的显示模式，分别做不同的显示处理
                switch (uiForm.CurrentUiType.UiFormsShowMode)
                {
                    // 将 窗体 加载到 _dicCurrentShowUiForms 中
                    case UIFormShowMode.Normal:         // 普通显示
                        uiForm.Display();       //显示当前窗体
                        break;
                    case UIFormShowMode.ReverseChange:  // 反向切换
                        if (_stackCurrentUiForms.Peek() == uiForm)
                        {
                            uiForm.Display();
                        }
                        else
                        {
                            uiForm.Freeze();
                        }
                        break;
                    case UIFormShowMode.HideOther:      //隐藏其他
                        foreach (UIFormBase item in tmplist)
                        {
                            item.Hiding();
                        }
                        tmplist.Clear();
                        uiForm.Display();
                        break;
                }
                tmplist.Add(uiForm);
            }

        }







        #region 显示“UI管理器”内部核心数据，供测试使用
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                Debug.Log("所有加载过的UI窗体的数量: " + ShowALLUIFormCount());
                Debug.Log("正常或隐藏其他UI窗体在内存中的数量： " + ShowCurrentUIFormCount());
                foreach (string item in _dicCurrentShowUiForms.Keys)
                {
                    Debug.Log(item);
                }
                Debug.Log("反向切换UI窗体在内存中的数量： " + ShowCurrentStackUIFormCount());
                foreach (UIFormBase item in _stackCurrentUiForms)
                {
                    Debug.Log(item.name);
                }
                Debug.Log("所有UI窗体 内存中的数量： " + ShowCurrentListUIFormCount());
                foreach (UIFormBase item in _allUiFormList)
                {
                    Debug.Log(item.name);
                }
            }
        }
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
        /// <summary>
        /// 显示 当前内存中的 UI窗体数量
        /// </summary>
        /// <returns></returns>
        public int ShowCurrentListUIFormCount()
        {
            if (_allUiFormList != null)
            {
                return _allUiFormList.Count;
            }
            else
            {
                return 0;
            }
        }

        #endregion

    }
}