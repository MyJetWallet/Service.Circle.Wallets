using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Circle.Models.Cards;
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
                if (request.OnlyActive)
                {
                    var cachedClientCards = await _writer.GetAsync(
                        CircleCardNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                        CircleCardNoSqlEntity.GenerateRowKey(request.ClientId));
                    if (cachedClientCards != null)
                    {
                        var cachedCard = cachedClientCards.Cards.Find(e => e.Id == request.CardId && e.IsActive);
                        return cachedCard != null
                            ? Grpc.Models.Response<CircleCard>.Success(cachedCard)
                            : Grpc.Models.Response<CircleCard>.Error("Card not found");
                    }
                }

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var clientCards = await ctx.Cards
                    .Where(t => t.BrokerId == request.BrokerId && t.ClientId == request.ClientId &&
                                (!request.OnlyActive || t.IsActive))
                    .ToListAsync();
                if (clientCards == null) return Grpc.Models.Response<CircleCard>.Error("Card not found");

                if (request.OnlyActive)
                {
                    var entity = CircleCardNoSqlEntity.Create(request.BrokerId, request.ClientId,
                        clientCards.ConvertAll(e => new CircleCard(e)));
                    await _writer.InsertAsync(entity);
                }

                var card = clientCards.Find(e => e.Id == request.CardId && e.IsActive);
                return card != null
                    ? Grpc.Models.Response<CircleCard>.Success(new CircleCard(card))
                    : Grpc.Models.Response<CircleCard>.Error("Card not found");
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
                if (request.OnlyActive)
                {
                    var cachedClientCards = await _writer.GetAsync(
                        CircleCardNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                        CircleCardNoSqlEntity.GenerateRowKey(request.ClientId));
                    if (cachedClientCards != null)
                    {
                        return Grpc.Models.Response<List<CircleCard>>.Success(cachedClientCards.Cards.FindAll(e => e.IsActive));
                    }
                }

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var clientCards = await ctx.Cards
                    .Where(t => t.BrokerId == request.BrokerId && t.ClientId == request.ClientId &&
                                (!request.OnlyActive || t.IsActive))
                    .ToListAsync();

                if (clientCards == null) return Grpc.Models.Response<List<CircleCard>>.Error("Card not found");

                if (request.OnlyActive)
                {
                    var entity = CircleCardNoSqlEntity.Create(request.BrokerId, request.ClientId,
                        clientCards.ConvertAll(e => new CircleCard(e)));
                    await _writer.InsertAsync(entity);
                }

                return Grpc.Models.Response<List<CircleCard>>.Success(clientCards.ConvertAll(e => new CircleCard(e)));
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Unable to get Circle cards due to {error}", ex.Message);
                return Grpc.Models.Response<List<CircleCard>>.Error(ex.Message);
            }
        }

        public async Task<Grpc.Models.Response<CircleCard>> AddCircleCard(AddClientCardRequest request)
        {
            try
            {
                var response = await _circleCardsService.AddCircleCard(
                    new AddCardRequest
                    {
                        BrokerId = request.BrokerId,
                        ClientId = request.ClientId,
                        IdempotencyKey = request.IdempotencyKey,
                        KeyId = request.KeyId,
                        EncryptedData = request.EncryptedData,
                        BillingName = request.BillingName,
                        BillingCity = request.BillingCity,
                        BillingCountry = request.BillingCountry,
                        BillingLine1 = request.BillingLine1,
                        BillingLine2 = request.BillingLine2,
                        BillingDistrict = request.BillingDistrict,
                        BillingPostalCode = request.BillingPostalCode,
                        ExpMonth = request.ExpMonth,
                        ExpYear = request.ExpYear,
                        SessionId = request.SessionId,
                        IpAddress = request.IpAddress
                    });

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                CircleCardEntity clientCardEntity;
                if (!response.IsSuccess)
                {
                    _logger.LogInformation("Unable to add Circle card due to {error}", response.ErrorMessage);
                    clientCardEntity = new CircleCardEntity
                    {
                        Id = Guid.NewGuid().ToString(),
                        BrokerId = request.BrokerId,
                        ClientId = request.ClientId,
                        CardName = request.CardName,
                        Status = CircleCardStatus.Failed,
                        Error = response.ErrorMessage,
                        IsActive = false,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    };
                    await ctx.AddAsync(clientCardEntity);
                    await ctx.SaveChangesAsync();

                    return Grpc.Models.Response<CircleCard>.Error(response.ErrorMessage);
                }

                clientCardEntity = new CircleCardEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    BrokerId = request.BrokerId,
                    ClientId = request.ClientId,
                    CardName = !string.IsNullOrEmpty(request.CardName) ? request.CardName : $"{response.Data.Network} *{response.Data.Last4}",
                    CircleCardId = response.Data.Id,
                    Last4 = response.Data.Last4,
                    Network = response.Data.Network,
                    ExpMonth = response.Data.ExpMonth,
                    ExpYear = response.Data.ExpYear,
                    Status = ConvertCardStatus(response.Data.Status),
                    Error = ConvertCardVerificationError(response.Data.ErrorCode)?.ToString(),
                    IsActive = ConvertCardStatus(response.Data.Status) != CircleCardStatus.Failed,
                    CreateDate = DateTime.Parse(response.Data.CreateDate),
                    UpdateDate = DateTime.Parse(response.Data.UpdateDate)
                };

                await ctx.AddAsync(clientCardEntity);
                await ctx.SaveChangesAsync();

                if (clientCardEntity.IsActive)
                {
                    var cachedClientCards = await _writer.GetAsync(
                        CircleCardNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                        CircleCardNoSqlEntity.GenerateRowKey(request.ClientId));
                    if (cachedClientCards != null)
                    {
                        cachedClientCards.Cards.Add(clientCardEntity);
                        await _writer.InsertOrReplaceAsync(cachedClientCards);
                    }
                    else
                    {
                        var entity = CircleCardNoSqlEntity.Create(request.BrokerId, request.ClientId,
                            new List<CircleCard> { clientCardEntity });
                        await _writer.InsertAsync(entity);
                    }
                }

                return Grpc.Models.Response<CircleCard>.Success(new CircleCard(clientCardEntity));
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Unable to add Circle card due to {error}", ex.Message);
                return Grpc.Models.Response<CircleCard>.Error(ex.Message);
            }
        }

        public async Task<Grpc.Models.Response<bool>> DeleteCircleCard(DeleteClientCardRequest request)
        {
            try
            {
                var existingNoSqlEntity = await _writer.GetAsync(
                    CircleCardNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                    CircleCardNoSqlEntity.GenerateRowKey(request.ClientId));
                var card = existingNoSqlEntity?.Cards.Find(e => e.Id == request.CardId);
                if (card != null)
                {
                    existingNoSqlEntity.Cards.Remove(card);
                    if (existingNoSqlEntity.Cards.Count > 0)
                    {
                        await _writer.InsertOrReplaceAsync(existingNoSqlEntity);
                    }
                    else
                    {
                        await _writer.DeleteAsync(existingNoSqlEntity.PartitionKey, existingNoSqlEntity.RowKey);
                    }
                }

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var existingPostgresEntity = await ctx.Cards
                    .Where(t => t.Id == request.CardId)
                    .Where(t => t.BrokerId == request.BrokerId && t.ClientId == request.ClientId).FirstOrDefaultAsync();

                if (existingPostgresEntity == null) return Grpc.Models.Response<bool>.Success(true);

                existingPostgresEntity.IsActive = false;
                existingPostgresEntity.UpdateDate = DateTime.Now;
                await ctx.UpdateAsync(existingPostgresEntity);
                await ctx.SaveChangesAsync();

                return Grpc.Models.Response<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Unable to delete Circle card due to {error}", ex.Message);
                return Grpc.Models.Response<bool>.Error(ex.Message);
            }
        }

        private CircleCardStatus ConvertCardStatus(CardStatus status)
        {
            switch (status)
            {
                case CardStatus.Pending:
                    return CircleCardStatus.Pending;
                case CardStatus.Complete:
                    return CircleCardStatus.Complete;
                case CardStatus.Failed:
                    return CircleCardStatus.Failed;
                default:
                    return CircleCardStatus.Pending;
            }
        }

        private CircleCardVerificationError? ConvertCardVerificationError(CardVerificationError? error)
        {
            switch (error)
            {
                case CardVerificationError.CardFailed:
                    return CircleCardVerificationError.CardFailed;
                case CardVerificationError.CardAddressMismatch:
                    return CircleCardVerificationError.CardAddressMismatch;
                case CardVerificationError.CardZipMismatch:
                    return CircleCardVerificationError.CardZipMismatch;
                case CardVerificationError.CardCvvInvalid:
                    return CircleCardVerificationError.CardCvvInvalid;
                case CardVerificationError.CardExpired:
                    return CircleCardVerificationError.CardExpired;
                case CardVerificationError.VerificationFailed:
                case CardVerificationError.VerificationFraudDetected:
                case CardVerificationError.VerificationDenied:
                case CardVerificationError.VerificationNotSupportedByIssuer:
                case CardVerificationError.VerificationStoppedByIssuer:
                case CardVerificationError.CardInvalid:
                case CardVerificationError.CardLimitViolated:
                case CardVerificationError.CardNotHonored:
                case CardVerificationError.CardCvvRequired:
                case CardVerificationError.CreditCardNotAllowed:
                case CardVerificationError.CardAccountIneligible:
                case CardVerificationError.CardNetworkUnsupported:
                    _logger.LogWarning("Unsupported Card Verification error {error}", error.ToString());
                    return CircleCardVerificationError.CardFailed;
                case null:
                    return null;
                default:
                    return CircleCardVerificationError.CardFailed;
            }
        }
    }
}