using System;
using System.Text;
using DespesasREST.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace DespesasREST
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // Definir o protocolo de Autenticação
            string tokenKey = Configuration["Jwt:Key"]; // Obtém a chave de tokens
            byte[] key = Encoding.ASCII.GetBytes(tokenKey); // Codifica a chave de tokens

            // Middleware's para Autenticação
            services.AddAuthentication(x =>
                {
                    // 1º - Esquemas de Autenticação e "Challenge" a usar - JwtBearer
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }) // 2º - Configurar JwtBearer - Valida JWT recebido no Header
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    // options.Audience = "https://localhost:44325/";
                    // options.Authority = "https://localhost:44325/";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero,
                    };
                });

            // Registar a classe que gere o token
            services.AddSingleton<IJwtAuthenticationManager>(new JwtAuthenticationManager(tokenKey));
            // Caso interesse partir apenas dos dados que estão no appsetttings.json para definir o JWT
            services.AddSingleton<IConfiguration>(Configuration);

            // Configurar o OpenAPI
            services.AddSwaggerDocument(o =>
                o.PostProcess = document =>
                {
                    document.Info.Title = "Despesas REST";
                    document.Info.Version = "v1";
                    document.Info.Description = "API REST";
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3(); // Nuget Package"NSwag.AspNetCore

            app.UseRouting();

            app.UseAuthentication(); // Para usar autenticação com token
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}