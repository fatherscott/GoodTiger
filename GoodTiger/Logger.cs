using NLog;
using System;

namespace GoodTiger
{
    public class _Logger
    {
        private NLog.Logger _logger {  get;  set;}

        public _Logger()
        {
            _logger = LogManager.GetLogger("basicLogger");
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Error(string message, Exception e)
        {
            _logger.Error(e, message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }
    }

    public class Logger
    {
        private static readonly Lazy<_Logger> lazy = new Lazy<_Logger>(() => new _Logger());
        public static _Logger Instance { get { return lazy.Value; } }
    }
}
