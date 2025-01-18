using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Player.Domain.Entities;

namespace Player.Application.UnitTests;

public class TestUserStore :
    IUserStore<User>,
    IUserClaimStore<User>,
    IUserPasswordStore<User>,
    IUserLockoutStore<User>,
    IUserEmailStore<User>
{
    public virtual void Dispose()
    {
        throw new NotImplementedException();
    }

    public virtual Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task SetNormalizedUserNameAsync(User user, string? normalizedName,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task SetPasswordHashAsync(User user, string? passwordHash, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<string?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task SetEmailAsync(User user, string? email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<string?> GetEmailAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<string?> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task SetNormalizedEmailAsync(User user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}