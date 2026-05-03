using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Message;

namespace AirportApp.Src.Service.Bot.Strategy
{
    public interface IBotStrategy
    {
        BotMessage ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(BotEngine activeBotEngineInstance, IMessage incomingUserMessage);

        public void ResetCurrentlyActiveConversationNodeToInitialStartingPoint();
    }
}
