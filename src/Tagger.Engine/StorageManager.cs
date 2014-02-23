using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tagger.Engine.DAL;

namespace Tagger.Engine
{
    public class StorageManager
    {
        public IEnumerable<FileLink> GetStoredFiles()
        {
            var items = DAL.DatabaseService.FileRepository.GetAll();
            return items.ToList();
        }

        public void UpdateStorage(IEnumerable<FileLink> fileData)
        {
            var storedItems = DAL.DatabaseService.FileRepository.GetAll();
            var joined = from files in fileData
                         join stored in storedItems on files.FullName equals stored.FullName into joinResult
                         from resultRow in joinResult.DefaultIfEmpty(null)
                         select new { Link = files, ID = resultRow == null? new DAL.Identifier():resultRow.AsPersistable().Key};

            var repo = DAL.DatabaseService.FileRepository;
            foreach (var item in joined.AsParallel())
            {
                if (item.ID.IsEmpty())
                {
                    repo.Add(item.Link);
                }
                else
                {
                    item.Link.AsPersistable().Key = item.ID;
                    repo.Write(item.Link);
                }
            }

        }
    }
}
