using System;
using System.Collections.Generic;
using System.Text;

namespace bot_framework_extensions
{
    internal static class SetField
    {
        internal static void NotNull<T>(out T data, string name, T input)
        {
            if (input == null)
                throw new ArgumentException($"{name} field doesn't accept null");
            data = input;
        }
    }
}
