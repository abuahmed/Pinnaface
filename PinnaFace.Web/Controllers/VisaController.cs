using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
    public class VisaController : Controller
    {
        private readonly IVisaService _visaService;

        public VisaController()
        {
            LoadEnums();
            _visaService = new VisaService(false, true);
        }

        public ActionResult Index(string searchText, int? page, int? pageSize, int? ptype,
            int? processStatusId, int? filterByAgencyId, int? filterByAgentId)
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
           

            var visas = GetVisaDtos(searchText, page, pageSize, ptype, processStatusId, filterByAgencyId,
                filterByAgentId);

            if (Request.IsAjaxRequest())
            {
                if (!Request.IsAuthenticated)
                    Redirect("~/Account/Login");

                return PartialView("_visa", visas);
            }
            return View(visas);
        }

        public ActionResult ShortDetail(string searchText, string employeeId)
        {
            var empId = Convert.ToInt32(employeeId);
            var visas = GetVisaDtos(searchText, null, null, null, 1, null, null).ToList();
            var empVisa = new EmployeeVisaViewModel();

            if (visas.Any())
                empVisa = new EmployeeVisaViewModel
                {
                    Visa = visas.FirstOrDefault(),
                    Employee = new EmployeeDTO {Id = empId},
                    IsVisaAssigned = true
                };

            return PartialView("_visaShortDetail", empVisa);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShortDetail(EmployeeVisaViewModel employeeVisaVm)
        {
            if (employeeVisaVm.Employee.Id != 0 && employeeVisaVm.Visa.Id != 0)
            {
                var empService = new EmployeeService(false, false);
                try
                {
                    var empDto = empService.Find(employeeVisaVm.Employee.Id.ToString());
                    empDto.VisaId = employeeVisaVm.Visa.Id;
                    empDto.AgentId = employeeVisaVm.Visa.ForeignAgentId;
                    empService.InsertOrUpdate(empDto);

                    //Set AgencyId=empDto.AgencyId - If AgencyId is null and Qty==1 in VisaDTO, and save Visa
                    //Otherwise Visa might be distributed to many Agencies
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                finally
                {
                    empService.Dispose();
                }
            }
            return RedirectToAction("Index");
        }

        public IEnumerable<VisaDTO> GetVisaDtos(string searchText, int? page, int? pageSize, int? ptype,
            int? processStatusId, int? filterByAgencyId, int? filterByAgentId)
        {
            var criteria = new SearchCriteria<VisaDTO>
            {
                CurrentUserId = WebSecurity.CurrentUserId
            };

            ViewData["VisaStatusHeader"] = "All";
            @ViewData["processStatusId"] = processStatusId;
            if (processStatusId != null && processStatusId > 0)
            {
                if (processStatusId == Convert.ToInt32(VisaAssignedTypes.NotAssgnedVisa))
                {
                    criteria.FiList.Add(p => p.Employees.Count < p.VisaQuantity);
                    ViewData["VisaStatusHeader"] = "Not Assigned";
                }
                else if (processStatusId == Convert.ToInt32(VisaAssignedTypes.AssignedVisa))
                {
                    criteria.FiList.Add(p => p.Employees.Count == p.VisaQuantity);
                    ViewData["VisaStatusHeader"] = "Assigned";
                }
            }

            #region Filter by Agency/Agent

            if (filterByAgencyId != null && filterByAgencyId > 0)
            {
                criteria.FiList.Add(v => v.AgencyId == filterByAgencyId);
            }

            if (filterByAgentId != null && filterByAgentId > 0)
            {
                criteria.FiList.Add(v => v.ForeignAgentId == filterByAgentId);
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
                criteria.FiList.Add(bp => bp.Sponsor.FullName.ToLower().Contains(searchText.ToLower()) ||
                                          bp.VisaNumber.ToLower().Contains(searchText.ToLower()) ||
                                          bp.Notes.ToLower().Contains(searchText.ToLower()));

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
            var visas = _visaService.GetAll(criteria, out totalCount).ToList();
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
            foreach (var visaDTO in visas)
            {
                visaDTO.SerialNumber = sNo;
                sNo++;
            }

            #endregion

            return visas;
        }

        public ActionResult Details(string visaId)
        {
            int visId = EncryptionUtility.Hash64Decode(visaId);
            var visaDTO = new VisaDTO
            {
                Sponsor = new VisaSponsorDTO
                {
                    Address = new AddressDTO()
                },
                Condition = new VisaConditionDTO()
            };

            if (visId != 0)
                visaDTO = new VisaService(true, true)
                    .GetAll()
                    .ToList()
                    .FirstOrDefault(v => v.Id == visId);

            return View(visaDTO);
        }

        public ActionResult Create(string visaId)
        {
            var visa = CommonUtility.GetNewVisaDTO(WebUtility.GetAgentIdForTheAgency(-1));

            return View(visa);
        }
        
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(VisaDTO visaDto)
        {
            try
            {
                if (visaDto.Id == 0)
                {
                    var visaService = new VisaService(false, true);
                    var visa = GetNewVisa();

                    MapVisa(ref visa, visaDto);

                    string stat = visaService.InsertOrUpdate(visa);
                    visaService.Dispose();

                    if (!string.IsNullOrEmpty(stat))
                    {
                        ModelState.AddModelError("", stat);
                        //return View();
                    }
                    else return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                //
            }
            // If we got this far, something failed, redisplay form
            return View();
        }

        public ActionResult Edit(string visaId)
        {
            if (!string.IsNullOrEmpty(visaId))
            {
                var vId = EncryptionUtility.Hash64Decode(visaId);
                var cri = new SearchCriteria<VisaDTO>()
                {
                    CurrentUserId = WebSecurity.CurrentUserId
                };
                cri.FiList.Add(v => v.Id == vId);
                var visaDTO = new VisaService(true, true)
                    .GetAll(cri).ToList().FirstOrDefault();

                return View(visaDTO);
            }
            return View("Create");
        }
        [HttpPost]
        public ActionResult Edit(VisaDTO visaDto)
        {
            try
            {
                var visaService = new VisaService(false, true);
                var cri = new SearchCriteria<VisaDTO>()
                {
                    CurrentUserId = WebSecurity.CurrentUserId
                };
                cri.FiList.Add(v => v.Id == visaDto.Id);
                var visa = visaService.GetAll(cri).ToList().FirstOrDefault();
                if (visa != null)
                {
                    MapVisa(ref visa, visaDto);

                    string stat = visaService.InsertOrUpdate(visa);
                    visaService.Dispose();

                    if (!string.IsNullOrEmpty(stat))
                    {
                        ModelState.AddModelError("", stat);
                        //return View();
                    }
                    else return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                //
            }
            // If we got this far, something failed, redisplay form
            return View();
        }

        public ActionResult Delete(string visaId)
        {
            int viId = EncryptionUtility.Hash64Decode(visaId);
            var visaDTO = new VisaService(true, true)
                .GetAll()
                .ToList()
                .FirstOrDefault(v => v.Id == viId);

            return View(visaDTO);
        }
        [HttpPost]
        public ActionResult Delete(VisaDTO visaDto)
        {
            try
            {
                var visaService = new VisaService(false, true);
                var visa = visaService.Find(visaDto.Id.ToString());

                if (visa != null)
                {
                    visaService.Delete(visa.Id.ToString());
                    visaService.Dispose();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Detach(string visaId)
        {
            int visId = EncryptionUtility.Hash64Decode(visaId);
            var visaDTO = new VisaService(true, true)
                .GetAll()
                .ToList()
                .FirstOrDefault(v => v.Id == visId);

            return View(visaDTO);
        }
        [HttpPost]
        public ActionResult Detach(VisaDTO visa)
        {
            try
            {
                var employeeService = new EmployeeService(false, false);
                var criteria = new SearchCriteria<EmployeeDTO>
                {
                    CurrentUserId = WebSecurity.CurrentUserId
                };
                criteria.FiList.Add(e => e.VisaId == visa.Id);
                var employees = employeeService.GetAll(criteria).ToList();

                foreach (var employeeDTO in employees)
                {
                    employeeDTO.VisaId = null;
                    employeeService.InsertOrUpdate(employeeDTO);
                }

                employeeService.Dispose();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public VisaDTO GetNewVisa()
        {
            //var agencyId = Singleton.Agency.Id;
            return CommonUtility.GetNewVisaDTO(WebUtility.GetAgentIdForTheAgency(-1));
            //return new VisaDTO
            //{
            //    VisaQuantity = 1,
            //    VisaDateArabic = "01/01/1440",
            //    ForeignAgentId = WebUtility.GetAgentIdForTheAgency(-1),
            //    Sponsor = new VisaSponsorDTO
            //    {
            //        Address = new AddressDTO
            //        {
            //            AddressType = AddressTypes.Foreign,
            //            Country = CountryList.SaudiArabia,
            //            CountryAmharic = CountryListAmharic.ሳውዲአረቢያ,
            //            City = EnumUtil.GetEnumDesc(CityList.Riyadh),
            //            CityAmharic = EnumUtil.GetEnumDesc(CityListAmharic.ሪያድ)
            //        },
            //        FullNameAmharic = "---"
            //    },
            //    Condition = new VisaConditionDTO
            //    {
            //        Age = AgeCategory.Bet1825,
            //        Religion = ReligionTypes.Muslim,
            //        Salary = 850,
            //        CurrencyType = CurrencyTypes.SaudiArabia,
            //        Profession = ProffesionTypes.Housemaid,
            //        ProfessionAmharic = ProffesionTypesAmharic.Housemaid,
            //        FirstTime = true,
            //        GoodLooking = true
            //    },
            //};
        }

        private void MapVisa(ref VisaDTO visa, VisaDTO visaDto)
        {
            visa.VisaNumber = visaDto.VisaNumber;
            visa.VisaQuantity = visaDto.VisaQuantity;
            visa.VisaDate = visaDto.VisaDate;

            visa.Sponsor.FullName = visaDto.Sponsor.FullName.ToUpper();
            visa.Sponsor.PassportNumber = visaDto.Sponsor.PassportNumber;
            visa.Sponsor.Address.Mobile = visaDto.Sponsor.Address.Mobile;
            visa.Sponsor.Address.City = visaDto.Sponsor.Address.City.ToUpper();

            if (visaDto.AgencyId!=null && visaDto.AgencyId != -1)
            {
                visa.AgencyId = visaDto.AgencyId;
                visa.Sponsor.AgencyId = visaDto.AgencyId;
                visa.Sponsor.Address.AgencyId = visaDto.AgencyId;//??null;
                visa.Condition.AgencyId = visaDto.AgencyId;
                visa.ForeignAgentId = WebUtility.GetAgentIdForTheAgency(visaDto.AgencyId);
            }
            else
            {
                visa.AgencyId = null;
                visa.Sponsor.AgencyId = null;
                visa.Sponsor.Address.AgencyId = null;
                visa.Condition.AgencyId = null;
            }

            visa.Condition.Salary = visaDto.Condition.Salary;
            visa.Condition.Religion = visaDto.Condition.Religion;
            visa.Condition.ContratPeriod = visaDto.Condition.ContratPeriod;
            visa.Condition.Profession = visaDto.Condition.Profession;
            visa.Condition.Age = visaDto.Condition.Age;
            visa.Condition.Complexion = visaDto.Condition.Complexion;
        }
        
        public void LoadEnums()
        {
            @ViewData["ReligionFilter"] = HelperUtility.ReligionEnums();
            @ViewData["ProfessionFilter"] = HelperUtility.ProfessionEnums();
            @ViewData["AgeCategoryFilter"] = HelperUtility.AgeCategoryEnums();
            @ViewData["ContratPeriodFilter"] = HelperUtility.ContratPeriodsEnums();
            @ViewData["ComplexionFilter"] = HelperUtility.ComplexionEnums();

            FilterLists();
        }

        public void FilterLists()
        {
            var visaStatusFilterList = new List<SelectListItem>
            {
                new SelectListItem {Value = "-1", Text = "All"},
                new SelectListItem {Value = "1", Text = "New Visas"},
                new SelectListItem {Value = "2", Text = "Assigned Visas"},
            };
            var cats = new SelectList(visaStatusFilterList, "value", "text");
            @ViewData["VisaStatus"] = cats;

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
            agentFilterList.AddRange(agents.Select(agent => new SelectListItem {Value = agent.Value, Text = agent.Text}));
            //agentFilterList.AddRange(HelperUtility.GetAgents());
            @ViewData["AgentFilter"] = new SelectList(agentFilterList, "value", "text");

            agentFilterList[0].Text = "All"; // new SelectListItem { Value = "-1", Text = "All" };
            @ViewData["AgentList"] = new SelectList(agentFilterList, "value", "text");
        }
    }
}