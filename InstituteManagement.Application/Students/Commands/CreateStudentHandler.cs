using InstituteManagement.Application.Common.Interfaces;
using InstituteManagement.Application.Students.Commands;
using MediatR;

//public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, Guid>
//{
//    //private readonly IAppDbContext _context;

//    //public CreateStudentHandler(IAppDbContext context)
//    //{
//    //    _context = context;
//    //}

//    //public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
//    //{
//    //    //var student = new Student
//    //    //{
//    //    //    Id = Guid.NewGuid(),
//    //    //    Name = request.Name,
//    //    //    Email = request.Email,
//    //    //    // Add other properties as needed
//    //    //};

//    //    //_context.Students.Add(student);
//    //    //await _context.SaveChangesAsync(cancellationToken);

//    //    //return student.Id;
//    //}
//}
