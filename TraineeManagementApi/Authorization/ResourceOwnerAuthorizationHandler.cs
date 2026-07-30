using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Models;
using TraineeManagement.Shared.Contracts;
using TraineeManagement.Shared.Models;

namespace TraineeManagementApi.Authorization;

// 1. Define the framework requirement marker
public class ResourceOwnerRequirement : IAuthorizationRequirement { }

// 2. Define the main centralized security validation logic
public class ResourceOwnerAuthorizationHandler : AuthorizationHandler<ResourceOwnerRequirement, IOwnedResource>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        ResourceOwnerRequirement requirement, 
        IOwnedResource resource)
    {
        // Admins bypass all ownership checks instantly
        if (context.User.IsInRole(nameof(Role.Admin)))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Extract and parse the unique User ID from the incoming JWT identity claims safely
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int authenticatedUid))
        {
            return Task.CompletedTask; // Fails closed securely if token is malformed
        }

        bool isMentor = context.User.IsInRole(nameof(Role.Mentor));

        // Mentors can read Submissions and Reviews, but CANNOT alter Trainee base accounts
        if (isMentor && resource is not Trainee)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        //Trainees must exactly match the resource's calculated owner ID
        if (resource.GetOwnerTraineeId() == authenticatedUid)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
