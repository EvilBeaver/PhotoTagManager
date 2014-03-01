using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading;
using System.Net;
using System.Windows.Threading;
using System.Windows;

namespace BeaverSoft.Common.Updates
{
    class UpdateChecker
    {

        public void CheckUpdates(UpdateCheckerResultHandler ResultHandler)
        {
            CheckUpdatesAsync(ResultHandler);
        }

        public IEnumerable<string> UrlsToCheck { get; set; }

        class RequestState
        {
            public WebRequest request;
            public WebResponse response;
            public UpdateCheckerResultHandler ResultHandler;
            public Queue<string> Urls = new Queue<string>();
            public string LastError;
        }

        private static AsyncCallback GUIContextCallback(AsyncCallback initialCallback)
        {
            SynchronizationContext sc = SynchronizationContext.Current;
            if (sc == null)
            {
                return initialCallback;
            }

            return (asyncResult)=>
                {
                    sc.Post((result)=>
                        {
                            initialCallback((IAsyncResult)result);
                        }, asyncResult);
                };

        }

        private void CheckUpdatesAsync(UpdateCheckerResultHandler ResultHandler)
        {
            RequestState state = new RequestState();

            foreach (var url in UrlsToCheck)
            {
                state.Urls.Enqueue(url);
            }

            if (state.Urls.Count == 0)
            {
                throw new InvalidOperationException("No update URLS defined");
            }

            CheckUpdateOnQueueAsync(ResultHandler, state);

        }

        private void CheckUpdateOnQueueAsync(UpdateCheckerResultHandler ResultHandler, RequestState state)
        {
            var reqState = (RequestState)state;
            string url;
            try
            {
                url = reqState.Urls.Dequeue();
            }
            catch (InvalidOperationException)
            {
                var result = new UpdateCheckerResult();
                result.Success = false;
                if (reqState.LastError != null)
                {
                    throw new UpdaterException(reqState.LastError);
                }
                else
                {
                    throw new UpdaterException("Ни один из адресов автообновления не вернул корректный ответ");
                }
                
            }

            HttpWebRequest client = (HttpWebRequest)HttpWebRequest.Create(url);
            client.Accept = "application/xml";
            client.UserAgent = "DBManager Autoupdate " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            client.AllowAutoRedirect = true;
            client.Proxy = WebRequest.DefaultWebProxy;
            client.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            
            state.request = client;
            state.ResultHandler = ResultHandler;

            client.BeginGetResponse(GUIContextCallback(DownloadFileCompleted), reqState);
        }

        void DownloadFileCompleted(IAsyncResult asynchronousResult)
        {
            RequestState state = (RequestState)asynchronousResult.AsyncState;

            UpdateCheckerResult CheckResult = new UpdateCheckerResult();

            try
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)state.request;
                state.response = (HttpWebResponse)myHttpWebRequest.EndGetResponse(asynchronousResult);
                if (state.response.ContentType == "application/xml")
                {
                    CheckResult.Updates = LoadResult(state.response);
                    CheckResult.Success = true;
                }
                else
                {
                    CheckUpdateOnQueueAsync(state.ResultHandler, state);
                    return;
                }
            }
            catch (Exception exc)
            {
                try
                {
                    state.LastError = exc.ToString();
                    CheckUpdateOnQueueAsync(state.ResultHandler, state);
                    return;
                }
                catch (Exception innerExc)
                {
                    CheckResult.Exception = innerExc;
                    CheckResult.Success = false;
                }
            }
            finally
            {
                if(state.response != null)
                    state.response.Close();
            }

            state.ResultHandler(this, CheckResult);

        }

        UpdateLog LoadResult(WebResponse response)
        {
            XDocument xmlDoc;

            using (var stream = response.GetResponseStream())
            {
                try
                {
                    xmlDoc = XDocument.Load(stream);
                }
                catch (Exception exc)
                {
                    throw new UpdaterException("Некорректные данные об обновлении", exc);
                }
            }

            var VersionList = xmlDoc.Root.Elements("version");
            var currentVer = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            UpdateLog log = new UpdateLog();

            foreach (var vDeclaration in VersionList)
            {
                Version inFile = Version.Parse(vDeclaration.Attribute("number").Value);
                if (currentVer < inFile)
                {
                    UpdateDefinition upd = new UpdateDefinition();
                    upd.Version = inFile.ToString();
                    upd.Url = vDeclaration.Element("url").Value;
                    upd.News = vDeclaration.Element("news").Value;
                    log.Add(upd);
                }
            }

            return log;
        }

    }

    class UpdateCheckerResult
    {
        public bool Success { get; set; }
        public UpdateLog Updates { get; set; }
        public Exception Exception { get; set; }
    }

    internal delegate void UpdateCheckerResultHandler(UpdateChecker sender, UpdateCheckerResult result);

    class UpdateDefinition
    {
        public string Version { get; set; }
        public string News { get; set; }
        public string Url { get; set; }
    }

    class UpdateLog : IEnumerable<UpdateDefinition>
    {
        
        public void Add(UpdateDefinition item)
        {
            m_List.Add(item);
        }

        public int Count
        {
            get
            {
                return m_List.Count;
            }
        }

        List<UpdateDefinition> m_List = new List<UpdateDefinition>();

        #region IEnumerable<UpdateDefinition> Members

        public IEnumerator<UpdateDefinition> GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        #endregion
    }

    public class UpdaterException : Exception
    {
        public UpdaterException(string message) : this(message,null)
        {
            
        }

        public UpdaterException(Exception inner) : this(inner.Message, inner)
        {
            
        }

        public UpdaterException(string message, Exception inner):base(message,inner)
        {

        }
    }

    public class AutoUpdateActivator
    {
        private DispatcherTimer _dispatcherTimer;
        private UpdateSettings _settings;

        public AutoUpdateActivator(UpdateSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException();
            }

            _settings = settings;
        }

        public void Activate()
        {
            if (_settings.LastUpdateCheck.Date != DateTime.Now.Date)
            {
                ActivateInternal();
            }
        }

        private void ActivateInternal()
        {
            if (_settings.CheckDelayInterval > 0)
            {
                _dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                _dispatcherTimer.Tick += dispatcherTimer_Tick;

                _dispatcherTimer.Interval = TimeSpan.FromSeconds(_settings.CheckDelayInterval);

                _dispatcherTimer.Start();
            }
            else
            {
                PerformCheck();
            }
        }

        public void Deactivate()
        {
            try
            {
                if (_dispatcherTimer != null)
                {
                    _dispatcherTimer.Stop();
                    _dispatcherTimer = null;
                }
            }
            catch
            {
            #if DEBUG
                throw;
            #endif
            }
        }

        public event EventHandler<UpdateCheckCompletedEventArgs> CheckCompleted;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var Timer = sender as System.Windows.Threading.DispatcherTimer;

            Timer.Stop();

            PerformCheck();

        }

        private void PerformCheck()
        {
            UpdateChecker chk = new UpdateChecker();
            try
            {
                chk.UrlsToCheck = _settings.UrlsToCheck;
                chk.CheckUpdates(UpdateCheckerCallback);
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }
        }

        private void UpdateCheckerCallback(UpdateChecker uc, UpdateCheckerResult result)
        {
            try
            {

                if (!result.Success) return;

                if (CheckCompleted != null)
                {
                    CheckCompleted(this, new UpdateCheckCompletedEventArgs()
                        {
                            HasUpdates = result.Updates.Count > 0
                        });
                }

                if (result.Updates.Count > 0)
                {
                    var answer = MessageBox.Show("Обнаружены новые версии. Обновить программу?", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (answer == MessageBoxResult.Yes)
                    {
                        var UpdWnd = new UpdatesWnd();
                        UpdWnd.Updates = result.Updates;
                        UpdWnd.Show();
                    }
                }

            }
            catch
            {
            #if DEBUG
                throw;
            #endif
            }
        }

    }

    public class UpdateSettings
    {
        public IEnumerable<string> UrlsToCheck { get; set; }
        public DateTime LastUpdateCheck { get; set; }
        public int CheckDelayInterval { get; set; }
    }

    public class UpdateCheckCompletedEventArgs : EventArgs
    {
        public bool HasUpdates { get; set; }
    }

}
