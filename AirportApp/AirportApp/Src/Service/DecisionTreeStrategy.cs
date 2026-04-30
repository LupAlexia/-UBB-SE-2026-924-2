using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Model.Faq.Bot;
using AirportApp.Src.Model.Message;
using AirportApp.Src.Repository;
using AirportApp.Src.Repository.Database;

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
            this.currentlyActiveConversationDecisionTreeNode = repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetById(1);
        }

        public BotMessage ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(BotEngine activeBotEngineInstance, IMessage incomingUserMessage)
        {
            string extractedTextContentFromIncomingUserMessage = incomingUserMessage.GetMessage();

            FAQOption? selectedUserOptionMatchingIncomingMessageText = currentlyActiveConversationDecisionTreeNode.options.FirstOrDefault((option) => option.label.Equals(extractedTextContentFromIncomingUserMessage));
            if (selectedUserOptionMatchingIncomingMessageText == null)
            {
                return new BotMessage.BotMessageBuilder(activeBotEngineInstance, incomingUserMessage.GetChat(), CONSTANT_VALUE_REPRESENTING_UNASSIGNED_DATABASE_IDENTIFIER,
                    repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetById((int)BotStandardMessages.RESTART_CONVERSATION)).Build();
            }

            FAQNode nextQuestion = repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetById(selectedUserOptionMatchingIncomingMessageText.nextOptionId);
            currentlyActiveConversationDecisionTreeNode = nextQuestion;

            return new BotMessage.BotMessageBuilder(activeBotEngineInstance, incomingUserMessage.GetChat(), CONSTANT_VALUE_REPRESENTING_UNASSIGNED_DATABASE_IDENTIFIER, nextQuestion).Build();
        }

        public void ResetCurrentlyActiveConversationNodeToInitialStartingPoint()
        {
            currentlyActiveConversationDecisionTreeNode = repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetById(1);
        }
    }
}
