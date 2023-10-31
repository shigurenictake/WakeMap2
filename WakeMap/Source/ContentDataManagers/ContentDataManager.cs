using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeMap
{
    internal class ContentDataManager
    {
        //
        public string strArg = null;

        public void SetStrArg(string strArg) { this.strArg = strArg; }
        public string GetStrArg() { return this.strArg; }

        //
        public virtual void InitData()
        {
            Console.WriteLine("■ContentDataManager.InitData");
            Console.WriteLine("　～この関数を使用する際はオーバーライドして下さい。～");
            Console.WriteLine("　例：public override void InitData() { ～ }　");
            Console.WriteLine("■ContentDataManager.InitData End");
        }

        //
        public virtual void GetModel()
        {
            Console.WriteLine("■ContentDataManager.GetModel");
            Console.WriteLine("　～この関数を使用する際はオーバーライドして下さい。～");
            Console.WriteLine("■ContentDataManager.GetModel End");
        }


        //
        public virtual void GetSearchResult(string searchCondition)
        {
            Console.WriteLine("■ContentDataManager.GetSearchResult");
            Console.WriteLine("　～この関数を使用する際はオーバーライドして下さい。～");
            Console.WriteLine("■ContentDataManager.GetSearchResult End");
        }
    }
}
