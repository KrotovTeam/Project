using Common.Constants;

namespace BusinessLogic.Abstraction
{
    public interface IConvertManager
    {
        double[,] ConvertSnapshot(string fileName, ChannelEnum channel);
    }
}
