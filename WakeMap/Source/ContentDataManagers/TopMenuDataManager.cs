using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeMap
{
    internal class TopMenuDataManager : ContentDataManager
    {
        public override void InitData()
        {
            Console.WriteLine("■TopMenuDataManager.InitData");
            Console.WriteLine("　～TopMenuDataManagerのデータ初期化を行います。");

            string strArg = this.GetStrArg();
            Console.WriteLine($"　～TopMenuDataManager.GetStrArg() = 「{strArg}」");

            Console.WriteLine("■TopMenuDataManager.InitData End");
        }
    }
}
