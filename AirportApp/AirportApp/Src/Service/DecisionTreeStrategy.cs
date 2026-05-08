using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Entity.Domain.Message;

namespace AirportApp.Src.Service.Bot.Strategy
{
    public class DecisionTreeStrategy : IBotStrategy
    {
        private const int CONSTANT_VALUE_REPRESENTING_UNASSIGNED_DATABASE_IDENTIFIER = -1;
        [AllowNull]
        private FAQNode currentlyActiveConversationDecisionTreeNode;

        private IRepository<int, FAQNode> repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes;

        public DecisionTreeStrategy(IRepository<int, FAQNode> faqRepository)
        {
            this.repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes = faqRepository;
            // Initialization of the first node is deferred to first use via async
        }

        private async Task EnsureInitializedAsync()
        {
            if (currentlyActiveConversationDecisionTreeNode == null)
            {
                currentlyActiveConversationDecisionTreeNode = await repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetByIdAsync(1);
            }
        }

        public async Task<BotMessage> ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(BotEngineIdentity activeBotEngineInstance, IMessage incomingUserMessage)
        {
            await EnsureInitializedAsync();

            string extractedTextContentFromIncomingUserMessage = incomingUserMessage.Text;

            FAQOption? selectedUserOptionMatchingIncomingMessageText = currentlyActiveConversationDecisionTreeNode.Options.FirstOrDefault((option) => option.label.Equals(extractedTextContentFromIncomingUserMessage));
            if (selectedUserOptionMatchingIncomingMessageText == null)
            {
                var restartNode = await repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetByIdAsync((int)BotStandardMessages.RestartConversation);
                return new BotMessage.BotMessageBuilder(activeBotEngineInstance, incomingUserMessage.GetChat(), CONSTANT_VALUE_REPRESENTING_UNASSIGNED_DATABASE_IDENTIFIER, restartNode).Build();
            }

            FAQNode nextQuestion = await repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetByIdAsync(selectedUserOptionMatchingIncomingMessageText.nextOptionId);
            currentlyActiveConversationDecisionTreeNode = nextQuestion;

            return new BotMessage.BotMessageBuilder(activeBotEngineInstance, incomingUserMessage.GetChat(), CONSTANT_VALUE_REPRESENTING_UNASSIGNED_DATABASE_IDENTIFIER, nextQuestion).Build();
        }

        public async Task ResetCurrentlyActiveConversationNodeToInitialStartingPointAsync()
        {
            currentlyActiveConversationDecisionTreeNode = await repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetByIdAsync(1);
        }
    }
}
