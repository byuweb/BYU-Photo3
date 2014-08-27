using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MvcPhoto2.Indexer.ObjectModel;

namespace MvcPhoto2.Indexer
{
    public partial class indexer : ServiceBase
    {
        public indexer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            IndexStart index = new IndexStart();
            index.begin();
        }

        protected override void OnStop()
        {
        }

        

    }
}
