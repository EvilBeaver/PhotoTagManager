using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    public static class DatabaseService
    {
        private static IDatabase _serviceInstance;
        
        private static Dictionary<Type, object> _repos;

        static DatabaseService()
        {
            InitRepoCollection();
        }

        private static void InitRepoCollection()
        {
            _repos = new Dictionary<Type, object>();
            _repos[typeof(FileRepository)] = null;
            _repos[typeof(FavoritesRepository)] = null;
        }

        private static T InitRepository<T>()
        {
            if (GetInstance() == null)
            {
                throw new InvalidOperationException("Database is not specified");
            }

            var type = typeof(T);
            var instance = _repos[type];
            if (instance == null)
            {
                // ugly if-else. will be refactored if will cause problems
                if (type == typeof(FileRepository))
                {
                    instance = FileRepository.Create();
                }
                else if (type == typeof(FavoritesRepository))
                {
                    instance = FavoritesRepository.Create();
                }
                else
                {
                    throw new InvalidOperationException();
                }

                _repos[type] = instance;

            }

            return (T)instance;

        }

        public static void RegisterInstance(IDatabase instance)
        {
            if (_serviceInstance != null)
            {
                ShutdownInstance();
            }

            _serviceInstance = instance;
            _serviceInstance.Init();
        }

        public static IDatabase GetInstance()
        {
            return _serviceInstance;
        }

        public static void ShutdownInstance()
        {
            if (_serviceInstance != null)
            {
                _serviceInstance.Shutdown();
                foreach (var item in _repos.Values)
                {
                    if (item is IDisposable)
                    {
                        ((IDisposable)item).Dispose();
                    }
                }
                
                InitRepoCollection();
                _serviceInstance = null;
            }
        }

        internal static FileRepository FileRepository
        {
            get
            {
                return InitRepository<FileRepository>();
            }
        }

        internal static FavoritesRepository FavoritesRepository
        {
            get
            {
                return InitRepository<FavoritesRepository>();
            }
        }

    }
}
