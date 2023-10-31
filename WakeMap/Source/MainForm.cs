using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WakeMap
{
    public partial class MainForm : Form
    {
        //ReqTxクラス
        private ReqTx reqTx = null;

        //ReqRxクラス
        private ReqRx reqRx = null;
        
        //コンストラクタ
        public MainForm(string url , string strArg)
        {
            InitializeComponent();

            //ReqTxクラス生成
            reqTx = new ReqTx(this, webView);

            //ReqRxクラス生成
            reqRx =  new ReqRx(this, webView,  strArg);

            //参照用オブジェクトセット
            reqRx.SetReference(reqTx, mapController);

            //URLを設定
            webView.Source = new Uri(url);

            //ロード関連のイベントを登録
            webView.NavigationStarting += webView_NavigationStarting;
            webView.SourceChanged += webView_SourceChanged;
            webView.NavigationCompleted += webView_NavigationCompleted;
        }

        //イベント ボタンクリック「詳細へ」
        private void buttonGoDetail_Click(object sender, EventArgs e)
        {
            reqTx.buttonClickGoDetail();
        }

        //イベント ボタンクリック「戻る」
        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.webView.GoBack();
        }

        //イベント ボタンクリック「ヘルプ」
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ヘルプ");
        }

        //イベント ボタンクリック「閉じる」
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //イベント WebView2 ロード開始
        private void webView_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            webView.ExecuteScriptAsync("console.log('■MainForm.webView_NavigationStarting');");
            LaodStarting();
            webView.ExecuteScriptAsync("console.log('■MainForm.webView_NavigationStarting End');");
        }

        //イベント WebView2 URL変更時
        private void webView_SourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            webView.ExecuteScriptAsync("console.log('■MainForm.webView_SourceChanged');");
            LaodStarting();
            webView.ExecuteScriptAsync("console.log('■MainForm.webView_SourceChanged End');");
        }

        //ロード開始直前処理
        private void LaodStarting()
        {
            try
            {
                if (webView.CoreWebView2 != null)
                {
                    //ツールフラグ = true 定義をJavaScriptへ送信　※速度重視のためメインフォームから直接送信
                    webView.ExecuteScriptAsync("const isLocalRefToolRunning = Boolean(1);");
                    webView.ExecuteScriptAsync("console.log('　isLocalRefToolRunning = ' + isLocalRefToolRunning);");

                    //JavaScriptからC#のメソッドが実行できる様に仕込む
                    webView.CoreWebView2.AddHostObjectToScript("reqRx", reqRx);
                }
                else MessageBox.Show("CoreWebView2==null");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //イベント WebView2 ロード完了
        private void webView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                if (webView.CoreWebView2 != null)
                {
                    //URLからファイル名を取得
                    string url = this.webView.Source.ToString();
                    string fimeneme = System.IO.Path.GetFileName(url);
                    //フォーム構成を切り変える
                    Transform(fimeneme);
                }
                else MessageBox.Show("CoreWebView2==null");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //htmlファイル名に応じてフォーム内の構成を変更する
        private void Transform(string fimeneme)
        {
            switch (fimeneme)
            {
                case "topMenu.html":
                case "patternA.html":
                case "subPatternA.html":
                    TransformA(fimeneme);
                    break;
                case "subPatternA2.html":
                    //TransformA2(fimeneme);
                    break;
                case "patternB.html":
                case "patternB1.html":
                case "patternB2.html":
                case "patternB3.html":
                case "subPatternB.html":
                    TransformB(fimeneme);
                    break;

                default:
                    Console.WriteLine("未登録のhtmlです");
                    break;
            }
        }

        //ウィンドウ構成A 全画面html
        private void TransformA(string fimeneme)
        {
            //コントロールのレイアウトを一時停止
            this.SuspendLayout();

            //htmlの閉じるボタンを削除
            this.reqTx.RemoveCloseBtn();
            //タイトルを設定
            this.reqTx.SetTitle();

            //右パネルを非表示
            this.splitContainerLR.Panel2Collapsed = true;
            //左上パネルを表示
            this.splitContainerLeftUD.Panel1Collapsed = false;
            //C#の「戻る/ヘルプ/閉じる」パネルを左上パネルに移動
            this.splitContainerRightUD.Panel1.Controls.Remove(this.panelBHC);
            this.splitContainerLeftUD.Panel1.Controls.Add(this.panelBHC);
            this.panelBHC.Location = new System.Drawing.Point(
                this.splitContainerLeftUD.Panel1.Width - this.panelBHC.Width,
                this.panelBHC.Location.Y);

            //戻るボタンの非表示・表示
            switch (fimeneme)
            {
                case "topMenu.html":
                    //戻るボタンの非表示
                    this.buttonBack.Visible = false;
                    break;
                default:
                    //戻るボタンの表示
                    this.buttonBack.Visible = true;
                    break;
            }

            //固有処理
            switch (fimeneme)
            {
                case "topMenu.html":
                    break;
                case "patternA.html":
                    this.reqTx.OperatePatternA();
                    break;
                default:
                    break;
            }

            //コントロールのレイアウトを再開
            this.ResumeLayout(false);
        }

        //ウィンドウ構成B 半画面html
        private void TransformB(string fimeneme)
        {
            //コントロールのレイアウトを一時停止
            this.SuspendLayout();

            //htmlの閉じるボタンを削除
            this.reqTx.RemoveCloseBtn();
            //タイトルを設定
            this.reqTx.SetTitle();

            //右パネルを表示
            this.splitContainerLR.Panel2Collapsed = false;
            //左上パネルを非表示
            this.splitContainerLeftUD.Panel1Collapsed = true;
            //C#の「戻る/ヘルプ/閉じる」パネルを右上パネルに移動
            this.splitContainerLeftUD.Panel1.Controls.Remove(this.panelBHC);
            this.splitContainerRightUD.Panel1.Controls.Add(this.panelBHC);
            this.panelBHC.Location = new System.Drawing.Point(
                this.splitContainerRightUD.Panel1.Width - this.panelBHC.Width,
                this.panelBHC.Location.Y);

            //戻るボタンの表示
            this.buttonBack.Visible = true;

            //スプリッターの位置を設定
            this.splitContainerLR.SplitterDistance = (int)(this.splitContainerLR.Width * 65 / 100);

            //固有処理
            switch (fimeneme)
            {
                case "patternB1.html":
                    reqTx.OperatePatternB1();
                    break;
                case "patternB.html":
                case "patternB2.html":
                case "patternB3.html":
                    reqTx.OperatePatternB();
                    break;
                default:
                    break;
            }

            //コントロールのレイアウトを再開
            this.ResumeLayout(false);
        }


        //SubForm生成
        public void GenerateSubForm(string formname, string url, string strArg)
        {
            //指定した名前のフォームがあれば取得する
            MainForm subform = this.GetSubFormByName(formname);

            //二重起動防止
            if (subform == null || subform.IsDisposed)
            {
                //ヌル、または破棄されていたら、新しいウィンドウで起動する
                subform = new MainForm(url, strArg);
                subform.Show();
                //フォーム名を設定
                subform.Name = formname;
            }
            //フォームにフォーカスを当てる。
            if (!subform.Visible)
            {
                subform.Show();
            }
            else
            {
                subform.Activate();
            }
        }

        //指定した名前のFormオブジェクトがあれば取得する
        public MainForm GetSubFormByName(string formname)
        {
            MainForm retform = null;
            foreach (MainForm form in Application.OpenForms)
            {
                if (form.Name == formname)
                {
                    retform = (MainForm)form;
                    break;
                }
            }
            return retform;
        }
    }
}
