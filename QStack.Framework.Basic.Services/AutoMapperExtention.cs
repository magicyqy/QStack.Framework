using AutoMapper;
using QStack.Framework.Basic.Model;
using QStack.Framework.Basic.Model.Articles;
using QStack.Framework.Basic.Model.Auth;
using QStack.Framework.Basic.ViewModel;
using QStack.Framework.Basic.ViewModel.Articles;
using QStack.Framework.Basic.ViewModel.Auth;
using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QStack.Framework.Basic.Services
{
    public static class AutoMapperExtention
    {
        /// <summary>
        ///遍历添加映射。 注意应确保实体类名是唯一的
        /// </summary>
        /// <param name="mapperConfiguration"></param>
        /// <param name="entityAssemblies"></param>
        public static void AddAutoMaperConfig(this IMapperConfigurationExpression mapperConfiguration, params Assembly[] entityAssemblies)
        {
            var dic = new Dictionary<Type, Type>();
            if (entityAssemblies != null)
            {
                var exportedTypes = entityAssemblies.SelectMany(a => a.ExportedTypes);
                var entitis = exportedTypes.Where(type => type.IsClass && type != typeof(EntityBase) && type != typeof(EntityRoot) && typeof(IEntityRoot).IsAssignableFrom(type));

                foreach (Type type in entitis)
                {
                    var dtoType = exportedTypes.FirstOrDefault(t => t.Name == type.Name + "Dto");

                    if (dtoType != null)
                        dic.Add(type, dtoType);

                }

            }
            foreach (var kp in dic)
                mapperConfiguration.CreateMap(kp.Key, kp.Value).ReverseMap();


            mapperConfiguration.CreateMap(typeof(PageModel<>), typeof(PageModel<>));
            mapperConfiguration.CreateMap<Catagory, CatagoryDto>().ForMember(d => d.ParentName, opt => opt.MapFrom(src => src.Parent.Name)).ReverseMap().ForPath(d => d.Parent.Name, opt => opt.Ignore());
            mapperConfiguration.CreateMap<NavigationMenu, NavigationMenuDto>().ForMember(d => d.ParentName, opt => opt.MapFrom(src => src.Parent.Name)).ReverseMap().ForPath(d => d.Parent.Name, opt => opt.Ignore());
            mapperConfiguration.CreateMap<Group, GroupDto>().ForMember(d => d.ParentName, opt => opt.MapFrom(src => src.Parent.Name)).ReverseMap().ForPath(d => d.Parent.Name, opt => opt.Ignore());
            mapperConfiguration.CreateMap<Function, FunctionDto>().ForMember(d => d.ParentName, opt => opt.MapFrom(src => src.Parent.Name)).ReverseMap().ForPath(d => d.Parent.Name, opt => opt.Ignore());
            mapperConfiguration.CreateMap<User, UserDto>().ReverseMap().ForPath(d => d.PassWord, opt => opt.Ignore()).ForPath(d => d.Group.Name, opt => opt.Ignore());
        }


        //    services.AddAutoMapper(configAction=> {
        //        configAction.CreateMap<PageModel<Article>, PageModel<ArticleDto>>().ReverseMap();
        //        configAction.CreateMap<PageModel<Product>, PageModel<ProductDto>>().ReverseMap();
        //        configAction.CreateMap<User, UserDto>().ReverseMap();
        //        configAction.CreateMap<Article, ArticleDto>().ReverseMap();
        //        configAction.CreateMap<Tag, TagDto>().ReverseMap();
        //        configAction.CreateMap<Catagory, CatagoryDto>().ForMember(d => d.ParentName, opt => opt.MapFrom(src => src.Parent.Name)).ReverseMap().ForPath(d=>d.Parent.Name,opt=>opt.Ignore());
        //        configAction.CreateMap<ArticleContent, ArticleContentDto>().ReverseMap();
        //        configAction.CreateMap<Comment, CommentDto>().ReverseMap();
        //        configAction.CreateMap<Role, RoleDto>().ReverseMap();
        //        configAction.CreateMap<UploadFile, UploadFileDto>().ReverseMap();
        //        configAction.CreateMap<Product, ProductDto>().ReverseMap();
        //        configAction.CreateMap<ProductCategory, ProductCategoryDto>().ReverseMap();
        //        configAction.CreateMap<ProductCategoryTag, ProductCategoryTagDto>().ReverseMap();
        //        configAction.CreateMap<ProductImage, ProductImageDto>().ReverseMap();
        //        configAction.CreateMap<ProductTag, ProductTagDto>().ReverseMap();
        //        configAction.CreateMap<ProductDownload, ProductDownloadDto>().ReverseMap();
        //        configAction.CreateMap<NavigationMenu, NavigationMenuDto>().ForMember(d => d.ParentName, opt => opt.MapFrom(src => src.Parent.Name)).ReverseMap().ForPath(d => d.Parent.Name, opt => opt.Ignore());

        //},
        //        new Assembly[] { });
    }
}
