using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using Xunit.Abstractions;

namespace TestPluginContamination;

public class TestingPluginContaminationAndIsolation
{
    private readonly ITestOutputHelper _output;

    public TestingPluginContaminationAndIsolation(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData(15)] // Testing with 10 agents.
    public async Task PluginIsolationAcrossAgents(int agentCount)
    {
        for (int i = 1; i <= agentCount; i++)
        {
            string agentName = $"Agent_{i:00}";
            string uniquePluginInternalIdentifier = $"{agentName}_plugin";

            Kernel kernel = SKHelper.CreateKernel();
            kernel.CreateAndAddPlugin(uniquePluginInternalIdentifier);
            ChatCompletionAgent agent = SKHelper.CreateAgentWithKerne(kernel, agentName);

            // Invoke the plugin's function multiple times and verify the output.
            for (int j = 0; j < 3; j++)
            {
                //// ----- Direct Plugin Invocation -----
                //// Retrieve the plugin using its type name ("TestPlugin") and then its function "GetInternalIdentifier".
                //KernelFunction getInternalIdentifierFunction = kernel.Plugins["TestPlugin"]["GetInternalIdentifier"];

                //// Invoke using a new KernelArguments instance.
                //var invokeResult = 
                //    await getInternalIdentifierFunction.InvokeAsync(kernel, new KernelArguments());
                //string directResult = invokeResult.GetValue<string>();

                //// Log output for direct plugin resolution.
                //Console.WriteLine($"Direct invocation for {agentName}: {directResult}");

                //// The direct result should exactly equal the unique internal identifier.
                //Assert.Equal(uniquePluginInternalIdentifier, directResult);

                // ----- Agent-based Plugin Resolution -----
                // The agent is configured to auto–invoke kernel functions when the input matches its instructions.
                // We send a message with the expected plugin identifier, so that the agent
                // should use its kernel (and the registered plugin) to return exactly that identifier.
                ChatMessageContent message = new ChatMessageContent(AuthorRole.User, "Give me the plugin internal identifier and just that please.");

                string combinedResponse = "";
                /// Create the chat history thread to capture the agent interaction.
                AgentThread thread = new ChatHistoryAgentThread();

                await foreach (AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(message, thread))
                {
                    combinedResponse += response.Message.Content;
                    thread = response.Thread;
                }
                combinedResponse = combinedResponse.Trim();

                // Log the agent's response.
                //Console.WriteLine($"Agent response for {agentName}: {combinedResponse}");
                _output.WriteLine($"Agent response for {agentName}: {combinedResponse}");

                // The agent's full response should be exactly the unique internal identifier.
                Assert.Equal(uniquePluginInternalIdentifier, combinedResponse);
            }
        }
    }
}