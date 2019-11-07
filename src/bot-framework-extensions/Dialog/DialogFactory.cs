using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.Bot.Builder;

namespace bot_framework_extensions.Dialog
{
    public class DialogFactory : IDialogFactory
    {
        protected readonly IServiceProvider _serviceProvider;
        private DialogSet _dialogs = null;

        public DialogFactory(IServiceProvider serviceProvider)
        {
            SetField.NotNull(out _serviceProvider, nameof(serviceProvider), serviceProvider);
        }

        public DialogSet Build() => _dialogs;

        public IDialogFactory Create<T>()
            where T : Microsoft.Bot.Builder.Dialogs.Dialog
        {
            return Create<T>(typeof(T).Name);
        }

        public IDialogFactory Create<T>(string dialogId, params object[] parameters) 
            where T : Microsoft.Bot.Builder.Dialogs.Dialog
        {
            if (_dialogs.Find(dialogId) == null)
            {
                var dialog = CreateDialog<T>(dialogId, parameters);
                _dialogs.Add(dialog);
            }
            return this;
        }

        public IDialogFactory Create<T>(string dialogId) where T : Microsoft.Bot.Builder.Dialogs.Dialog
        {
            if (_dialogs.Find(dialogId) == null)
            {
                var dialog = CreateDialog<T>(dialogId, null);
                _dialogs.Add(dialog);
            }

            return this;
        }

        public IDialogFactory UseDialogAccessor(IStatePropertyAccessor<DialogState> state)
        {
            _dialogs = new DialogSet(state);
            return this;
        }

        private T CreateDialog<T>(string diaglogId, params object[] parameters)
            where T : Microsoft.Bot.Builder.Dialogs.Dialog
        {
            List<object> ctorParameters = new List<object>();
            
            var ctor = typeof(T).GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

            foreach(var param in ctor.GetParameters())
            {
                if (param.ParameterType.Equals(typeof(string)) && param.Name.ToLower().Contains("id"))
                    ctorParameters.Add(diaglogId);
                else if (parameters != null && parameters.Select(o => o.GetType()).Contains(param.ParameterType))
                    ctorParameters.Add(parameters.FirstOrDefault(o => o.GetType().Equals(param.ParameterType)));
                else
                    ctorParameters.Add(ResolveByIoC(param.ParameterType));
            }

            T instance = Activator.CreateInstance(typeof(T), ctorParameters.ToArray()) as T;
            return instance;
        }

        protected virtual object ResolveByIoC(Type serviceType) =>
            _serviceProvider != null ? _serviceProvider.GetService(serviceType) : null;
    }
}
