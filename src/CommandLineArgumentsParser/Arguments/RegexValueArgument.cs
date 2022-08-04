using System;
using System.Text.RegularExpressions;
using CommandLineParser.Compatibility;
using CommandLineParser.Exceptions;

namespace CommandLineParser.Arguments;

/// <summary>
/// Use RegexValueArgument for an argument whose value must match a regular expression. 
/// </summary>
public class RegexValueArgument : CertifiedValueArgument<string>
{
    /// <summary>
    /// Regular expression which the value must match 
    /// </summary>
    public Regex? Regex { get; private set; }

    /// <summary>
    /// Sample value that would be displayed to the user as a suggestion when the user enters a wrong value. 
    /// </summary>
    public string? SampleValue { get; private set; }

    #region constructor

    /// <summary>
    /// Creates new argument with a <see cref="Argument.ShortName">short name</see>,
    /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>.
    /// </summary>
    /// <param name="shortName">Short name of the argument</param>
    /// <param name="regex">regular expressin which the value must match</param>
    public RegexValueArgument(char shortName, Regex regex) : base(shortName)
    {
        Regex = regex;
    }

    /// <summary>
    /// Creates new argument with a <see cref="Argument.ShortName">short name</see>,
    /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>.
    /// </summary>
    /// <param name="longName">Long name of the argument </param>
    /// <param name="regex">regular expressin which the value must match</param>
    public RegexValueArgument(string longName, Regex regex) : base(longName)
    {
        Regex = regex;
    }

    /// <summary>
    /// Creates new argument with a <see cref="Argument.ShortName">short name</see>,
    /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>.
    /// </summary>
    /// <param name="shortName">Short name of the argument</param>
    /// <param name="longName">Long name of the argument </param>
    /// <param name="regex">regular expressin which the value must match</param>
    public RegexValueArgument(char shortName, string longName, Regex regex) : base(shortName, longName)
    {
        Regex = regex;
    }

    /// <summary>
    /// Creates new argument with a <see cref="Argument.ShortName">short name</see>,
    /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>.
    /// </summary>
    /// <param name="shortName">Short name of the argument</param>
    /// <param name="longName">Long name of the argument </param>
    /// <param name="description">description of the argument</param>
    /// <param name="regex">regular expressin which the value must match</param>
    public RegexValueArgument(char shortName, string longName, string description, Regex regex) : base(shortName, longName, description)
    {
        Regex = regex;
    }
    #endregion

    protected override void Certify(string value)
    {
        // override the Certify method to validate value against regex
        if (Regex != null)
        {
            if (!Regex.IsMatch(value))
            {
                if (SampleValue == null)
                {
                    throw new CommandLineArgumentOutOfRangeException($"Argument '{value}' does not match the regex pattern '{Regex}'.", Name);
                }

                throw new CommandLineArgumentOutOfRangeException($"Argument '{value}' does not match the regex pattern '{Regex}'. An example of a valid value would be '{SampleValue}'.", Name);
            }
        }
    }
}

/// <summary>
/// <para>
/// Attribute for declaring a class' field a <see cref="RegexValueArgumentAttribute"/> and 
/// thus binding a field's value to a certain command line switch argument.
/// </para>
/// <para>
/// Instead of creating an argument explicitly, you can assign a class' field an argument
/// attribute and let the CommandLineParse take care of binding the attribute to the field.
/// </para>
/// </summary>
/// <remarks>Applicable to fields and properties (public).</remarks>
/// <remarks>Use <see cref="CommandLineParser.ExtractArgumentAttributes"/> for each object 
/// you where you have declared argument attributes.</remarks>
public sealed class RegexValueArgumentAttribute : ArgumentAttribute
{
    private readonly Type _argumentType;

    /// <summary>
    /// Creates new instance of RegexValueArgumentAttribute. RegexValueArgumentAttribute
    /// uses underlying <see cref="RegexValueArgument"/>.
    /// </summary>
    /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
    /// <param name="pattern">Regex pattern</param>
    public RegexValueArgumentAttribute(char shortName, string pattern)
        : base(typeof(RegexValueArgument), shortName, new Regex(pattern))
    {
        _argumentType = typeof(RegexValueArgument);
    }

    /// <summary>
    /// Creates new instance of RegexValueArgumentAttribute. RegexValueArgumentAttribute
    /// uses underlying <see cref="RegexValueArgument"/>.
    /// </summary>
    /// <param name="longName"><see cref="Argument.LongName">short name</see> of the underlying argument</param>
    /// <param name="pattern">Regex pattern</param>
    public RegexValueArgumentAttribute(string longName, string pattern)
        : base(typeof(RegexValueArgument), longName, new Regex(pattern))
    {
        _argumentType = typeof(RegexValueArgument);
    }

    /// <summary>
    /// Creates new instance of RegexValueArgument. RegexValueArgumentAttribute
    /// uses underlying <see cref="RegexValueArgument"/>.
    /// </summary>
    /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
    /// <param name="longName"><see cref="Argument.LongName">long name</see> of the underlying argument</param>
    /// <param name="pattern">Regex pattern</param>
    public RegexValueArgumentAttribute(char shortName, string longName, string pattern)
        : base(typeof(RegexValueArgument), shortName, longName, new Regex(pattern))
    {
        _argumentType = typeof(RegexValueArgumentAttribute);
    }

    /// <summary>
    /// Default value
    /// </summary>
    public object? DefaultValue
    {
        get => _argumentType.GetPropertyValue<object>("DefaultValue", Argument);
        set => _argumentType.SetPropertyValue("DefaultValue", Argument, value);
    }

    /// <summary>
    /// Sample value that would be displayed to the user as a suggestion when 
    /// the user enters a wrong value. 
    /// </summary>
    public string? SampleValue
    {
        get => _argumentType.GetPropertyValue<string>("SampleValue", Argument);
        set => _argumentType.SetPropertyValue("SampleValue", Argument, value);
    }

    /// <summary>
    /// When set to true, argument can appear on the command line with or without value, e.g. both is allowed: 
    /// <code>
    /// myexe.exe -Arg Value
    /// OR
    /// myexe.exe -Arg
    /// </code>
    /// </summary>
    public bool ValueOptional
    {
        get => _argumentType.GetPropertyValue<bool>("ValueOptional", Argument);
        set => _argumentType.SetPropertyValue("ValueOptional", Argument, value);
    }
}