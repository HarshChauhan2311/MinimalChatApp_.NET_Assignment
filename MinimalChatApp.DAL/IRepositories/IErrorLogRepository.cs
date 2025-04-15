using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Entity;

namespace MinimalChatApp.DAL.IRepositories
{
    public interface IErrorLogRepository
    {
        Task LogErrorAsync(ErrorLog log);
        Task SaveChangesAsync();
    }
}
