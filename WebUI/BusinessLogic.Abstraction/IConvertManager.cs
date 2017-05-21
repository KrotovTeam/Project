using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.Dtos;
using Common.Constants;

namespace BusinessLogic.Abstraction
{
    public interface IConvertManager
    {
        Task<IEnumerable<Point>> ConvertSnapshotAsync(string fileName, ChannelEnum channel);
    }
}
