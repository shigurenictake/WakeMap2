using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeMap.ContentDataManagers
{
    internal class PatternB1DataManager : ContentDataManager
    {
        //
        WakeManager refWakeManager = null;
        public void SetRefWakeManager(WakeManager wakeManager) { this.refWakeManager = wakeManager;  }


        public override void InitData()
        {
            Console.WriteLine("■PatternB1DataManager.InitData");
            Console.WriteLine("　～PatternB1DataManager のデータ初期化を行います。");

            string strArg = this.GetStrArg();
            Console.WriteLine($"　～PatternB1DataManager.GetStrArg() = 「{strArg}」");

            Console.WriteLine("■PatternB1DataManager.InitData End");
        }


        public override void GetSearchResult(string searchCondition)
        {
            Console.WriteLine("■PatternB1DataManager.GetSearchResult");
            Console.WriteLine($"　～searchCondition = 「{searchCondition}」 ～");
            Console.WriteLine("■PatternB1DataManager.GetSearchResult End");
        }

        //
        public void PatternB1InitWake(
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
