using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    public static class DatabaseService
    {
        private static IDatabase _serviceInstance;
        private static FileRepository _fileRepo;

        static DatabaseService()
        {
        }

        public static void InitByConnectionString(string connString)
        {
            var db = new Tagger.Engine.DAL.SQLiteDatabase(connString);
            Tagger.Engine.DAL.DatabaseService.RegisterInstance(db);
        }

        internal static void RegisterInstance(IDatabase instance)
        {
            _serviceInstance = instance;
            _serviceInstance.Init();
        }

        internal static IDatabase GetInstance()
        {
            return _serviceInstance;
        }

        internal static void ShutdownInstance()
        {
            if (_serviceInstance != null)
            {
                _serviceInstance.Shutdown();
                _fileRepo = null;
                _serviceInstance = null;
            }
        }

        internal static FileRepository FileRepository
        {
            get
            {
                if (_fileRepo == null)
                {
                    if (GetInstance() == null)
                    {
                        throw new InvalidOperationException("Database is not specified");
                    }

                    _fileRepo = FileRepository.Create();
                }

                return _fileRepo;
            }
        }
        
    }
}
