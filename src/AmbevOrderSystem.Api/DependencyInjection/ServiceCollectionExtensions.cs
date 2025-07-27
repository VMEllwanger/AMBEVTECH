using AmbevOrderSystem.Api.ModelState;
using AmbevOrderSystem.Infrastructure.Data;
using AmbevOrderSystem.Infrastructure.DTOs;
using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Infrastructure.Services;
using AmbevOrderSystem.Services.Decorators;
using AmbevOrderSystem.Services.Factories;
using AmbevOrderSystem.Services.Implementations;
using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Order;
using AmbevOrderSystem.Services.Models.Commands.Outbox;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AmbevOrderSystem.Services.Models.Responses.Order;
using AmbevOrderSystem.Services.Models.Responses.Outbox;
using AmbevOrderSystem.Services.Models.Responses.Reseller;
using AmbevOrderSystem.Services.Services;
using AmbevOrderSystem.Services.UseCases.Order;
using AmbevOrderSystem.Services.UseCases.Outbox;
using AmbevOrderSystem.Services.UseCases.Reseller;
using AmbevOrderSystem.Services.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Polly;
using System.Text.Json.Serialization;

namespace AmbevOrderSystem.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("AmbevOrderSystemDb"));

            services.AddScoped<IResellerRepository, ResellerRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOutboxRepository, OutboxRepository>();

            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IOutboxService, OutboxService>();
            services.AddScoped<IAmbevApiService, MockAmbevApiService>();

            services.AddScoped<IResellerFactory, ResellerFactory>();
            services.AddScoped<IOrderFactory, OrderFactory>();

            return services;
        }

        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddScoped<CreateResellerUseCase>();
            services.AddScoped<GetResellerByIdUseCase>();
            services.AddScoped<GetResellerByCnpjUseCase>();
            services.AddScoped<GetAllResellersUseCase>();
            services.AddScoped<UpdateResellerUseCase>();
            services.AddScoped<DeleteResellerUseCase>();

            services.AddScoped<CreateCustomerOrderUseCase>();
            services.AddScoped<GetOrderByIdUseCase>();
            services.AddScoped<GetOrdersByResellerUseCase>();
            services.AddScoped<ProcessPendingOrdersUseCase>();

            services.AddScoped<GetPendingCountUseCase>();
            services.AddScoped<GetMessagesByCorrelationIdUseCase>();
            services.AddScoped<ProcessPendingMessagesUseCase>();
            services.AddScoped<ProcessRetryMessagesUseCase>();

            services.AddScoped<AbstractValidator<CreateResellerCommand>, CreateResellerCommandValidator>();
            services.AddScoped<AbstractValidator<UpdateResellerCommand>, UpdateResellerCommandValidator>();

            services.AddScoped<IUseCase<CreateResellerCommand, ResellerResponse>>(provider =>
            {
                var validator = provider.GetRequiredService<AbstractValidator<CreateResellerCommand>>();
                var useCase = provider.GetRequiredService<CreateResellerUseCase>();
                var logger = provider.GetRequiredService<ILogger<ValidationDecorator<CreateResellerCommand, ResellerResponse>>>();

                return new ValidationDecorator<CreateResellerCommand, ResellerResponse>(validator, useCase, logger);
            });

            services.AddScoped<IUseCase<UpdateResellerCommand, ResellerResponse>>(provider =>
            {
                var validator = provider.GetRequiredService<AbstractValidator<UpdateResellerCommand>>();
                var useCase = provider.GetRequiredService<UpdateResellerUseCase>();
                var logger = provider.GetRequiredService<ILogger<ValidationDecorator<UpdateResellerCommand, ResellerResponse>>>();

                return new ValidationDecorator<UpdateResellerCommand, ResellerResponse>(validator, useCase, logger);
            });

            services.AddScoped<IUseCase<GetResellerByIdCommand, ResellerResponse>, GetResellerByIdUseCase>();
            services.AddScoped<IUseCase<GetResellerByCnpjCommand, ResellerResponse>, GetResellerByCnpjUseCase>();
            services.AddScoped<IUseCase<GetAllResellersCommand, GetAllResellersResponse>, GetAllResellersUseCase>();
            services.AddScoped<IUseCase<DeleteResellerCommand, DeleteResellerResponse>, DeleteResellerUseCase>();

            services.AddScoped<IUseCase<CreateCustomerOrderCommand, CreateCustomerOrderResponse>, CreateCustomerOrderUseCase>();
            services.AddScoped<IUseCase<GetOrderByIdCommand, GetOrderResponse>, GetOrderByIdUseCase>();
            services.AddScoped<IUseCase<GetOrdersByResellerCommand, GetAllOrdersResponse>, GetOrdersByResellerUseCase>();
            services.AddScoped<IUseCase<ProcessPendingOrdersCommand, ProcessPendingOrdersResponse>, ProcessPendingOrdersUseCase>();

            services.AddScoped<IUseCase<GetPendingCountCommand, GetPendingCountResponse>, GetPendingCountUseCase>();
            services.AddScoped<IUseCase<GetMessagesByCorrelationIdCommand, GetMessagesByCorrelationIdResponse>, GetMessagesByCorrelationIdUseCase>();
            services.AddScoped<IUseCase<ProcessPendingMessagesCommand, ProcessMessagesResponse>, ProcessPendingMessagesUseCase>();
            services.AddScoped<IUseCase<ProcessRetryMessagesCommand, ProcessMessagesResponse>, ProcessRetryMessagesUseCase>();

            services.AddHostedService<OutboxProcessorService>();

            return services;
        }

        public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddResiliencePipeline<string, AmbevOrderResponse>("ambev-api-policy", builder =>
            {
                builder.AddRetry(new()
                {
                    ShouldHandle = new PredicateBuilder<AmbevOrderResponse>()
                        .Handle<HttpRequestException>()
                        .Handle<TaskCanceledException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential
                })
                .AddTimeout(TimeSpan.FromSeconds(30));
            });

            return services;
        }

        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });

            services.Configure<ApiBehaviorOptions>(x => x.InvalidModelStateResponseFactory = ctx => new CustomModelStateResponseFactory());

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ambev Order System API",
                    Version = "v1",
                    Description = "Sistema de gerenciamento de pedidos para revendas Ambev"
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            services.AddHealthChecks();

            return services;
        }
    }
}