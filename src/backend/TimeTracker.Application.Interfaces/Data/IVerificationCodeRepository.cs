using TimeTracker.Application.Interfaces.Data.Base;
using TimeTracker.Domain.Auth;

namespace TimeTracker.Application.Interfaces.Data;

public interface IVerificationCodeRepository : IBaseRepository<VerificationCode, Guid>
{
}