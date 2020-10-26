﻿using System;
using PinnaFace.DAL.Interfaces;

namespace PinnaFace.Repository
{
    public class UnitOfWork : UnitOfWorkCommon
    {
        public UnitOfWork(IDbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("context");

            Context = dbContext;
            _instanceId = Guid.NewGuid();
        }
    }
}
