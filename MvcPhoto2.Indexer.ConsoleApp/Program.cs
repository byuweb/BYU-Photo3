using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcPhoto2.Indexer.ObjectModel;

namespace MvcPhoto2.Indexer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IndexStart index = new IndexStart();
            index.begin();
        }
    }
}
