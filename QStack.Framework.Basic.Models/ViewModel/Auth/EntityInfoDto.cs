using QStack.Framework.Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Basic.Model.ViewModel.Auth
{
    public class EntityInfoDto
    {
        public virtual string Name { get; set; }

        public virtual string EntityType { get; set; }

        public virtual string EntityName { get; set; }

        public virtual List<PropertyInfoDto> Properties { get; set; }


    }

    public class PropertyInfoDto
    {
        public virtual string EntityType { get; set; }
        public virtual string propertyName { get; set; }
         public virtual string Name { get; set; }
        public virtual FieldKind FieldKind { get; set; }
        public virtual string PropertyType { get; set; }
        public virtual string GenericType { get; set; }
        public virtual List<PropertyInfoDto> Children { get; set; }

        public virtual bool Leaf { get; set; }
    }
}
