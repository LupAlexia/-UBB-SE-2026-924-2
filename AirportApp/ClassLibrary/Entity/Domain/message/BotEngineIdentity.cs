using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Domain.Message
{
    [NotMapped]
    public class BotEngineIdentity : Sender
    {
        public const int CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER = -1; // ChatBot is always identified as the first
        private IBotStrategy activeStrategyForFormulatingBotResponses;

        protected BotEngineIdentity()
            : base(CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER, "Carlos", "customer-support@cloudspritzers.com")
        {
            activeStrategyForFormulatingBotResponses = null!;
            Discriminator = "Bot";
        }

        public BotEngineIdentity(IBotStrategy responseStrategy)
            : base(CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER, "Carlos", "customer-support@cloudspritzers.com")
        {
            this.activeStrategyForFormulatingBotResponses = responseStrategy;
            Discriminator = "Bot";
        }

        public async Task<BotMessage> GenerateAppropriateResponseBasedOnCurrentStrategyAsync(IMessage message)
        {
            return await activeStrategyForFormulatingBotResponses.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(this, message);
        }

        public override string RetrieveConfiguredEmailAddressForBotContact()
        {
            return "customer-support@cloudspritzers.com";
        }

        public override string RetrieveConfiguredDisplayFullNameForBot()
        {
            return "Carlos";
        }

        public override int RetrieveUniqueDatabaseIdentifierForBot()
        {
            return CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER;
        }

        public async Task ResetBotConversationStateToInitialRootNodeAsync()
        {
            await activeStrategyForFormulatingBotResponses.ResetCurrentlyActiveConversationNodeToInitialStartingPointAsync();
        }
    }
}
