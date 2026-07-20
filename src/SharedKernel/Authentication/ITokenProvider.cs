using Domain.Users;

namespace SharedKernel.Authentication;

public interface ITokenProvider
{
    string Create(User user);
}
