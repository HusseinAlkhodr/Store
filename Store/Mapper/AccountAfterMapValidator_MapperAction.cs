using AutoMapper;
using Store.Core.Unit;
using Store.Models.Authenitication;

namespace Store.Mapper
{
    public class AccountAfterMapValidator_MapperAction : IMappingAction<object, Account>
    {
        private readonly IUnitOfWork unitOfWork;

        public AccountAfterMapValidator_MapperAction(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public void Process(object source, Account destination, ResolutionContext context)
        {
            destination.Validiate(unitOfWork).Wait();
        }
    }
}
