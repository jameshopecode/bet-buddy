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
            1. When user ask about fixtures then use fixtures database to answer. Use Fixtures tools - SearchFixturesAsyncByType when user ask for specific type [Sport or Esport] about fixtures related thins. Use SearchFixturesAsync when not question related to type
            2. When user ask general question about gambling, betting, rules use your knowledge
            3. When using Fixtures tool, create `query` for SearchFixtures from user input to find matches/markets/teams/games for example convert user intent to
                3.1. barcelona match in LaLiga
                3.2. Esport match beetwen Heroic vs Mouze
                3.3. availiable marketes incloude handicap
                3.4. [COMPETITION: CHAMPIONS LEAGUE]
                3.5. [TYPE: Esport]
            4. Only answer on question related to fixtures, betting and gambling, sport and esport teames, competitions
            IMPORTANT!:
            1. ""answer"" should be written in a short form and it needs to be clear summary 
            2. If question is related to specific match/market - provided answer need to be limited to it and in ""metadata"" return data related only to it
            3. DON'T include additional matches/markets in ""answer"" if user did not ask for it
            4. DON'T use lists, enumerations, tables in ""answer"" 
            5. DON'T include any match id, market id, section id in ""answer"" 
            6. In response ""metadata"" field return from found in Fixtures database all matchIds and markets related to search result 
            7. If your are not asked explicit for all markets  limit markets in response to top 5. [RESPOND WITH VALID JSON]. Is dictionary object in json representation where key is matchId and value is markets array
            
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
            Kernel = kernel
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
            NumPredict = 6000
        }) };
        var watch = System.Diagnostics.Stopwatch.StartNew();
        await foreach (var message in _chatCompletionAgent.InvokeAsync(prompt,chatHistory, options: options))
        {
            response += message.Message.Content;
        }
        watch.Stop();
        Console.WriteLine($"LLM Processing time = {watch.Elapsed.Seconds}");
        return response;
    }

    public void Cleanup()
    {
        _chatHistoryDict.Clear();
    }
}