using Core.Interfaces.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class Logger
    {
        private static ILogService? _instance;
        private static IServiceProvider? _serviceProvider;
        private static readonly object LockObject = new object();

        public static ILogService Instance
        {
            get
            {
                lock (LockObject)
                {
                    return _instance ?? GetService();
                }
            }
        }

        public static void Configure(IServiceProvider serviceProvider)
        {
            if (_instance == null)
            {
                _serviceProvider = serviceProvider;
                _instance = GetService();
            }
        }

        private static ILogService GetService()
        {
            if (_serviceProvider == null) throw new NullReferenceException(nameof(Logger) + Constants.Arrow + nameof(GetService) + Constants.Arrow + nameof(_serviceProvider) + "is null");
            
            return _serviceProvider
                    .CreateScope()
                    .ServiceProvider
                    .GetService<ILogService>() ??
                    throw new NullReferenceException(nameof(Logger) + Constants.Arrow + nameof(Configure) + Constants.Arrow + "can't get ILogService");
        }
    }
}
