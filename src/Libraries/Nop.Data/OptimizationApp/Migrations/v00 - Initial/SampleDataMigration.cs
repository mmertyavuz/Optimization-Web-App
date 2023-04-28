using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentMigrator;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Data.Migrations;
using RandomNameGeneratorLibrary;

namespace Nop.Data.OptimizationApp.Migrations.v00___Initial
{
    [NopMigration("2023-04-28 12:19:00", "4.60.0", UpdateMigrationType.Data, MigrationProcessType.Update)]
    public class SampleDataMigration : Migration
    {
        private readonly INopDataProvider _dataProvider;

        public SampleDataMigration(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            #region Doğa Bilimleri Fakultesi

            {
                var faculty = new Faculty
                {
                    Name = "Faculty of Engineering and Natural Sciences",
                    Description =
                        $"The Faculty of Engineering and Natural Sciences is an academic institution that focuses on teaching and research in the fields of engineering and natural sciences."
                };

                _dataProvider.InsertEntity(faculty);

                var departments = new List<EducationalDepartment>()
                {
                    new EducationalDepartment
                    {
                        Name = "Computer Engineering",
                        Code = "CMP",
                        Description =
                            "The Computer Engineering department focuses on the design, development, and analysis of computer systems and networks.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Biomedical Engineering",
                        Code = "BME",
                        Description =
                            "The Biomedical Engineering department combines the principles of engineering and biology to develop solutions for healthcare and medical problems.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Electrical and Electronics Engineering",
                        Code = "EEE",
                        Description =
                            "The Electrical and Electronics Engineering department focuses on the design and development of electrical and electronic systems and devices.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Industrial Engineering",
                        Code = "IE",
                        Description =
                            "The Industrial Engineering department focuses on optimizing processes and systems in order to improve efficiency and productivity in various industries.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Energy Systems Engineering",
                        Code = "ESE",
                        Description =
                            "The Energy Systems Engineering department focuses on the design and development of renewable and sustainable energy systems.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Civil Engineering",
                        Code = "CE",
                        Description =
                            "The Civil Engineering department focuses on the design, construction, and maintenance of infrastructure and buildings.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Business Engineering",
                        Code = "BE",
                        Description =
                            "The Business Engineering department combines the principles of business and engineering to develop solutions for complex business problems.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Mathematics",
                        Code = "MAT",
                        Description =
                            "The Mathematics department focuses on the study of numbers, quantities, and shapes, as well as their relationships and operations.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Mechatronics Engineering",
                        Code = "ME",
                        Description =
                            "The Mechatronics Engineering department combines the principles of mechanical, electrical, and computer engineering to design and develop complex systems.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Molecular Biology and Genetics",
                        Code = "MBG",
                        Description =
                            "The Molecular Biology and Genetics department focuses on the study of biological molecules and their interactions, as well as genetic inheritance and variation.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Artificial Intelligence Engineering",
                        Code = "AIE",
                        Description =
                            "The Artificial Intelligence Engineering department focuses on the development and application of artificial intelligence technologies, such as machine learning and natural language processing.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Software Engineering",
                        Code = "SEN",
                        Description =
                            "The Software Engineering department focuses on the design, development, and maintenance of software systems.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                };

                foreach (var department in departments)
                {
                    var departmentLead = InsertCustomer(department.Code.ToLower(),
                        NopCustomerDefaults.DepartmentLeadRoleName);

                    department.DepartmentLeadCustomerId = departmentLead.Id;

                    _dataProvider.InsertEntity(department);
                }
            }

            #endregion

            #region Mimarlık Fakultesi

            {
                var faculty = new Faculty
                {
                    Name = "Faculty of Architecture and Design",
                    Description =
                        "The Faculty of Architecture and Design is an academic institution that focuses on teaching and research in the fields of architecture and design."
                };

                _dataProvider.InsertEntity(faculty);

                var departments = new List<EducationalDepartment>()
                {
                    new EducationalDepartment
                    {
                        Name = "Architecture",
                        Code = "ARC",
                        Description =
                            "The Architecture department focuses on the design and planning of buildings and structures.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Interior Design",
                        Code = "ID",
                        Description =
                            "The Interior Design department focuses on the design and planning of interior spaces, such as homes, offices, and public spaces.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Landscape Architecture",
                        Code = "LAR",
                        Description =
                            "The Landscape Architecture department focuses on the design and planning of outdoor spaces, such as parks, gardens, and urban landscapes.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Industrial Design",
                        Code = "IND",
                        Description =
                            "The Industrial Design department focuses on the design and development of products, such as furniture, appliances, and consumer goods.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Graphic Design",
                        Code = "GRD",
                        Description =
                            "The Graphic Design department focuses on the design and creation of visual content, such as logos, advertisements, and publications.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Fashion Design",
                        Code = "FAD",
                        Description =
                            "The Fashion Design department focuses on the design and creation of clothing and accessories, as well as fashion trends and marketing.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    }
                };

                foreach (var department in departments)
                {
                    var departmentLead = InsertCustomer(department.Code.ToLower(),
                        NopCustomerDefaults.DepartmentLeadRoleName);

                    department.DepartmentLeadCustomerId = departmentLead.Id;

                    _dataProvider.InsertEntity(department);
                }
            }

            #endregion

            #region Eğitim Bilimkeri Fakultesi

            {
                var faculty = new Faculty
                {
                    Name = "Faculty of Educational Sciences",
                    Description =
                        "The Faculty of Educational Sciences is an academic institution that focuses on teaching and research in the fields of education and pedagogy."
                };

                _dataProvider.InsertEntity(faculty);

                var departments = new List<EducationalDepartment>()
                {
                    new EducationalDepartment
                    {
                        Name = "Curriculum and Instruction",
                        Code = "CI",
                        Description =
                            "The Curriculum and Instruction department focuses on the development and implementation of educational programs and teaching methods.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Educational Psychology",
                        Code = "EP",
                        Description =
                            "The Educational Psychology department focuses on the study of human learning and development, as well as the application of psychological theories to educational settings.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Special Education",
                        Code = "SE",
                        Description =
                            "The Special Education department focuses on the education of individuals with special needs, such as those with disabilities or learning difficulties.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Educational Technology",
                        Code = "ET",
                        Description =
                            "The Educational Technology department focuses on the integration of technology into educational settings, as well as the development of educational software and hardware.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Early Childhood Education",
                        Code = "ECE",
                        Description =
                            "The Early Childhood Education department focuses on the education of young children, from infancy to age 8.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                    new EducationalDepartment
                    {
                        Name = "Higher Education",
                        Code = "HE",
                        Description =
                            "The Higher Education department focuses on the study of colleges and universities, as well as the development of policies and practices related to higher education.",
                        FacultyId = faculty.Id,
                        DepartmentLeadCustomerId = 0
                    },
                };

                foreach (var department in departments)
                {
                    var departmentLead = InsertCustomer(department.Code.ToLower(),
                        NopCustomerDefaults.DepartmentLeadRoleName);

                    department.DepartmentLeadCustomerId = departmentLead.Id;

                    _dataProvider.InsertEntity(department);
                }
            }

            #endregion
        }

        private Customer InsertCustomer(string emailPrefix, params string[] customerRoleNames)
        {
            var personGenerator = new PersonNameGenerator();

            var crRegistered = _dataProvider.GetTable<CustomerRole>().FirstOrDefault(customerRole =>
                customerRole.SystemName == NopCustomerDefaults.RegisteredRoleName);

            if (crRegistered == null)
                throw new ArgumentNullException(nameof(crRegistered));

            //default store 
            var defaultStore = _dataProvider.GetTable<Store>().FirstOrDefault();

            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            var storeId = defaultStore.Id;

            //second user
            var email = emailPrefix + "@bahcesehir.edu.tr";
            var user = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = email,
                Username = email,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId,
                FirstName = personGenerator.GenerateRandomFirstName(),
                LastName = personGenerator.GenerateRandomLastName()
            };

            _dataProvider.InsertEntityAsync(user);

            _dataProvider.InsertEntity(new CustomerCustomerRoleMapping
                {CustomerId = user.Id, CustomerRoleId = crRegistered.Id});

            _dataProvider.InsertEntity(
                new CustomerPassword
                {
                    CustomerId = user.Id,
                    Password = "11",
                    PasswordFormat = PasswordFormat.Clear,
                    PasswordSalt = string.Empty,
                    CreatedOnUtc = DateTime.UtcNow
                });

            if (customerRoleNames != null)
                foreach (var customerRoleName in customerRoleNames)
                {
                    var customerRole = _dataProvider.GetTable<CustomerRole>()
                        .FirstOrDefault(customerRole => customerRole.SystemName == customerRoleName);

                    if (customerRole != null)
                        _dataProvider.InsertEntity(new CustomerCustomerRoleMapping
                            {CustomerId = user.Id, CustomerRoleId = customerRole.Id});
                }

            return user;
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}