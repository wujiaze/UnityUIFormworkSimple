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

using SimpleUIFramework;
using UnityEngine;
namespace DemoProject
{
	public class StartProject : MonoBehaviour 
	{
	    void Start()
	    {
	        UIManager.GetInstance().ShowUIForms(ProjectConfig.LOGON_FORM);
            DontDestroyOnLoad(this);
	        for (int i = 0; i < 50; i++)
	        {
	            Log.Write("测试Log", Level.High);
            }
            
	    }
	}
}