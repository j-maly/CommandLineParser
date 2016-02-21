using System.Resources;
using CommandLineParser.Arguments;

namespace CommandLineParser
{
    /// <summary>
    /// Interface for resources. Give your resource classes this interface so that 
    /// they can be used for <see cref="CommandLineParser.FillDescFromResource"/> call. 
    /// That way you can assign the <see cref="Argument.FullDescription"/> property of 
    /// your arguments defined using declarative syntax to strings in resource class. 
    /// In argument declarations, use the resource key as a value of FullDescription property
    /// and then call <see cref="CommandLineParser.FillDescFromResource"/> after the parser
    /// is initialized. It will replace the key with the string from resource in the 
    /// arguments. See help file for an example
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Gets the resource manager.
        /// </summary>
        /// <value>The resource manager.</value>
        ResourceManager ResourceManager { get; }
    }
}