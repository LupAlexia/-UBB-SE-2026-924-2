using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Domain.Message
{
    public interface IBotStrategy
    {
        BotMessage ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(BotEngine activeBotEngineInstance, IMessage incomingUserMessage);

        public void ResetCurrentlyActiveConversationNodeToInitialStartingPoint();
    }
}
