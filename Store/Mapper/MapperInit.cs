using AutoMapper;
using Store.DTO;
using Store.DTO.Pagination;
using Store.DTO.Result;
using Store.Models;
using Store.Models.Invoice;

namespace Store.Mapper
{
    public class MapperInit : Profile
    {
        public MapperInit()
        {
            CreateMap(typeof(ListItem<>), typeof(ListItem<>));
            CreateMap(typeof(PagedList<>), typeof(PagedList<>));
            CreateMap<Item, GetItemDTO>()
                .ForMember(des => des.Vendor, opt => opt.MapFrom(src => src.Vendor.Name))
                .ForMember(des => des.Division, opt => opt.MapFrom(src => src.Division.Name));
            CreateMap<AddItemDTO, Item>().ReverseMap();
            CreateMap<UpdateItemDTO, Item>().ReverseMap();

            CreateMap<Division, GetDivisionDTO>().ReverseMap();
            CreateMap<DivisionDTO, Division>().ReverseMap();
            CreateMap<AddDivisionDTO, Division>().ReverseMap();
            CreateMap<UpdateDivisionDTO, Division>().ReverseMap();

            CreateMap<Vendor, GetVendorDTO>().ReverseMap();
            CreateMap<VendorDTO, Vendor>().ReverseMap();
            CreateMap<AddVendorDTO, Vendor>().ReverseMap();
            CreateMap<UpdateVendorDTO, Vendor>().ReverseMap();

            CreateMap<SaleBill, GetSaleBillDTO>()
                .ForMember(des => des.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy.FullName))
                .ForMember(des => des.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("dd/MMM/yyyy - hh:mm")));
        }
    }
}
