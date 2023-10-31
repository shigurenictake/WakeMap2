using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WakeMap
{
    //C#からJavaScriptにアクセス
    public class ReqTx
    {
        public int TestCsToJsPubInt = 0;

        //参照元クラスのWebView2取得用(初期化は参照元クラスで行う)
        private Microsoft.Web.WebView2.WinForms.WebView2 refWebView;

        //参照フォーム(初期化は参照元クラスで行う)
        private MainForm refMainForm;

        //コンストラクタ
        public ReqTx(MainForm mainForm, Microsoft.Web.WebView2.WinForms.WebView2 webView)
        {
            refMainForm = mainForm;
            refWebView = webView;
        }

        //テスト
        public async void TestSendStr(string strArg)
        {
            System.Text.StringBuilder js = new System.Text.StringBuilder();

            js.AppendLine($"console.log('■ReqTx.TestSendStr');");
            js.AppendLine($"console.log('　{strArg}');");
            js.AppendLine($"console.log('■ReqTx.TestSendStr End');");

            await refWebView.ExecuteScriptAsync(js.ToString());
        }

        //JavaScript実行要求
        public async void RemoveCloseBtn()
        {
            System.Text.StringBuilder js = new System.Text.StringBuilder();
            //js.AppendLine("console.log('RemoveCloseBtn start');");
            js.AppendLine("var element = document.getElementById('btn-close');");
            js.AppendLine("element.parentNode.remove();");//親要素と一緒に削除
            //js.AppendLine("console.log('RemoveCloseBtn end');");
            await refWebView.ExecuteScriptAsync(js.ToString());
        }

        //タイトルを設定する
        public async void SetTitle()
        {
            //htmlからタイトルを取得する
            string strTitle = await refWebView.ExecuteScriptAsync("document.title");
            //タイトルを設定
            refMainForm.Text = strTitle.Trim('"');
        }

        //PatternA固有操作
        public async void OperatePatternA()
        {
            System.Text.StringBuilder js = new System.Text.StringBuilder();
            js.AppendLine("var hw1Element = document.getElementById('hw1');");
            js.AppendLine("hw1Element.style.width = '80%'; ");// widthの値を変更
            await refWebView.ExecuteScriptAsync(js.ToString());
        }

        //PatternB1操作
        public async void OperatePatternB1()
        {
            System.Text.StringBuilder js = new System.Text.StringBuilder();
            //詳細へボタンの非表示
            js.AppendLine("document.getElementById('btn-godetail').style.display='none';");
            await refWebView.ExecuteScriptAsync(js.ToString());
        }

        //PatternB固有操作
        public async void OperatePatternB()
        {
            System.Text.StringBuilder js = new System.Text.StringBuilder();
            //詳細へボタンの非表示
            js.AppendLine("document.getElementById('btn-godetail').style.display='none';");
            await refWebView.ExecuteScriptAsync(js.ToString());

            await refWebView.ExecuteScriptAsync("InitWake();");

        }

        //詳細へボタンをクリックする
        public async void buttonClickGoDetail()
        {
            System.Text.StringBuilder js = new System.Text.StringBuilder();
            js.AppendLine("var element = document.getElementById('btn-godetail').click();");
            //js.AppendLine("element.click();");
            await refWebView.ExecuteScriptAsync(js.ToString());
        }

        //詳細へボタンをクリックする
        public async void ClickSelectWake()
        {
            System.Text.StringBuilder js = new System.Text.StringBuilder();
            js.AppendLine("console.log('ClickSelectWake!!!!!!!!!!!!!!');");
            await refWebView.ExecuteScriptAsync(js.ToString());
        }

    }
}
