using AirportApp.ClassLibrary.Entity.Dto;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.Message
{
    public sealed partial class MessageView : UserControl
    {
        public MessageDTO DataTransferObjectContainingMessageDetailsForViewModelBinding => (MessageDTO)DataContext;

        public MessageView()
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