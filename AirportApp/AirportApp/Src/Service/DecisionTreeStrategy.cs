using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
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
            this.currentlyActiveConversationDecisionTreeNode = repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetById(1);
        }

        public BotMessage ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(BotEngineIdentity activeBotEngineInstance, IMessage incomingUserMessage)
        {
            string extractedTextContentFromIncomingUserMessage = incomingUserMessage.Text;

            FAQOption? selectedUserOptionMatchingIncomingMessageText = currentlyActiveConversationDecisionTreeNode.Options.FirstOrDefault((option) => option.Label.Equals(extractedTextContentFromIncomingUserMessage));
            if (selectedUserOptionMatchingIncomingMessageText == null)
            {
                return new BotMessage.BotMessageBuilder(activeBotEngineInstance, incomingUserMessage.GetChat(), CONSTANT_VALUE_REPRESENTING_UNASSIGNED_DATABASE_IDENTIFIER,
                    repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetById((int)BotStandardMessages.RestartConversation)).Build();
               
            }

            FAQNode nextQuestion = repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetById(selectedUserOptionMatchingIncomingMessageText.NextOptionId);
            currentlyActiveConversationDecisionTreeNode = nextQuestion;

            return new BotMessage.BotMessageBuilder(activeBotEngineInstance, incomingUserMessage.GetChat(), CONSTANT_VALUE_REPRESENTING_UNASSIGNED_DATABASE_IDENTIFIER, nextQuestion).Build();
        }

        public void ResetCurrentlyActiveConversationNodeToInitialStartingPoint()
        {
            currentlyActiveConversationDecisionTreeNode = repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetById(1);
        }
    }
}
