using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace BetBuddy.Backend.Api.Ai;

public interface IBetBuddyAgent
{
    Task<string> Interact(string prompt, string modelUserId);
}
public class BetBuddyAgent : IBetBuddyAgent
{
    private readonly Kernel kernel;
    private readonly ChatCompletionAgent _chatCompletionAgent;
    private readonly Dictionary<string, ChatHistory> _chatHistoryDict;
    public BetBuddyAgent(Kernel kernel)
    {
        _chatHistoryDict = new Dictionary<string, ChatHistory>();
        this.kernel = kernel;
        _chatCompletionAgent = new ChatCompletionAgent
        {
            Name = "BetBuddy",
            Instructions = @"You are betting and gambling assistant with access to our Fixtures database.
            INSTRUCTIONS:
            1. When user ask about fixtures then use fixtures database to answer
            2. When user ask general question about gambling, betting, rules use your knowledge
            3. Only answer on question related to fixtures, betting and gambling
            4. answer should be written in a short form and it needs to be clear summary. IMPORTANT! Don't use lists, enumerations, tables.
            5. Answer must be ready in format to text-to-speech
6. In response ""metadata"" field return from found in Fixtures database all matchIds and related markets as is provided in [RESPOND WITH VALID JSON]. Is dictionary object in json representation where key is matchId and value is markets array

            RESPOND WITH VALID JSON:
            {
                ""answer"":""answer for general question"",
                ""metadata"" : {
                                  ""matchId1"": [
                                    101,
                                    102,
                                    103
                                  ],
                                  ""matchId2"": [
                                    201,
                                    202
                                  ],
,
                                  ""matchId3"": [
                                    301,
                                    302
                                  ]
                                }
            }

        EXAMPLE RESPOND:
            {
                ""answer"":""answer for general question"",
                ""metadata"" : {
                                  ""1"": [
                                    101,
                                    102,
                                    103
                                  ],
                                  ""2"": [
                                    201,
                                    202
                                  ]
                                }
            }
",
            Kernel = kernel,
            Arguments = new KernelArguments(new OllamaPromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                Temperature = 1
            })
        };
    }

    public async Task<string> Interact(string prompt, string userId)
    {
        ChatHistory? chatHistory;
        if (!_chatHistoryDict.TryGetValue(userId, out chatHistory))
        {
            chatHistory = new ChatHistory();
            _chatHistoryDict.Add(userId, chatHistory);
        }

        chatHistory.AddUserMessage(prompt);

        var allResponses = new List<ChatMessageContent>();
        var response = "";
        var options = new AgentInvokeOptions { KernelArguments = new KernelArguments(new OllamaPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = 1
        }) };
        await foreach (var message in _chatCompletionAgent.InvokeAsync(prompt, options: options))
        {
            response += message.Message.Content;
            chatHistory.Add(message);
            allResponses.Add(message.Message);
        }

        return response;
    }
}