using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace WakeMap
{
    public class WakeManager
    {
        //参照用
        private ReqTx refReqTx = null;
        private MapController refUserControlMap = null;

        //シーン
        enum Scene {
            SceneA, SceneB, SceneC
        };
        private Scene g_scene;

        //ディクショナリー
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> g_dictAWake;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> g_dictDTrack;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> g_dictBWake;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> g_dictCPlace;

        //ディクショナリー(時刻連動選択用)
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> g_dictSelectAWake;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> g_dictSelectDTrack;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> g_dictSelectBWake;

        //航跡のコンフィグ
        public struct WakeCongfig {
            public string layername; //レイヤ名
            public bool isPoint; //ポイント描画の有無
            public System.Drawing.Brush pointColor; //ポイント色
            public float pointSize; //ポイントサイズ
            public bool isLine; //ライン描画の有無
            public System.Drawing.Color lineColor; //ライン色
            public float lineWidth; //ラインの太さ
            public bool isLineDash; //ラインを破線にするかどうか
            public bool isLineArrow; //ラインを破線にするかどうか
            public bool isLabel; //ラベル描画の有無
            public System.Drawing.Color labelBackColor; //ラベル背景色
            public System.Drawing.Color labelForeColor; //ラベル背景色
        }
        private WakeCongfig g_cfgAWake = new WakeCongfig();
        private WakeCongfig g_cfgDTrack = new WakeCongfig();
        private WakeCongfig g_cfgBWake = new WakeCongfig();
        private WakeCongfig g_cfgCPlace = new WakeCongfig();
        //選択用
        private WakeCongfig g_cfgSelectAWake = new WakeCongfig();
        private WakeCongfig g_cfgSelectDTrack = new WakeCongfig();
        private WakeCongfig g_cfgSelectBWake = new WakeCongfig();
        private WakeCongfig g_cfgSelectCPlace = new WakeCongfig();


        //コンストラクタ
        public WakeManager()
        {
        }

        //参照用インスタンスセット
        public void SetReference(ReqTx reqTx, MapController userControlMap)
        {
            this.refUserControlMap = userControlMap;
            this.refReqTx = reqTx;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="strDictAWake"></param>
        /// <param name="strDictDTrack"></param>
        /// <param name="strDictBWake"></param>
        /// <param name="strDictCPlace"></param>
        public void InitWake(
            string scene,
            string strDictAWake,
            string strDictDTrack,
            string strDictBWake,
            string strDictCPlace
            )
        {
            //mapBoxの初期化
            refUserControlMap.InitLayerOtherThanBase();

            //コンフィグを初期化
            InitWakeConfig();

            //シーンを記憶、辞書を生成
            JsonParser jsonParser = new JsonParser();
            switch (scene)
            {
                //case "SceneA":
                //    g_scene = Scene.SceneA;
                //    g_dictAWake = jsonParser.ParseDictSDictSDictSS(strDictAWake);
                //    break;
                case "SceneB":
                    g_scene = Scene.SceneB;
                    g_dictAWake = jsonParser.ParseDictSDictSDictSS(strDictAWake);
                    g_dictDTrack = jsonParser.ParseDictSDictSDictSS(strDictDTrack);
                    g_dictBWake = jsonParser.ParseDictSDictSDictSS(strDictBWake);
                    g_dictCPlace = jsonParser.ParseDictSDictSDictSS(strDictCPlace);
                    break;
                case "SceneC":
                    g_scene = Scene.SceneC;
                    g_dictAWake = jsonParser.ParseDictSDictSDictSS(strDictAWake);
                    g_dictDTrack = jsonParser.ParseDictSDictSDictSS(strDictDTrack);
                    g_dictBWake = jsonParser.ParseDictSDictSDictSS(strDictBWake);
                    g_dictCPlace = jsonParser.ParseDictSDictSDictSS(strDictCPlace);
                    break;
                default:
                    break;
            }

            //描画
            switch (g_scene)
            {
                //case Scene.SceneA:
                //    //コンフィグ変更
                //    g_cfgAWake.lineColor = System.Drawing.Color.Red;
                //    g_cfgSelectAWake.pointColor = System.Drawing.Brushes.Orange;
                //
                //    //Mapに描画する
                //    GenerateWakeLayer(ref g_dictAWake, ref g_cfgAWake);
                //
                //    //空(から)の選択用レイヤを生成
                //    GenerateSelectLayer(ref g_cfgSelectAWake);
                //
                //    //mapBoxを再描画
                //    refUserControlMap.mapBox.Refresh();
                //
                //    break;
                case Scene.SceneB:
                    //ラベル
                    g_cfgAWake.isLabel = false;

                    //Mapに描画する
                    GenerateWakeLayer(ref g_dictAWake, ref g_cfgAWake);
                    GenerateWakeLayer(ref g_dictDTrack, ref g_cfgDTrack);
                    GenerateWakeLayer(ref g_dictBWake, ref g_cfgBWake);
                    GenerateWakeLayer(ref g_dictCPlace, ref g_cfgCPlace);

                    //空(から)の選択用レイヤを生成
                    GenerateSelectLayer(ref g_cfgSelectAWake);
                    GenerateSelectLayer(ref g_cfgSelectDTrack);
                    GenerateSelectLayer(ref g_cfgSelectBWake);
                    GenerateSelectLayer(ref g_cfgSelectCPlace);

                    //mapBoxを再描画
                    refUserControlMap.mapBox.Refresh();

                    break;
                case Scene.SceneC:
                    //コンフィグ変更
                    g_cfgCPlace.isLine = true;
                    g_cfgSelectCPlace.isLine = true;
                    //連動選択は点とする
                    g_cfgSelectAWake.isPoint = true;
                    g_cfgSelectAWake.isLine = false;
                    g_cfgSelectDTrack.isPoint = true;
                    g_cfgSelectDTrack.isLine = false;
                    g_cfgSelectBWake.isPoint = true;
                    g_cfgSelectBWake.isLine = false;
                    //ラベル
                    g_cfgBWake.isLabel = false;
                    g_cfgCPlace.isLabel = true;

                    //Mapに描画する
                    GenerateWakeLayer(ref g_dictAWake, ref g_cfgAWake);
                    GenerateWakeLayer(ref g_dictDTrack, ref g_cfgDTrack);
                    GenerateWakeLayer(ref g_dictBWake, ref g_cfgBWake);
                    GeneratePointArrowLayer(ref g_dictCPlace, ref g_cfgCPlace);

                    //空(から)の選択用レイヤを生成
                    GenerateSelectLayer(ref g_cfgSelectAWake);
                    GenerateSelectLayer(ref g_cfgSelectDTrack);
                    GenerateSelectLayer(ref g_cfgSelectBWake);
                    GenerateSelectLayer(ref g_cfgSelectCPlace);

                    //mapBoxを再描画
                    refUserControlMap.mapBox.Refresh();

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="dictAWake"></param>
        public void InitWakeB1()
        {
            //mapBoxの初期化
            refUserControlMap.InitLayerOtherThanBase();

            //コンフィグを初期化
            InitWakeConfig();

            //SceneA ============================================================================
            g_scene = Scene.SceneA;
            g_dictAWake = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>
            {
                {
                    "aWake1",
                    new Dictionary<string, Dictionary<string, string>> {
                        { "info", new Dictionary<string, string> { { "row", "1" }, { "id", "1" } } },
                        { "pos1", new Dictionary<string, string> { { "x", "120.007" }, { "y", "35.846" }, { "time", "20230101001111" } } },
                        { "pos2", new Dictionary<string, string> { { "x", "124.496" }, { "y", "33.370" }, { "time", "20230101001122" } } },
                        { "pos3", new Dictionary<string, string> { { "x", "121.259" }, { "y", "31.974" }, { "time", "20230101001133" } } },
                        { "pos4", new Dictionary<string, string> { { "x", "123.925" }, { "y", "30.197" }, { "time", "20230101001144" } } }
                    }
                },
                {
                    "aWake2",
                    new Dictionary<string, Dictionary<string, string>>
                    {
                        { "info", new Dictionary<string, string> { { "row", "2" }, { "id", "2" } } },
                        { "pos1", new Dictionary<string, string> { { "x", "136.700" }, { "y", "39.000" }, { "time", "20230101001100" } } },
                        { "pos2", new Dictionary<string, string> { { "x", "136.200" }, { "y", "39.000" }, { "time", "20230101001110" } } },
                        { "pos3", new Dictionary<string, string> { { "x", "135.500" }, { "y", "38.600" }, { "time", "20230101001120" } } },
                        { "pos4", new Dictionary<string, string> { { "x", "134.800" }, { "y", "38.500" }, { "time", "20230101001130" } } },
                        { "pos5", new Dictionary<string, string> { { "x", "134.200" }, { "y", "38.800" }, { "time", "20230101001140" } } },
                        { "pos6", new Dictionary<string, string> { { "x", "133.800" }, { "y", "38.800" }, { "time", "20230101001150" } } },
                        { "pos7", new Dictionary<string, string> { { "x", "133.572" }, { "y", "39.781" }, { "time", "20230101001200" } } },
                        { "pos8", new Dictionary<string, string> { { "x", "136.238" }, { "y", "40.479" }, { "time", "20230101001300" } } },
                        { "pos9", new Dictionary<string, string> { { "x", "134.080" }, { "y", "41.495" }, { "time", "20230101001400" } } }
                    }
                },
                {
                    "aWake3",
                    new Dictionary<string, Dictionary<string, string>>
                    {
                        { "info", new Dictionary<string, string> { { "row", "3" }, { "id", "3" } } },
                        { "pos1", new Dictionary<string, string> { { "x", "143.855" }, { "y", "34.703" }, { "time", "20230102001111" } } },
                        { "pos2", new Dictionary<string, string> { { "x", "145.505" }, { "y", "33.307" }, { "time", "20230102001122" } } },
                        { "pos3", new Dictionary<string, string> { { "x", "143.030" }, { "y", "32.545" }, { "time", "20230102001133" } } },
                        { "pos4", new Dictionary<string, string> { { "x", "145.378" }, { "y", "31.276" }, { "time", "20230102001144" } } }
                    }
                }
            };

            //描画
            switch (g_scene)
            {
                case Scene.SceneA:
                    //コンフィグ変更
                    g_cfgAWake.lineColor = System.Drawing.Color.Red;
                    g_cfgSelectAWake.pointColor = System.Drawing.Brushes.Orange;

                    //Mapに描画する
                    GenerateWakeLayer(ref g_dictAWake, ref g_cfgAWake);

                    //空(から)の選択用レイヤを生成
                    GenerateSelectLayer(ref g_cfgSelectAWake);

                    //mapBoxを再描画
                    refUserControlMap.mapBox.Refresh();

                    break;
                default:
                    break;
            }
        }


        //コンフィグを初期化
        private void InitWakeConfig()
        {
            //=====未選択用=====

            g_cfgAWake.layername = "layAWake";
            g_cfgAWake.isPoint = false;
            g_cfgAWake.pointColor = System.Drawing.Brushes.White;
            g_cfgAWake.pointSize = 0;
            g_cfgAWake.isLine = true;
            g_cfgAWake.lineColor = System.Drawing.Color.LightPink;
            g_cfgAWake.lineWidth = 1;
            g_cfgAWake.isLineDash = false;
            g_cfgAWake.isLineArrow = true;
            g_cfgAWake.isLabel = true;
            g_cfgAWake.labelBackColor = System.Drawing.Color.LightPink;
            g_cfgAWake.labelForeColor = System.Drawing.Color.Black;

            g_cfgDTrack.layername = "layDTrack";
            g_cfgDTrack.isPoint = false;
            g_cfgDTrack.pointColor = System.Drawing.Brushes.White;
            g_cfgDTrack.pointSize = 0;
            g_cfgDTrack.isLine = true;
            g_cfgDTrack.lineColor = System.Drawing.Color.MediumSeaGreen;
            g_cfgDTrack.lineWidth = 1;
            g_cfgDTrack.isLineDash = false;
            g_cfgDTrack.isLineArrow = true;
            g_cfgDTrack.isLabel = true;
            g_cfgDTrack.labelBackColor = System.Drawing.Color.MediumSeaGreen;
            g_cfgDTrack.labelForeColor = System.Drawing.Color.Black;

            g_cfgBWake.layername = "layBWake";
            g_cfgBWake.isPoint = false;
            g_cfgBWake.pointColor = System.Drawing.Brushes.White;
            g_cfgBWake.pointSize = 0;
            g_cfgBWake.isLine = true;
            g_cfgBWake.lineColor = System.Drawing.Color.MediumPurple;
            g_cfgBWake.lineWidth = 1;
            g_cfgBWake.isLineDash = false;
            g_cfgBWake.isLineArrow = true;
            g_cfgBWake.isLabel = true;
            g_cfgBWake.labelBackColor = System.Drawing.Color.MediumPurple;
            g_cfgBWake.labelForeColor = System.Drawing.Color.Black;

            g_cfgCPlace.layername = "layCPlace";
            g_cfgCPlace.isPoint = true;
            g_cfgCPlace.pointColor = System.Drawing.Brushes.CornflowerBlue;
            g_cfgCPlace.pointSize = 5;
            g_cfgCPlace.isLine = false;
            g_cfgCPlace.lineColor = System.Drawing.Color.CornflowerBlue;
            g_cfgCPlace.lineWidth = 1;
            g_cfgCPlace.isLineDash = false;
            g_cfgCPlace.isLineArrow = true;
            g_cfgCPlace.isLabel = false;
            g_cfgCPlace.labelBackColor = System.Drawing.Color.CornflowerBlue;
            g_cfgCPlace.labelForeColor = System.Drawing.Color.White;

            //=====選択用=====

            g_cfgSelectAWake.layername = "laySelectAWake";
            g_cfgSelectAWake.isPoint = false;
            g_cfgSelectAWake.pointColor = System.Drawing.Brushes.Red;
            g_cfgSelectAWake.pointSize = 5;
            g_cfgSelectAWake.isLine = true;
            g_cfgSelectAWake.lineColor = System.Drawing.Color.Red;
            g_cfgSelectAWake.lineWidth = 2;
            g_cfgSelectAWake.isLineDash = false;
            g_cfgSelectAWake.isLineArrow = true;
            g_cfgSelectAWake.isLabel = false;
            g_cfgSelectAWake.labelBackColor = System.Drawing.Color.Empty;
            g_cfgSelectAWake.labelForeColor = System.Drawing.Color.Empty;

            g_cfgSelectDTrack.layername = "laySelectDTrack";
            g_cfgSelectDTrack.isPoint = false;
            g_cfgSelectDTrack.pointColor = System.Drawing.Brushes.Green;
            g_cfgSelectDTrack.pointSize = 5;
            g_cfgSelectDTrack.isLine = true;
            g_cfgSelectDTrack.lineColor = System.Drawing.Color.Green;
            g_cfgSelectDTrack.lineWidth = 2;
            g_cfgSelectDTrack.isLineDash = false;
            g_cfgSelectDTrack.isLineArrow = false;
            g_cfgSelectDTrack.isLabel = false;
            g_cfgSelectDTrack.labelBackColor = System.Drawing.Color.Empty;
            g_cfgSelectDTrack.labelForeColor = System.Drawing.Color.Empty;

            g_cfgSelectBWake.layername = "laySelectBWake";
            g_cfgSelectBWake.isPoint = false;
            g_cfgSelectBWake.pointColor = System.Drawing.Brushes.DarkViolet;
            g_cfgSelectBWake.pointSize = 5;
            g_cfgSelectBWake.isLine = true;
            g_cfgSelectBWake.lineColor = System.Drawing.Color.DarkViolet;
            g_cfgSelectBWake.lineWidth = 2;
            g_cfgSelectBWake.isLineDash = false;
            g_cfgSelectBWake.isLineArrow = true;
            g_cfgSelectBWake.isLabel = false;
            g_cfgSelectBWake.labelBackColor = System.Drawing.Color.Empty;
            g_cfgSelectBWake.labelForeColor = System.Drawing.Color.Empty;

            g_cfgSelectCPlace.layername = "laySelectCPlace";
            g_cfgSelectCPlace.isPoint = true;
            g_cfgSelectCPlace.pointColor = System.Drawing.Brushes.Blue;
            g_cfgSelectCPlace.pointSize = 5;
            g_cfgSelectCPlace.isLine = false;
            g_cfgSelectCPlace.lineColor = System.Drawing.Color.Blue;
            g_cfgSelectCPlace.lineWidth = 2;
            g_cfgSelectCPlace.isLineDash = false;
            g_cfgSelectCPlace.isLineArrow = true;
            g_cfgSelectCPlace.isLabel = false;
            g_cfgSelectCPlace.labelBackColor = System.Drawing.Color.Empty;
            g_cfgSelectCPlace.labelForeColor = System.Drawing.Color.Empty;
        }

        //航跡レイヤ生成
        private void GenerateWakeLayer(
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            ref WakeCongfig refWakeCongfig
            )
        {
            if (refDictWake == null) { return; }
            //レイヤ生成
            refUserControlMap.GenerateLayer(refWakeCongfig.layername);
            //スタイル設定
            SetStyleToLayer(ref refWakeCongfig);
            //点オブジェクト追加
            if (refWakeCongfig.isPoint) { AddPointObject(ref refDictWake, ref refWakeCongfig); }
            //線オブジェクト追加
            if (refWakeCongfig.isLine) { AddLineObject(ref refDictWake, ref refWakeCongfig); }
            //ラベルレイヤ生成
            if (refWakeCongfig.isLabel) { GenerateLabelLayer(ref refDictWake, ref refWakeCongfig); }
        }

        // PointArrowレイヤ生成
        private void GeneratePointArrowLayer(
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            ref WakeCongfig refWakeCongfig
            )
        {
            if (refDictWake == null) { return; }
            //レイヤ生成
            refUserControlMap.GenerateLayer(refWakeCongfig.layername);
            //スタイル設定
            SetStyleToLayer(ref refWakeCongfig);
            //点と矢印のオブジェクト追加
            if (refWakeCongfig.isPoint && refWakeCongfig.isLine) { AddPointArrowObject(ref refDictWake, ref refWakeCongfig); }
            //ラベルレイヤ生成
            if (refWakeCongfig.isLabel) { GenerateLabelLayer(ref refDictWake, ref refWakeCongfig); }
        }

        //スタイル設定（レイヤの点の色など）
        private void SetStyleToLayer(ref WakeCongfig refWakeCongfig)
        {
            //レイヤ取得(参照)
            VectorLayer layer = refUserControlMap.sharpMapHelper.GetVectorLayerByName(refUserControlMap.mapBox, refWakeCongfig.layername);

            if (refWakeCongfig.isPoint)
            {
                //ポイントの色、サイズを設定
                layer.Style.PointColor = refWakeCongfig.pointColor;
                layer.Style.PointSize = refWakeCongfig.pointSize;
            }
            if (refWakeCongfig.isLine)
            {
                //ラインの色、太さを設定
                layer.Style.Line = new Pen(refWakeCongfig.lineColor, refWakeCongfig.lineWidth);

                if (refWakeCongfig.isLineDash)
                {
                    //破線にする { 破線の長さ, 間隔 }
                    layer.Style.Line.DashPattern = new float[] { 3.0F, 3.0F };
                }

                if (refWakeCongfig.isLineArrow)
                {
                    //矢印にする (width, height, isFilled)
                    layer.Style.Line.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(4f, 4f, true);
                }
            }
        }

        //点オブジェクト追加
        private void AddPointObject(
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            ref WakeCongfig refWakeCongfig
            )
        {
            //wakeを取得
            foreach (var wake in refDictWake)
            {
                string row = null;
                string starttime = null;
                string endtime = null;
                int cnt = 0;
                List<Coordinate> listCoordinats = new List<Coordinate>();
                //座標を取得
                foreach (var pos in wake.Value)
                {
                    if (pos.Key.Contains("info")) { row = pos.Value["row"]; } //行を取得
                    if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                    {
                        //取得した座標をリストに追加
                        Coordinate coordinate = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                        listCoordinats.Add(coordinate);
                        //開始、終了時刻を取得
                        if (cnt == 1) { starttime = pos.Value["time"]; }
                        if (cnt == wake.Value.Count - 1) { endtime = pos.Value["time"]; }
                    }
                    cnt++;
                }
                //配列に変換
                Coordinate[] coordinates = listCoordinats.ToArray();
                //ユーザーデータ作成
                string userdata = "{row:" + row + ",starttime:" + starttime + ",endtime:" + endtime + "}";
                //レイヤーにオブジェクトを追加
                refUserControlMap.AddPointToLayer(refWakeCongfig.layername, coordinates, userdata);
            }
        }

        //線オブジェクト追加
        private void AddLineObject(
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            ref WakeCongfig refWakeCongfig
            )
        {
            //wakeを取得
            foreach (var wake in refDictWake)
            {
                string row = null;
                string starttime = null;
                string endtime = null;
                int cnt = 0;
                //座標リストを作成
                List<Coordinate> listCoordinate = new List<Coordinate>();
                foreach (var pos in wake.Value)
                {
                    if (pos.Key.Contains("info")) { row = pos.Value["row"]; } //行を取得
                    if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                    {
                        //取得した座標をリストに追加
                        Coordinate coordinate = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                        listCoordinate.Add(coordinate);
                        //開始、終了時刻を取得
                        if (cnt == 1) { starttime = pos.Value["time"]; }
                        if (cnt == wake.Value.Count - 1) { endtime = pos.Value["time"]; }
                    }
                    cnt++;
                }
                //配列に変換
                Coordinate[] coordinates = listCoordinate.ToArray();
                //ユーザーデータ作成
                string userdata = "{row:" + row + ",starttime:" + starttime + ",endtime:" + endtime + "}";
                //レイヤーにオブジェクトを追加
                refUserControlMap.AddLineToLayer(refWakeCongfig.layername, coordinates, userdata);
            }
        }

        //点と矢印のオブジェクト追加
        private void AddPointArrowObject(
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            ref WakeCongfig refWakeCongfig
            )
        {
            //wakeを取得
            foreach (var wake in refDictWake)
            {
                string row = null;
                string starttime = null;
                string endtime = null;
                int cnt = 0;
                //座標リストを作成
                List<Coordinate> listCoordinate = new List<Coordinate>();
                foreach (var pos in wake.Value)
                {
                    if (pos.Key.Contains("info")) { row = pos.Value["row"]; } //行を取得
                    if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                    {
                        //開始点の取得
                        Coordinate start = new Coordinate(
                            double.Parse(pos.Value["x"]),
                            double.Parse(pos.Value["y"]));
                        //方位の取得
                        float direction = float.Parse(pos.Value["direction"]);
                        //距離の取得
                        float distance = float.Parse(pos.Value["distance"]);
                        //終点の算出
                        double radian = direction * Math.PI / 180.0;
                        double xStart = start.X;
                        double yStart = start.Y;
                        double x = xStart + distance * Math.Cos(radian);
                        double y = yStart + distance * Math.Sin(radian);
                        Coordinate end = new Coordinate(x, y);
                        //取得した座標をリストに追加
                        listCoordinate.Add(start);
                        listCoordinate.Add(end);
                        //開始、終了時刻を取得
                        if (cnt == 1) { starttime = pos.Value["time"]; }
                        if (cnt == wake.Value.Count - 1) { endtime = pos.Value["time"]; }
                    }
                    cnt++;
                }
                //配列に変換
                Coordinate[] coordinates = listCoordinate.ToArray();
                //ユーザーデータ作成
                string userdata = "{row:" + row + ",starttime:" + starttime + ",endtime:" + endtime + "}";
                //レイヤーに点と矢印オブジェクトを追加
                refUserControlMap.AddPointArrowToLayer(refWakeCongfig.layername, coordinates, userdata);
            }
        }

        //ラベルレイヤ生成
        private void GenerateLabelLayer(
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            ref WakeCongfig refWakeCongfig
            )
        {
            VectorLayer lyr = null;
            LabelLayer llyr = null;
            {
                //カラム生成 {カラム1,カラム2,…}。 下記では0列目をラベル用とした
                System.Data.DataColumn[] columns = new[] { new System.Data.DataColumn("Label", typeof(string)) };

                //ラベルレイヤの生成
                {
                    //ラベル位置レイヤ生成し、カラムを設定
                    var fdt = new FeatureDataTable();
                    fdt.Columns.AddRange(columns);
                    lyr = new VectorLayer(refWakeCongfig.layername + "LabelsPos", new GeometryFeatureProvider(fdt));

                    //ラベルレイヤ生成、ラベル位置レイヤのDataSourceとカラムを共有する
                    var lblLayer = new LabelLayer(refWakeCongfig.layername + "Labels");
                    lblLayer.DataSource = lyr.DataSource; //ターゲットレイヤのDataSourceを共有
                    lblLayer.LabelColumn = columns[0].ColumnName; //0列目をラベル用として紐づけ
                    llyr = lblLayer;

                    //ラベルレイヤのスタイルを設定
                    llyr.Style.BackColor = new SolidBrush(refWakeCongfig.labelBackColor);//ラベル背景色
                    llyr.Style.ForeColor = Color.White;//ラベル文字色
                    llyr.Style.CollisionDetection = false; //true = ラベルが衝突するときは片方を非表示 , false = ラベルが衝突しても重ねて表示
                    llyr.Style.HorizontalAlignment = SharpMap.Styles.LabelStyle.HorizontalAlignmentEnum.Left; //水平位置合わせ(Left,Center,Right)
                    llyr.Style.VerticalAlignment = SharpMap.Styles.LabelStyle.VerticalAlignmentEnum.Middle; //垂直位置合わせ(Top,Middle,Bottom)

                    refUserControlMap.mapBox.Map.Layers.Add(llyr);
                }
            }
            //wakeを取得
            foreach (var wake in refDictWake)
            {
                foreach (var pos in wake.Value)
                {
                    if (pos.Key == "pos1")
                    {
                        Coordinate wpos = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                        //ラベルに記載する文字列
                        string strLabelText = Regex.Replace(wake.Key, @"[^0-9]", "");
                        //オブジェクト生成
                        {
                            //ポイント(ジオメトリ)生成
                            var geom = new NetTopologySuite.Geometries.Point(wpos); //点
                                                                                    //データソースのFeaturesに新しい行を生成
                            var fp = (GeometryFeatureProvider)llyr.DataSource;
                            var fdr = fp.Features.NewRow();
                            fdr[0] = strLabelText; //★ 0列目にラベル(文字列)を代入
                            fdr.Geometry = geom; //ジオメトリを設定
                            fp.Features.AddRow(fdr);
                        }
                        break;
                    }
                }
            }
        }

        //==============================================

        //選択用レイヤー生成
        private void GenerateSelectLayer(ref WakeCongfig refWakeCongfig)
        {
            //レイヤ生成
            refUserControlMap.GenerateLayer(refWakeCongfig.layername);
            //スタイル設定
            SetStyleToLayer(ref refWakeCongfig);
        }

        //航跡を選択
        public void ClickSelectWake(System.Drawing.Point clickPos)
        {
            bool isHit = false;

            //選択処理
            switch (g_scene)
            {
                case Scene.SceneA:
                    {
                        {
                            string strjson = null;
                            isHit = SelectIgeoms(ref strjson, ref g_cfgAWake, ref g_cfgSelectAWake, clickPos);

                            if (isHit)
                            {
                                //mapBoxを再描画
                                refUserControlMap.mapBox.Refresh();
                            }

                            refReqTx.ClickSelectWake();

                            break;
                        }
                    }

                case Scene.SceneB:
                    {
                        {
                            string strjson = null;
                            isHit = SelectIgeoms(ref strjson, ref g_cfgBWake, ref g_cfgSelectBWake, clickPos);
                            if (isHit)
                            {
                                //行と時刻範囲を取り出す
                                Dictionary<string, string> dict = new JsonParser().ParseDictSS(strjson);
                                string row = dict["row"];
                                string startTime = dict["starttime"];
                                string endTime = dict["endtime"];

                                //Row連動 CPlaceを選択
                                RowLinkSelect(ref g_cfgCPlace, ref g_cfgSelectCPlace, row);
                                //Time連動 AWakeを選択
                                TimeLinkSelect(ref g_dictSelectAWake, ref g_dictAWake, ref g_cfgSelectAWake, startTime, endTime);
                                //Time連動 DTrackを選択
                                TimeLinkSelect(ref g_dictSelectDTrack, ref g_dictDTrack, ref g_cfgSelectDTrack, startTime, endTime);

                                //mapBoxを再描画
                                refUserControlMap.mapBox.Refresh();

                                break;
                            }
                        }
                        {
                            string strjson = null;
                            isHit = SelectIgeoms(ref strjson, ref g_cfgCPlace, ref g_cfgSelectCPlace, clickPos);
                            if (isHit)
                            {
                                //行と時刻範囲を取り出す
                                Dictionary<string, string> dict = new JsonParser().ParseDictSS(strjson);
                                string row = dict["row"];
                                string startTime = dict["starttime"];
                                string endTime = dict["endtime"];

                                //Row連動 BWakeを選択
                                RowLinkSelect(ref g_cfgBWake, ref g_cfgSelectBWake, row);
                                //Time連動 AWakeを選択
                                TimeLinkSelect(ref g_dictSelectAWake, ref g_dictAWake, ref g_cfgSelectAWake, startTime, endTime);
                                //Time連動 DTrackを選択
                                TimeLinkSelect(ref g_dictSelectDTrack, ref g_dictDTrack, ref g_cfgSelectDTrack, startTime, endTime);

                                //mapBoxを再描画
                                refUserControlMap.mapBox.Refresh();

                                break;
                            }
                        }
                    }

                    break;

                case Scene.SceneC:
                    {
                        {
                            string strjson = null;
                            isHit = SelectIgeoms(ref strjson, ref g_cfgCPlace, ref g_cfgSelectCPlace, clickPos);
                            if (isHit)
                            {
                                //行と時刻範囲を取り出す
                                Dictionary<string, string> dict = new JsonParser().ParseDictSS(strjson);
                                string row = dict["row"];
                                string startTime = dict["starttime"];
                                string endTime = dict["endtime"];

                                //Time連動 AWakeを選択
                                TimeLinkSelect(ref g_dictSelectAWake, ref g_dictAWake, ref g_cfgSelectAWake, startTime, endTime);
                                //Time連動 DTrackを選択
                                TimeLinkSelect(ref g_dictSelectDTrack, ref g_dictDTrack, ref g_cfgSelectDTrack, startTime, endTime);
                                //Time連動 BWakeを選択
                                TimeLinkSelect(ref g_dictSelectBWake, ref g_dictBWake, ref g_cfgSelectBWake, startTime, endTime);

                                //mapBoxを再描画
                                refUserControlMap.mapBox.Refresh();
                            }
                        }

                        break;
                    }

                default:
                    break;
            }
        }

        //図形を選択する
        private bool SelectIgeoms(
            ref string strjson,
            ref WakeCongfig refWakeCongfig,
            ref WakeCongfig refSelectWakeCongfig,
            System.Drawing.Point clickPos)
        {
            bool isHit = false;
            Collection<IGeometry> selectIgeoms = new Collection<IGeometry>();

            //===== Geometryの当たり判定 =====
            {
                //レイヤ取得
                VectorLayer layer = refUserControlMap.sharpMapHelper.GetVectorLayerByName(refUserControlMap.mapBox, refWakeCongfig.layername);

                //クリック点(イメージ座標)から左上5ピクセル、右下5ピクセルのイメージ座標を算出
                System.Drawing.Point clickPosLeftUp = new System.Drawing.Point(clickPos.X - 5, clickPos.Y - 5);
                System.Drawing.Point clickPosRightDown = new System.Drawing.Point(clickPos.X + 5, clickPos.Y + 5);

                //地理座標に変換
                Coordinate coordinate1 = refUserControlMap.mapBox.Map.ImageToWorld(clickPosLeftUp);
                Coordinate coordinate2 = refUserControlMap.mapBox.Map.ImageToWorld(clickPosRightDown);

                //経度 緯度を取得
                double x1 = coordinate1.X;
                double x2 = coordinate2.X;
                double y1 = coordinate1.Y;
                double y2 = coordinate2.Y;

                //ジオメトリの当たり判定
                //指定領域に重なるジオメトリを返す Envelope( x1 , x2 , y1, y2)
                //図形の領域はIGeometry.EnvelopeInternal{xmin,xmax,ymin,ymax}の四角形となる
                Collection<IGeometry> igeoms =
                    layer.DataSource.GetGeometriesInView(
                        new GeoAPI.Geometries.Envelope(x1, x2, y1, y2)
                    );

                //ジオメトリの情報を取得
                if (igeoms.Count > 0)
                {
                    foreach (IGeometry g in igeoms)
                    {
                        if (g.GeometryType == "MultiPoint")
                        {
                            for (int i = 0; i < g.Coordinates.Count(); i++)
                            {
                                //点とクリック位置の距離を計算
                                System.Drawing.Point point = refUserControlMap.TransPosWorldToImage(g.Coordinates[i]);
                                int distance = DistancePointToPoint(point, clickPos);
                                //衝突判定
                                if (distance < 5)
                                {
                                    //選択用ジオメトリに反映
                                    selectIgeoms.Add(g);
                                    isHit = true;
                                    break;
                                }
                            }
                        }
                        if (g.GeometryType == "LineString" || g.GeometryType == "GeometryCollection")
                        {
                            for (int i = 1; i < g.Coordinates.Count(); i++)
                            {
                                //線とクリック位置の距離を計算
                                System.Drawing.Point start = refUserControlMap.TransPosWorldToImage(g.Coordinates[i - 1]);
                                System.Drawing.Point end = refUserControlMap.TransPosWorldToImage(g.Coordinates[i]);
                                int distance = DistancePointToLine(start, end, clickPos);
                                //衝突判定
                                if (distance < 5)
                                {
                                    //選択用ジオメトリに反映
                                    selectIgeoms.Add(g);
                                    isHit = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //===== 描画 =====
            {
                if (isHit)
                {
                    //レイヤ取得(参照)
                    VectorLayer layer = refUserControlMap.sharpMapHelper.GetVectorLayerByName(refUserControlMap.mapBox, refSelectWakeCongfig.layername);

                    //ジオメトリをレイヤに反映
                    GeometryProvider gpro = new GeometryProvider(selectIgeoms);
                    layer.DataSource = gpro;
                }
            }

            //===== 行と時刻範囲を取り出す =====
            {
                if (isHit)
                {
                    strjson = selectIgeoms[0].UserData.ToString();
                }
            }
            return isHit;
        }

        //行番号で連動選択する
        private void RowLinkSelect(
            ref WakeCongfig refWakeCongfig,
            ref WakeCongfig refSelectWakeCongfig,
            string row)
        {
            bool isHit = false;
            Collection<IGeometry> selectIgeoms = new Collection<IGeometry>();

            //===== 行のデータを取得 =====
            {
                //レイヤ取得(参照)
                VectorLayer layer = refUserControlMap.sharpMapHelper.GetVectorLayerByName(refUserControlMap.mapBox, refWakeCongfig.layername);
                //ジオメトリ取得
                Collection<IGeometry> igeoms = refUserControlMap.sharpMapHelper.GetIGeometriesAllByVectorLayer(layer);
                //ジオメトリ数ループ
                foreach (IGeometry g in igeoms)
                {
                    //ユーザーデータを取得
                    string strjson = g.UserData.ToString();
                    //行と時刻範囲を取り出す
                    Dictionary<string, string> dict = new JsonParser().ParseDictSS(strjson);
                    if (dict["row"] == row)
                    {
                        //選択用ジオメトリに反映
                        selectIgeoms.Add(g);
                        isHit = true;
                        break;
                    }
                }
            }

            //===== 描画 =====
            {
                if (isHit)
                {
                    //レイヤ取得(参照)
                    VectorLayer layer = refUserControlMap.sharpMapHelper.GetVectorLayerByName(refUserControlMap.mapBox, refSelectWakeCongfig.layername);

                    //ジオメトリをレイヤに反映
                    GeometryProvider gpro = new GeometryProvider(selectIgeoms);
                    layer.DataSource = gpro;
                }
            }
        }

        //時刻で連動選択する
        private void TimeLinkSelect(
        ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictSelectWake,
        ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
        ref WakeCongfig refSelectWakeCongfig,
        string startTime,
        string endTime)
        {
            refDictSelectWake = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            //===== 時刻のデータを取得 =====
            {
                foreach (var wake in refDictWake)
                {
                    Dictionary<string,Dictionary<string, string>> dict = new Dictionary<string, Dictionary<string, string>>();
                    foreach (var pos in wake.Value)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            if ((int.Parse(pos.Value["time"].Substring(0, 8)) >= int.Parse(startTime.Substring(0, 8))) && //dete
                                (int.Parse(pos.Value["time"].Substring(0, 8)) <= int.Parse(endTime.Substring(0, 8))) &&
                                (int.Parse(pos.Value["time"].Substring(8, 6)) >= int.Parse(startTime.Substring(startTime.Length - 6, 6))) && //time
                                (int.Parse(pos.Value["time"].Substring(8, 6)) <= int.Parse(endTime.Substring(endTime.Length - 6, 6)))
                                )
                            {
                                dict.Add(pos.Key, pos.Value);
                            }
                        }
                    }
                    refDictSelectWake.Add(wake.Key, dict);
                }
            }

            //===== 線の描画 =====
            {
                //レイヤ取得(参照)
                VectorLayer layer = refUserControlMap.sharpMapHelper.GetVectorLayerByName(refUserControlMap.mapBox, refSelectWakeCongfig.layername);
                //空のジオメトリ生成
                Collection<IGeometry> igeoms = new Collection<IGeometry>();
                //図形生成クラス
                GeometryFactory gf = new GeometryFactory();

                foreach (var wake in refDictSelectWake)
                {
                    //座標リストを作成
                    List<Coordinate> listCoordinate = new List<Coordinate>();
                    foreach (var pos in wake.Value)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            Coordinate coordinate = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                            listCoordinate.Add(coordinate);
                        }
                    }
                    //配列に変換
                    Coordinate[] coordinates = listCoordinate.ToArray();
                    if (refSelectWakeCongfig.isPoint)
                    {
                        //線をジオメトリに追加
                        igeoms.Add(gf.CreateMultiPointFromCoords(coordinates));
                    }
                    if (refSelectWakeCongfig.isLine && coordinates.Length >= 2 )
                    {
                        //線をジオメトリに追加
                        igeoms.Add(gf.CreateLineString(coordinates));
                    }
                }
                //ジオメトリをレイヤに反映
                GeometryProvider gpro = new GeometryProvider(igeoms);
                layer.DataSource = gpro;
            }
        }

        //==============================================

        //線分と点の距離計算
        private int DistancePointToLine(
            System.Drawing.Point start, 
            System.Drawing.Point end, 
            System.Drawing.Point point
            )
        {
            //線の始点と終点が同じなら点と点の距離で計算する
            if (start == end)
            {
                return DistancePointToPoint(start, point);
            }

            int xA = start.X;
            int yA = start.Y;
            int xB = end.X;
            int yB = end.Y;
            int xC = point.X;
            int yC = point.Y;

            //==== 線分ABの範囲内で点Cからの距離 ====
            //線分ABの長さの二乗
            double segmentLengthSquared = (xB - xA) * (xB - xA) + (yB - yA) * (yB - yA);

            //点Cから線分ABに垂直に下ろした垂線のベクトルと線分ABのベクトルとの内積を計算
            double dotProduct = (xC - xA) * (xB - xA) + (yC - yA) * (yB - yA);
            
            //内積の値を線分ABの長さの二乗で割り、tの値を求める。tは垂線が線分AB上にある位置を示すパラメータ。
            //Math.Max(0, Math.Min(1, ...)) の部分は、tの値が0以下の場合は0、1以上の場合は1となるように制限。
            //これにより、垂線が線分ABの範囲外に出る場合でも正しい結果を得ることができる。
            double t = Math.Max(0, Math.Min(1, dotProduct / segmentLengthSquared));
            
            //tを使って垂線の交点の座標を計算。線分AB上のtの値に基づいて、x座標とy座標を求める。
            double xProjection = xA + t * (xB - xA);
            double yProjection = yA + t * (yB - yA);
            
            //点Cと垂線の交点の座標との距離を計算。2次元平面上の距離の公式を使用して、点と点の距離を求めている。
            double distance = Math.Sqrt((xC - xProjection) * (xC - xProjection) + (yC - yProjection) * (yC - yProjection));

            return (int)distance;
        }

        //点と点の距離計算
        private int DistancePointToPoint(
            System.Drawing.Point pointA,
            System.Drawing.Point pointB
            )
        {
            int xA = pointA.X;
            int yA = pointA.Y;
            int xB = pointB.X;
            int yB = pointB.Y;
            double distance = Math.Sqrt((xB - xA) * (xB - xA) + (yB - yA) * (yB - yA));
            return (int)distance;
        }
        
        //==============================================

    }
}
