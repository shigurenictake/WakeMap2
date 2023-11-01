
function Init() {
    console.log("■patternB1.html　Init");

    chrome.webview.hostObjects.reqRx.ReqInit("patternB1");
    chrome.webview.hostObjects.reqRx.ReqGetModel();
    chrome.webview.hostObjects.reqRx.ReqGetSearchResult("patternB1の検索条件");

    chrome.webview.hostObjects.reqRx.ReqPatternB1InitWake();

    console.log("■patternB1.html　Init end");
}


function buttonClickGoDetail() {
    windowOpenSelf('./subPatternB.html');
}

