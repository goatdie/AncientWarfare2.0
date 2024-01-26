using System.Text;

namespace Figurebox.Utils.extensions;

public static class StringBuilderExtension
{
    public static void AppendFormatLine(this StringBuilder pBuilder, string pFormat, params object[] pArgs)
    {
        pBuilder.AppendFormat(pFormat, pArgs);
        pBuilder.AppendLine();
    }
}