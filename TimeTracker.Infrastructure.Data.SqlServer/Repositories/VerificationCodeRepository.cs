using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain.Auth;
using TimeTracker.Infrastructure.Data.SqlServer.Base;
using TimeTracker.Infrastructure.Data.SqlServer.Interfaces;

namespace TimeTracker.Infrastructure.Data.SqlServer.Repositories;

public class VerificationCodeRepository : BaseRepository<VerificationCode, Guid>, IVerificationCodeRepository
{
    public VerificationCodeRepository(IDatabaseContext context) : base(context)
    {
    }
}