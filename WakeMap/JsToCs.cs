using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WakeMap
{
    /// <summary>WebView2に読み込ませるためのJsで実行する関数を保持させたクラス</summary>
    //[System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class JsToCs
    {
        //参照用メインフォーム
        private Form refMainForm = null;

        //参照用WebView2
        private Microsoft.Web.WebView2.WinForms.WebView2 refWebView = null;

        //参照用CsToJs
        private CsToJs refCsToJs = null;

        //WakeManagerクラス参照用
        private WakeManager refWakeManager = null;

        //JSON文字列の引数
        private string strJsonArgs = null;

        //コンストラクタ
        public JsToCs(MainForm mainForm, Microsoft.Web.WebView2.WinForms.WebView2 webView, string strJsonArgs)
        {
            this.refMainForm = mainForm;
            this.refWebView = webView;
            this.strJsonArgs = strJsonArgs;
        }

        //参照用インスタンスセット
        public void SetReference(CsToJs CsToJs, WakeManager wakeManager)
        {
            this.refCsToJs = CsToJs;
            this.refWakeManager = wakeManager;
        }

        //JSからコール　Init
        public void Init()
        {
            //Console.WriteLine("■JsToCs.Init");
            refWebView.ExecuteScriptAsync("console.log('■JsToCs.Init');");
            refCsToJs.TestSendStr("　～てすと　JsToCs.Init()→CsToJs.TestSendStr()処理");
            refWebView.ExecuteScriptAsync("console.log('■JsToCs.Init End');");
            //Console.WriteLine("■JsToCs.Init End");
        }

        //C#_WebViewチェック
        public string CheckCsharpWebView()
        {
            return "CsharpWebView_OK";
        }

        //htmlを新しいウィンドウで開く
        public void WindowOpen(string url)
        {
            string fimeneme = System.IO.Path.GetFileName(url);

            switch (fimeneme)
            {
                case "subPatternA.html":
                    (new SubFormGenerator()).CreateSubForm(fimeneme, url);
                    break;
                case "subPatternB.html":
                    (new SubFormGenerator()).CreateSubForm(fimeneme, url);
                    break;
                default:
                    break;
            }
        }

        //航跡を初期化
        public void InitWake(
            string scene,
            string strDictAWake,
            string strDictDTrack,
            string strDictBWake,
            string strDictCPlace
            )
        {
            refWakeManager.InitWake(
                scene,
                strDictAWake,
                strDictDTrack,
                strDictBWake,
                strDictCPlace
                );
        }
    }
}
