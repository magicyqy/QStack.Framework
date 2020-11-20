using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model;
using QStack.Framework.Basic.Model.ViewModel.Auth;
using QStack.Framework.Core.Entity;
using QStack.Framework.Core;
using QStack.Framework.Util;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Text.RegularExpressions;

namespace QStack.Web.Areas.Api.Controllers
{
    [Route("{area:exists}/[controller]/[action]/{id?}")]
    public class DataAuthController : ApiBaseController
    {
        private IDataAuthService _dataAuthService;
        public DataAuthController(IDataAuthService dataAuthService)
        {
            _dataAuthService = dataAuthService;
            
        }

        public async Task<ResponseResult> Post(RuleGroup dto)
        {
            var dataAuthRule = new DataAuthRuleDto
            {
                Id=dto.Id,
                EntityType = dto.EntityType,
                RuleGroup = JsonSerializer.Serialize(dto),
                Repository = dto.Repository,
                RuleState = QStack.Framework.Basic.Model.DataAuthRuleState.Stopped,
                LambdaExpression = dto.GetExpression().ToExpressionString(),
                Title=dto.Title

            };
          
            
            var item = await _dataAuthService.AddOrUpdate<DataAuthRuleDto, int>(dataAuthRule);
            var result = new ResponseResult<int>(item.Id);

            return result;
         

        }

        public async Task<ResponseResult<RuleGroup>> Get(int id)
        {
            var dataItem =await _dataAuthService.QueryById<DataAuthRuleDto>(id);
            var ruleGroup = JsonSerializer.Deserialize<RuleGroup>(dataItem.RuleGroup);
            ruleGroup.Id = dataItem.Id;
            return new ResponseResult<RuleGroup>(ruleGroup);
        }
        public async Task<ResponseResult<PageModel<DataAuthRuleDto>>> GetPages(DataTableOption query)
        {
            return await base.SearchQuery<DataAuthRuleDto, IDataAuthService>(query);
        }
        public async Task<ResponseResult> Delete(int id)
        {
            var result = new ResponseResult();


            await _dataAuthService.DeleteByIdAsync(id);


            return result;
        }
        public async Task<ResponseResult> UpdateState(int ruleId, DataAuthRuleState state)
        {
            await _dataAuthService.UpdateState(ruleId,state);

            return new ResponseResult();
        }

        public async Task<ResponseResult> GenerateExpresssion(RuleGroup dto)
        {
            var result = new ResponseResult<string>();
            try
            {
                var context = HttpContext.RequestServices.GetService(typeof(IEnviromentContext)) as IEnviromentContext;
                var envDic = new Dictionary<string, object>();
                if (context != null)
                {
                    envDic = context.ToEnviromentDictionary();
                    
                }
                result.Data = dto.GetExpression(envDic).ToExpressionString();

            }
            catch(Exception e)
            {
                result.Data = e.Message;
            }

            return await Task.FromResult(result);
        }

        public async Task<ResponseResult<dynamic>> GetDaoFactotyInfo()
        {
            var items = _dataAuthService.GetDaoFactoryEntityInfo();

            var resultItems = items.Select(i => new { Name = i.Key, Entities = i.Value });

            var result = new ResponseResult<dynamic>();
            result.Data = resultItems;

            return await Task.FromResult(result);
        }
        public async Task<ResponseResult<List<PropertyInfoDto>>> GetEnityPropertyList([FromForm]string entityType,[FromForm]bool isFirstLevel)
        {
            var type = Type.GetType(entityType);
            if (type.IsGenericType)
                type = type.GetGenericArguments().First();
            var propertyInfos = GetPropertyInfoFromEntityType(type);
            if (isFirstLevel)
            {
                propertyInfos= propertyInfos.Concat(GetEnviromentProperties()).ToList();
            }
            var result = new ResponseResult<List<PropertyInfoDto>>(propertyInfos);

            return await Task.FromResult(result);
        }

        private IEnumerable<PropertyInfoDto> GetEnviromentProperties()
        {
            var properties = GetPropertyInfoFromEntityType(typeof(IEnviromentContext));
            foreach(var p in properties)
            {
                p.FieldKind = FieldKind.EnvType;
                p.propertyName = $"{{{p.propertyName}}}";
                p.Name = Regex.Match(p.Name, @"\(\S+\)$").Value.TrimStart('(').TrimEnd(')');
                p.Name =$"{{{p.Name}}}";
                p.Leaf = true;
            }
          
            return properties;
        }

        private List<PropertyInfoDto> GetPropertyInfoFromEntityType(Type type)
        {
            var properties = type.GetProperties().Where(p => !p.IsDefined(typeof(NotMappedAttribute)));

            var list = new List<PropertyInfoDto>();
            foreach(var item in properties)
            {
                var displayName = item.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                var propertyInfo = new PropertyInfoDto
                {
                    Name = item.Name+ (displayName.IsNullOrWhiteSpace()?"":$"({displayName})") ,
                    EntityType = type.AssemblyQualifiedName,
                    propertyName = item.Name,
                    FieldKind =typeof(IEntityRoot).IsAssignableFrom(item.PropertyType)? FieldKind.Reference :typeof(string)!=item.PropertyType&&IsAssignableToGenericType( item.PropertyType,typeof(IEnumerable<>)) ? FieldKind.ICollection : FieldKind.ValueType,                    
                    PropertyType=item.PropertyType.AssemblyQualifiedName
                };
                if (propertyInfo.FieldKind != FieldKind.Reference)
                    propertyInfo.Leaf = true;
                if (propertyInfo.FieldKind == FieldKind.ICollection)
                    propertyInfo.GenericType = item.PropertyType.GetGenericArguments()[0].AssemblyQualifiedName;
                //以下如果循环引用导致overflow exception
                //if(propertyInfo.FieldKind==FieldKind.Reference)                
                //{
                //    propertyInfo.Children = GetPropertyInfoFromEntityType(item.PropertyType);
                //}
                //if(propertyInfo.FieldKind==FieldKind.ICollection)
                //{
                //    var genericType = item.PropertyType.GetGenericArguments().First();
                //    propertyInfo.Children = GetPropertyInfoFromEntityType(genericType);
                //}
                list.Add(propertyInfo); 
            }
            return list;
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}