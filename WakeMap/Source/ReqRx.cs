using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WakeMap.ContentDataManagers;

namespace WakeMap
{
    /// <summary>WebView2に読み込ませるためのJsで実行する関数を保持させたクラス</summary>
    //[System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ReqRx
    {
        //参照用メインフォーム
        private MainForm refMainForm = null;

        //参照用WebView2
        private Microsoft.Web.WebView2.WinForms.WebView2 refWebView = null;

        //参照用ReqTx
        private ReqTx refReqTx = null;

        //JSON文字列の引数
        private string strArg = null;

        //共通関数コール用
        ContentDataManager contentDataManager = null;

        //コンテンツデータマネージャーズ
        TopMenuDataManager topMenuDataManager = null;
        PatternADataManager patternADataManager = null;
        SubPatternADataManager subPatternADataManager = null;
        PatternB1DataManager patternB1DataManager = null;

        //WakeManagerクラス参照用
        WakeManager wakeManager = null;
        MapController refMapController = null;

        //コンストラクタ
        public ReqRx(MainForm mainForm, Microsoft.Web.WebView2.WinForms.WebView2 webView, string strArg)
        {
            this.refMainForm = mainForm;
            this.refWebView = webView;
            this.strArg = strArg;
        }

        //参照用インスタンスセット
        public void SetReference(ReqTx reqTx, MapController mapController)
        {
            this.refReqTx = reqTx;
            this.refMapController = mapController;
        }

        //JSからコール　ReqInit
        public void ReqInit(string viewName)
        {
            //Console.WriteLine("■ReqRx.ReqInit");
            refWebView.ExecuteScriptAsync("console.log('■ReqRx.ReqInit');");
            refReqTx.TestSendStr("　～てすと　ReqRx.ReqInit()→reqTx.TestSendStr()処理");

            switch (viewName)
            {
                case "topMenu" :
                    topMenuDataManager = new TopMenuDataManager();
                    contentDataManager = topMenuDataManager;
                    break;
                case "patternA":
                    patternADataManager = new PatternADataManager();
                    contentDataManager = patternADataManager;
                    break;
                case "subPatternA":
                    subPatternADataManager = new SubPatternADataManager();
                    contentDataManager = subPatternADataManager;
                    break;
                case "patternB1":
                    patternB1DataManager = new PatternB1DataManager();
                    contentDataManager = patternB1DataManager;

                    wakeManager = new WakeManager();
                    patternB1DataManager.SetRefWakeManager(wakeManager);
                    wakeManager.SetReference(refReqTx, refMapController);
                    refMapController.SetReference(wakeManager);

                    break;
                default:
                    break;
            }

            contentDataManager.SetStrArg(strArg);
            contentDataManager.InitData();

            refWebView.ExecuteScriptAsync("console.log('■ReqRx.ReqInit End');");
            //Console.WriteLine("■ReqRx.ReqInit End");
        }

        //JSからコール　ReqGetModel
        public void ReqGetModel()
        {
            contentDataManager.GetModel();
        }

        //JSからコール　ReqGetSearchResult
        public void ReqGetSearchResult(string SearchCondition)
        {
            contentDataManager.GetSearchResult(SearchCondition);
        }


        //htmlを新しいウィンドウで開く
        public void ReqGenerateSubForm(string url, string strArg)
        {
            string formName = System.IO.Path.GetFileName(url);
            refMainForm.GenerateSubForm(formName, url, strArg);
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
            wakeManager.InitWake(
                scene,
                strDictAWake,
                strDictDTrack,
                strDictBWake,
                strDictCPlace
                );
        }

        //PatternB1 航跡を初期化
        public void ReqPatternB1InitWake()
        {
            patternB1DataManager.PatternB1InitWake();
        }
    }
}
