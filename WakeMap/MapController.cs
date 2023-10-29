using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using SharpMap.Data.Providers;
using SharpMap;
using SharpMap.Layers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace WakeMap
{
    public partial class MapController : UserControl
    {
        private WakeManager refWakeManager;

        //SharpMap補助クラス
        public SharpMapHelper sharpMapHelper = new SharpMapHelper();

        //クラス変数
        public Coordinate g_worldPos = new Coordinate();                       //地理座標
        public System.Drawing.Point g_imagePos = new System.Drawing.Point();   //イメージ座標

        public MapController()
        {
            InitializeComponent();

            //SharpMap初期化
            this.InitializeMap();
        }

        //参照用
        public void SetReference(WakeManager wakeManager)
        {
            refWakeManager = wakeManager;
        }

        //マップ初期化
        private void InitializeMap()
        {
            //baseLayerレイヤ初期化
            this.InitializeBaseLayer();

            //Zoom制限
            mapBox.Map.MinimumZoom = 0.1;
            mapBox.Map.MaximumZoom = 360.0;

            //レイヤ全体を表示する(全レイヤの範囲にズームする)
            mapBox.Map.ZoomToExtents();

            //mapBoxを再描画
            mapBox.Refresh();
        }

        //基底レイヤ初期化
        private void InitializeBaseLayer()
        {
            //Map生成
            mapBox.Map = new Map(new Size(mapBox.Width, mapBox.Height));
            mapBox.Map.BackColor = System.Drawing.Color.LightBlue;

            //レイヤーの作成
            VectorLayer baseLayer = new VectorLayer("baseLayer");

            try
            {
                baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\polbnda_jpn\polbnda_jpn.shp");
                //baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\ne_10m_coastline\ne_10m_coastline.shp");
            } catch//(Exception ex)
            {
                //開発中はこっち（カレントディレクトリがWakeMap.slnの階層になる）
                baseLayer.DataSource = new ShapeFile(@".\WakeMap\ShapeFiles\polbnda_jpn\polbnda_jpn.shp");
                //baseLayer.DataSource = new ShapeFile(@".\WakeMap\ShapeFiles\ne_10m_coastline\ne_10m_coastline.shp");
            }

            baseLayer.Style.Fill = Brushes.LimeGreen;
            baseLayer.Style.Outline = Pens.Black;
            baseLayer.Style.EnableOutline = true;

            //マップにレイヤーを追加
            mapBox.Map.Layers.Add(baseLayer);
        }

        //レイヤ生成
        public void GenerateLayer(string layername)
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer(layername);
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
            //layer.Style.PointColor = Brushes.Red;
            //layer.Style.Line = new Pen(Color.DarkRed, 1.0f);
            //レイヤをmapBoxに追加
            mapBox.Map.Layers.Add(layer);
        }

        ////イベント - 地図上でマウス移動
        private void mapBox_MouseMove(Coordinate worldPos, MouseEventArgs imagePos)
        {
            g_worldPos = worldPos;//地理座標系上の座標の更新
            g_imagePos = imagePos.Location;//画面上のイメージ座標の更新
        }

        ////イベント - 地図上でクリック(ボタンを離した瞬間)

        private void mapBox_Click(object sender, EventArgs e)
        {
            refWakeManager.ClickSelectWake(g_imagePos);
        }

        ////イベント - [Pan処理] ラジオボタン「パン」変更時
        //private void radioButtonClickModePan_CheckedChanged(object sender, EventArgs e)
        //{
        //    ////「クリックモード == パン」ならばActiveToolをPanにする
        //    //if (this.radioButtonClickModePan.Checked == true)
        //    //{
        //    //    mapBox.ActiveTool = MapBox.Tools.Pan;
        //    //}
        //    //else
        //    //{
        //    //    mapBox.ActiveTool = MapBox.Tools.None;
        //    //}
        //}
        ////イベント - [Pan処理] マウスボタンが離れた瞬間
        //private void mapBox1_MouseUp(Coordinate worldPos, MouseEventArgs imagePos)
        //{
        //    //ActiveToolがPanならばパン処理を行う
        //    if (mapBox.ActiveTool == MapBox.Tools.Pan)
        //    {
        //        //「押した瞬間のイメージ座標」から「離れた瞬間のイメージ座標」がほぼ移動していなければ、地図は動かさない
        //        if (sharpMapHelper.Distance(g_mouseDownImagePos, imagePos.Location) <= 1.0)
        //        {
        //            //ActiveToolをNoneとすることでパンさせない
        //            mapBox.ActiveTool = MapBox.Tools.None;
        //
        //            //指定時間（ミリ秒）後、Panに戻す
        //            DelayActivePan(500);
        //        }
        //    }
        //}
        ////非同期処理 - [Pan処理] 指定時間後、ActiveToolをPanにする
        //private async void DelayActivePan(int msec)
        //{
        //    await Task.Delay(msec);
        //    mapBox.ActiveTool = MapBox.Tools.Pan;
        //}

        //指定レイヤにPoint追加
        public void AddPointToLayer(string layername, Coordinate[] worldPos, string userdata)
        {
            //レイヤ取得
            VectorLayer layer = sharpMapHelper.GetVectorLayerByName(mapBox, layername);
            //ジオメトリ取得
            Collection<IGeometry> igeoms = sharpMapHelper.GetIGeometriesAllByVectorLayer(layer);
            //点をジオメトリに追加
            GeometryFactory gf = new GeometryFactory();
            IMultiPoint ipoint = gf.CreateMultiPointFromCoords(worldPos);
            ipoint.UserData = userdata;
            //ジオメトリのコレクションに追加
            igeoms.Add(ipoint);
            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
        }

        //ラインを追加
        public void AddLineToLayer(string layername, Coordinate[] coordinates, string userdata)
        {
            //レイヤ取得
            VectorLayer layer = sharpMapHelper.GetVectorLayerByName(mapBox, layername);
            //ジオメトリ取得
            Collection<IGeometry> igeoms = sharpMapHelper.GetIGeometriesAllByVectorLayer(layer);
            //図形生成クラス
            GeometryFactory gf = new GeometryFactory();
            //座標リストの線を生成し、ジオメトリのコレクションに追加
            ILineString ilinestring = gf.CreateLineString(coordinates);
            ilinestring.UserData = userdata;
            //ジオメトリのコレクションに追加
            igeoms.Add(ilinestring);
            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
        }

        //PointArrowを追加
        public void AddPointArrowToLayer(string layername, Coordinate[] coordinates, string userdata)
        {
            //レイヤ取得
            VectorLayer layer = sharpMapHelper.GetVectorLayerByName(mapBox, layername);
            //ジオメトリ取得
            Collection<IGeometry> igeoms = sharpMapHelper.GetIGeometriesAllByVectorLayer(layer);
            //図形生成クラス
            GeometryFactory gf = new GeometryFactory();
            //PointArrow生成
            IPoint ipoint = gf.CreatePoint(coordinates[0]);
            ILineString ilinestring = gf.CreateLineString(coordinates);
            IGeometry[] igs = new IGeometry[2] { ipoint, ilinestring };
            IGeometryCollection igeomPointArrow = gf.CreateGeometryCollection(igs);
            igeomPointArrow.UserData = userdata;
            //ジオメトリのコレクションに追加
            igeoms.Add(igeomPointArrow);
            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
        }

        /// <summary>
        /// ベース以外の全レイヤ削除
        /// </summary>
        public void InitLayerOtherThanBase()
        {
            //ベース(0番目)以外のレイヤ削除
            while (mapBox.Map.Layers.Count > 1)
            {
                mapBox.Map.Layers.RemoveAt((mapBox.Map.Layers.Count - 1));
            }
            //mapBoxを再描画
            mapBox.Refresh();
        }

        //地図座標→イメージ座標に変換
        public System.Drawing.Point TransPosWorldToImage(Coordinate worldPos)
        {
            return System.Drawing.Point.Round(this.mapBox.Map.WorldToImage(worldPos));
        }
    }
}
