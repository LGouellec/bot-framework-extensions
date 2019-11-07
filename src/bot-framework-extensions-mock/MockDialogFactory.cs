using System;
using ai_chatbot_support_crosscutting.Dialog;
using Autofac;

namespace ai_chatbot_support_mock
{
    internal class MockServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return null;
        }
    }

    public class MockDialogFactory : DialogFactory
    {
        public ILifetimeScope Scope { get; set; }

        public MockDialogFactory() 
            : base(new MockServiceProvider())
        {
        }

        protected override object ResolveByIoC(Type serviceType)
        {
            if (_serviceProvider != null)
            {
                try
                {
                    return _serviceProvider.GetService(serviceType);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
                return null;
        }
    }
}
