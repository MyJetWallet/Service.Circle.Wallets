using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.Circle.Signer.Grpc.Models;
using Service.Circle.Wallets.Domain.Models;
using Service.Circle.Wallets.Grpc;
using Service.Circle.Wallets.Grpc.Models;
using Service.Circle.Wallets.Postgres;
using Service.Circle.Wallets.Postgres.Models;
using ICircleSignerCardsService = Service.Circle.Signer.Grpc.ICircleCardsService;

// ReSharper disable InconsistentLogPropertyNaming

namespace Service.Circle.Wallets.Services
{
    public class CircleCardsService : ICircleCardsService
    {
        private readonly ILogger<CircleCardsService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IMyNoSqlServerDataWriter<CircleCardNoSqlEntity> _writer;
        private readonly ICircleSignerCardsService _circleCardsService;

        public CircleCardsService(ILogger<CircleCardsService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IMyNoSqlServerDataWriter<CircleCardNoSqlEntity> writer, ICircleSignerCardsService circleCardsService)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _writer = writer;
            _circleCardsService = circleCardsService;
        }

        public async Task<Grpc.Models.Response<CircleCard>> GetCircleClientCard(GetClientCardRequest request)
        {
            try
            {
                var cachedClientCards = await _writer.GetAsync(
                    CircleCardNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                    CircleCardNoSqlEntity.GenerateRowKey(request.ClientId));
                if (cachedClientCards != null)
                {
                    var card = cachedClientCards.Cards.Find(e => e.Id == request.CardId);
                    return card != null
                        ? Grpc.Models.Response<CircleCard>.Success(card)
                        : Grpc.Models.Response<CircleCard>.Error("Card not found");
                }

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var clientCards = await ctx.Cards
                    .Where(t => t.BrokerId == request.BrokerId && t.ClientId == request.ClientId).ToListAsync();
                if (clientCards != null)
                {
                    var entity = CircleCardNoSqlEntity.Create(request.BrokerId, request.ClientId,
                        clientCards.ConvertAll(e => new CircleCard(e)));
                    await _writer.InsertAsync(entity);
                    var card = clientCards.Find(e => e.Id == request.CardId);
                    return card != null
                        ? Grpc.Models.Response<CircleCard>.Success(card)
                        : Grpc.Models.Response<CircleCard>.Error("Card not found");
                }

                return Grpc.Models.Response<CircleCard>.Error("Card not found");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Unable to get Circle card due to {error}", ex.Message);
                return Grpc.Models.Response<CircleCard>.Error(ex.Message);
            }
        }

        public async Task<Grpc.Models.Response<List<CircleCard>>> GetCircleClientAllCards(
            GetClientAllCardsRequest request)
        {
            try
            {
                var cachedClientCards = await _writer.GetAsync(
                    CircleCardNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                    CircleCardNoSqlEntity.GenerateRowKey(request.ClientId));
                if (cachedClientCards != null)
                {
                    return Grpc.Models.Response<List<CircleCard>>.Success(cachedClientCards.Cards);
                }

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var clientCards = await ctx.Cards
                    .Where(t => t.BrokerId == request.BrokerId && t.ClientId == request.ClientId).ToListAsync();

                if (clientCards == null) return Grpc.Models.Response<List<CircleCard>>.Error("Card not found");

                var entity = CircleCardNoSqlEntity.Create(request.BrokerId, request.ClientId,
                    clientCards.ConvertAll(e => new CircleCard(e)));
                await _writer.InsertAsync(entity);
                return Grpc.Models.Response<List<CircleCard>>.Success(entity.Cards);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Unable to get Circle cards due to {error}", ex.Message);
                return Grpc.Models.Response<List<CircleCard>>.Error(ex.Message);
            }
        }

        public async Task<Grpc.Models.Response<CircleCard>> AddCircleCard(AddClientCardRequest card)
        {
            try
            {
                var response = await _circleCardsService.AddCircleCard(
                    new AddCardRequest
                    {
                        BrokerId = card.BrokerId,
                        IdempotencyKey = card.IdempotencyKey,
                        KeyId = card.KeyId,
                        EncryptedData = card.EncryptedData,
                        BillingName = card.BillingName,
                        BillingCity = card.BillingCity,
                        BillingCountry = card.BillingCountry,
                        BillingLine1 = card.BillingLine1,
                        BillingLine2 = card.BillingLine2,
                        BillingDistrict = card.BillingDistrict,
                        BillingPostalCode = card.BillingPostalCode,
                        ExpMonth = card.ExpMonth,
                        ExpYear = card.ExpYear,
                        Email = card.Email,
                        PhoneNumber = card.PhoneNumber,
                        SessionId = card.SessionId,
                        IpAddress = card.IpAddress
                    });

                if (!response.IsSuccess)
                {
                    _logger.LogInformation("Unable to add Circle card due to {error}", response.ErrorMessage);
                    return Grpc.Models.Response<CircleCard>.Error(response.ErrorMessage);
                }

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var clientCardEntity = new CircleCardEntity
                {
                    BrokerId = card.BrokerId,
                    ClientId = card.ClientId,
                    CircleCardId = response.Data.Id,
                    BillingName = response.Data.BillingDetails.Name,
                    BillingCity = response.Data.BillingDetails.City,
                    BillingCountry = response.Data.BillingDetails.Country,
                    BillingLine1 = response.Data.BillingDetails.Line1,
                    BillingLine2 = response.Data.BillingDetails.Line2,
                    BillingDistrict = response.Data.BillingDetails.District,
                    BillingPostalCode = response.Data.BillingDetails.PostalCode,
                    ExpMonth = response.Data.ExpMonth,
                    ExpYear = response.Data.ExpYear,
                    Email = response.Data.Metadata.Email,
                    PhoneNumber = response.Data.Metadata.PhoneNumber,
                    SessionId = response.Data.Metadata.SessionId,
                    IpAddress = response.Data.Metadata.IpAddress,
                    Status = response.Data.Status.ToString(),
                    Network = response.Data.Network,
                    Last4 = response.Data.Last4,
                    Bin = response.Data.Bin,
                    IssuerCountry = response.Data.IssuerCountry,
                    FundingType = response.Data.FundingType,
                    Fingerprint = response.Data.Fingerprint,
                    ErrorCode = response.Data.ErrorCode,
                    CreateDate = response.Data.CreateDate,
                    UpdateDate = response.Data.CreateDate
                };
                try
                {
                    await ctx.AddAsync(clientCardEntity);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Unable to save Circle card due to {error}", ex.Message);
                    return Grpc.Models.Response<CircleCard>.Error(ex.Message);
                }

                var cachedClientCards = await _writer.GetAsync(
                    CircleCardNoSqlEntity.GeneratePartitionKey(card.BrokerId),
                    CircleCardNoSqlEntity.GenerateRowKey(card.ClientId));
                if (cachedClientCards != null)
                {
                    cachedClientCards.Cards.Add(clientCardEntity);
                }
                else
                {
                    var entity = CircleCardNoSqlEntity.Create(card.BrokerId, card.ClientId,
                        new List<CircleCard> { clientCardEntity });
                    await _writer.InsertAsync(entity);
                }

                return Grpc.Models.Response<CircleCard>.Success(clientCardEntity);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Unable to add Circle card due to {error}", ex.Message);
                return Grpc.Models.Response<CircleCard>.Error(ex.Message);
            }
        }

        public async Task<Grpc.Models.Response<CircleCard>> UpdateCircleCard(
            UpdateClientCardRequest request)
        {
            try
            {
                var existingCard = await GetCircleClientCard(new GetClientCardRequest
                {
                    BrokerId = request.BrokerId,
                    ClientId = request.ClientId,
                    CardId = request.CardId
                });
                if (!existingCard.IsSuccess || existingCard.Data == null)
                {
                    _logger.LogInformation("Unable to update Circle card, card not found");
                    return Grpc.Models.Response<CircleCard>.Error("Unable to update Circle card, card not found");
                }

                var updateCardResponse = await _circleCardsService.UpdateCircleCard(new UpdateCardRequest
                {
                    BrokerId = request.BrokerId,
                    CardId = existingCard.Data.CircleCardId,
                    EncryptedData = request.EncryptedData,
                    ExpMonth = request.ExpMonth,
                    ExpYear = request.ExpYear,
                    KeyId = request.KeyId
                });

                if (updateCardResponse.IsSuccess)
                {
                    await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                    var existingPostgresEntity = await ctx.Cards
                        .Where(t => t.Id == existingCard.Data.Id).FirstAsync();
                    existingPostgresEntity.ExpMonth = request.ExpMonth;
                    existingPostgresEntity.ExpYear = request.ExpYear;
                    try
                    {
                        await ctx.UpdateAsync(existingPostgresEntity);
                        await ctx.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("Unable to save Circle card due to {error}", ex.Message);
                        return Grpc.Models.Response<CircleCard>.Error(ex.Message);
                    }
                    
                    var existingNoSqlEntity = await _writer.GetAsync(
                        CircleCardNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                        CircleCardNoSqlEntity.GenerateRowKey(request.ClientId));

                    if (existingNoSqlEntity != null)
                    {
                        var card = existingNoSqlEntity.Cards.Find(e => e.Id == existingCard.Data.Id);
                        if (card != null)
                        {
                            card.ExpMonth = request.ExpMonth;
                            card.ExpYear = request.ExpYear;
                            await _writer.InsertOrReplaceAsync(existingNoSqlEntity);
                        }
                    }
                    
                    return Grpc.Models.Response<CircleCard>.Success(existingPostgresEntity);
                }
                else
                {
                    _logger.LogInformation("Unable to update Circle card due to {error}", updateCardResponse.ErrorMessage);
                    return Grpc.Models.Response<CircleCard>.Error(updateCardResponse.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Unable to update Circle card due to {error}", ex.Message);
                return Grpc.Models.Response<CircleCard>.Error(ex.Message);
            }
        }
    }
}