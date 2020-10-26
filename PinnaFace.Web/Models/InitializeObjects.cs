using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Service;
using WebMatrix.WebData;

namespace PinnaFace.Web.Models
{
    public class InitializeObjects
    {
        public void InitializeWebSecurity()
        {
            var dbContext2 = DbContextUtil.GetDbContextInstance();
            try
            {
                if (!WebSecurity.Initialized)
                    WebSecurity.InitializeDatabaseConnection(
                        Singleton.ConnectionStringName, 
                        Singleton.ProviderName, 
                        "Users",
                        "UserId", 
                        "UserName", false);

                if (!new UserService(true).GetAll().Any())
                {
                    #region Seed Default Roles and Users
                    
                    //Check if roles are not allready added
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
                    

                    WebSecurity.CreateUserAndAccount("superweb", "P@ssw0rd1!",
                        new
                        {
                            Status = 1,
                            Enabled = true,
                            RowGuid = Guid.NewGuid(),
                            Email = "superuser@pinnaface.com",
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        });
                    WebSecurity.CreateUserAndAccount("adminweb", "P@ssw0rd",
                        new
                        {
                            Status = 0,
                            Enabled = true,
                            RowGuid = Guid.NewGuid(),
                            Email = "adminuser@pinnaface.com",
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        });
                    WebSecurity.CreateUserAndAccount("pinnaweb", "pa12345",
                        new
                        {
                            Status = 0,
                            Enabled = true,
                            RowGuid = Guid.NewGuid(),
                            Email = "pinnauser@pinnaface.com",
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        });

                    //add row guid for membership table members
                    //var members = new UserService().GetAllMemberShips();
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
                    foreach (var role in lofRoles)
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId("superweb")
                        });
                    }

                    foreach (var role in lofRoles)//.Skip(1)
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId("adminweb")
                        });
                    }

                    foreach (var role in lofRoles)//.Skip(6)
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId("pinnaweb")
                        });
                    }

                    dbContext2.SaveChanges();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                ////MessageBox.Show("Problem on InitializeWebSecurity" +
                ////    Environment.NewLine + ex.Message +
                ////    Environment.NewLine + ex.InnerException);
            }
            finally
            {
                dbContext2.Dispose();
            }
        }
    }
}