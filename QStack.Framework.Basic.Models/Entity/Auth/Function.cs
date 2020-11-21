using QStack.Framework.Core.Model;
using System.Collections.Generic;
using System.ComponentModel;

namespace QStack.Framework.Basic.Model.Auth
{
    public class Function:EntityBase
    {

        [DisplayName("类型")]
        public virtual FunctionType FunctionType { get; set; }
        [DisplayName("图标")]
        public virtual string IconUrl { get; set; }
        [DisplayName("代码")]
        public virtual string Code { get; set; }
        [DisplayName("名称")]
        public virtual string Name { get; set; }
        [DisplayName("路由名称")]//一般指客户端路由名
        public virtual string RouteName { get; set; }
        [DisplayName("是否隐藏")]//menu类型才有效
        public virtual bool Hidden { get; set; }
        public virtual int? ParentId { get; set; }
        [DisplayName("父节点")]
        public virtual Function Parent { get; set; }
        [DisplayName("路由")]
        public virtual string Path { get; set; }
        [DisplayName("子功能操作")]
        public virtual IList<FunctionOperation> FunctionOperations { get; set; }
        [DisplayName("角色表")]
        public virtual IList<RoleFunction> RoleFunctions { get; set; }
      
        [DisplayName("描述")]
        public virtual string Describe { get; set; }
        [DisplayName("序号")]
        public virtual int Sequence { get; set; }
        [DisplayName("叶节点")]
        public virtual bool IsLeaf { get; set; }

        public virtual List<Function> Children { get; set; }

    }
}