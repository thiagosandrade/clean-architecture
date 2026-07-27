using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Elastic.Documents;

public sealed class UserSearchDocument
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime CreatedOn { get; init; }
    public DateTime? UpdatedOn { get; init; }
}
