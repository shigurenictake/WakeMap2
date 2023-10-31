/* 共通処理 */

//C#_WebView判定フラグ(false:未使用 , true:使用)
//htmlロード後にC#からtrueがセットされる
var isLocalRefTool = Boolean(0); //初期値=false


window.onload = (event) => {
    console.log("■common.js　window.onload");

    if (typeof isLocalRefToolRunning !== 'undefined') {
        console.log('　isLocalRefToolRunning = ' + isLocalRefToolRunning); // 値
        console.log('　typeof isLocalRefToolRunning = ' + typeof isLocalRefToolRunning); // 型

        isLocalRefTool = Boolean(1); //true
    } else {
        console.log("　isLocalRefToolRunningは定義されていません。");
        isLocalRefTool = Boolean(0); //false
    }

    Init();

    console.log("■common.js　window.onload end");
};

//C#からJavaScriptを呼び出す
function jsFunc1(str1) {
    //タグを生成
    const tag = document.createElement("div");
    tag.innerText = str1;
    //未完了のリストに追加
    document.getElementById("CsToJs").appendChild(tag);
    return "success"
}

//JavaScriptからC#を呼び出す
function ButtonClick_JsToCs() {
    //テキストボックスから値取得
    const sendtext = document.getElementById('sendtext');
    //C#の関数の実行
    chrome.webview.hostObjects.reqRx.JsToCsMethod(sendtext.value);
}

//htmlからウィンドウを閉じる
function closeWindow() {
    window.close();
}

function openSubWindow(relativePath,strArg){
    if (isLocalRefTool == false){
        openSubWindowByHtml(relativePath);
    }else{
        openSubWindowByCs(relativePath, strArg);
    }
}

//htmlからサブウィンドウを開く
function openSubWindowByHtml(relativePath) {
    window.open(relativePath, "_blank", "width=800,height=300");
}

//C#からサブウィンドウを開く
function openSubWindowByCs(relativePath, strArg) {
    // 現在のURLの情報を取得する
    const currentUrl = new URL(window.location.href);
    // 相対パスをフルパスに変換する
    const fullPath = new URL(relativePath, currentUrl).href;
    console.log(fullPath);
    //C#の関数の実行。htmlを新しいフォームで開く
    chrome.webview.hostObjects.reqRx.ReqGenerateSubForm(fullPath, strArg);
}

//htmlでURL切り替え
function windowOpenSelf(relativePath) {
    window.open(relativePath, "_self");
}
