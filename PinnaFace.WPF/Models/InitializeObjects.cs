using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Service;
using WebMatrix.WebData;

namespace PinnaFace.WPF.Models
{
    public static class InitializeObjects
    {
        public static void InitializeWebSecurity()
        {
            var dbContext2 = DbContextUtil.GetDbContextInstance();

            try
            {
                if (!WebSecurity.Initialized)
                {
                    //System.Web.Security.Roles.Enabled = true;
                    //System.Web.PreApplicationStartMethodAttribute;
                    WebSecurity.InitializeDatabaseConnection(Singleton.ConnectionStringName, Singleton.ProviderName,
                        "Users",
                        "UserId", "UserName", false);
                }


                if (!new UserService(true).GetAll().Any())
                {
                    #region Seed Default Roles and Users

                    IList<RoleDTO> listOfRoles = CommonUtility.GetRolesList();

                    int sno = 0;
                    foreach (var rol in listOfRoles)
                    {
                        var role = rol;
                        role.RowGuid = Guid.Parse(CommonUtility.GetRolesGuidList()[sno]);
                        dbContext2.Set<RoleDTO>().Add(role);
                        sno++;
                    }
                    dbContext2.SaveChanges();

                    var superName = Singleton.ProductActivation.SuperName; // "superadmin";
                    var superPass = Singleton.ProductActivation.SuperPass; //"P@ssw0rd1!";
                    WebSecurity.CreateUserAndAccount(superName, superPass,
                        new
                        {
                            Status = 1,
                            AgencyId = Singleton.Agency.Id,
                            Enabled = true,
                            Synced = false,
                            RowGuid = Guid.NewGuid(),
                            Email = Singleton.Agency.Address.PrimaryEmail, // "superadmin@pinnaface.com",
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        }, true);

                    var adminName = Singleton.ProductActivation.AdminName; // "adminuser";
                    var adminPass = Singleton.ProductActivation.AdminPass; //"P@ssw0rd";
                    WebSecurity.CreateUserAndAccount(adminName, adminPass,
                        new
                        {
                            Status = 0,
                            AgencyId = Singleton.Agency.Id,
                            Enabled = true,
                            Synced = false,
                            RowGuid = Guid.NewGuid(),
                            Email = "", //adminuser@pinnaface.com
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        });
                    var user1Name = Singleton.ProductActivation.User1Name; // "oneface01";
                    var user1Pass = Singleton.ProductActivation.User1Pass; // "pa12345";
                    WebSecurity.CreateUserAndAccount(user1Name, user1Pass,
                        new
                        {
                            Status = 0,
                            AgencyId = Singleton.Agency.Id,
                            Enabled = true,
                            Synced = false,
                            RowGuid = Guid.NewGuid(),
                            Email = "", //pinnauser@pinnaface.com
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        });

                    //add row guid for membership table members
                    //var members = new UserService().GetAllMemberShips().ToList();
                    var members = dbContext2.Set<MembershipDTO>().ToList();
                    foreach (var membershipDTO in members)
                    {
                        membershipDTO.Synced = false;
                        membershipDTO.RowGuid = Guid.NewGuid();
                        membershipDTO.Enabled = true;
                        membershipDTO.CreatedByUserId = 1;
                        membershipDTO.DateRecordCreated = DateTime.Now;
                        membershipDTO.ModifiedByUserId = 1;
                        membershipDTO.DateLastModified = DateTime.Now;
                        dbContext2.Set<MembershipDTO>().Add(membershipDTO);
                        dbContext2.Entry(membershipDTO).State = EntityState.Modified;
                    }
                    dbContext2.SaveChanges();

                    var lofRoles = new UserService().GetAllRoles().ToList();
                    foreach (var role in lofRoles.Skip(3))
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId(superName)
                        });
                    }

                    foreach (var role in lofRoles.Skip(4))
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId(adminName)
                        });
                    }

                    foreach (var role in lofRoles.Skip(5))
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId(user1Name)
                        });
                    }

                    dbContext2.SaveChanges();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem on InitializeWebSecurity" +
                                Environment.NewLine + ex.Message +
                                Environment.NewLine + ex.InnerException);
            }
            finally
            {
                dbContext2.Dispose();
            }
        } 
    }
}