using NUnit.Framework;
using QStack.Framework.Basic.Model.Auth;
using QStack.Framework.Core.Entity;
using QStack.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QStack.Framework.Core.Entity.Tests
{
    [TestFixture()]
    public class DataPrivilegeRuleTests
    {
        [Test()]
        public void GetExpressionTest()
        {
            var user = new User
            {
                Name = "user1",
                GroupId = 1,
                Group = new Group { Id = 1, Name = "group1" }

            };
            var role1 = new Role { Id = 1, Name = "role1" };
            var role2 = new Role { Id = 2, Name = "role2" };
            user.Roles = new List<Role> { role1, role2 };

            var ruleGroup = new RuleGroup
            {
                EntityType = typeof(User).AssemblyQualifiedName,
                ConditionType = Query.Condition.OrElse,
                Rules = new List<DataPrivilegeRule>
                 {
                     new DataPrivilegeRule
                     {
                          EntityType=typeof(User).AssemblyQualifiedName,
                           FieldKind=FieldKind.ValueType,
                           PropertyNames=new string[]{"Name" },
                           FilterValue="user1",
                           Operator=Query.Operators.Equal,
                           PropertyType=typeof(string).AssemblyQualifiedName,
                           RuleGroup=null
                     },
                     new DataPrivilegeRule
                     {
                           EntityType=typeof(User).AssemblyQualifiedName,
                           PropertyNames=new string[]{"UserRoles" },
                           FieldKind=FieldKind.ICollection,
                           PropertyType=typeof(List<UserRole>).AssemblyQualifiedName,
                           RuleGroup=new RuleGroup
                           {
                                EntityType=typeof(UserRole).AssemblyQualifiedName,
                                Rules = new List<DataPrivilegeRule>
                                 {
                                     new DataPrivilegeRule
                                     {
                                          EntityType=typeof(UserRole).AssemblyQualifiedName,
                                           FieldKind=FieldKind.ValueType,
                                           PropertyNames=new string[]{"Role","Name" },
                                           FilterValue="role1",
                                           Operator=Query.Operators.Equal,
                                            PropertyType=typeof(string).AssemblyQualifiedName,
                                             RuleGroup=null
                                     }
                                 }
                           }
                     }
                 },
                 NestedRules=    new List<RuleGroup>
                     {
                        new RuleGroup
                        {
                            EntityType=typeof(User).AssemblyQualifiedName,
                            Rules = new List<DataPrivilegeRule>
                                 {
                                     new DataPrivilegeRule
                                     {
                                          EntityType=typeof(User).AssemblyQualifiedName,
                                           FieldKind=FieldKind.ValueType,
                                           PropertyNames=new string[]{"Group","Name" },
                                           FilterValue="user1",
                                           Operator=Query.Operators.Equal,
                                            PropertyType=typeof(string).AssemblyQualifiedName,
                                             RuleGroup=null
                                     }
                                 }
                        }
                        
                     }
            };

            var expression = ruleGroup.GetExpression();
            Console.WriteLine(expression.ToExpressionString());
            var users = new List<User> { user };
            var item= users.Where((Func<User, bool>)expression.Compile()).First();
            Console.WriteLine(item.Name);
            Assert.IsNotNull(item);
        }
    }
}