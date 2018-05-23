/*
 *
 *		Title: "SimpleUIFramework" UI框架项目
 *			    主题: Josn 解析异常
 *		Description:
 *				功能: 专门负责对于Json 路径错误，或者格式错误造成的异常，进行捕获
 *
 *		Date: 2018.4.25
 *		Version: 0.1
 *		Modify Recoder:
 *
 *
 */

using System;
namespace SimpleUIFramework
{
	public class JsonAnalysisException : Exception 
	{
	    public JsonAnalysisException():base(){}

	    public JsonAnalysisException(string execptionMessage):base(execptionMessage){}
	}
}