using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace bot_framework_extensions.Extension
{
    public static class DialogContextExtensions
    {
        public static async Task CancelDialogs(this DialogContext dialogContext, CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < dialogContext.Stack.Count; ++i)
                await dialogContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
