using Authentication.Models;
using AuthEntities.Domain.Entities;
using AutoMapper;
using WalletApi.Features.AuthFeature.Dtos;
using WalletApi.Features.CardFeature.Dtos;
using WalletApi.Features.TransactionFeature.Dtos;
using WalletEntities.Domain.Dtos;
using WalletEntities.Domain.Entities;
using Transaction = WalletEntities.Domain.Entities.Transaction;

namespace WalletApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserRegistrationRequest, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name));
            CreateMap<AccessTokenData, AuthToken>();
            CreateMap<AuthToken, AccessTokenData>();

            CreateMap<CreateTransactionRequest, Transaction>();
            CreateMap<Transaction, TransactionResponse>();

            CreateMap<AuthorizedUser, AuthorizedUserResponse>();

            CreateMap<CreateCardRequest, Card>();
            CreateMap<Card, CardResponse>();
        }
    }
}