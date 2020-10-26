using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PinnaFace.Core.Enumerations;

namespace PinnaFace.Core
{
    public class SearchCriteria<TEntity> where TEntity : EntityBase
    {
        public SearchCriteria()
        {
            FiList = new List<Expression<Func<TEntity, bool>>>();
            CurrentUserId = -1;
            TransactionType = -1;
            PaymentType = -1;
            PaymentListType = -1;
            PaymentMethodType = -1;
            
            Page = 0;
            PageSize = 0;
            TotalCount = 0;
        }

        public int? SelectedWarehouseId { get; set; }
        public int? BusinessPartnerId { get; set; }
        public int CurrentUserId { get; set; }
        
        public DateTime? BeginingDate { get; set; }
        public DateTime? EndingDate { get; set; }

        public IList<Expression<Func<TEntity, bool>>> FiList { get; set; }

        public int TransactionType { get; set; }
        public int PaymentType { get; set; }
        public int PaymentListType { get; set; }
        public int PaymentMethodType { get; set; }
        public ReportTypes ReportType { get; set; }

        #region Get Page
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; } 
        #endregion
    }

    public class UserSearchCriteria<TEntity> where TEntity : UserEntityBase
    {
        public UserSearchCriteria()
        {
            FiList = new List<Expression<Func<TEntity, bool>>>();
            CurrentUserId = -1;
            TransactionType = -1;
            PaymentType = -1;
            PaymentListType = -1;
            PaymentMethodType = -1;

            //ReportType = ReportTypes.LabourMonthly;

            Page = 0;
            PageSize = 0;
            TotalCount = 0;
        }

        //public IEnumerable<WarehouseDTO> WarehousesList { get; set; }
        public int? SelectedWarehouseId { get; set; }
        public int? BusinessPartnerId { get; set; }
        public int CurrentUserId { get; set; }
        //public UserDTO CurrentUser { get; set; }
        public DateTime? BeginingDate { get; set; }
        public DateTime? EndingDate { get; set; }

        public IList<Expression<Func<TEntity, bool>>> FiList { get; set; }

        public int TransactionType { get; set; }
        public int PaymentType { get; set; }
        public int PaymentListType { get; set; }
        public int PaymentMethodType { get; set; }
        public ReportTypes ReportType { get; set; }

        #region Get Page
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        #endregion
    }

    public class WebSearchCriteria<TEntity> where TEntity : EntityBase
    {
        public string SearchText { get; set; }
        public int? ProcessStatus { get; set; }

        public int? AgencyId { get; set; }
        public int? AgentId { get; set; }

        public int? AgeCategory { get; set; }
        public int? ReligionCategory { get; set; }
        public int? LanguageCategory { get; set; }
        public int? ExperienceCategory { get; set; }

        #region Get Page
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int? Ptype { get; set; }
        #endregion
    }
}