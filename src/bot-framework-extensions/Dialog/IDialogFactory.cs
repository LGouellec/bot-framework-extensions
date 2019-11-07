using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace bot_framework_extensions.Dialog
{
    public interface IDialogFactory
    {
        IDialogFactory UseDialogAccessor(IStatePropertyAccessor<DialogState> state);
        DialogSet Build();
        IDialogFactory Create<T>() where T : Microsoft.Bot.Builder.Dialogs.Dialog;
        IDialogFactory Create<T>(string dialogId) where T : Microsoft.Bot.Builder.Dialogs.Dialog;
        IDialogFactory Create<T>(string dialogId, params object[] parameters) where T : Microsoft.Bot.Builder.Dialogs.Dialog;

    }
}
