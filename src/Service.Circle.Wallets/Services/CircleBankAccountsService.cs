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
using Service.Circle.Wallets.Domain.Models.WireTransfers;
using Service.Circle.Wallets.Grpc;
using Service.Circle.Wallets.Grpc.Models;
using Service.Circle.Wallets.Grpc.Models.BankAccounts;
using Service.Circle.Wallets.Postgres;
using Service.Circle.Wallets.Postgres.Models;

// ReSharper disable InconsistentLogPropertyNaming

namespace Service.Circle.Wallets.Services
{
    public class CircleBankAccountsService : ICircleBankAccountsService
    {
        private readonly ILogger<CircleBankAccountsService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IMyNoSqlServerDataWriter<CircleBankAccountNoSqlEntity> _writer;
        private readonly Service.Circle.Signer.Grpc.ICircleBankAccountsService _circleBankAccountsService;

        public CircleBankAccountsService(ILogger<CircleBankAccountsService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IMyNoSqlServerDataWriter<CircleBankAccountNoSqlEntity> writer,
            Service.Circle.Signer.Grpc.ICircleBankAccountsService circleBankAccountsService)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _writer = writer;
            _circleBankAccountsService = circleBankAccountsService;
        }

        public async Task<Grpc.Models.Response<CircleBankAccount>> GetCircleBankAccount(GetClientBankAccountRequest request)
        {
            try
            {
                if (request.OnlyActive)
                {
                    var cachedBankAccounts = await _writer.GetAsync(
                        CircleBankAccountNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                        CircleBankAccountNoSqlEntity.GenerateRowKey(request.ClientId));
                    if (cachedBankAccounts != null)
                    {
                        var cached = cachedBankAccounts.BankAccounts.Find(e => e.BankAccountId == request.BankAccountId && e.IsActive);
                        return cached != null
                            ? Grpc.Models.Response<CircleBankAccount>.Success(cached)
                            : Grpc.Models.Response<CircleBankAccount>.Error("BankAccount not found");
                    }
                }

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var accounts = await ctx.BankAccounts
                    .Where(t => t.BrokerId == request.BrokerId && t.ClientId == request.ClientId &&
                                (!request.OnlyActive || t.IsActive))
                    .ToListAsync();

                if (accounts == null) return Grpc.Models.Response<CircleBankAccount>.Error("BankAccount not found");

                if (request.OnlyActive)
                {
                    var entity = CircleBankAccountNoSqlEntity.Create(request.BrokerId, request.ClientId,
                        accounts.ConvertAll(e => new CircleBankAccount(e)));
                    await _writer.InsertOrReplaceAsync(entity);
                }

                var account = accounts.Find(e => e.BankAccountId == request.BankAccountId && e.IsActive);
                return account != null
                    ? Grpc.Models.Response<CircleBankAccount>.Success(new CircleBankAccount(account))
                    : Grpc.Models.Response<CircleBankAccount>.Error("Bank not found");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Unable to get Circle Bank account due to {error}", ex.Message);
                return Grpc.Models.Response<CircleBankAccount>.Error(ex.Message);
            }
        }

        public async Task<Grpc.Models.Response<CircleBankAccount>> GetCircleBankAccountByIdOnly(GetClientBankAccountByIdRequest request)
        {
            try
            {
                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var account = await ctx.BankAccounts
                    .FirstOrDefaultAsync(t => t.BankAccountId == request.BankAccountId);

                if (account == null) return Grpc.Models.Response<CircleBankAccount>.Error("BankAccount not found");

                return Grpc.Models.Response<CircleBankAccount>.Success(account);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Unable to get Circle Bank account due to {error}", ex.Message);
                return Grpc.Models.Response<CircleBankAccount>.TechnicalError(ex.Message);
            }
        }

        public async Task<Grpc.Models.Response<List<CircleBankAccount>>> GetCircleClientAllBankAccounts(GetClientAllBankAccountsRequest request)
        {
            try
            {
                if (request.OnlyActive)
                {
                    var cachedClientCards = await _writer.GetAsync(
                        CircleBankAccountNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                        CircleBankAccountNoSqlEntity.GenerateRowKey(request.ClientId));
                    if (cachedClientCards != null)
                    {
                        return Grpc.Models.Response<List<CircleBankAccount>>.Success(cachedClientCards.BankAccounts.FindAll(e => e.IsActive));
                    }
                }

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var clientCards = await ctx.BankAccounts
                    .Where(t => t.BrokerId == request.BrokerId && t.ClientId == request.ClientId &&
                                (!request.OnlyActive || t.IsActive))
                    .ToListAsync();

                if (clientCards == null) return Grpc.Models.Response<List<CircleBankAccount>>.Error("BankAccount not found");

                if (request.OnlyActive)
                {
                    var entity = CircleBankAccountNoSqlEntity.Create(request.BrokerId, request.ClientId,
                        clientCards.ConvertAll(e => new CircleBankAccount(e)));
                    await _writer.InsertOrReplaceAsync(entity);
                }

                return Grpc.Models.Response<List<CircleBankAccount>>.Success(clientCards.ConvertAll(e => new CircleBankAccount(e)));
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Unable to get BankAccounts due to {error}", ex.Message);
                return Grpc.Models.Response<List<CircleBankAccount>>.Error(ex.Message);
            }
        }

        public async Task<Grpc.Models.Response<CircleBankAccount>> AddCircleBankAccount(AddClientBankAccountRequest request)
        {
            try
            {
                bool isSepa = !string.IsNullOrEmpty(request.Iban);

                var bankAddress = new MyJetWallet.Circle.Models.WireTransfers.BankAddress
                {
                    BankName = request.BankAddressBankName,
                    City = request.BankAddressCity,
                    Country = request.BankAddressCountry,
                    District = request.BankAddressDistrict,
                    Line1 = request.BankAddressLine1,
                    Line2 = request.BankAddressLine2
                };
                
                var billingDetails = new MyJetWallet.Circle.Models.WireTransfers.BillingDetails
                {
                    City = request.BillingDetailsCity,
                    Country = request.BillingDetailsCountry,
                    District = request.BillingDetailsDistrict,
                    Line1 = request.BillingDetailsLine1,
                    Line2 = request.BillingDetailsLine2,
                    Name = request.BillingDetailsName,
                    PostalCode = request.BillingDetailsPostalCode,
                };
                var response = isSepa ? await _circleBankAccountsService.AddCircleSepaBankAccount(
                    new AddSepaBankAccountRequest
                    {
                        BrokerId = request.BrokerId,
                        IdempotencyKey = request.Id,
                        Iban = request.Iban,
                        BankAddress = bankAddress,
                        BillingDetails = billingDetails
                    }) : await _circleBankAccountsService.AddCircleUsSwiftBankAccount(
                    new AddUsSwiftBankAccountRequest
                    {
                        BrokerId = request.BrokerId,
                        IdempotencyKey = request.Id,
                        AccountNumber = request.AccountNumber,
                        RoutingNumber = request.RoutingNumber,
                        BankAddress = bankAddress,
                        BillingDetails = billingDetails
                    });

                if (!response.IsSuccess)
                {
                    _logger.LogInformation("Unable to add Circle BankAccount to {error}", response.ErrorMessage);
                    
                    return Grpc.Models.Response<CircleBankAccount>.Error(response.ErrorMessage);
                }

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var entity = new CircleBankAccountEntity
                {
                    Id = request.Id,
                    BrokerId = request.BrokerId,
                    ClientId = request.ClientId,
                    Iban = request.Iban,
                    AccountNumber = request.AccountNumber,
                    RoutingNumber = request.RoutingNumber,
                    BankAccountId = response.Data.Id,
                    BankAccountStatus = response.Data.Status,
                    BankAddressBankName = response.Data.BankAddress.BankName,
                    BankAddressCity = response.Data.BankAddress.City,
                    BankAddressCountry = response.Data.BankAddress.Country,
                    BankAddressDistrict = response.Data.BankAddress.District,
                    BankAddressLine1 = response.Data.BankAddress.Line1,
                    BankAddressLine2 = response.Data.BankAddress.Line2,
                    BillingDetailsCity = response.Data.BillingDetails.City,
                    BillingDetailsCountry = response.Data.BillingDetails.Country,
                    BillingDetailsDistrict = response.Data.BillingDetails.District,
                    BillingDetailsLine1 = response.Data.BillingDetails.Line1,
                    BillingDetailsLine2 = response.Data.BillingDetails.Line2,
                    BillingDetailsName = response.Data.BillingDetails.Name,
                    BillingDetailsPostalCode = response.Data.BillingDetails.PostalCode,
                    Description = response.Data.Description,
                    FingerPrint = response.Data.Fingerprint,
                    TrackingRef = response.Data.TrackingRef,
                    Error = response.ErrorMessage,
                    IsActive = true,
                    CreateDate = response.Data.CreateDate,
                    UpdateDate = response.Data.UpdateDate
                };

                await ctx.AddAsync(entity);
                await ctx.SaveChangesAsync();

                if (entity.IsActive)
                {
                    var cached = await _writer.GetAsync(
                        CircleBankAccountNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                        CircleBankAccountNoSqlEntity.GenerateRowKey(request.ClientId));
                    if (cached != null)
                    {
                        cached.BankAccounts.Add(entity);
                        await _writer.InsertOrReplaceAsync(cached);
                    }
                    else
                    {
                        var cache = CircleBankAccountNoSqlEntity.Create(request.BrokerId, request.ClientId,
                            new List<CircleBankAccount> { entity });
                        await _writer.InsertOrReplaceAsync(cache);
                    }
                }

                return Grpc.Models.Response<CircleBankAccount>.Success(new CircleBankAccount(entity));
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Unable to add Circle card due to {error}", ex.Message);
                return Grpc.Models.Response<CircleBankAccount>.Error(ex.Message);
            }
        }

        public async Task<Grpc.Models.Response<bool>> DeleteCircleBankAccount(DeleteClientBankAccountRequest request)
        {
            try
            {
                var existingNoSqlEntity = await _writer.GetAsync(
                    CircleBankAccountNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                    CircleBankAccountNoSqlEntity.GenerateRowKey(request.ClientId));
                var account = existingNoSqlEntity?.BankAccounts.Find(e => e.BankAccountId == request.BankAccountId);
                if (account != null)
                {
                    existingNoSqlEntity.BankAccounts.Remove(account);
                    if (existingNoSqlEntity.BankAccounts.Count > 0)
                    {
                        await _writer.InsertOrReplaceAsync(existingNoSqlEntity);
                    }
                    else
                    {
                        await _writer.DeleteAsync(existingNoSqlEntity.PartitionKey, existingNoSqlEntity.RowKey);
                    }
                }

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var existingPostgresEntity = await ctx.BankAccounts
                    .Where(t => t.BankAccountId == request.BankAccountId)
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
                _logger.LogInformation("Unable to delete Circle BankAccount due to {error}", ex.Message);
                return Grpc.Models.Response<bool>.Error(ex.Message);
            }
        }

        public async Task<Grpc.Models.Response<Grpc.Models.BankAccounts.CircleBankWireTransferDetails>> GetCircleBankWireTransferDetails(GetCircleBankWireTransferDetailsRequest request)
        {
            try
            {
                var response = await _circleBankAccountsService.GetBankWireTransferDetails(new()
                {
                    BankAccountId = request.BankAccountId,
                    BrokerId = request.BrokerId,
                });

                if (!response.IsSuccess)
                {
                    _logger.LogInformation("Unable to CircleBankWireTransferDetails to {error}", response.ErrorMessage);

                    return Grpc.Models.Response<Grpc.Models.BankAccounts.CircleBankWireTransferDetails>.Error(response.ErrorMessage);
                }

                return Grpc.Models.Response<Grpc.Models.BankAccounts.CircleBankWireTransferDetails>.Success(new ()
                {
                    Beneficiary = response.Data.Beneficiary,
                    BeneficiaryBank = response.Data.BeneficiaryBank,
                    TrackingRef = response.Data.TrackingRef,
                });
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Unable to GetCircleBankWireTransferDetails due to {error}", ex.Message);
                return Grpc.Models.Response<Grpc.Models.BankAccounts.CircleBankWireTransferDetails>.Error(ex.Message);
            }
        }
    }
}