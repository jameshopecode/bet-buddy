using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace BetBuddy.Backend.Api.Ai;

public interface IBetBuddyAgent
{
    Task<string> Interact(string prompt, string modelUserId);
    void Cleanup();
}
public class BetBuddyAgent : IBetBuddyAgent
{
    private readonly Kernel kernel;
    private readonly ChatCompletionAgent _chatCompletionAgent;
    private readonly Dictionary<string, ChatHistoryAgentThread> _chatHistoryDict;
    public BetBuddyAgent(Kernel kernel)
    {
        _chatHistoryDict = new Dictionary<string, ChatHistoryAgentThread>();
        this.kernel = kernel;
        _chatCompletionAgent = new ChatCompletionAgent
        {
            Name = "BetBuddy",
            Instructions = @"You are betting and gambling assistant with access to our Fixtures database.
            INSTRUCTIONS:
            1. When user ask about fixtures then use fixtures database to answer
            2. When user ask general question about gambling, betting, rules use your knowledge
            3. Analyze matches to identity game if is sport or e-sport. 
            4. Only answer on question related to fixtures, betting and gambling
            5. Answer should be written in a short form and it needs to be clear summary. If question is related to specific match/market - provided answer need to be limited to it and in metadata return data related only to it. IMPORTANT! Don't use lists, enumerations, tables. Don't include in answer any match id, market id, section id!! DON'T include all markets if user don't ask for it
            6. Answer must be ready in format to text-to-speech
            7. In response ""metadata"" field return from found in Fixtures database all matchIds and markets related to search result, if your are not asked explicit for all markets  limit markets in response to top 5. [RESPOND WITH VALID JSON]. Is dictionary object in json representation where key is matchId and value is markets array
            
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
                Temperature = 0.6f
            })
        };
    }

    public async Task<string> Interact(string prompt, string userId)
    {
        ChatHistoryAgentThread? chatHistory;
        if (!_chatHistoryDict.TryGetValue(userId, out chatHistory))
        {
            chatHistory = new ChatHistoryAgentThread();
            _chatHistoryDict.Add(userId, chatHistory);
        }
        
        var response = "";
        var options = new AgentInvokeOptions { KernelArguments = new KernelArguments(new OllamaPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = 0.6f
        }) };
        
        await foreach (var message in _chatCompletionAgent.InvokeAsync(prompt,chatHistory, options: options))
        {
            response += message.Message.Content;
        }

        return response;
    }

    public void Cleanup()
    {
        _chatHistoryDict.Clear();
    }
}