using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Persistent.EFCore
{
    public class MasterModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
            //=> new MasterModelCacheKey(context);
            => (context as EFCoreDao).CacheKey;
    }


    class MasterModelCacheKey : ModelCacheKey
    {
        int _hashcode;

        public MasterModelCacheKey(DbContext context)
            : base(context)
        {

            _hashcode = (context as EFCoreDao).CacheKey;
        }

        protected override bool Equals(ModelCacheKey other)
        {
            Console.WriteLine($"ModelCacheKey => current:{_hashcode}，other:{(other as MasterModelCacheKey)?._hashcode}");
           return base.Equals(other)
                 && (other as MasterModelCacheKey)?._hashcode == _hashcode;
        }

        //force to regerate this if there are different tenants
        public override int GetHashCode()
        {
            //get the hashCode that is attached right now to the context
            var hashCode = base.GetHashCode();

            //if what is in the base.GetHashCode() it's the same with what is in context that means there are the same tenants & soft delete option and we are not regenerating the model 
            if (_hashcode != hashCode)
            {
                hashCode = _hashcode;
            }

            return hashCode;
        }
    }

}
