namespace CommandLineParser
{
    internal static class Messages
    {
        public static string CERT_REMARKS = "Argument combinations remarks:";

        public static string EXC_ADDITIONAL_ARGS_TOO_EARLY = "AdditionalArguments cannot be accessed before ParseCommandLine is called.";

        public static string EXC_ADDITONAL_ARGS_FORBIDDEN = "AcceptAdditionalArguments is set to false therefore AdditionalArguments can not be read.";

        public static string EXC_ARG_BOUNDED_GREATER_THAN_MAX = "Argument value { 0} is greater then maximum value {1}";

        public static string EXC_ARG_BOUNDED_LESSER_THAN_MIN = "Argument value {0} is lesser then minimum value {1}";

        public static string EXC_ARG_ENUM_OUT_OF_RANGE = "Value {0} is not allowed for argument {1}";

        public static string EXC_ARG_NOT_ONE_CHAR = "ShortName of an argument must not be whitespace character.";

        public static string EXC_ARG_NOT_ONE_WORD = "LongName of an argument must be one word.";

        public static string EXC_ARG_SWITCH_PRINT = "Argument: {0} value: {1}";

        public static string EXC_ARG_UNKNOWN = "Unknown argument found: {0}.";

        public static string EXC_ARG_VALUE_MISSING = "Value argument {0} must be followed by a value, another argument({1}) found instead";

        public static string EXC_ARG_VALUE_MISSING2 = "Value argument {0} must be followed by a value.";

        public static string EXC_ARG_VALUE_MULTIPLE_OCCURS = "Argument {0} can not be used multiple times.";

        public static string EXC_ARG_VALUE_PRINT = "Argument: {0}, type: {3}, value: {2} (converted from: {1})";

        public static string EXC_ARG_VALUE_PRINT_MULTIPLE = "Argument: {0}, type: {3}, occured {1}x values: {2}";

        public static string EXC_ARG_VALUE_STANDARD_CONVERT_FAILED = "Failed to convert string {0} to type {1}. Use strings in accepted format or define custom conversion using ConvertValueHandler.";

        public static string EXC_ARG_VALUE_STRINGVALUE_ACCESS = "Arguments StringValue can be read after ParseCommandLine is called.";

        public static string EXC_ARG_VALUE_USER_CONVERT_MISSING = "Type {0} of argument {1} is not a built-in type. Set ConvertValueHandler to a conversion routine for this type or define static method Parse(string stringValue, CultureInfo cultureInfo) that can Parse your type from string. ";

        public static string EXC_BAD_ARG_IN_GROUP = "Grouping of multiple short name arguments in one word(e.g. -a -b into -ab) is allowed only for switch arguments.Argument {0} is not a switch argument.";

        public static string EXC_BINDING = "Binding of the argument {0} to the field {1} of the object {2} failed.";

        public static string EXC_DIR_NOT_FOUND = "Directory not found : {0} and DirectoryMustExist flag is set to true.";

        public static string EXC_FILE_MUST_EXIST = "OpenFile should not be called when FileMustExist flag is not set.";

        public static string EXC_FILE_NOT_FOUND = "File not found : {0} and FileMustExist flag is set to true.";

        public static string EXC_FORMAT_LONGNAME_PREFIX = "Only short argument names(single character) are allowed after single '-' character(e.g. -v). For long names use double '-' format(e.g. '--ver'). Wrong argument is: {0}";

        public static string EXC_FORMAT_SHORTNAME_PREFIX = "If short name argument is used, it must be prefixed with single '-' character.Wrong argument is: {0}";

        public static string EXC_FORMAT_SINGLEHYPHEN = "Found character '-' not followed by an argument.";

        public static string EXC_GROUP_AT_LEAST_ONE = "At least one of these arguments: {0} must be used.";

        public static string EXC_GROUP_DISTINCT = "None of these arguments: {0} can be used together with any of these: {1}.";

        public static string EXC_GROUP_EXACTLY_ONE_MORE_USED = "Only one of these arguments: {0} can be used.";

        public static string EXC_GROUP_EXACTLY_ONE_NONE_USED = "One of these arguments: {0} must be used.";

        public static string EXC_GROUP_ONE_OR_NONE_MORE_USED = "These arguments can not be used together: {0}.";

        public static string EXC_GROUP_ARGUMENTS_REQUIRED_BY_ANOTHER_ARGUMENT = "Argument: {0} requires the following arguments: {1}.";

        public static string EXC_NONNEGATIVE = "The value must be non negative.";

        public static string MSG_ADDITIONAL_ARGUMENTS = "Additional arguments:";

        public static string MSG_COMMAND_LINE = "Command line:";

        public static string MSG_OPTIONAL = "[optional]";

        public static string MSG_NOT_PARSED_ARGUMENTS = "Arguments not specified:";

        public static string MSG_PARSED_ARGUMENTS = "Parsed Arguments:";

        public static string MSG_PARSING_RESULTS = "Parsing results:";

        public static string MSG_USAGE = "Usage:";

        public static string EXC_MISSING_MANDATORY_ARGUMENT = "Argument {0} is not marked as optional and was not found on the command line.";

        public static string EXC_ADDITIONAL_ARGUMENTS_FOUND = "Additional arguments found and parser does not accept additional arguments. Set AcceptAdditionalArguments to true if you want to accept them. ";

        public static string EXC_NOT_ENOUGH_ADDITIONAL_ARGUMENTS = "Not enough additional arguments. Needed {0} additional arguments.";

        public static string MSG_EXAMPLE_FORMAT = "Example: {0}";

        public static string EXC_GROUP_ALL_OR_NONE_USED_NOT_ALL_USED = "All or none of these arguments: {0} must be used.";

        public static string EXC_GROUP_ALL_USED_NOT_ALL_USED = "All of these arguments: {0} must be used.";

        public static string GROUP_ALL_OR_NONE_USED = "All or none of these arguments: {0} must be used.";

        public static string GROUP_ALL_USED = "All of these arguments: {0} must be used.";

        public static string GROUP_AT_LEAST_ONE_USED = "At least one of these arguments: {0} must be used.";

        public static string GROUP_EXACTLY_ONE_USED = "One (and only one) of these arguments: {0} must be used.";

        public static string GROUP_ONE_OR_NONE_USED = "These arguments can not be used together: {0}.";

        public static string EXC_FORMAT_DOUBLESLASH = "Invalid sequence \"//\" in the command line.";

        public static string EXC_FORMAT_SINGLESLASH = "Found character '/' not followed by an argument.";
    }
}