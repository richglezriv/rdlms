﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RD.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RDModelContainer : DbContext
    {
        public RDModelContainer()
            : base("name=RDModelContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserCourse> UserCourses { get; set; }
        public virtual DbSet<Scorm> Scorms { get; set; }
        public virtual DbSet<ApplicationLog> ApplicationLogs { get; set; }
    }
}
