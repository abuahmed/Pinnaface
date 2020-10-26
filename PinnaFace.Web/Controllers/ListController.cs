using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using Kendo.Mvc.Extensions;

namespace PinnaFace.Web.Controllers
{
    public class ListController : Controller
    {
        public ActionResult GetEmployees()
        {

            IEnumerable<EmployeeDTO> categories = new EmployeeService().GetAll().ToList();
            IList<ListViewModel> empVms = categories.Select(employeeDTO => new ListViewModel()
            {
                Name = employeeDTO.EmployeeDetail2,
                Id = employeeDTO.Id
            }).ToList();

            return Json(empVms, JsonRequestBehavior.AllowGet);
            
        }

        public SelectList GetAgencies()
        {
            var agncies = new LocalAgencyService(true).GetAll().ToList();
            return new SelectList(agncies,"Id","AgencyName");
        }

        public ActionResult GetBps(string bpType)
        {
            //var businessPartnerType = (BusinessPartnerTypes)Convert.ToInt32(bpType);
            //var criteria = new SearchCriteria<BusinessPartnerDTO>();
            //criteria.FiList.Add(b => b.BusinessPartnerType == businessPartnerType);

            //IEnumerable<BusinessPartnerDTO> bps = new BusinessPartnerService(true)
            //    .GetAll(criteria);
            //IList<ListViewModel> bpVms = bps.Select(bpDTO => new ListViewModel()
            //{
            //    Name = bpDTO.DisplayName,
            //    Id = bpDTO.Id
            //}).ToList();

            //return Json(bpVms, JsonRequestBehavior.AllowGet);
            return null;
        }
        public ActionResult GetCategories(string catType)
        {
            //var categoryType = (NameTypes)Convert.ToInt32(catType);
            //var criteria = new SearchCriteria<CategoryDTO>();
            //criteria.FiList.Add(b => b.NameType == categoryType);

            //IList<CategoryDTO> categories = new List<CategoryDTO>();
            //categories.AddRange(new CategoryService(true).GetAll(criteria));
            //categories.Insert(0, new CategoryDTO()
            //{
            //    Id = -1,
            //    DisplayName = "Show All"
            //});

            //return Json(categories, JsonRequestBehavior.AllowGet);
            return null;
        }
        public ActionResult GetItems()
        {
            //IEnumerable<ItemDTO> items = new ItemService(true).GetAll();
            //IList<ListViewModel> wareVms = items.Select(itemDTO => new ListViewModel()
            //{
            //    Name = itemDTO.ItemDetail,
            //    Id = itemDTO.Id
            //}).ToList();

            //return Json(wareVms, JsonRequestBehavior.AllowGet);
            return null;
        }

        public ActionResult Track(string trackId)
        {
            var employeeVisa = new EmployeeVisaViewModel
            {
                IsVisaAssigned = false,
                Comment=""
            };

            if (!string.IsNullOrEmpty(trackId))
            {
                //int empId = Convert.ToInt32(trackId.Substring(1));
                //var employeeId = empId / 741705; //Convert.ToInt32(trackId);

                var criteria = new SearchCriteria<EmployeeDTO>
                {
                    CurrentUserId = 1
                };

                var searchText = trackId.ToLower();
               
                criteria.FiList.Add(bp => bp.PassportNumber.Contains(searchText) ||
                             (bp.Address != null && bp.Address.Mobile.Contains(searchText)) ||
                             (bp.Visa != null && bp.Visa.VisaNumber.Contains(searchText)) ||
                             (bp.Visa != null && bp.Visa.Sponsor != null &&
                             (bp.Visa.Sponsor.PassportNumber.Contains(searchText) || 
                             (bp.Visa.Sponsor.Address != null && bp.Visa.Sponsor.Address.Mobile.Contains(searchText))))
                             );

                var employee = new EmployeeService(true, true).GetAll(criteria).FirstOrDefault();
                
                if (employee != null && employee.Visa != null)
                {
                    employeeVisa = new EmployeeVisaViewModel
                    {
                        Employee = employee,
                        Visa = employee.Visa,
                        IsVisaAssigned = true,
                        Comment = ""
                    };
                }
                else employeeVisa.Comment = "No visa found...";
            }
            if (Request.IsAjaxRequest())
            {
                if (!Request.IsAuthenticated)
                    Redirect("~/Account/Login");
                
                return PartialView("_visaTrack", employeeVisa);
            }

            return View(employeeVisa);
        }

    }

    public class ListViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
