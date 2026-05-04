using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Domain.Message
{
    public class BotEngineIdentity : ISender
    {
        public const int CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER = 0; // ChatBot is always identified as the first
        private IBotStrategy activeStrategyForFormulatingBotResponses;

        public BotEngineIdentity(IBotStrategy responseStrategy)
        {
            this.activeStrategyForFormulatingBotResponses = responseStrategy;
        }

        public async Task<BotMessage> GenerateAppropriateResponseBasedOnCurrentStrategyAsync(IMessage message)
        {
            return await activeStrategyForFormulatingBotResponses.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(this, message);
        }

        public string RetrieveConfiguredEmailAddressForBotContact()
        {
            return "customer-support@cloudspritzers.com";
        }

        public string RetrieveConfiguredDisplayFullNameForBot()
        {
            return "Carlos";
        }

        public int RetrieveUniqueDatabaseIdentifierForBot()
        {
            return CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER;
        }

        public async Task ResetBotConversationStateToInitialRootNodeAsync()
        {
            await activeStrategyForFormulatingBotResponses.ResetCurrentlyActiveConversationNodeToInitialStartingPointAsync();
        }
    }
}
