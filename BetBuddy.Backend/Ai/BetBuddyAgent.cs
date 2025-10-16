using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace BetBuddy.Backend.Api.Ai;

public interface IBetBuddyAgent
{
    Task<string> Interact(string prompt);
}
public class BetBuddyAgent : IBetBuddyAgent
{
    private readonly Kernel kernel;
    private readonly ChatCompletionAgent _chatCompletionAgent;
    public BetBuddyAgent(Kernel kernel)
    {
        this.kernel = kernel;
        _chatCompletionAgent = new ChatCompletionAgent
        {
            Name = "BetBuddy",
            Instructions = @"You are betting and gambling assistant with access to our fixtures database.
            INSTRUCTIONS:
            1. When user ask about fixtures then use fixtures database to answer
            2. When user ask general question about gambling, betting, rules use your knowledge
            3. Only answer on question related to fixtures, betting and gambling
            4. answer should be written in a short form and it needs to be clear summary. IMPORTANT! Don't use lists, enumerations, tables.
            5. Answer must be ready in format to text-to-speech

            RESPOND WITH VALID JSON:
            {
                ""answer"":""answer for general question"",
                ""metadata"" : [
{""fixture_id"":123, ""markets"":[]},
{""fixture_id"":1234, ""markets"":[]}
                ]
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

    public async Task<string> Interact(string prompt)
    {
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage(prompt);

        var allResponses = new List<ChatMessageContent>();
        var response = "";
        await foreach (var message in _chatCompletionAgent.InvokeAsync(prompt))
        {
            response += message.Message.Content;
            chatHistory.Add(message);
            allResponses.Add(message.Message);
        }

        return response;
    }
}