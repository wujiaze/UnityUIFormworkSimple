/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: UI遮罩管理器
 *		Description:
 *				功能: 负责 “弹出窗体” 模态显示实现
 *
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */
using UnityEngine;
using UnityEngine.UI;

namespace SimpleUIFramework
{
    public class UiMaskManager : MonoBehaviour
    {
        /* 单例 */
        private static UiMaskManager _Instance = null;

        public static UiMaskManager GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("_UiMaskManager").AddComponent<UiMaskManager>();
            }
            return _Instance;
        }

        /* 字段 */
        // UI根节点对象
        private GameObject _GoCanvasRoot = null;

        // UI脚本节点对象
        private Transform _TraUIScriptsNode = null;

        // 顶层面板
        private GameObject _GoTopPanel = null;
        // 顶层面板在层级视图中的index
        private int _GoTopPanelIndex = 0;
        // 遮罩面板
        private GameObject _GoMaskPanel = null;

        // UI摄像机
        private Camera _UICamera = null;

        // UI摄像机 “初始的层深”
        private float _OriginalUICameraDepth = 0;

        void Awake() 
        {
            // 得到UI根节点对象
            _GoCanvasRoot = GameObject.FindGameObjectWithTag(UIFrameSysDefine.SYS_TAG_CANVAS);
            // UI脚本节点对象,并挂载本脚本实例
            _TraUIScriptsNode = UnityHelper.FindTheChildNode(_GoCanvasRoot, UIFrameSysDefine.SYS_SCRIPTMANAGER_NODE);
            UnityHelper.AddChildNodeToParentNode(_TraUIScriptsNode, transform);
            // 顶层面板 
            _GoTopPanel = _GoCanvasRoot;
            _GoTopPanelIndex = _GoTopPanel.transform.GetSiblingIndex();
            // 遮罩面板
            _GoMaskPanel = UnityHelper.FindTheChildNode(_GoCanvasRoot, UIFrameSysDefine.SYS_UIMASK_PANEL_NODE).gameObject;
            _GoMaskPanel.SetActive(false);
            // UI摄像机 和 UI摄像机 “初始的层深”
            _UICamera = UnityHelper.FindTheChildNode(_GoCanvasRoot, UIFrameSysDefine.SYS_UICAMERA_NODE).GetComponent<Camera>();
            if (_UICamera != null)
            {
                //UI摄像机 “初始的层深”
                _OriginalUICameraDepth = _UICamera.depth;
            }
            else
            {
                Debug.Log(GetType()+"/Awake()/UIcamera is NULL");
            }
        }


        /// <summary>
        /// 设置遮罩状态
        /// </summary>
        /// <param name="goDisplayUIForm">需要显示的UI窗体</param>
        /// <param name="lucenyType">显示窗体透明度属性</param>
        public void SetMaskForm(GameObject goDisplayUIForm,UIFormLucenyType lucenyType = UIFormLucenyType.Lucency)
        {
            // 顶层窗体下移（UGUI特性，面板中最下面，现在越前面）
            // SetAsLastSibling 方法：下移到是本层次中的最后
            _GoTopPanel.transform.SetAsLastSibling(); 
            // 启用遮罩窗体以及设置透明度
            switch (lucenyType)
            {
                // 完全透明，不能穿透
                case UIFormLucenyType.Lucency:
                    _GoMaskPanel.SetActive(true);
                    Color newColor1 = new Color(UIFrameSysDefine.SYS_UIMASK_LUCENCY_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_LUCENCY_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_LUCENCY_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_LUCENCY_COLOR_RGB_A);
                    _GoMaskPanel.GetComponent<Image>().color = newColor1;
                    break;
                    // 半透明，不能穿透
                case UIFormLucenyType.Translucence:
                    _GoMaskPanel.SetActive(true);
                    Color newColor2 = new Color(UIFrameSysDefine.SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB_A);
                    _GoMaskPanel.GetComponent<Image>().color = newColor2;
                    break;
                    // 低透明，不能穿透
                case UIFormLucenyType.ImPenetrable:
                    _GoMaskPanel.SetActive(true);
                    Color newColor3 = new Color(UIFrameSysDefine.SYS_UIMASK_IMPENETRABLE_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_IMPENETRABLE_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_IMPENETRABLE_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_IMPENETRABLE_COLOR_RGB_A);
                    _GoMaskPanel.GetComponent<Image>().color = newColor3;
                    break;
                    // 完全透明，可以穿透
                case UIFormLucenyType.Pentrate:
                    if (_GoMaskPanel.activeInHierarchy)
                    {
                        _GoMaskPanel.SetActive(false);
                    }
                    break;
                    default:
                    break;
            }
            // 遮罩窗体下移（UGUI特性，面板中最下面，现在越前面）
            _GoMaskPanel.transform.SetAsLastSibling();
            // 需要显示的窗体下移
            goDisplayUIForm.transform.SetAsLastSibling();
            // 增加当前UI摄像机的层深（保证当前摄像机为最前显示）
            if (_UICamera!=null)
            {
                _UICamera.depth = _OriginalUICameraDepth + UIFrameSysDefine.SYS_UICAMERA_DEPTH;
            }
        }

        /// <summary>
        /// 取消遮罩状态
        /// </summary>
        public void CancelMaskForm(GameObject goDisplayUIForm)
        {
            // 顶层窗体回原来的位置（因为顶层窗体，没有隐藏自身，所以需要回到原来的位置）
            _GoTopPanel.transform.SetSiblingIndex(_GoTopPanelIndex);
            // 禁用遮罩窗体
            if (_GoMaskPanel.activeInHierarchy)
            {
                _GoMaskPanel.SetActive(false);
            }
            // 需要显示的窗体上移到最上面（因为取消遮罩，也是意味着自己不显示，所有直接放到最上面就可以了）
            goDisplayUIForm.transform.SetAsFirstSibling();
            // 恢复层深
            if (_UICamera != null)
            {
                _UICamera.depth = _OriginalUICameraDepth;
            }
        }














    }
}