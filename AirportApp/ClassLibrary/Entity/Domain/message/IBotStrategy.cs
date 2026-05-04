using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Domain.Message
{
    public interface IBotStrategy
    {
        BotMessage ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(BotEngineIdentity activeBotEngineInstance, IMessage incomingUserMessage);
        //BotMessage ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(BotEngineIdentity botEngine, IMessage message);
        public void ResetCurrentlyActiveConversationNodeToInitialStartingPoint();
    }
}
