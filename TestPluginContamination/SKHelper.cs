using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPluginContamination;

public static class SKHelper
{

    //create a semantic kernel and return it
    public static Kernel CreateKernel()
    {
        var builder = Kernel.CreateBuilder();
        Kernel kernel = builder.AddAzureOpenAIChatCompletion( 
            
                                deploymentName: EnvironmentWellKnown.DeploymentName,
                                endpoint: EnvironmentWellKnown.Endpoint,
                                apiKey: EnvironmentWellKnown.ApiKey)
                            .Build();
        return kernel;
    }

    public static void CreateAndAddPlugin(this Kernel kernel, string pluginIdentifier)
    {
        // Create the instance of the plugin with the private identifier and Add the plugin to the kernel
        var testPlugin = new TestPlugin(pluginIdentifier);
        kernel.Plugins.AddFromObject(testPlugin);
    }

    public static ChatCompletionAgent CreateAgentWithKerne(Kernel kernel, string agentIdentifier)
    {
        return new ChatCompletionAgent
        {
            Name = agentIdentifier,
            Instructions = "You are a helpful and concise agent ready to help. you will be asked by the internal plugin identifier. return just that, nothing more, nothing else. just the identifier. no extra characters. return 0 if you cannot return the identifier.",
            Kernel = kernel,
            Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                })
        };
    }

}
