using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MvcPhoto2.Models.Folders;
using MvcPhoto2.Models.ViewModels;


namespace MvcPhoto2.Models
{
    public interface IPhotorepo
    {
        List<EmailCart> getAllEmailItems();
        void AddToEmail(EmailCart emailcart);
        void removeEmailItem(EmailCart shoot);
        void removeAllEmailItems(List<EmailCart> shoots);
        EmailCart getEmailItem(int id);
        List<folderCategory> getAllFolderNames(string path, int depth = 0);
        List<foldersJson> GetAll();
        List<shootList> getAllShoots(string url);

        void Dispose();
    }
}