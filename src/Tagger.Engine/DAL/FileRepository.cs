using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    class FileRepository : IDataRepository<FileLink>
    {

        private Database _db;

        #region IDataRepository<FileLink> Members

        public void Initialize()
        {
            _db = new Database("tagsbase.db3");
            _db.Init();
        }

        public void Add(IPersistable<FileLink> item)
        {
            using (var con = _db.OpenConnection())
            {
                var cmd = con.CreateCommand();
                
            }
        }

        public void Remove(IPersistable<FileLink> item)
        {
            throw new NotImplementedException();
        }

        public void Write(IPersistable<FileLink> item)
        {
            throw new NotImplementedException();
        }

        public IPersistable<FileLink> FindByKey(Identifier key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPersistable<FileLink>> GetAll()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    class FileLinkPersistor : IPersistable<FileLink>
    {
        private Identifier _id;

        #region IPersistable<FileLink> Members

        public void Hydrate(IDataRepository<FileLink> repo)
        {
            throw new NotImplementedException();
        }

        public void Persist(IDataRepository<FileLink> repo)
        {
            throw new NotImplementedException();
        }

        public Identifier Key
        {
            get { return _id; }
        }

        #endregion
    }
}
