﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.CustomValidationAttributes;

namespace PinnaFace.Core.Models
{
    [Table("Users")]
    public class UserDTO : UserEntityBase
    {
        public UserDTO()
        {
            Roles = new HashSet<UsersInRoles>();
            AgenciesWithAgents = new HashSet<UserAgencyAgentDTO>();
            Status = UserTypes.Waiting;
        }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        [MinLength(6, ErrorMessage = "User Name Can't be less than Six charactes")]
        [MaxLength(20, ErrorMessage = "User Name Can't be greater than 20 charactes")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "Contains invalid letters")]
        public string UserName
        {
            get { return GetValue(() => UserName); }
            set { SetValue(() => UserName, value); }
        }

        [DataType(DataType.EmailAddress)]
        [MaxLength(50, ErrorMessage = "Exceeded 50 letters")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is invalid")]
        [StringLength(50)]
        public string Email
        {
            get { return GetValue(() => Email); }
            set { SetValue(() => Email, value); }
        }

        [NotMapped]
        public string Password
        {
            get { return GetValue(() => Password); }
            set { SetValue(() => Password, value); }
        }

        [DataType(DataType.Password)]
        [NotMapped]
        public string ConfirmPassword
        {
            get { return GetValue(() => ConfirmPassword); }
            set { SetValue(() => ConfirmPassword, value); }
        }

        
        [StringLength(150)]
        [DisplayName("Full Name")]
        [MaxLength(150, ErrorMessage = "Exceeded 150 letters")]
        public string FullName
        {
            get { return GetValue(() => FullName); }
            set { SetValue(() => FullName, value); }
        }
        
        [NotMapped]
        public string NewPassword
        {
            get { return GetValue(() => NewPassword); }
            set { SetValue(() => NewPassword, value); }
        }

        public string TempPassword
        {
            get { return GetValue(() => TempPassword); }
            set { SetValue(() => TempPassword, value); }
        }

        //[Required]
        public UserTypes? Status
        {
            get { return GetValue(() => Status); }
            set { SetValue(() => Status, value); }
        }
        
        public ICollection<UsersInRoles> Roles
        {
            get { return GetValue(() => Roles); }
            set { SetValue(() => Roles, value); }
        }


        public ICollection<UserAgencyAgentDTO> AgenciesWithAgents
        {
            get { return GetValue(() => AgenciesWithAgents); }
            set { SetValue(() => AgenciesWithAgents, value); }
        }

        [ForeignKey("Agency")]
        public int? AgencyId { get; set; }
        public AgencyDTO Agency
        {
            get { return GetValue(() => Agency); }
            set { SetValue(() => Agency, value); }
        }

        [ForeignKey("Agent")]
        public int? AgentId { get; set; }
        public AgentDTO Agent
        {
            get { return GetValue(() => Agent); }
            set { SetValue(() => Agent, value); }
        }

        [NotMapped]
        public MembershipDTO Membership
        {
            get { return GetValue(() => Membership); }
            set { SetValue(() => Membership, value); }
        }

        [NotMapped]
        public string AccountOwner
        {
            get
            {
                string ownerName = "No One";
                if (AgencyId != null && Agency != null)
                    ownerName = Agency.AgencyName;
                else if (AgentId != null && Agent != null)
                    ownerName = Agent.AgentName;
                return ownerName;
            }
            set { SetValue(() => AccountOwner, value); }
        }

        [NotMapped]
        public string UserDetail
        {
            get
            {
                string ownerName = UserName;
                if (!string.IsNullOrEmpty(FullName))
                    ownerName = ownerName +"/"+FullName;
                if (!string.IsNullOrEmpty(Email))
                    ownerName = ownerName + "/" + Email;
                return ownerName;
            }
            set { SetValue(() => UserDetail, value); }
        }
        [NotMapped]
        public string UserAccountOwnerDetail
        {
            get
            {
                return AccountOwner +" / "+ UserDetail;
            }
            set { SetValue(() => UserAccountOwnerDetail, value); }
        }
    }

    [Table("webpages_Membership")]
    public class MembershipDTO : UserEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        public int UserId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string ConfirmationToken { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime? LastPasswordFailureDate { get; set; }
        public int PasswordFailuresSinceLastSuccess { get; set; }
        public string Password { get; set; }
        public DateTime? PasswordChangedDate { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordVerificationToken { get; set; }
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }
    }

    [Table("webpages_Roles")]
    public class RoleDTO : UserEntityBase
    {
        public RoleDTO()
        {
            Users = new List<UsersInRoles>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; }
        
        [StringLength(255)]
        [MaxLength(255, ErrorMessage = "Exceeded 255 letters")]
        public string RoleDescription
        {
            get { return GetValue(() => RoleDescription); }
            set { SetValue(() => RoleDescription, value); }
        }

        public string RoleDescriptionShort
        {
            get { return GetValue(() => RoleDescriptionShort); }
            set { SetValue(() => RoleDescriptionShort, value); }
        }

        public ICollection<UsersInRoles> Users
        {
            get { return GetValue(() => Users); }
            set { SetValue(() => Users, value); }
        }

    }

    [Table("webpages_UsersInRoles")]
    public class UsersInRoles : UserEntityBase
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public UserDTO User
        {
            get { return GetValue(() => User); }
            set { SetValue(() => User, value); }
        }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public RoleDTO Role
        {
            get { return GetValue(() => Role); }
            set { SetValue(() => Role, value); }
        }
    }
}
