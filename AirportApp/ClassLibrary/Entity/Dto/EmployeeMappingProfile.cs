using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            CreateMap<Employee, EmployeeDTO>()
                .ConstructUsing(employee => new EmployeeDTO(
                    employee.RetrieveConfiguredDisplayFullNameForBot(),
                    employee.RetrieveConfiguredEmailAddressForBotContact()))
                .ForAllMembers(options => options.Ignore());
        }
    }
}
