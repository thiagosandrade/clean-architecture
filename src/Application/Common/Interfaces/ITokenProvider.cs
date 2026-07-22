using Domain.Users;

namespace Application.Common.Interfaces;

public interface ITokenProvider
{
    string Create(User user);
}
