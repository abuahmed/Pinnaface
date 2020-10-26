using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.Service.Interfaces;
using PinnaFace.Web.Models;
using WebMatrix.WebData;

namespace PinnaFace.Web.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        //
        // GET: /Employee/
        private readonly IEmployeeService _employeeService;

        public EmployeeController()
        {
            LoadEnums();
            _employeeService = new EmployeeService();
        }

        public ActionResult Index(string searchText, int? page, int? pageSize, int? ptype,
            int? processStatusId, DateTime? fromDateId, DateTime? endDateId, 
            int? filterByAgencyId, int? filterByAgentId)
        {
            if (Singleton.User.AgentId != null)
            {
                ViewData["AgencyFilterVisibility"] = "visible"; 
                ViewData["AgentFilterVisibility"] = "hidden";
            }
            else
            {
                ViewData["AgencyFilterVisibility"] = "hidden";
                ViewData["AgentFilterVisibility"] = "visible";
            }

            var employees = GetEmployeeDtos(searchText, page, pageSize, ptype, processStatusId, 
                null, null, null, null, fromDateId, endDateId, filterByAgencyId, filterByAgentId);

            if (Request.IsAjaxRequest() )
            {
                if (!Request.IsAuthenticated)
                    Redirect("~/Account/Login");
                else
                    return PartialView("_employee", employees);
            }
            return View(employees);
        }

        public ActionResult Thumbnail(string searchText, int? page, int? pageSize, int? ptype,
            int? processStatusId, int? ageCategoryId, int? religionId, int? languageId, int? experienceId)
        {
            var employees = GetEmployeeDtos(searchText, page, pageSize, ptype, 0, ageCategoryId, religionId, languageId,
                experienceId,null,null,null,null);

            if (Request.IsAjaxRequest())
            {
                if (!Request.IsAuthenticated)
                    Redirect("~/Account/Login");
                else
                return PartialView("_employeeThumbnail", employees);
            }
            return View(employees);
        }
        
        public IEnumerable<EmployeeDTO> GetEmployeeDtos(string searchText, int? page, int? pageSize, int? ptype,
            int? processStatusId, int? ageCategoryId, int? religionId, int? languageId, int? experienceId,
            DateTime? fromDateId, DateTime? endDateId, int? filterByAgencyId, int? filterByAgentId)
        {

            var criteria = new SearchCriteria<EmployeeDTO>
            {
                CurrentUserId = WebSecurity.CurrentUserId
            };

            #region By Status

            ViewData["EmployeeStatusHeader"] = "All";
            @ViewData["processStatusId"] = processStatusId;
            if (processStatusId != null && processStatusId != -1)
            {
                if (processStatusId == 0) //0 is fo Visa Waiting
                {
                    criteria.FiList.Add(e => e.Visa == null);
                    ViewData["EmployeeStatusHeader"] = "Visa Waiting";
                }
                else
                {
                    ViewData["EmployeeStatusHeader"] = EnumUtil.GetEnumDesc((ProcessStatusTypes) processStatusId);
                    criteria.FiList.Add(p => p.CurrentStatus == (ProcessStatusTypes) processStatusId);
                }
            }

            #endregion

            #region Filter by Agency/Agent

            if (filterByAgencyId != null && filterByAgencyId > 0)
            {
                criteria.FiList.Add(v => v.AgencyId == filterByAgencyId);
            }

            if (filterByAgentId != null && filterByAgentId > 0)
            {
                criteria.FiList.Add(v => v.AgentId == filterByAgentId);
            }

            #endregion

            #region Age Filter

            if (ageCategoryId != null && ageCategoryId != -1)
            {
                @ViewData["ageCategoryId"] = ageCategoryId;
                const int init = 21;
                var start = init + (int) ageCategoryId*5;
                var end = start + 4;

                var beginDate1 = DateTime.Now.AddYears(-end);
                var endDate1 = DateTime.Now.AddYears(-start);

                var beginDate=new DateTime(beginDate1.Year,1,1,0,0,0,000);
                var endDate = new DateTime(endDate1.Year, 12, 31, 23, 59, 59, 999);

                criteria.FiList.Add(e => e.DateOfBirth >= beginDate && e.DateOfBirth <= endDate);
            }

            #endregion

            #region By Religion

            if (religionId != null && religionId != -1)
            {
                @ViewData["religionId"] = religionId;
                criteria.FiList.Add(p => p.Religion == (ReligionTypes) religionId);
            }

            #endregion

            #region By Language

            if (languageId != null && languageId != -1)
            {
                @ViewData["languageId"] = languageId;
                criteria.FiList.Add(p => p.Education != null &&
                                         p.Education.ArabicLanguage == (LanguageExperience) languageId);
            }

            #endregion

            #region By Experience

            if (experienceId != null && experienceId != -1)
            {
                @ViewData["experienceId"] = experienceId;
                if (experienceId == 0)
                    criteria.FiList.Add(p => !p.Experience.HaveWorkExperience);
                else if (experienceId == 1)
                    criteria.FiList.Add(p => p.Experience.HaveWorkExperience);
            }

            #endregion

            #region Search Text

            if (!string.IsNullOrEmpty(searchText))
            {
                ViewData["ItemSearch"] = "search results for '<strong style='font-size:14px;'>" + searchText +
                                         "</strong>'";
                ViewData["SearchText"] = searchText;
                ViewData["ItemSearchHidden"] = "visible";
            }
            else
            {
                ViewData["ItemSearch"] = string.Empty;
                ViewData["SearchText"] = string.Empty;
                ViewData["ItemSearchHidden"] = "hidden";
            }

            if (!string.IsNullOrEmpty(searchText))
                criteria.FiList.Add(bp => bp.FullName.ToLower().Contains(searchText.ToLower()) ||
                                          bp.PassportNumber.ToLower().Contains(searchText.ToLower()) ||
                                          bp.MoreNotes.ToLower().Contains(searchText.ToLower()));

            #endregion

            #region Duration

            if (fromDateId != null && endDateId != null)
            {
                var beginDate = new DateTime(fromDateId.Value.Year, fromDateId.Value.Month,
                             fromDateId.Value.Day, 0, 0, 0, 000);
                var endDate = new DateTime(endDateId.Value.Year, endDateId.Value.Month,
                             endDateId.Value.Day, 23, 59, 59,999);

                criteria.FiList.Add(l => l.CurrentStatus == ProcessStatusTypes.OnGoodCondition && l.FlightProcess != null &&
                         l.FlightProcess.SubmitDate >= beginDate && l.FlightProcess.SubmitDate <= endDate);
            }
            #endregion

            #region Paging

            if (page != null && ptype != null && pageSize != null)
            {
                criteria.Page = (int) (page + ptype);
                criteria.PageSize = (int) pageSize;

                if (criteria.Page < 1)
                    criteria.Page = 1;
            }
            else
            {
                criteria.Page = 1;
                criteria.PageSize = 50;
            }
            if (criteria.Page == 1)
                ViewData["prevDisabled"] = "none";
            ViewData["Page"] = criteria.Page;
            ViewData["PageSize"] = criteria.PageSize;

            #endregion

            int totalCount;
            var employees = _employeeService.GetAll(criteria, out totalCount).ToList();
            @ViewData["totalNumber"] = totalCount;

            #region Paging

            var pages = HelperUtility.GetPages(totalCount, criteria.PageSize);
            ViewData["totalPages"] = pages;

            if (pages == 0)
            {
                criteria.Page = 0;
                ViewData["Page"] = 0;
            }

            if (criteria.Page == pages)
                ViewData["nextDisabled"] = "none";

            #endregion

            #region For Serial Number

            var sNo = (criteria.Page - 1)*criteria.PageSize + 1;
            foreach (var employeeDTO in employees)
            {
                employeeDTO.SerialNumber = sNo;
                sNo++;
            }

            #endregion

            return employees;
        }

        public ActionResult Details(string empId, string searchText, int? page, int? pageSize, int? ptype,
            int? processStatusId, int? ageCategoryId, int? religionId, int? languageId, int? experienceId)
        {
            try
            {
                EmployeeDTO employee;

                if (!string.IsNullOrEmpty(empId))
                {
                    var criteria = new SearchCriteria<EmployeeDTO>
                    {
                        CurrentUserId = Singleton.User.UserId
                    };

                    if (processStatusId != null && processStatusId != -1)
                    {
                        if (processStatusId == 0)
                            criteria.FiList.Add(e => e.Visa == null);
                        else
                            criteria.FiList.Add(a => a.CurrentStatus == (ProcessStatusTypes) processStatusId);
                    }

                    #region Age Filter

                    if (ageCategoryId != null && ageCategoryId != -1)
                    {
                        @ViewData["ageCategoryId"] = ageCategoryId;
                        const int init = 21;
                        var start = init + (int) ageCategoryId*5;
                        var end = start + 4;

                        var beginDate = DateTime.Now.AddYears(-start);
                        var endDate = DateTime.Now.AddYears(-end);

                        criteria.FiList.Add(e => e.DateOfBirth >= endDate && e.DateOfBirth <= beginDate);
                    }

                    #endregion

                    #region By Religion

                    if (religionId != null && religionId != -1)
                    {
                        @ViewData["religionId"] = religionId;
                        criteria.FiList.Add(p => p.Religion == (ReligionTypes) religionId);
                    }

                    #endregion

                    #region By Language

                    if (languageId != null && languageId != -1)
                    {
                        @ViewData["languageId"] = languageId;
                        criteria.FiList.Add(p => p.Education != null &&
                                                 p.Education.ArabicLanguage == (LanguageExperience) languageId);
                    }

                    #endregion

                    #region By Experience

                    if (experienceId != null && experienceId != -1)
                    {
                        @ViewData["experienceId"] = experienceId;
                        if (experienceId == 0)
                            criteria.FiList.Add(p => !p.Experience.HaveWorkExperience);
                        else if (experienceId == 1)
                            criteria.FiList.Add(p => p.Experience.HaveWorkExperience);
                    }

                    #endregion

                    var employees = _employeeService.GetAll(criteria).ToList();
                    var empss = employees.Select(e => e.Id).ToList();

                    var employeeId = EncryptionUtility.Hash64Decode(empId);
                    if (employeeId < 1)
                        return View("Error", new HandleErrorInfo(new Exception("Invalid url"), "Employee", "Index"));
                            // Redirect("/Employee/Index");
                    var indexofemployee = empss.IndexOf(employeeId);

                    var emps =
                        GetEmployeeDtos(searchText, indexofemployee, 1, 1, processStatusId, ageCategoryId, religionId,
                            languageId, experienceId, null, null, null, null).ToList();
                    employee = emps.FirstOrDefault(v => v.Id == employeeId);
                }
                else
                {
                    employee =
                        GetEmployeeDtos(searchText, page, 1, ptype, processStatusId, ageCategoryId, religionId,
                            languageId, experienceId, null, null, null, null).FirstOrDefault();
                }
                //var photoPath = HelperUtility.GetDestinationPhotoPath();

                //if (employee != null && employee.Photo != null)
                //{
                //    if (employee.Photo.AttachmentUrl != null)
                //    {
                //        string fiName = employee.Photo.AttachmentUrl;
                //        var fname = Path.Combine(photoPath, fiName);

                //        employee.MoreNotes = fname;
                //    }
                //}

                if (Request.IsAjaxRequest())
                {
                    if (!Request.IsAuthenticated)
                        Redirect("~/Account/Login");
                    return PartialView("_employeeDetail", employee);
                }
                return View(employee);
            }
            catch
            {
                return Redirect("~/Employee/Index");
            }
        }

        public ActionResult Assign(string empId, string visaId)
        {
            var employeeVisa = new EmployeeVisaViewModel();
            if (!string.IsNullOrEmpty(empId))
            {
                employeeVisa.IsEmployeeAssigned = true;
                var employeeId = EncryptionUtility.Hash64Decode(empId);
                if (employeeId < 1)
                    return View("Error", new HandleErrorInfo(new Exception("Invalid url"), "Employee", "Index"));
                        // Redirect("/Employee/Index");

                var employee = new EmployeeService(true, true).GetAll().FirstOrDefault(e => e.Id == employeeId);
                if (employee != null)
                {
                    employeeVisa.Employee = employee;
                    if (employee.Visa != null)
                    {
                        employeeVisa.Visa = employee.Visa;
                        employeeVisa.IsVisaAssigned = true;
                    }
                }
            }
            if (!string.IsNullOrEmpty(visaId))
            {
                employeeVisa.IsVisaAssigned = true;
            }

            return View(employeeVisa);
        }

        //Employee will ONLY be created from the wpf app::
        [Authorize(Roles = "AddEmployee")]
        public ActionResult Create()
        {
            var employee = new EmployeeDTO
            {
                DateOfBirth = DateTime.Now.AddYears(-20),
                PassportIssueDate = DateTime.Now,
                PassportExpiryDate = DateTime.Now.AddYears(5),
                Sex = Sex.Female
            };
            if(Singleton.Agency!=null)
            employee = CommonUtility.GetNewEmployeeDTO();
            
            return View(employee);
        }
        [HttpPost]
        public ActionResult Create(EmployeeDTO employee)
        {
            try
            {
                var empDto = AddNewEmployee();
                empDto.PassportNumber = employee.PassportNumber;
                empDto.FirstName = employee.FirstName;
                //empDto.MiddleName = "BBB";//employee.MiddleName;
                empDto.LastName = employee.LastName;
                empDto.PlaceOfBirth = employee.PlaceOfBirth;
                var msg = _employeeService.InsertOrUpdate(empDto);
                if (!string.IsNullOrEmpty(msg))
                    return View(empDto);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public EmployeeDTO AddNewEmployee()
        {
            return new EmployeeDTO
            {
                PassportIssueDate = DateTime.Now,
                PassportExpiryDate = DateTime.Now.AddYears(5),
                DateOfBirth = DateTime.Now,
                MaritalStatus = MaritalStatusTypes.Single,
                Sex = Sex.Female,
                Religion = ReligionTypes.Muslim,
                CurrentStatus = ProcessStatusTypes.OnProcess,
                CurrentStatusDate = DateTime.Now,
                //AppliedCountry = CountryList.SaudiArabia,
                AppliedProfession = ProffesionTypes.Housemaid,
                NumberOfChildren = Numbers.Zero,
                Complexion = Complexion.Brown,
                Address = new AddressDTO
                {
                    AddressType = AddressTypes.Local,
                    Country = CountryList.Ethiopia,
                    City = EnumUtil.GetEnumDesc(CityList.AddisAbeba)
                },
                Photo = new AttachmentDTO(),
                StandPhoto = new AttachmentDTO(),
                Salary = 700,
                CurrencyType = CurrencyTypes.SaudiArabia,
                ContratPeriod = ContratPeriods.Two,
                Synced = false,
                DocumentReceivedDate = DateTime.Now,
            };
            //EmptyControlVisibility = true;
        }

        public ActionResult Edit(string empId)
        {
            if (!string.IsNullOrEmpty(empId))
            {
                var employeeId = EncryptionUtility.Hash64Decode(empId);
                var cri = new SearchCriteria<EmployeeDTO>();
                cri.FiList.Add(v => v.Id == employeeId);
                var employeeDTO = new EmployeeService(true, true)
                    .GetAll(cri).ToList().FirstOrDefault();

                return View(employeeDTO);
            }
            return View("Create");
        }
        [HttpPost]
        public ActionResult Edit(EmployeeDTO employeeDto)
        {
            try
            {
                var employeeService = new EmployeeService(false, true);
                var cri = new SearchCriteria<EmployeeDTO>();
                cri.FiList.Add(v => v.Id == employeeDto.Id);
                var employee = employeeService.GetAll(cri).ToList().FirstOrDefault();
                if (employee != null)
                {
                    employee.PassportNumber = employeeDto.PassportNumber;
                    employee.FirstName = employeeDto.FirstName;
                    employee.LastName = employeeDto.LastName;
                    employee.Address.Mobile = employeeDto.Address.Mobile;

                    employeeService.InsertOrUpdate(employee);
                    employeeService.Dispose();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        public ActionResult Detail(string empId, string searchText)
        {
            bool employeefound = false;
            var employeeDTO = new EmployeeDTO();

            if (!string.IsNullOrEmpty(empId))
            {
                int employeeId = EncryptionUtility.Hash64Decode(empId);
                employeeDTO = new EmployeeService(true, true)
                    .GetAll()
                    .ToList()
                    .FirstOrDefault(v => v.Id == employeeId);
                if (employeeDTO != null)
                {
                    employeefound = true;
                }
            }
            else if (!string.IsNullOrEmpty(searchText))
            {
                var criteria = new SearchCriteria<EmployeeDTO>();
                if (!string.IsNullOrEmpty(searchText))
                    criteria.FiList.Add(bp => bp.Visa != null &&
                                              bp.Visa.Sponsor != null &&
                                              (bp.FullName.ToLower().Contains(searchText.ToLower()) ||
                                               bp.PassportNumber.ToLower().Contains(searchText.ToLower()) ||
                                               bp.Visa.Sponsor.PassportNumber.ToLower().Contains(searchText.ToLower()) ||
                                               bp.Visa.Sponsor.FullName.ToLower().Contains(searchText.ToLower()) ||
                                               bp.MoreNotes.ToLower().Contains(searchText.ToLower())));

                employeeDTO = new EmployeeService(true, true)
                    .GetAll(criteria).ToList().FirstOrDefault();
                if (employeeDTO != null)
                {
                    employeefound = true;
                }
            }


            if (Request.IsAjaxRequest())
            {
                if (!Request.IsAuthenticated)
                    Redirect("~/Account/Login");
                if (!employeefound)
                {
                    employeeDTO = new EmployeeDTO
                    {
                        MoreNotes = "No Employee/Visa is found, please try your search again..."
                    };
                }
                return PartialView("_employeeDetail2", employeeDTO);
            }
            return View(employeeDTO);
        }
        
        public ActionResult Delete(string empId)
        {
            int employeeId = EncryptionUtility.Hash64Decode(empId);
            var employeeDTO = new EmployeeService(true, true)
                .GetAll()
                .ToList()
                .FirstOrDefault(v => v.Id == employeeId);
            return View(employeeDTO);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Detach(string empId)
        {
            int employeeId = EncryptionUtility.Hash64Decode(empId);
            var employeeDTO = new EmployeeService(true, true)
                .GetAll()
                .ToList()
                .FirstOrDefault(v => v.Id == employeeId);
            return View(employeeDTO);
        }
        [HttpPost]
        public ActionResult Detach(EmployeeDTO employee)
        {
            try
            {
                var empSerive = new EmployeeService(false, false);

                try
                {
                    var employeeDTO = empSerive.Find(employee.Id.ToString());
                    employeeDTO.VisaId = null;
                    empSerive.InsertOrUpdate(employeeDTO);
                }
                catch
                {
                }
                finally
                {
                    empSerive.Dispose();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public void LoadEnums()
        {
            @ViewData["ReligionFilter"] = HelperUtility.ReligionEnums();
            @ViewData["ProfessionFilter"] = HelperUtility.ProfessionEnums();
            @ViewData["SexFilter"] = HelperUtility.SexEnums();
            @ViewData["MaritalStatusFilter"] = HelperUtility.MaritalStatusTypesEnums();
            @ViewData["ContratPeriodFilter"] = HelperUtility.ContratPeriodsEnums();
            @ViewData["ComplexionFilter"] = HelperUtility.ComplexionEnums();

            @ViewData["ExperienceCountryFilter"] = HelperUtility.CountryListEnums();
            @ViewData["LanguageFilter"] = HelperUtility.LanguageExperienceEnums();

            FilterLists();
        }

        public void FilterLists()
        {
            var employeeStatusFilterList = new List<SelectListItem>
            {
                new SelectListItem {Value = "-1", Text = "All"},
                new SelectListItem {Value = "0", Text = "Visa Waiting"},
                new SelectListItem {Value = "11", Text = "Visa Assigned"},
                new SelectListItem
                {
                    Value = Convert.ToInt32(ProcessStatusTypes.OnProcess).ToString(),
                    Text = "On Process"
                },
                new SelectListItem {Value = "4", Text = "Stammped & Flight Processing"},
                new SelectListItem {Value = "44", Text = "Booked or Departured"},
                new SelectListItem
                {
                    Value = Convert.ToInt32(ProcessStatusTypes.OnGoodCondition).ToString(),
                    Text = "Arrived & On Good Condition"
                },
                new SelectListItem
                {
                    Value = Convert.ToInt32(ProcessStatusTypes.Returned).ToString(),
                    Text = "Returned"
                },
                new SelectListItem
                {
                    Value = Convert.ToInt32(ProcessStatusTypes.Lost).ToString(),
                    Text = "Lost"
                },
                new SelectListItem
                {
                    Value = Convert.ToInt32(ProcessStatusTypes.WithComplain).ToString(),
                    Text = "With Complain"
                }
            };
            var cats = new SelectList(employeeStatusFilterList, "value", "text");
            @ViewData["EmployeeStatus"] = cats;

            //Experince
            var experienceFilterList = new List<SelectListItem>
            {
                new SelectListItem {Value = "-1", Text = "All"},
                new SelectListItem {Value = "0", Text = "No Experience"},
                new SelectListItem {Value = "1", Text = "With Experience"},
            };
            var exps = new SelectList(experienceFilterList, "value", "text");
            @ViewData["ExperienceFilter"] = exps;

            //Age
            var ageFilterList = new List<SelectListItem>
            {
                new SelectListItem {Value = "-1", Text = "All"}
            };

            ageFilterList.AddRange(HelperUtility.AgeCategoryEnums());

            @ViewData["AgeFilter"] = new SelectList(ageFilterList, "value", "text");

            //Religion
            var religionFilterList = new List<SelectListItem>
            {
                new SelectListItem {Value = "-1", Text = "All"}
            };

            religionFilterList.AddRange(HelperUtility.ReligionEnums());

            @ViewData["ReligionFilter2"] = new SelectList(religionFilterList, "value", "text");

            //Language
            var languageFilterList = new List<SelectListItem>
            {
                new SelectListItem {Value = "-1", Text = "All"}
            };

            languageFilterList.AddRange(HelperUtility.LanguageExperienceEnums());

            @ViewData["LanguageFilter2"] = new SelectList(languageFilterList, "value", "text");

            //Agency
            var agencies = HelperUtility.GetAgencies();

            var agencyFilterList = new List<SelectListItem>
            {
                new SelectListItem {Value = "-1", Text = "Do Not Assign"}
            };
            agencyFilterList.AddRange(agencies);
            @ViewData["AgencyFilter"] = new SelectList(agencyFilterList, "value", "text");

            var agencyList = new List<SelectListItem>
            {
                new SelectListItem {Value = "-1", Text = "All"}
            };
            agencyList.AddRange(agencies);
            @ViewData["AgencyList"] = new SelectList(agencyList, "value", "text");

            //Agents
            var agentFilterList = new List<SelectListItem>
            {
                new SelectListItem {Value = "-1", Text = "Do Not Assign"}
            };

            var agents = HelperUtility.GetAgents();
            agentFilterList.AddRange(agents.Select(agent => new SelectListItem { Value = agent.Value, Text = agent.Text }));
            //agentFilterList.AddRange(HelperUtility.GetAgents());
            @ViewData["AgentFilter"] = new SelectList(agentFilterList, "value", "text");

            agentFilterList[0].Text = "All"; // new SelectListItem { Value = "-1", Text = "All" };
            @ViewData["AgentList"] = new SelectList(agentFilterList, "value", "text");
        }

        //public void AgeFilter(int beginAge, int endAge, out SearchCriteria<EmployeeDTO> searchCriteria)
        //{
        //    //var criteria = new SearchCriteria<EmployeeDTO>();

        //    var beginDate = DateTime.Now.AddYears(-beginAge);
        //    var endDate = DateTime.Now.AddYears(-endAge);

        //    if(searchCriteria!=null)
        //    searchCriteria.FiList.Add(e=>e.DateOfBirth>beginDate && e.DateOfBirth<endDate);

        //}
    }


    public class EmployeeVisaViewModel
    {
        public EmployeeVisaViewModel()
        {
            IsEmployeeAssigned = false;
            IsVisaAssigned = false;
            Comment = "";
        }

        public EmployeeDTO Employee { get; set; }
        public VisaDTO Visa { get; set; }
        public bool IsEmployeeAssigned { get; set; }
        public bool IsVisaAssigned { get; set; }
        public string Comment { get; set; }
    }
}