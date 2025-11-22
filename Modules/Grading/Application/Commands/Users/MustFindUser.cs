using ApiClients.OpenApi.Clients.Personal;

using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Users;

/// <summary>
/// Attempts to find user in database and in case of failure tries to find it in personal module.
/// If the user does not exist in the Personal module throws a Errors.User.NotFound exception
/// </summary>
/// <exception cref="NotFound"></exception>
public class MustFindUserCommand(Guid userId) : IRequest<User>
{
    public Guid UserId { get; set; } = userId;
}

public class MustFindUserCommandHandler(PersonalClient personalClient, GradingContext context)
    : IRequestHandler<MustFindUserCommand, User>
{
    public async Task<User> Handle(MustFindUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is not null)
            return user;

        var foundUsers = await personalClient.Api.Personal.SearchByIds.PostAsync(
            new[] { request.UserId }.Cast<Guid?>().ToList(),
            cancellationToken: cancellationToken
        );

        if (foundUsers is null || foundUsers.Count == 0)
            throw Errors.User.NotFound;

        user = new User { Id = request.UserId };
        await context.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return user;
    }
}