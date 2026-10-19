using System.Text;

namespace MyMc;

public class MyMcRegistration
{
    public static void Register()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}
