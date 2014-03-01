using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BeaverSoft.Common.UI
{
    internal class MutexGuard : IDisposable
    {
        private Mutex _mtx;
        private string _mtxName;
        private int _ownershipCounter;
        
        public MutexGuard(string MutexName)
        {
            _mtx = new Mutex(false, MutexName);
            _mtxName = MutexName;
            _ownershipCounter = 0;
        }


        public void Lock()
        {
            try
            {
                _mtx.WaitOne();
            }
            catch (AbandonedMutexException)
            {
            }

            _ownershipCounter++;
            
        }

        public void Release()
        {
            lock (_mtx)
            {
                if (_ownershipCounter > 0)
                {
                    _ownershipCounter--;
                    _mtx.ReleaseMutex();
                }
            }
        }

        public bool Lock(int timeout)
        {
            bool result;

            try
            {
                result = _mtx.WaitOne(timeout);
            }
            catch (AbandonedMutexException)
            {
                result = true;
            }

            if (result)
            {
                _ownershipCounter++;
            }

            return result;
        }


        #region IDisposable Members

        public void Dispose()
        {
            Release();
            _mtx.Dispose();
            _ownershipCounter = 0;
        }

        #endregion
    }
}
