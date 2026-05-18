using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service.Bot.Strategy
{
    public class DecisionTreeStrategy : IBotStrategy
    {
        private const int CONSTANT_VALUE_REPRESENTING_UNASSIGNED_DATABASE_IDENTIFIER = -1;

        [AllowNull]
        private FAQNode currentlyActiveConversationDecisionTreeNode;

        private readonly IDecisionTreeService decisionTreeService;

        public DecisionTreeStrategy(IDecisionTreeService decisionTreeService)
        {
            this.decisionTreeService = decisionTreeService;
        }

        private async Task EnsureInitializedAsync()
        {
            if (currentlyActiveConversationDecisionTreeNode == null)
            {
                currentlyActiveConversationDecisionTreeNode = await decisionTreeService.GetNodeByIdAsync(1);
            }
        }

        public async Task<BotMessage> ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(BotEngineIdentity activeBotEngineInstance, IMessage incomingUserMessage)
        {
            await EnsureInitializedAsync();

            string extractedTextContentFromIncomingUserMessage = incomingUserMessage.Text;

            FAQOption? selectedUserOptionMatchingIncomingMessageText = currentlyActiveConversationDecisionTreeNode.Options
                .FirstOrDefault(option => option.Label.Equals(extractedTextContentFromIncomingUserMessage));

            if (selectedUserOptionMatchingIncomingMessageText == null)
            {
                var restartNode = await decisionTreeService.GetNodeByIdAsync((int)BotStandardMessages.RestartConversation);
                return new BotMessage.BotMessageBuilder(activeBotEngineInstance, incomingUserMessage.GetChat(), CONSTANT_VALUE_REPRESENTING_UNASSIGNED_DATABASE_IDENTIFIER, restartNode).Build();
            }

            FAQNode nextQuestion = selectedUserOptionMatchingIncomingMessageText.NextOption
                ?? await decisionTreeService.GetNodeByIdAsync(1);

            if (nextQuestion != null)
            {
                nextQuestion = await decisionTreeService.GetNodeByIdAsync(nextQuestion.NodeId);
            }

            currentlyActiveConversationDecisionTreeNode = nextQuestion;

            return new BotMessage.BotMessageBuilder(activeBotEngineInstance, incomingUserMessage.GetChat(), CONSTANT_VALUE_REPRESENTING_UNASSIGNED_DATABASE_IDENTIFIER, nextQuestion).Build();
        }

        public async Task ResetCurrentlyActiveConversationNodeToInitialStartingPointAsync()
        {
            currentlyActiveConversationDecisionTreeNode = await decisionTreeService.GetNodeByIdAsync(1);
        }
    }
}