using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine
{
    public class StorageManager
    {
        public IEnumerable<FileLink> GetStoredFiles()
        {
            var items = DAL.DatabaseService.FileRepository.GetAll();
            return items.Select((x) => x.Value).ToList();
        }

        public void UpdateStorage(IEnumerable<FileLink> fileData)
        {
            var storedItems = DAL.DatabaseService.FileRepository.GetAll();
            var joined = from files in fileData
                         join stored in storedItems on files.FullName equals stored.Value.FullName into joinResult
                         from resultRow in joinResult.DefaultIfEmpty(null)
                         select new { Link = files, ID = resultRow == null? new DAL.Identifier():resultRow.Key};

            var repo = DAL.DatabaseService.FileRepository;
            foreach (var item in joined.AsParallel())
            {
                var persistable = DAL.FileLinkPersistor.Create(item.Link);
                if (item.ID.IsEmpty())
                {
                    repo.Add(persistable);
                }
                else
                {
                    persistable.Key = item.ID;
                    repo.Write(persistable);
                }
            }

        }
    }
}
