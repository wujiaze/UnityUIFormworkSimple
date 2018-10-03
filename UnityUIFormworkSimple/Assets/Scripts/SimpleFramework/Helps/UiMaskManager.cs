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
 */
using UnityEngine;
using UnityEngine.UI;

namespace SimpleUIFramework
{
    public class UiMaskManager : MonoBehaviour
    {
        /* 单例 */
        private static UiMaskManager _Instance = null;

        public static UiMaskManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new GameObject("_UiMaskManager").AddComponent<UiMaskManager>();
                }
                return _Instance;
            }

        }

        /* 字段 */
        // UI根节点对象
        private GameObject _goCanvasRoot = null;

        // UI脚本节点对象
        private Transform _traUiScriptsNode = null;

        // 顶层面板
        private GameObject _goTopPanel = null;
        // 顶层面板在层级视图中的index
        private int _goTopPanelIndex = 0;
        // 遮罩面板
        private GameObject _goMaskPanel = null;

        // UI摄像机
        private Camera _uiCamera = null;

        // UI摄像机 “初始的层深”
        private float _originalUiCameraDepth = 0;

        void Awake()
        {
            // 得到UI根节点对象
            _goCanvasRoot = GameObject.FindGameObjectWithTag(UIFrameSysDefine.SYS_TAG_CANVAS);
            // UI脚本节点对象,并挂载本脚本实例
            _traUiScriptsNode = UnityHelper.FindTheChildNode(_goCanvasRoot, UIFrameSysDefine.SYS_SCRIPTMANAGER_NODE);
            UnityHelper.AddChildNodeToParentNode(_traUiScriptsNode, transform);
            // 顶层面板 
            _goTopPanel = _goCanvasRoot;
            _goTopPanelIndex = _goTopPanel.transform.GetSiblingIndex();
            // 遮罩面板
            _goMaskPanel = UnityHelper.FindTheChildNode(_goCanvasRoot, UIFrameSysDefine.SYS_UIMASK_PANEL_NODE).gameObject;
            _goMaskPanel.SetActive(false);
            // UI摄像机 和 UI摄像机 “初始的层深”
            _uiCamera = UnityHelper.FindTheChildNode(_goCanvasRoot, UIFrameSysDefine.SYS_UICAMERA_NODE).GetComponent<Camera>();
            if (_uiCamera != null)
            {
                //UI摄像机 “初始的层深”
                _originalUiCameraDepth = _uiCamera.depth;
            }
            else
            {
                Debug.Log(GetType() + "/Awake()/UIcamera is NULL");
            }
        }


        /// <summary>
        /// 设置遮罩状态
        /// </summary>
        /// <param name="goDisplayUiForm">需要显示的UI窗体</param>
        /// <param name="lucenyType">显示窗体透明度属性</param>
        public void SetMaskForm(GameObject goDisplayUiForm, UIFormLucenyType lucenyType)
        {
            // 顶层窗体下移（UGUI特性，面板中最下面，现在越前面）
            // SetAsLastSibling 方法：下移到是本层次中的最后
            _goTopPanel.transform.SetAsLastSibling();
            // 启用遮罩窗体以及设置透明度
            switch (lucenyType)
            {
                // 完全透明，不能穿透
                case UIFormLucenyType.Lucency:
                    _goMaskPanel.SetActive(true);
                    _goMaskPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    break;
                // 半透明，不能穿透
                case UIFormLucenyType.Translucence:
                    _goMaskPanel.SetActive(true);
                    Color newColor2 = new Color(UIFrameSysDefine.SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB_A);
                    _goMaskPanel.GetComponent<Image>().color = newColor2;
                    break;
                // 低透明，不能穿透
                case UIFormLucenyType.ImPenetrable:
                    _goMaskPanel.SetActive(true);
                    Color newColor3 = new Color(UIFrameSysDefine.SYS_UIMASK_IMPENETRABLE_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_IMPENETRABLE_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_IMPENETRABLE_COLOR_RGB, UIFrameSysDefine.SYS_UIMASK_IMPENETRABLE_COLOR_RGB_A);
                    _goMaskPanel.GetComponent<Image>().color = newColor3;
                    break;
                // 完全透明，可以穿透
                case UIFormLucenyType.Pentrate:
                    if (_goMaskPanel.activeInHierarchy)
                    {
                        _goMaskPanel.SetActive(false);
                    }
                    break;
                default:
                    break;
            }
            // 遮罩窗体下移（UGUI特性，面板中最下面，现在越前面）
            _goMaskPanel.transform.SetAsLastSibling();
            // 需要显示的窗体下移
            goDisplayUiForm.transform.SetAsLastSibling();
            // 增加当前UI摄像机的层深（保证当前摄像机为最前显示）
            if (_uiCamera != null)
            {
                _uiCamera.depth = _originalUiCameraDepth + UIFrameSysDefine.SYS_UICAMERA_DEPTH;
            }
        }

        /// <summary>
        /// 取消遮罩状态
        /// </summary>
        public void CancelMaskForm(GameObject goHideUiForm)
        {
            // 顶层窗体回原来的位置（因为顶层窗体，没有隐藏自身，所以需要回到原来的位置）
            _goTopPanel.transform.SetSiblingIndex(_goTopPanelIndex);
            // 禁用遮罩窗体
            if (_goMaskPanel.activeInHierarchy)
            {
                _goMaskPanel.SetActive(false);
            }
            // 隐藏窗体（这一步关系不大）
            goHideUiForm.transform.SetAsFirstSibling();
            // 恢复层深
            if (_uiCamera != null)
            {
                _uiCamera.depth = _originalUiCameraDepth;
            }
        }

    }
}