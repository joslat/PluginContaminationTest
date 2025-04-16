using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace TestPluginContamination;

class TestPlugin
{
    private readonly string _internalPluginIdentifier;

    public TestPlugin(string pluginIdentifier)
    {
        _internalPluginIdentifier = pluginIdentifier;
    }

    [KernelFunction, Description("Returns the plugin internal identifier.")]
    public string GetInternalIdentifier()
    {
        return _internalPluginIdentifier;
    }
}
