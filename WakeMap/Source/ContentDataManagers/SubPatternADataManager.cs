using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeMap.ContentDataManagers
{
    internal class SubPatternADataManager : ContentDataManager
    {
        public override void InitData()
        {
            Console.WriteLine("■SubPatternADataManager.InitData");
            Console.WriteLine("　～SubPatternADataManager のデータ初期化を行います。");

            string strArg = this.GetStrArg();
            Console.WriteLine($"　～SubPatternADataManager.GetStrArg() = 「{strArg}」");

            Console.WriteLine("■SubPatternADataManager.InitData End");
        }
    }
}
