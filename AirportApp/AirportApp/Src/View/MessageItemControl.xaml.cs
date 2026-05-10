using AirportApp.ClassLibrary.Entity.Dto;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.Message
{
    public sealed partial class MessageControl : UserControl
    {
        public MessageDTO DataTransferObjectContainingMessageDetailsForViewModelBinding => (MessageDTO)DataContext;

        public MessageControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (senderObjectTriggeringEvent, eventArgumentsContainingDataContextInformation) =>
            {
                if (eventArgumentsContainingDataContextInformation.NewValue is MessageDTO)
                {
                    this.Bindings.Update();
                }
            };
        }
    }
}