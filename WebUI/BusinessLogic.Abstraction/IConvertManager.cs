using System.Threading.Tasks;
using Common.Constants;

namespace BusinessLogic.Abstraction
{
    public interface IConvertManager
    {
        Task<float[,]> ConvertSnapshot(string fileName, ChannelEnum channel);
    }
}
