using DotNetCore.Database.Entities;

namespace DotNetCore.Contracts
{
    public interface IMembershipService
    {
        User CurrentUser { get; }
    }
}
