using System.ComponentModel;
using System.Runtime.CompilerServices;
using DevExpress.ExpressApp;

namespace ExemploChurrasqueira.Module.Helper
{
    public static class ToastHelper
    {
        public static MessageOptions Toast(string message, InformationType type)
        {
            MessageOptions options = new MessageOptions();
            options.Message = message;
            options.Duration = 5000;
            options.Type = type;

            return options;
        }
    }
}
