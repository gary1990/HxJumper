using HxJumper.Models.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HxJumper.Models.DAL
{
    public class UnitOfWork : IDisposable
    {
        public JumperContext context = new JumperContext();
        private GenericRepository<JumperUser> jumperUserRepository;
        private GenericRepository<JumperRole> jumperRoleRepository;
        private GenericRepository<Permission> permissionRepository;
        private GenericRepository<ProductType> productTypeRepository;
        private GenericRepository<RemarkMessage> remarkMessageRepository;
        private GenericRepository<TestClassNumber> testClassNumberRepository;
        private GenericRepository<TestResult> testResultRepository;
        public GenericRepository<TestResultItem> testResultItemRepository;
        private GenericRepository<TestResultValue> testResultValueRepository;
        private GenericRepository<LineNumber> lineNumberRepository;
        private GenericRepository<TestItem> testItemRepository;
        private GenericRepository<TestResultPim> testResultPimRepository;
        private GenericRepository<TestEquipment> testEquipmentRepository;
        private GenericRepository<ImOrder> imOrderRepository;
        private GenericRepository<Carrier> carrierRepository;
        private GenericRepository<TestResultPimPoint> testResultPimPointRepository;
        private GenericRepository<LimitValue> limitValueRepository;
        public GenericRepository<JumperUser> JumperUserRepository 
        {
            get 
            {
                if (this.jumperUserRepository == null)
                {
                    this.jumperUserRepository = new GenericRepository<JumperUser>(context);
                }
                return jumperUserRepository;
            }
        }

        public GenericRepository<JumperRole> JumperRoleRepository
        {
            get
            {
                if (this.jumperRoleRepository == null)
                {
                    this.jumperRoleRepository = new GenericRepository<JumperRole>(context);
                }
                return jumperRoleRepository;
            }
        }

        public GenericRepository<Permission> PermissionRepository
        {
            get
            {
                if (this.permissionRepository == null)
                {
                    this.permissionRepository = new GenericRepository<Permission>(context);
                }
                return permissionRepository;
            }
        }

        public GenericRepository<ProductType> ProductTypeRepository
        {
            get
            {
                if (this.productTypeRepository == null)
                {
                    this.productTypeRepository = new GenericRepository<ProductType>(context);
                }
                return productTypeRepository;
            }
        }

        public GenericRepository<RemarkMessage> RemarkMessageRepository
        {
            get
            {
                if (this.remarkMessageRepository == null)
                {
                    this.remarkMessageRepository = new GenericRepository<RemarkMessage>(context);
                }
                return remarkMessageRepository;
            }
        }

        public GenericRepository<TestClassNumber> TestClassNumberRepository
        {
            get
            {
                if (this.testClassNumberRepository == null)
                {
                    this.testClassNumberRepository = new GenericRepository<TestClassNumber>(context);
                }
                return testClassNumberRepository;
            }
        }

        public GenericRepository<TestResult> TestResultRepository
        {
            get
            {
                if (this.testResultRepository == null)
                {
                    this.testResultRepository = new GenericRepository<TestResult>(context);
                }
                return testResultRepository;
            }
        }

        public GenericRepository<TestResultItem> TestResultItemRepository 
        {
            get
            {
                if (this.testResultItemRepository == null)
                {
                    this.testResultItemRepository = new GenericRepository<TestResultItem>(context);
                }
                return testResultItemRepository;
            }
        }

        public GenericRepository<TestResultValue> TestResultValueRepository
        {
            get
            {
                if (this.testResultValueRepository == null)
                {
                    this.testResultValueRepository = new GenericRepository<TestResultValue>(context);
                }
                return testResultValueRepository;
            }
        }
        public GenericRepository<LineNumber> LineNumberRepository 
        {
            get 
            {
                if (this.lineNumberRepository == null)
                {
                    this.lineNumberRepository = new GenericRepository<LineNumber>(context);
                }
                return lineNumberRepository;
            }
        }

        public GenericRepository<TestItem> TestItemRepository 
        {
            get 
            {
                if(this.testItemRepository == null)
                {
                    this.testItemRepository = new GenericRepository<TestItem>(context);
                }
                return testItemRepository;
            }
        }

        public GenericRepository<TestResultPim> TestResultPimRepository
        {
            get
            {
                if (this.testResultPimRepository == null)
                {
                    this.testResultPimRepository = new GenericRepository<TestResultPim>(context);
                }
                return testResultPimRepository;
            }
        }

        public GenericRepository<TestEquipment> TestEquipmentRepository
        {
            get
            {
                if (this.testEquipmentRepository == null)
                {
                    this.testEquipmentRepository = new GenericRepository<TestEquipment>(context);
                }
                return testEquipmentRepository;
            }
        }

        public GenericRepository<ImOrder> ImOrderRepository
        {
            get
            {
                if (this.imOrderRepository == null)
                {
                    this.imOrderRepository = new GenericRepository<ImOrder>(context);
                }
                return imOrderRepository;
            }
        }

        public GenericRepository<Carrier> CarrierRepository
        {
            get
            {
                if (this.carrierRepository == null)
                {
                    this.carrierRepository = new GenericRepository<Carrier>(context);
                }
                return carrierRepository;
            }
        }

        public GenericRepository<TestResultPimPoint> TestResultPimPointRepository
        {
            get
            {
                if (this.testResultPimPointRepository == null)
                {
                    this.testResultPimPointRepository = new GenericRepository<TestResultPimPoint>(context);
                }
                return testResultPimPointRepository;
            }
        }

        public GenericRepository<LimitValue> LimitValueRepository
        {
            get
            {
                if (this.limitValueRepository == null)
                {
                    this.limitValueRepository = new GenericRepository<LimitValue>(context);
                }
                return limitValueRepository;
            }
        }

        public void JumperSaveChanges() 
        {
            foreach(var deleteEntity in context.ChangeTracker.Entries<BaseModel>())
            {
                if(deleteEntity.State == EntityState.Deleted)
                {
                    deleteEntity.State = EntityState.Unchanged;
                    deleteEntity.Entity.IsDeleted = true;
                }
            }
            context.SaveChanges();
        }

        public void DbSaveChanges()
        {
            context.SaveChanges();
        }

        public void JumperUserSave() 
        {
            foreach(var deletedEntity in context.ChangeTracker.Entries<JumperUser>())
            {
                if(deletedEntity.State == EntityState.Deleted)
                {
                    deletedEntity.State = EntityState.Unchanged;
                    deletedEntity.Entity.IsDeleted = true;
                }
            }
            context.SaveChanges();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}