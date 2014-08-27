using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcPhoto2.Indexer.ObjectModel
{
    public class IndexStart
    {
        public void begin()
        {
            string path = Path.Combine(PathsClass.ServerPath, "Shoots");
            startActivityMonitoring(path);
            /*PhotoSearch.ClearLuceneIndex();
            MetaDataRepository meta = new MetaDataRepository(Path.Combine(PathsClass.ServerPath, "Shoots"));
            PhotoSearch.AddUpdateLuceneIndex(meta.GetAll());
            PhotoSearch.Optimize();*/
            Console.ReadLine();
        }

        // System.IO

        //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")] 
        private static void startActivityMonitoring(string sPath)
        {
            FileSystemWatcher _watchFolder = new FileSystemWatcher();
            // This is the path we want to monitor
            _watchFolder.Path = sPath;
            _watchFolder.IncludeSubdirectories = true;
            // Make sure you use the OR on each Filter because we need to monitor
            // all of those activities

            _watchFolder.NotifyFilter = System.IO.NotifyFilters.DirectoryName;

            _watchFolder.NotifyFilter =
            _watchFolder.NotifyFilter | System.IO.NotifyFilters.FileName;
            _watchFolder.NotifyFilter =
            _watchFolder.NotifyFilter | System.IO.NotifyFilters.Attributes;

            // Now hook the triggers(events) to our handler (eventRaised)
            _watchFolder.Changed += new FileSystemEventHandler(eventRaised);
            _watchFolder.Created += new FileSystemEventHandler(eventRaised);
            _watchFolder.Deleted += new FileSystemEventHandler(eventRaised);

            // Occurs when a file or directory is renamed in the specific path
            _watchFolder.Renamed += new System.IO.RenamedEventHandler(eventRenameRaised);

            // And at last.. We connect our EventHandles to the system API (that is all
            // wrapped up in System.IO)
            try
            {
                _watchFolder.EnableRaisingEvents = true;
            }
            catch (ArgumentException)
            {
                abortAcitivityMonitoring();
            }
        }
        /// <summary>
        /// Triggered when an event is raised from the folder activity monitoring.
        /// All types exists in System.IO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">containing all data send from the event that got executed.</param>
        private static void eventRaised(object sender, System.IO.FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    //TS_AddLogText(string.Format("File {0} has been modified\r\n", e.FullPath));
                    Console.WriteLine(e.FullPath + "\n");
                    Console.WriteLine(e.Name + "\n");
                    break;
                case WatcherChangeTypes.Created:
                    //TS_AddLogText(string.Format("File {0} has been created\r\n", e.FullPath));
                    MetaDataRepository meta = new MetaDataRepository(Path.Combine(PathsClass.ServerPath, "Shoots"));
                    meta.getUpdated(e.FullPath);
                    //PhotoSearch.AddUpdateLuceneIndex(t);
                    //PhotoSearch.Optimize();
                    Console.WriteLine(e.FullPath + "\n");
                    Console.WriteLine(e.Name + "\n");
                    break;
                case WatcherChangeTypes.Deleted:
                    //TS_AddLogText(string.Format("File {0} has been deleted\r\n", e.FullPath));
                    Console.WriteLine(e.FullPath + "\n");
                    Console.WriteLine(e.Name + "\n");
                    break;
                default: // Another action
                    break;
            }
        }
        public static void eventRenameRaised(object sender, System.IO.RenamedEventArgs e)
        {
            //TS_AddLogText(string.Format("File {0} has been renamed to {1}\r\n", e.OldName, e.Name));
        }
        private static void abortAcitivityMonitoring()
        {
            //btnStart_Stop.Text = "Start";
            //txtActivity.Focus();
        }
    }
}
