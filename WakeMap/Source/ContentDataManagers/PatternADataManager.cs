using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeMap.ContentDataManagers
{
    internal class PatternADataManager : ContentDataManager
    {
        public override void InitData()
        {
            Console.WriteLine("■PatternADataManager.InitData");
            Console.WriteLine("　～PatternADataManager のデータ初期化を行います。");

            string strArg = this.GetStrArg();
            Console.WriteLine($"　～PatternADataManager.GetStrArg() = 「{strArg}」");

            Console.WriteLine("■PatternADataManager.InitData End");
        }


        public override void GetSearchResult(string searchCondition)
        {
            Console.WriteLine("■PatternADataManager.GetSearchResult");
            Console.WriteLine($"　～searchCondition = 「{searchCondition}」 ～");
            Console.WriteLine("■PatternADataManager.GetSearchResult End");
        }
    }
}
