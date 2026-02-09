
namespace FGUFW.Console
{
    public static class ConsoleHelpCommands
    {
        [Command("help","提示帮助信息;")]
        public static string Help()
        {
            string msg = 
@"格式: command arg1 arg2 ...
常用指令:
    help : 提示帮助信息;
    all-command : 打印所有指令;
    clean-console : 清理控制台记录;
";
            return msg;
        }
    }
}