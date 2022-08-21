using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;
using Application.Core;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Activities
{
    public class UpdateAttendance
    {
            public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _accessor;

            public Handler(DataContext context, IUserAccessor accessor)
            {
                _context = context;
                _accessor = accessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                .Include(x => x.Attendees).ThenInclude(u => u.AppUser)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

                if(activity == null ) return null;

                var user = await _context.Users.FirstOrDefaultAsync(
                    x => x.UserName == _accessor.GetUserName());
                
                if(user == null ) return null;

                var hostUsername = activity.Attendees.FirstOrDefault(
                    x => x.IsHost)?.AppUser?.UserName;
                
                var attendance = activity.Attendees.FirstOrDefault(
                    x => x.AppUser.UserName == user.UserName
                );

                if (attendance  != null && hostUsername == user.UserName)
                {
                    activity.IsCancelled = !activity.IsCancelled;
                }

                if (attendance  != null )
                {
                    attendance = new ActivityAttendee{
                        AppUser = user,
                        Activity = activity,
                        IsHost = false
                    };

                    activity.Attendees.Add(attendance);
                }

                 if (attendance  == null && hostUsername != user.UserName)
                {
                    activity.Attendees.Remove(attendance);
                }


               var result =  await _context.SaveChangesAsync() > 0;
             
               return result ? Result<Unit>.Success(Unit.Value)
                : Result<Unit>.Failure("Problem updating attendance");
            }
        }

    }


 

}