using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Domain.Message
{
    public interface IBotStrategy
    {
        Task<BotMessage> ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(BotEngineIdentity activeBotEngineInstance, IMessage incomingUserMessage);
        Task ResetCurrentlyActiveConversationNodeToInitialStartingPointAsync();
    }
}
