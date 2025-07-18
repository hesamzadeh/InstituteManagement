using MediatR;
using System;

namespace InstituteManagement.Application.Students.Commands
{
    public record CreateStudentCommand(string Name, string Email) : IRequest<Guid>;
}
