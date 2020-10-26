using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.Service.Interfaces;
using PinnaFace.Web.Models;
using WebMatrix.WebData;

namespace PinnaFace.Web.Controllers
{
    [Authorize]
    public class ComplainController : Controller
    {
        private readonly IComplainService _complainService;
        
        public ComplainController()
        {
            LoadEnums();
            _complainService = new ComplainService();
        }

        public ActionResult Index(string searchText, int? page, int? pageSize, int? ptype, int? processStatusId)
        {
            var criteria = new SearchCriteria<ComplainDTO>
            {
                CurrentUserId = WebSecurity.CurrentUserId
            };

            @ViewData["processStatusId"] = processStatusId;
            if (processStatusId != null && processStatusId > -1)
            {
                var compStatus = (ComplainStatusTypes) processStatusId;
                criteria.FiList.Add(p => p.Status == compStatus);

                @ViewData["ComplainStatusHeader"] = compStatus;
            }
            else if (processStatusId == -1)
            {
                @ViewData["ComplainStatusHeader"] = "All";    
            }
            else if (processStatusId == null)
            {
                criteria.FiList.Add(p => p.Status == ComplainStatusTypes.Opened ||
                                            p.Status == ComplainStatusTypes.ReOpened ||
                                            p.Status == ComplainStatusTypes.OnProcess);
                @ViewData["ComplainStatusHeader"] = "New/Reopened/OnProcess";
            }
            #region Search Text
            if (!string.IsNullOrEmpty(searchText))
            {
                ViewData["ItemSearch"] = "search results for '<strong style='font-size:14px;'>" + searchText + "</strong>'";
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
                criteria.FiList.Add(bp => bp.Complain.ToLower().Contains(searchText.ToLower()) ||
                                    bp.RaisedByName.ToLower().Contains(searchText.ToLower()));
            #endregion

            #region Paging
            if (page != null && ptype != null && pageSize != null)
            {
                criteria.Page = (int)(page + ptype);
                criteria.PageSize = (int)pageSize;

                if (criteria.Page < 1)
                    criteria.Page = 1;
            }
            else
            {
                criteria.Page = 1;
                criteria.PageSize = 10;
            }
            if (criteria.Page == 1)
                ViewData["prevDisabled"] = "none";
            ViewData["Page"] = criteria.Page;
            ViewData["PageSize"] = criteria.PageSize;

            #endregion

            int totalCount;
            var complains = _complainService.GetAll(criteria, out totalCount);
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
            var sNo = (criteria.Page - 1) * criteria.PageSize + 1;
            foreach (var complainDTO in complains)
            {
                complainDTO.SerialNumber = sNo;
                sNo++;
            }
            #endregion


            if (Request.IsAjaxRequest())
            {
                if (!Request.IsAuthenticated)
                    Redirect("~/Account/Login");
                return PartialView("_complain", complains);
            }
            return View(complains);
        }

        
        public ActionResult Details(string compId)
        {
            int complainId = EncryptionUtility.Hash64Decode(compId);
            var complainDTO = new ComplainDTO
            {
                Employee = new EmployeeDTO()
            };
            if (complainId != 0)
            complainDTO = new ComplainService(true)
                .GetAll()
                .ToList()
                .FirstOrDefault(v => v.Id == complainId );
            return View(complainDTO);
        }
        

        public ActionResult Create(string empId, string searchText)
        {
            bool employeefound = false;
            var complainDTO = new ComplainDTO
            {
                AgencyId = Singleton.Agency.Id,
                ComplainDate = DateTime.Now,
                Priority = ComplainProrityTypes.Medium,
                Type = ComplainTypes.StatusNotKnown
            };

            if (!string.IsNullOrEmpty(empId))
            {
                int employeeId = EncryptionUtility.Hash64Decode(empId);
                var employee = new EmployeeService(true, true)
                    .GetAll().ToList()
                    .FirstOrDefault(v => v.Id == employeeId);
                employeefound = true;

                complainDTO.EmployeeId = employeeId;
                complainDTO.Employee = employee;
                complainDTO.Complain = "...";
            }
            else if (!string.IsNullOrEmpty(searchText))
            {
                var criteria = new SearchCriteria<EmployeeDTO>();
                if (!string.IsNullOrEmpty(searchText))
                    criteria.FiList.Add(bp => bp.Visa != null &&
                                              bp.Visa.Sponsor != null &&
                                              bp.CurrentStatus == ProcessStatusTypes.OnGoodCondition &&
                                              (bp.FullName.ToLower().Contains(searchText.ToLower()) ||
                                               bp.PassportNumber.ToLower().Contains(searchText.ToLower()) ||
                                               bp.Visa.Sponsor.PassportNumber.ToLower().Contains(searchText.ToLower()) ||
                                               bp.Visa.Sponsor.FullName.ToLower().Contains(searchText.ToLower()) ||
                                               bp.MoreNotes.ToLower().Contains(searchText.ToLower())));

                var employee = new EmployeeService(true, true)
                    .GetAll(criteria).ToList().FirstOrDefault();

                if (employee != null)
                {
                    employeefound = true;
                    complainDTO.EmployeeId = employee.Id;
                    complainDTO.Employee = employee;
                    complainDTO.Complain = "...";
                }
            }



            if (Request.IsAjaxRequest())
            {
                if (!Request.IsAuthenticated)
                    Redirect("~/Account/Login");

                if (!employeefound)
                complainDTO.Complain = "No Employee/Visa is found, please try your search again...";
            
                return PartialView("_employeeVisaShortDetail", complainDTO);
            }

            return View(complainDTO);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComplainDTO complainDTO)
        {
            //try{}catch{}
                if (ModelState.IsValid)
                {
                    if (complainDTO.EmployeeId == 0)
                        return RedirectToAction("Create");
                    
                    var empService = new EmployeeService();
                    var emp=empService.Find(complainDTO.EmployeeId.ToString());
                    emp.Complains.Add(complainDTO);
                    emp.CurrentComplain = complainDTO;
                    //emp.CurrentStatus=ProcessStatusTypes.WithComplain;
                    empService.InsertOrUpdate(emp);
                    empService.Dispose();
                    
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                return View(complainDTO);
            
        }
        

        public ActionResult AddRemark(string compId)
        {
            int complainId = EncryptionUtility.Hash64Decode(compId);
            var complainRemarkDTO = new ComplainRemarkDTO
            {
                AgencyId = Singleton.Agency.Id,
                Complain = new ComplainDTO()
            };
            if (complainId != 0)
            {
                var complainDTO = new ComplainService(true)
                    .GetAll()
                    .ToList()
                    .FirstOrDefault(v => v.Id == complainId);

                if (complainDTO != null)
                {
                    complainRemarkDTO.Complain = complainDTO;
                    complainRemarkDTO.ComplainId = complainDTO.Id;
                    complainRemarkDTO.Complain.Remarks =
                        complainDTO.Remarks.OrderByDescending(c => c.RemarkDate).ToList();
                    complainRemarkDTO.RemarkDate = DateTime.Now;
                }
                else
                {
                    return RedirectToAction("Index");
                }
                
            }
            return View(complainRemarkDTO);
            
        }
        [HttpPost]
        public ActionResult AddRemark(ComplainRemarkDTO complainRemarkDTO)
        {
            try
            {
                var complainService = new ComplainService(false);
                var compDTO = complainService.Find(complainRemarkDTO.ComplainId.ToString());

                var complainRemarkDTO2 = new ComplainRemarkDTO
                {
                    Complain = compDTO,
                    AgencyId =  Singleton.Agency.Id,
                    RemarkDate = complainRemarkDTO.RemarkDate,
                    Remark = complainRemarkDTO.Remark
                };

                complainService.InsertOrUpdateRemark(complainRemarkDTO2);
                
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult CloseComplain(string compId)
        {
            int complainId = EncryptionUtility.Hash64Decode(compId);
            var complainDTO = new ComplainDTO
            {
                Employee = new EmployeeDTO()
            };
            if (complainId != 0)
            {
                complainDTO = new ComplainService(true)
                    .GetAll()
                    .ToList()
                    .FirstOrDefault(v => v.Id == complainId);
                if (complainDTO != null) complainDTO.FinalSolutionDate = DateTime.Now;
            }
            return View(complainDTO);
            
        }
        [HttpPost]
        public ActionResult CloseComplain(ComplainDTO complainDTO)
        {
            try
            {
                var complainService = new ComplainService(false);
                var compDTO=complainService.Find(complainDTO.Id.ToString());
                compDTO.FinalSolution = complainDTO.FinalSolution;
                compDTO.FinalSolutionDate = complainDTO.FinalSolutionDate;
                compDTO.Status=ComplainStatusTypes.Closed;
                complainService.InsertOrUpdate(compDTO);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        

        public ActionResult Edit(int id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
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
        

        public ActionResult Confirm(string compId)
        {
            int complainId = EncryptionUtility.Hash64Decode(compId);
            var complainDTO = new ComplainDTO
            {
                Employee = new EmployeeDTO()
            };
            if (complainId != 0)
            {
                complainDTO = new ComplainService(true)
                    .GetAll()
                    .ToList()
                    .FirstOrDefault(v => v.Id == complainId);
                if (complainDTO != null) complainDTO.ConfirmationDate = DateTime.Now;
            }
            return View(complainDTO);
        }
        [HttpPost]
        public ActionResult Confirm(ComplainDTO complainDTO)
        {
            try
            {
                var empService = new EmployeeService(false,true);

                var cri = new SearchCriteria<EmployeeDTO>();
                cri.FiList.Add(e=>e.Id==complainDTO.EmployeeId);
                var emp = empService.GetAll(cri).FirstOrDefault();

                if (emp != null)
                {
                    var compDTO = emp.Complains.FirstOrDefault(c=>c.Id==complainDTO.Id);

                    if (compDTO != null)
                    {
                        compDTO.Confirmation = complainDTO.Confirmation;
                        compDTO.ConfirmationDate = complainDTO.ConfirmationDate;
                        compDTO.Status = ComplainStatusTypes.Confirmed;
                
                        emp.CurrentStatus = ProcessStatusTypes.OnGoodCondition;
                        empService.InsertOrUpdate(emp);
                        empService.Dispose();
                    }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        

        public ActionResult ReOpen(string compId)
        {
            int complainId = EncryptionUtility.Hash64Decode(compId);
            var complainDTO = new ComplainDTO
            {
                AgencyId =  Singleton.Agency.Id,
                Employee = new EmployeeDTO()
            };
            if (complainId != 0)
            {
                complainDTO = new ComplainService(true)
                    .GetAll()
                    .ToList()
                    .FirstOrDefault(v => v.Id == complainId);
                if (complainDTO != null) complainDTO.ReOpeningDate = DateTime.Now;
            }
            return View(complainDTO);
        }
        [HttpPost]
        public ActionResult ReOpen(ComplainDTO complainDTO)
        {
            try
            {
                var complainService = new ComplainService(false);
                var compDTO = complainService.Find(complainDTO.Id.ToString());
                compDTO.ReOpening = complainDTO.ReOpening;
                compDTO.ReOpeningDate = complainDTO.ReOpeningDate;
                compDTO.FinalSolution ="";
                compDTO.FinalSolutionDate = null;
                compDTO.Status = ComplainStatusTypes.ReOpened;
                complainService.InsertOrUpdate(compDTO);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult Delete(string compId)
        {
            int complainId = EncryptionUtility.Hash64Decode(compId);
            var complainDTO = new ComplainDTO
            {
                Employee = new EmployeeDTO()
            };
            if (complainId != 0)
            {
                complainDTO = new ComplainService(true)
                    .GetAll()
                    .ToList()
                    .FirstOrDefault(v => v.Id == complainId);
            }
            //else RedirectToAction("Index");

            return View(complainDTO);
        }
        [HttpPost]
        public ActionResult Delete(ComplainDTO complainDTO)
        {
            try
            {
                var compService = new ComplainService(false);
                compService.Delete(complainDTO.Id.ToString());
                compService.Dispose();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(complainDTO);
            }
        }


        public void LoadEnums()
        {
            @ViewData["ComplainPriorityFilter"] = HelperUtility.ComplainProrityTypesEnums();
            @ViewData["ComplainTypeFilter"] = HelperUtility.ComplainTypesEnums();
            @ViewData["ComplainStatusFilter"] = HelperUtility.ComplainStatusTypesEnums();
            FilterLists();
            
        }
        
        public void FilterLists()
        {
            var complainStatusFilterList = new List<SelectListItem>
            {
                new SelectListItem {Value = "-1", Text = "All"},
                new SelectListItem {Value = "0", Text = "New"},
                new SelectListItem {Value = "4", Text = "Re-Opened"},
                new SelectListItem {Value = "1", Text = "OnProcess"},
                new SelectListItem {Value = "2", Text = "Closed"},
                new SelectListItem {Value = "3", Text = "Confirmed"},
               
            };
            var cats = new SelectList(complainStatusFilterList, "value", "text");
            @ViewData["ComplainStatus"] = cats;

        }
    }
}